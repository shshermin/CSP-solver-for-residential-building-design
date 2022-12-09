using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Geometry;

namespace ConsoleApp1
{
    /// <summary>
    /// keeps specific data anf functions related to a single face
    /// </summary>
    public class Ray_Face
    {
        /// <summary>
        /// gives the interssection points of light surfaces and rays of the face
        /// </summary>
        public List<Point3d> Srf_Intersections { get; private set; }
        /// <summary>
        /// gives the origin of successful ray intersections
        /// </summary>
        public List<Point3d> Face_Rays_Origins { get; private set; }
        /// <summary>
        /// gets the corners of the face
        /// </summary>
        public List<Point3d> FaceCorners { get; private set; }
        /// <summary>
        /// gets the index of the face
        /// </summary>
        public int FaceIndex { get; private set; }


        public Ray_Face(List<Point3d> srf_Intersection_Counts,List<Point3d> originpoints, List<Point3d> facecorners,int faceindex)
        {
            this.Srf_Intersections = srf_Intersection_Counts;
            this.FaceCorners = facecorners;
            this.FaceIndex = faceindex;
            this.Face_Rays_Origins = originpoints;
        }
        /// <summary>
        /// gets the count of the intersection points
        /// </summary>
        /// <returns>count</returns>
        public int Count()
        {
            return Srf_Intersections.Count;
        }
    }
}
