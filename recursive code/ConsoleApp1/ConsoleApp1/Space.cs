using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Geometry;

namespace ConsoleApp1
{
    public class Space
    {
        //variables needed for each house
        public int Type { get; private set; }
        public int Light { get; private set; }
        public double Area { get; private set; }
        public Mesh mesh { get; private set; }
        /// <summary>
        /// ray face objects of each face
        /// </summary>
        public Ray_Face[] rayfaces { get; set; }
        /// <summary>
        /// face index that is recieving light
        /// </summary>
        public int lightface { get; set; }
        /// <summary>
        /// plane of the lightface
        /// </summary>
        public Plane WinFacePlane { get; set; }
        /// <summary>
        /// indexes of faces conectiong to other spaces of the same House object
        /// </summary>
        public List<int> ConnectedFaces { get; set; }
        public Space(int type, int light, double area, Mesh mesh)
        {
            if (area <= 0)
            {
                throw new ArgumentOutOfRangeException("area", "areas should be non negative and greater than 0");
            }
            if (!mesh.IsValid)
            {
                throw new ArgumentNullException("mesh", "input mesh is invalid");
            }
            this.Type = type;
            this.Light = light;
            this.Area = area;
            this.mesh = mesh;
            this.rayfaces = new Ray_Face[6];
            this.ConnectedFaces = new List<int>();

        }

        public void AddConnectedFace(int index)
        {
            ConnectedFaces.Add(index);
        }
        /// <summary>
        /// add a RayFace instance based on the index to the space
        /// </summary>
        /// <param name="rf">RayFace instance</param>
        /// <param name="index"></param>
        public void AddRayFaceFunction(Ray_Face rf, int index)
        {
            if(rf.FaceIndex != index)
            {
                throw new ArgumentException("faceindex does not match the given index","index");
            }
            rayfaces[index] = rf;
        }
        public List<Point3d> GetOriginPoints(int faceindex)
        {
            return rayfaces[faceindex].Face_Rays_Origins;
        }
        
        /// <summary>
        /// gets the number of recieved rays for given face 
        /// </summary>
        /// <param name="facenumber"></param>
        /// <returns></returns>
        public int EachFaceLightCount(int facenumber)
        {
            var temp = rayfaces[facenumber];
            if (temp != null)
            {
                return rayfaces[facenumber].Count();
            }
            else
            {
                Console.WriteLine("no rays was added to this face");
                return 0;
            }
        }
        /// <summary>
        /// create plains for a face(usually used for windows)
        /// </summary>
        /// <param name="faceindex">index of face </param>
        /// <returns>the plane</returns>
        public Plane CreateFacePlane(int faceindex)
        {
            Plane plane = Plane.Unset;
            try
            {
                List<Point3d> corners = rayfaces[faceindex].FaceCorners;
                Point3d center = Generals.AvaragePointCalculator(corners);
                Point3d yaxis = (corners[2] + corners[3]) / 2;
                var xaxis = (corners[1] + corners[3]) / 2;
                plane = new Plane(center, xaxis, yaxis);
                if (!plane.IsValid)
                {
                    throw new ArgumentNullException("built plain is null");
                }
            }
            catch(Exception)
            {

            }
                      
            return plane;
        }
        /// <summary>
        /// checks if the face satisfy the light condition
        /// </summary>
        /// <param name="accuracy">the parameter from GenLight class fields</param>
        /// <param name="conditionlist">the list of conditions for light</param>
        /// <returns>true if the light condition is satisfied</returns>
        public bool LightCheck(int accuracy, List<string> conditionlist,double tolerance=0.5)
        {   
            if (Light == 0)
            {
                return true;
            }
            else 
            {
                accuracy--;
                int[] templist = Generals.GetString(conditionlist[Light-1]);
                for (int i = 0; i < templist.Length; i++)
                {
                    
                    if (EachFaceLightCount(templist[i]) >= (accuracy * accuracy) * tolerance)
                    {
                        WinFacePlane = CreateFacePlane(templist[i]);
                        lightface = templist[i];
                        return true;
                    }

                }
                return false;

            }

        }

    }

}

