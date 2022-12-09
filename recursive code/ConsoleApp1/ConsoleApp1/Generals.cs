using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Geometry;
using System.Linq;
using Rhino;

namespace ConsoleApp1
{
  

    public static class Generals
    {
        /// <summary>
        /// creats a deep copy of a list of points
        /// </summary>
        /// <param name="list">list of points</param>
        /// <returns>deep copy</returns>
        public static List<int> CopyListOfPoints(List<int> list)
        {
            List<int> Copy = new List<int>();
            for(int i =0;i< list.Count;i++)
            {
                Copy.Add(list[i]);
            }
            return Copy;
        }
        /// <summary>
        /// removing unwanted points in a brep
        /// </summary>
        /// <param name="ptlist">grid points</param>
        /// <param name="brp">brep</param>
        /// <param name="tel">tolerance</param>
        /// <returns></returns>
        public static List<Point3d> CheckPointINBrep(List<Point3d> ptlist, Brep brp, double tel)
        {           
            if(!brp.IsValid)
            {
                throw new ArgumentNullException("brep", "brep is not valid");
            }
            if(ptlist.Count == 0)
            {
                throw new ArgumentNullException("Point list", "The list is empty");
            }
            var unwantedpts = new List<Point3d>();
            for (int i = 0; i < ptlist.Count; i++)
                {
                    if (brp.IsPointInside(ptlist[i], tel, false))
                    {
                        unwantedpts.Add(ptlist[i]);
                    }
                }                         
            return unwantedpts;
        }
      
        /// <summary>
        /// finds the avarage point between a list of points
        /// </summary>
        /// <param name="pts">list of points</param>
        /// <returns></returns>
        public static Point3d AvaragePointCalculator(List<Point3d> pts)
        {
            if(pts.Count == 0)
            {
                RhinoApp.WriteLine("the avaragecalculator only recieved one point");
            }
            var sum = new Point3d(0, 0, 0);
            for (int i = 0; i < pts.Count; i++)
            {
                sum += pts[i];
            }
            return sum /(pts.Count);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static double CaclulateSegments(Point3d k1, Point3d k2, int accuracy)
        {   if(accuracy == 1)
            {
                RhinoApp.WriteLine("beware that no division will happen");
            }
            if(accuracy <= 0)
            {
                throw new ArgumentOutOfRangeException("accuracy", "accuracy must be geater than 0 otherwise zero division or othe exceptions might happen");
            }
            if(!k1.IsValid || !k2.IsValid)
            {
                throw new ArgumentNullException("point", "unvalid point was given");
            }
            var temp = DistanceBetweenPoints(k2, k1) / accuracy;
            return temp;
        }

        /// <summary>
        /// calculates the 2d(xy plane) distance between points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double DistanceBetweenPoints2D(Point3d a, Point3d b)
        {
            if( !a.IsValid || !b.IsValid)
            {
                throw new ArgumentNullException("invalid point detected");
            }
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            //var dz = a.Z - b.Z;
            // calculate the distance
            var distance = Math.Sqrt(dx * dx + dy * dy);
            return distance;
        }
        /// <summary>
        /// calculate the distance between 2 points
        /// </summary>
        /// <param name="a">first point</param>
        /// <param name="b">second point</param>
        /// <returns>the distance</returns>
        public static double DistanceBetweenPoints(Point3d a, Point3d b)
        {   if(!a.IsValid || !b.IsValid)
            {
                throw new ArgumentNullException("point", "unvalid point was given");
            }
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            var dz = a.Z - b.Z;
            // calculate the distance
            var distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return distance;
        }
        /// <summary>
        /// move a point with a vector
        /// </summary>
        /// <param name="vec">the vector</param>
        /// <param name="pts">the point</param>
        /// <returns>moved point</returns>
        public static List<Point3d> AddPointWithVector(Vector3d vec, List<Point3d> pts)
        {
            if(!vec.IsValid )
            {
                throw new ArgumentNullException("vector", "given vector was not valid");
            }
            if(vec.IsZero)
            {
                throw new ArgumentException("the vector is zero !");
            }
            if(pts.Count == 0)
            {
                throw new ArgumentException("list of points is empty");
            }
            var ListCheck = new List<Point3d>();
            foreach (var pt in pts)
            {
                var temp = pt + vec;
                ListCheck.Add(temp);
            }
            return ListCheck;
        }
        /// <summary>
        /// checks if there are any duplicates in a list of points
        /// </summary>
        /// <param name="ptsToCheck">first list</param>
        /// <param name="CheckList">second list</param>
        /// <returns></returns>
        public static bool CheckForDuplicatePoints(List<Point3d> ptsToCheck, List<Point3d> CheckList)
        {
            var templist = new List<bool>();
            for (int i = 0;i< ptsToCheck.Count;i++)
            {
                for(int j = 0;j< CheckList.Count;j++)
                {
                    if (DistanceBetweenPoints(ptsToCheck[i], CheckList[j]) < 0.2)
                    {
                        templist.Add(false);
                    }
                }
            }
            
            if (templist.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// remove points that have same x and y (change the main list)
        /// </summary>
        /// <param name="pts">list of points</param>
        /// <returns> corrected list </returns>
        public static List<Point3d> RemoveDuplicate2D(List<Point3d> pts)
        {
            for (int i = 0; i < pts.Count; i++)
            {
                for (int j = pts.Count - 1; j > i; j--)
                {
                    if (Generals.DistanceBetweenPoints2D(pts[i], pts[j]) < 0.1)
                    {
                        pts.RemoveAt(j);
                    }
                }
            }
            return pts;
        }

        /// <summary>
        /// gets the permutation of a list for example for (1,2,3) --> (1,1),(1,2),(1,3),(2,1),(2,2),(2,3),(3,1),(3,2),(3,3)
        /// </summary>
        /// <param name="list">list of objects</param>
        /// <param name="length">number of objects in each permutation</param>
        /// <returns>all permutations</returns>
        private static IEnumerable<IEnumerable<int>> GetPermutationsWithRept(IEnumerable<int> list, int length)
        {
            if (length == 1)
            {
                return list.Select(t => new int[] { t });
            }
            return GetPermutationsWithRept(list, length - 1)
              .SelectMany(t => list,
              (t1, t2) => t1.Concat(new int[] { t2 }));
        }
        /// <summary>
        /// gets the permutation of a list without repetition for example for (1,2,3) --> (1,2),(1,3),(2,1),(2,3),(3,1),(3,2)
        /// </summary>
        /// <param name="list">list of objects</param>
        /// <param name="length">number of objects in each permutation</param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<int>>GetPermutations(IEnumerable<int> list, int length)
        {
            if (length == 1) return list.Select(t => new int[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new int[] { t2 }));
        }
        /// <summary>
        /// gets combinations for example for (1,2,3) --> (1,1),(1,2),(1,3),(2,2),(2,3),(3,3)
        /// </summary>
        /// <param name="list">list of objects</param>
        /// <param name="length">number of objects in each combination</param>
        /// <returns>combinations</returns>
        private static IEnumerable<IEnumerable<int>> GetKCombsWithRept(IEnumerable<int> list, int length)
        {
            if (length == 1) return list.Select(t => new int[] { t });
            {
                return GetKCombsWithRept(list, length - 1).SelectMany(t => list.Where(o => o.CompareTo(t.Last()) >= 0),
                  (t1, t2) => t1.Concat(new int[] { t2 }));
            }
        }
        /// <summary>
        /// gets permutation or combination and converts IEnumerations to list
        /// </summary>
        /// <param name="input">raw list</param>
        /// <param name="len">length of each part</param>
        /// <param name="perorcomb">"Combination" or "Permutation"</param>
        /// <returns></returns>
        public static List<List<int>> PermutationToList(List<int> input, int len, string perorcomb)
        {
            var tempnahayee = new List<List<int>>();
            try
            {


                IEnumerable<IEnumerable<int>> temp;
                if (perorcomb == "Permutation")
                {
                    temp = GetPermutations(input, len);
                }
                else if (perorcomb == "Combination")
                {
                    temp = GetKCombsWithRept(input, len);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("perorcomb", "input can only be Permutation or Combination");
                }

                var tempToList = temp.ToList();

                for (int i = 0; i < tempToList.Count; i++)
                {
                    tempnahayee.Add(tempToList[i].ToList());
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with PermutationToList function");
            }
            return tempnahayee;
        }
        /// <summary>
        /// checks if a point is in a list of points
        /// </summary>
        /// <param name="pt">the point</param>
        /// <param name="inputlist">the list to check</param>
        /// <returns>true if the point is not in the list</returns>
        public static bool CheckDupPoinInList(Point3d pt, List<Point3d> inputlist, double tolerance= 0.1)
        {
            if(!pt.IsValid)
            {
                throw new ArgumentNullException("point", "given point is not valid");
            }
            if(inputlist.Count == 0)
            {
                return true;
            }
            foreach (var ptcheck in inputlist)
            {
                if (DistanceBetweenPoints(pt, ptcheck) < tolerance)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// from a list checks if points are on the list of surfaces
        /// </summary>
        /// <param name="inputpoints">array of intersection points</param>
        /// <param name="srfs">list of surfaces</param>
        /// <param name="tolerance">the tolerance of near points to surface</param>
        /// <returns>points that where on the surface</returns>
        public static List<Point3d> CheckToBeOnSurface(List<Point3d[]> inputpoints, List<Point3d> inputorigins, List<Surface> srfs, out List<Point3d> outputorigins, double tolerance = 0.5)
        {
            for (int i = 0;i<srfs.Count;i++)
            {
                if(!srfs[i].IsValid)
                {
                    throw new ArgumentException("unvalid surface detected");
                }
            }
            var OutPutOringins = new List<Point3d>(); ;
            var OutPutList = new List<Point3d>();
            try
            {
                for (int i = 0; i < inputpoints.Count; i++)
                {
                    for (int j = 0; j < inputpoints[i].Length; j++)
                    {
                        foreach (var srf in srfs)
                        {
                            double u;
                            double v;
                            var test = srf.ClosestPoint(inputpoints[i][j], out u, out v);
                            var temppt = srf.PointAt(u, v);
                            var dist = temppt.DistanceTo(inputpoints[i][j]);
                            if (dist < tolerance)
                            {
                                OutPutList.Add(inputpoints[i][j]);
                                OutPutOringins.Add(inputorigins[i]);
                            }
                        }

                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with CheckToBeOnSurface function");
            }
            outputorigins = OutPutOringins;
            return OutPutList;

        }
       
        /// <summary>
        /// calculates the normal of a face 
        /// </summary>
        /// <param name="corners">corners of the face</param>
        /// <returns>the normal</returns>
        internal static Vector3d CalculateNormalOfFace(List<Point3d> corners)
        {   for(int i = 0;i<corners.Count;i++)
            {
                if(!corners[i].IsValid)
                {
                    throw new ArgumentNullException("corners", "invalid point detected");
                }
            }
            Vector3d a = corners[1] - corners[0];
            Vector3d b = corners[2] - corners[0];
            var result = Vector3d.CrossProduct(a, b);
            return result;
        }
        /// <summary>
        /// flatten a list of arrays
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns>flattend list</returns>
        public static List<Point3d> FlattenArray(List<Point3d[]> arrays)
        {
            var list = new List<Point3d>();
            try
            {
                for (int i = 0; i < arrays.Count; i++)
                {
                    foreach (var arry in arrays[i])
                    {
                        list.Add(arry);
                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with FlattenArray function");
            }
            return list;
        }
        public static List<Point3d> Flatten(List<List<Point3d>> list)
        {            
            var listflatten = new List<Point3d>();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list[i].Count; j++)
                    {
                        listflatten.Add(list[i][j]);
                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with Flatten function ");
            }
            return listflatten;           
        }
        /// <summary>
        /// gets a string with the format int,int,... and convert it to a list of integer
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="len">lenght of each output array</param>
        /// <returns>ineger array</returns>
        public static int[] GetString(string str)
        {  //making sure the input is correct
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException("input string", "input srting is null or whitespace");
            }
            int[] arrayint = new int[0];
            try
            {
                int[] result = new int[1];
                int res;
                if (int.TryParse(str, out res))
                {
                    result[0] = res;
                    return result;
                }


                string[] temp = str.Split(',');
                arrayint = new int[temp.Length];

                for (int i = 0; i < temp.Length; i++)
                {
                    try
                    {
                        int p = int.Parse(temp[i]);
                        arrayint[i] = p;
                    }
                    catch
                    {
                        throw new ArgumentOutOfRangeException("input string", "string inputs are not convertable to integers.");
                    }
                }
            }
            catch (InvalidCastException)
            {
                Console.WriteLine("wrong input string cast");
            }
            catch (Exception)
            {
                Console.WriteLine("wrong input string");
            }

            return arrayint;
        }
        /// <summary>
        /// checks if a number is in a list of numbers
        /// </summary>
        /// <param name="list">list of numbers</param>
        /// <param name="check">number to check</param>
        /// <returns>true if the number is not in the list</returns>
        public static bool CheckDupIntInList(List<int> list,int check)
        {
            
            for(int i =0;i< list.Count;i++)
            {
                if(Math.Abs(check-list[i])<0.001)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
