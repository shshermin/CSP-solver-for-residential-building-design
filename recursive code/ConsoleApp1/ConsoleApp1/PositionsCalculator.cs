using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace ConsoleApp1
{
    class PositionsCalculator
    {
        /// <summary>
        /// temp numbers are the indexes of houses in each floor
        /// </summary>
        public List<List<int>> TempNumbers { get; set; }
        /// <summary>
        /// enterance list is the list of enterance points in each floor
        /// </summary>
        public List<List<Point3d>> EnteranceList { get; set; }

        public PositionsCalculator(List<List<Point3d>> enteranceList, List<List<int>> tempNumbers)
        {
            this.EnteranceList = enteranceList;
            this.TempNumbers = tempNumbers;
        }


        public List<AllPositions> AllPositions(int floor,List<int> tedadvehed)
        {
            
            var finallistineachfloor = new List<List<MoveCodes>>();
            var FinalAllPossiblePositions = new List<AllPositions>();
            //calculating all the floors seperately
            for (int i = 0;i<floor;i++)
            {
                finallistineachfloor.Add(HousePositionCalculator(tedadvehed[i], i));
            }
            //recursive loop
            var array = new MoveCodes[floor];
            Set(array, 0, floor, finallistineachfloor,ref FinalAllPossiblePositions);
            return FinalAllPossiblePositions;
            
        }
        public List<MoveCodes> HousePositionCalculator(int TedadVahed, int floor)
        {
            var List_MoveCodes = new List<MoveCodes>();
            List<List<int>> CombinationHouseIndexList = Generals.PermutationToList(TempNumbers[floor], TedadVahed, "Combination");
            List<int> EnteranceIndex = GenLoop.GetEnteranceIndexList(EnteranceList[floor]);
            var copyEnteranceIndex = Generals.CopyListOfPoints(EnteranceIndex);

            foreach (var houseindexes in CombinationHouseIndexList)
            {
                //building enterancepointpermutations
                List<List<int>> PermutationEnteranceIndexList = Generals.PermutationToList(EnteranceIndex, TedadVahed, "Permutation");
                PermutationEnteranceIndexList = GenLoop.RemoveDupObjects(copyEnteranceIndex, houseindexes, PermutationEnteranceIndexList);
                for(int i = 0;i< PermutationEnteranceIndexList.Count;i++ )
                {
                    MoveCodes Temp = new MoveCodes(floor+1);
                    Temp.AddHouses(houseindexes);
                    Temp.AddPlaces(PermutationEnteranceIndexList[i]);
                    List_MoveCodes.Add(Temp);
                }

            }

            return List_MoveCodes;
        }

        public static void Set(MoveCodes[] array, int index, int floor,List<List<MoveCodes>> refrencelist , ref List<AllPositions> final)
        {
            if (index == floor)
            {
                var Listtemp = new AllPositions();
                foreach (var item in array)
                {
                    //Console.Write(item.ToString());
                    //Console.Write(Environment.NewLine);
                    Listtemp.Add(item);
                }
                final.Add(Listtemp);
                return;
            }
            for (int i = 0; i < refrencelist[0].Count ; i++)
            {
                array[index] = refrencelist[0][i];
                Set(array, index + 1, floor, refrencelist.Skip(1).ToList(), ref final);
            }

        }



    }
}
