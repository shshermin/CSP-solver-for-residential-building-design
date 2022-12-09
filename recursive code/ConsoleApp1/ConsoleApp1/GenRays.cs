using System;
using System.Collections.Generic;
using System.Text;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace ConsoleApp1
{
    public static class GenRays
    {
        
        /// <summary>
        /// calculate the ray intersection points
        /// </summary>
        /// <param name="rays">list of rays</param>
        /// <param name="breps">list of breps</param>
        /// <returns></returns>
        public static List<Point3d[]> IntersectionRays(List<Ray3d> rays, List<Brep> breps,out List<Point3d> originpoints,int maxreflection=3)
        {
            var Intersections = new List<Point3d[]>();
            var origins = new List<Point3d>();
            try
            {
                foreach (var ray in rays)
                {
                    var temp = Intersection.RayShoot(ray, breps, maxreflection);
                    if (temp != null)
                    {
                        Intersections.Add(temp);
                        origins.Add(ray.Position);
                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem in IntersectionRays method found");
            }
            originpoints = origins;
            return Intersections;
        }
        /// <summary>
        /// creates list of ray3d objects 
        /// </summary>
        /// <param name="pts">list of base points</param>
        /// <param name="normal">normal</param>
        /// <returns>list of Ray3d objects</returns>
        public static List<Ray3d> FaceRays(List<Point3d> pts, Vector3d normal)
        {
            if(!normal.IsValid || normal.IsZero)
            {
                throw new ArgumentNullException("normal", "normal vector is either zero or unvalid");
            }
            List<Ray3d> RayList = new List<Ray3d>();
            for (int i = 0; i < pts.Count; i++)
            {
                RayList.Add(new Ray3d(pts[i], normal));
            }
            return RayList;
        }

        /// <summary>
        /// gets all corners of the bounding box 
        /// </summary>
        /// <returns>all 8 corners</returns>
        internal static Point3d[] GetAllCorners(Mesh mesh)
        {
            if(!mesh.IsValid )
            {
                throw new ArgumentNullException("mesh", "input mesh is invalid");
            }
            var bdBox = mesh.GetBoundingBox(false);
            return bdBox.GetCorners();
        }      
        /// <summary>
        /// gets all face corners based on the number of Faceindex enum
        /// </summary>
        /// <param name="index"></param>
        /// <returns>4point corners</returns>
        public static List<Point3d> GetFaceCorners(int index,Mesh mesh)
        {
            var FaceCorners = new List<Point3d>();
            var Corners = GetAllCorners(mesh);

            switch ((FaceIndex)index)
            {
                case (FaceIndex.Face1):
                    {
                        FaceCorners.Add(Corners[0]);
                        FaceCorners.Add(Corners[1]);
                        FaceCorners.Add(Corners[4]);
                        FaceCorners.Add(Corners[5]);
                        break;
                    }
                case (FaceIndex.Face2):
                    {
                        FaceCorners.Add(Corners[1]);
                        FaceCorners.Add(Corners[2]);
                        FaceCorners.Add(Corners[5]);
                        FaceCorners.Add(Corners[6]);
                        break;
                    }
                case (FaceIndex.Face3):
                    {
                        FaceCorners.Add(Corners[2]);
                        FaceCorners.Add(Corners[3]);
                        FaceCorners.Add(Corners[6]);
                        FaceCorners.Add(Corners[7]);
                        break;
                    }
                case (FaceIndex.Face4):
                    {
                        FaceCorners.Add(Corners[3]);
                        FaceCorners.Add(Corners[0]);
                        FaceCorners.Add(Corners[7]);
                        FaceCorners.Add(Corners[4]);
                        break;
                    }
                case (FaceIndex.Face5):
                    {
                        FaceCorners.Add(Corners[4]);
                        FaceCorners.Add(Corners[5]);
                        FaceCorners.Add(Corners[7]);
                        FaceCorners.Add(Corners[6]);
                        break;
                    }
                case (FaceIndex.Face6):
                    {
                        FaceCorners.Add(Corners[1]);
                        FaceCorners.Add(Corners[0]);
                        FaceCorners.Add(Corners[2]);
                        FaceCorners.Add(Corners[3]);
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException("index", "index can only be from FaceIndex Enum numbers");
                    }
            }

            return FaceCorners;

        }
        /// <summary>
        /// calculate all the light points of a face
        /// </summary>
        /// <param name="Accuracy">number of light points (x*x)</param>
        /// <param name="face">index of the face</param>
        /// <returns></returns>
        public static List<Point3d> GetFaceLightPoints(int Accuracy, int face,Mesh mesh)
        {
            var FacePoints = new List<Point3d>();        
            var FaceCorners = GetFaceCorners(face,mesh);
            try
            {
                var SegmentAmountxy = Generals.CaclulateSegments(FaceCorners[0], FaceCorners[1], Accuracy);
                var SegmentAmountz = Generals.CaclulateSegments(FaceCorners[0], FaceCorners[2], Accuracy);
                //creating xy vectors
                Vector3d vectorxy = (FaceCorners[1] - FaceCorners[0]);
                vectorxy.Unitize();
                vectorxy *= SegmentAmountxy;
                //creating z vectors
                Vector3d vectorz = (FaceCorners[2] - FaceCorners[0]);
                vectorz.Unitize();
                vectorz *= SegmentAmountz;

                for (int i = 1; i < Accuracy; i++)
                {
                    for (int j = 1; j < Accuracy; j++)
                    {
                        var tempvec = i * vectorxy + j * vectorz;
                        var temppoint = FaceCorners[0] + tempvec;
                        FacePoints.Add(temppoint);

                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with the GetFaceLightPoints method");
            }
            return FacePoints;

        }
       
        
    }
}
