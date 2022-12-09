using System;
using System.Collections.Generic;
using System.Text;
using Rhino;
using Rhino.Geometry;

namespace ConsoleApp1
{
     public class GenLight
    {
        public int Accuracy { get; set; }
        public List<string> LightConditions { get; set; }
        public List<Vector3d> Normals { get; set; }
        public List<Surface> SurfaceList { get; set; }

        public GenLight(int accuracy,List<string> lightconditions,List<Vector3d> normals, List<Surface> surfacelist)
        {
            this.Accuracy = accuracy;
            this.LightConditions = lightconditions;
            this.Normals = normals;
            this.SurfaceList = surfacelist;
        }

        /// <summary>
        /// adds rays to a spaces of a list of Houses
        /// </summary>
        /// <param name="listhouse">House instances</param>
        /// <param name="Allmeshs">light blockers</param>
        public  void AddLightToListOfHouse(List<House> listhouse, List<Brep> Allmeshs)
        {
            try
            {
                for (int i = 0; i < listhouse.Count; i++)
                {
                    AddLightToHouse(listhouse[i], Allmeshs);
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("a problem happened in AddLightToListOfHouse method ");
            }
        }
        /// <summary>
        /// adds rays to each Space of a House
        /// </summary>
        /// <param name="house">the House instance</param>
        /// <param name="Allmeshs">light blockers</param>
        public  void AddLightToHouse(House house, List<Brep> Allmeshs)
        {
            try
            {
                for (int i = 0; i < house.Each_House.Count; i++)
                {
                    AddRayToSpace(house.Each_House[i], Allmeshs);
                }
            }
            catch
            {
                RhinoApp.WriteLine("a problem happened in AddLightToHouse method ");
            }
        }
        
        /// <summary>
        /// take the space and add the rays based on the lightconditions
        /// </summary>
        /// <param name="space">mesh of the space</param>
        /// <param name="Allmeshs">all meshes for light blockers</param>
        private void AddRayToSpace(Space space, List<Brep> Allmeshs)
        {
            try
            {
                foreach (var normal in Normals)
                {

                    if (space.Light != 0)
                    {
                        var light = space.Light;
                        // [light-1] because the zero is no light 
                        var Condition_1 = LightConditions[light - 1];
                        int[] cond_1 = Generals.GetString(Condition_1);
                        var checkconnectedfaces = space.ConnectedFaces;
                        var count = 0;
                        for (int i = 0; i < cond_1.Length; i++)
                        {
                            if (!Generals.CheckDupIntInList(checkconnectedfaces, cond_1[i]))
                            {
                                count++;
                            }
                        }
                        if (count == cond_1.Length)
                        {
                            throw new ArgumentException(String.Format("light condition {0} for a space is set badly due to their adjacent connected spaces and will not give any true alternative", space.Light));
                        }
                        for (int i = 0; i < cond_1.Length; i++)
                        {
                            if (Generals.CheckDupIntInList(checkconnectedfaces, cond_1[i]))
                            {
                                AddRaysToFace(space, cond_1[i], normal, Allmeshs);
                            }

                        }

                    }
                }
            }
            catch (Exception)
            {
                RhinoApp.WriteLine("a problem happened in AddRayToSpace method ");
            }
        }
        /// <summary>
        /// add rays to the a face based on its index
        /// </summary>
        /// <param name="mesh">mesh of the space</param>
        /// <param name="faceindex">index of the face based on FaceIndex enum</param>
        /// <param name="boundbx">bounding box of the mesh</param>
        /// <param name="raybase"></param>
        /// <param name="normal">normal of light</param>
        /// <param name="Allmeshs">all meshes for light blockers</param>

        private void AddRaysToFace(Space space, int faceindex, Vector3d normal, List<Brep> Allmeshs)
        {
            try
            {
                var mesh = space.mesh;
                Vector3d FaceNormal = Generals.CalculateNormalOfFace(GenRays.GetFaceCorners(faceindex, mesh));
                if (Vector3d.VectorAngle(FaceNormal, normal) < (Math.PI / 2) - 00.1)
                {

                    List<Point3d> pts = GenRays.GetFaceLightPoints(Accuracy, faceindex, mesh);
                    //here we calcullate the face rays
                    List<Ray3d> rays = GenRays.FaceRays(pts, normal);
                    List<Point3d> OriginRayPoints = new List<Point3d>();
                    List<Point3d[]> intersection_with_Breps = GenRays.IntersectionRays(rays, Allmeshs, out OriginRayPoints);
                    List<Point3d> FinalOriginPoints = new List<Point3d>();
                    List<Point3d> GetPointsOnSurface = Generals.CheckToBeOnSurface(intersection_with_Breps, OriginRayPoints, SurfaceList, out FinalOriginPoints);
                    Ray_Face TempRayFace = new Ray_Face(GetPointsOnSurface, FinalOriginPoints, GenRays.GetFaceCorners(faceindex, mesh), faceindex);
                    //add rayfaces to the rayface property of space
                    space.AddRayFaceFunction(TempRayFace, faceindex);
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("a problem happened in  AddRaysToFace method ");
            }

        }
    }
}
