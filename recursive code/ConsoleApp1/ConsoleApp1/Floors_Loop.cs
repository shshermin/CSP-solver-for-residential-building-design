using System;
using System.Collections.Generic;
using System.Text;
using Rhino;
using Rhino.Geometry;

namespace ConsoleApp1
{
    public class Floors_Loop
    {
        public int Accuracy { get; set; }
        public List<Brep> AdditionalBreps { get; set; }
        public List<List<Point3d>> EnteranceList { get; set; }
        public List<House> Houses { get; set; }
        public List<string> LightConditions { get; set; }
        public List<Surface> LightSurfaces { get; set; }
        public List<Vector3d> NormalList { get; set; }
        public List<double> PrivecyOptions { get; set; }
        public List<int> TempNumbers { get; set; }
        public List<Point3d> UnWantedPts { get; set; }

        private readonly GenLight _GeneralLight;
        public bool PrivecyButton { get; set; }
        public bool LightButton { get; set; }
        public Floors_Loop(int accuracy, List<Brep> additionalBreps, List<List<Point3d>> enteranceList, List<House> houses,
                            bool lightButton, List<string> lightConditions, List<Surface> lightSurfaces, List<Vector3d> normalList,
                            bool privecyButton, List<double> privecyOptions, List<int> tempNumbers, List<Point3d> unWantedPts)
        {
            this.Accuracy = accuracy;
            this.AdditionalBreps = additionalBreps;
            this.EnteranceList = enteranceList;
            this.Houses = houses;
            this.LightButton = lightButton;
            this.LightConditions = lightConditions;
            this.LightSurfaces = lightSurfaces;
            this.NormalList = normalList;
            this.PrivecyButton = privecyButton;
            this.PrivecyOptions = privecyOptions;
            this.TempNumbers = tempNumbers;
            this.UnWantedPts = unWantedPts;
            this._GeneralLight = new GenLight(Accuracy, LightConditions, normalList, LightSurfaces);
                
        }
     

        /// <summary>
        /// Main Loop Function
        /// </summary>
        /// <param name="TedadVahed">Number of Houses in each floor</param>
        /// <param name="floor">the index of floor</param>
        /// <param name="FinaleList">List of Houses completed</param>
        /// <returns>All Possible alternatives to the floor</returns>
        public  List<AllHouses> Tabaghat(int TedadVahed, int floor, List<AllHouses> FinaleList)
        {
            var FinaleListTemp = new List<AllHouses>();
            if (floor == 0)
            {
                FinaleList.Add(new AllHouses(UnWantedPts));
            }
            
            for (int m = 0; m < FinaleList.Count; m++)
            {
                List<List<int>> CombinationHouseIndexList = Generals.PermutationToList(TempNumbers, TedadVahed, "Combination");

                List<int> EnteranceIndex = GenLoop.GetEnteranceIndexList(EnteranceList[floor]);
                var copyEnteranceIndex = Generals.CopyListOfPoints(EnteranceIndex);
                //check if any of the enterancepoint are in unwanted pts
                for (int i = EnteranceIndex.Count-1; i >-1 ; i--)
                {                                          
                     bool Check1 = Generals.CheckDupPoinInList(EnteranceList[floor][i], FinaleList[m].pointcheck);
                    
                    if (Check1 == false)
                    {
                        EnteranceIndex.RemoveAt(EnteranceIndex[i]);
                    }
                }

                
                foreach (var houseindexes in CombinationHouseIndexList)
                {
                    //building enterancepointpermutations
                    List<List<int>> PermutationEnteranceIndexList = Generals.PermutationToList(EnteranceIndex, TedadVahed, "Permutation");
                    PermutationEnteranceIndexList = GenLoop.RemoveDupObjects(copyEnteranceIndex, houseindexes, PermutationEnteranceIndexList);


                    //add chosen houses to a list based on their indexes
                    var listhouses = new List<House>();
                    for (int t = 0; t < TedadVahed; t++)
                    {
                        listhouses.Add(Houses[houseindexes[t]]);
                    }

                    

                    for (int j = 0; j < PermutationEnteranceIndexList.Count; j++)
                    {
                        bool CHECK = true;
                        var VecList = new List<Vector3d>();
                        //copying PointCheckList
                        var PoinCheckCopy = new List<Point3d>();
                        foreach (var pt in FinaleList[m].pointcheck)
                        {
                            PoinCheckCopy.Add(pt);
                        }
                        //////////////////////////////////////////////////////
                        //building move vectors and checking if they overlap
                        //////////////////////////////////////////////////////
                        ///
                        for (int k = 0; k < listhouses.Count; k++)
                        {

                            Point3d EnteranceCenter = listhouses[k].GetEnteranceCenter();

                            Point3d destPoint = EnteranceList[floor][PermutationEnteranceIndexList[j][k]];
                            // now we create the vec for move
                            Vector3d vec = new Vector3d(destPoint - EnteranceCenter);
                            VecList.Add(vec);

                            //getting all center points to check
                            List<Point3d> Check = listhouses[k].GetAllCenters();
                            List<Point3d> CkeckList = Generals.AddPointWithVector(vec, Check);

                            //check if they are in pointcheck list
                            bool CHECKDup = Generals.CheckForDuplicatePoints(CkeckList, PoinCheckCopy);
                            if (CHECKDup == true)
                            {
                                foreach (var pt in CkeckList)
                                {
                                    PoinCheckCopy.Add(pt);
                                }
                            }
                            else
                            {
                                CHECK = false;
                                

                            }
                        }

                        ////////////////////////////////////////////
                        //moving
                        ////////////////////////////////////////////
                        if (CHECK == true)
                        {

                            List<List<House>> PreviousHousesList = new List<List<House>>();
                            var MovedList = new List<House>();
                            for (int t = 0; t < TedadVahed; t++)
                            {
                                MovedList.Add((House.HouseMove(VecList[t], listhouses[t])));
                            }

                            for (int c = 0; c < floor; c++)
                            {
                                PreviousHousesList.Add(FinaleList[m].Floors[c]);
                            }
                            PreviousHousesList.Add(MovedList);


                            List<Brep> AllBreps = AllHouses.GetAllHouseBreps(PreviousHousesList, LightSurfaces, AdditionalBreps);

                            /////////////////////////////////////
                            ///Conditions
                            /////////////////////////////////////
                            
                            if (LightButton == false && PrivecyButton == true)
                            {
                                throw new ArgumentException("if you want the privecy condition , light condition must be on too ");
                            }
                            try
                            {
                                var templight = new Light(LightButton, PreviousHousesList, _GeneralLight, AllBreps, LightConditions);
                                var tempprivecy = new Privecy(PrivecyButton, PreviousHousesList, PrivecyOptions);
                                var ListConditions = new List<Condition>() { templight, tempprivecy };
                                var conditionCheck = true;
                                foreach (var condition in ListConditions)
                                {
                                    var tempcheck = condition.ExecuteCondition();
                                    if (tempcheck == false)
                                    {
                                        conditionCheck = false;
                                        break;
                                    }
                                }
                                if (conditionCheck)
                                {
                                    var AllHouseTemp = new AllHouses(PoinCheckCopy);
                                    AllHouseTemp.AssignFloor(PreviousHousesList);
                                    FinaleListTemp.Add(AllHouseTemp);
                                }
                            }
                            catch (Exception)
                            {
                                RhinoApp.WriteLine("problem with Conditions part of Tabaghat class");
                            }

                            

                        }

                    }
                }

            }

            return FinaleListTemp;
        }
    }
}
