using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino;
using Rhino.Geometry;

namespace ConsoleApp1
{
    static class GenLoop
    {
        /// <summary>
        /// creats a list of integers for House List indexes
        /// </summary>
        /// <param name="houses">list of houses</param>
        /// <returns>indexes</returns>
        public static List<int> GetHouseNumbersForPermutation(List<House> houses)
        {
            var TempNumbers = new List<int>();
            var it = 0;
            foreach (var a in houses)
            {
                TempNumbers.Add(it);
                it++;
            }
            return TempNumbers;
        }
        /// <summary>
        /// builds the list of point indices
        /// </summary>
        /// <param name="enterance_points_of_a_floor">list of points</param>
        /// <returns>list of indices</returns>
        public static List<int> GetEnteranceIndexList(List<Point3d> enterance_points_of_a_floor)
        {
            var indexlist = new List<int>();
            var count = enterance_points_of_a_floor.Count;
            for(int i =0; i< count;i++)
            {
                indexlist.Add(i);
            }
            return indexlist;

        }
        /// <summary>
        /// removes the alternatives that are simillar 
        /// </summary>
        /// <param name="Enteranceindexes">the initial array</param>
        /// <param name="Combination">list of combinations</param>
        /// <param name="Permutation">list of permutations</param>
        /// <returns></returns>
        public static List<List<int>> RemoveDupObjects(List<int> Enteranceindexes, List<int> Combination, List<List<int>> Permutation)
        {
            
            if(Enteranceindexes == null || Combination == null || Permutation == null)
            {
                throw new ArgumentNullException("input", "one of the inputs is null");
            }
            var result = new List<int[]>();
            try
            {
                for (int j = Permutation.Count - 1; j > -1; j--)
                {
                    var temp = ArrayWithDefaulValue(Enteranceindexes, -1);
                    for (int k = 0; k < Permutation[j].Count; k++)
                    {
                        temp[Permutation[j][k]] = Combination[k];
                        if (!CheckDupArray(temp, result))
                        {
                            Permutation.RemoveAt(j);

                        }
                    }
                    result.Add(temp);
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with  RemoveDupObjects method");
            }
            
            return Permutation;

        }
        /// <summary>
        /// builds an array with a default value
        /// </summary>
        /// <param name="listtoclone"></param>
        /// <param name="defaultvalue">default value</param>
        /// <returns>the array</returns>
        public static int[] ArrayWithDefaulValue(List<int> listtoclone,int defaultvalue)
        {

            var EmptyArray = new int[listtoclone.Count];
            try
            {
                for (int i = 0; i < EmptyArray.Length; i++)
                {
                    EmptyArray[i] = defaultvalue;
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with ArrayWithDefaulValue method");
            }
            return EmptyArray;
        }
        /// <summary>
        /// true if the array is not in the list of arrays
        /// </summary>
        /// <param name="arraytocheck">single array to check</param>
        /// <param name="listofarrays">list of arrays</param>
        /// <returns></returns>
        public static bool CheckDupArray(int[] arraytocheck,List<int[]> listofarrays)
        {
            try
            {
                for (int i = 0; i < listofarrays.Count; i++)
                {
                    if (checkEquality(arraytocheck, listofarrays[i]))
                    {
                        return false;
                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with CheckDupArray method");
            }
            return true;
        }
        /// <summary>
        /// if true both arrays are equal
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool checkEquality (int[] first, int[] second)
        {
            return Enumerable.SequenceEqual(first, second);
        }

    }
}
