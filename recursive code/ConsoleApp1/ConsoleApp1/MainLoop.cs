using System;
using System.Collections.Generic;
using System.Text;
using Rhino;
using Rhino.Geometry;

namespace ConsoleApp1
{
    class MainLoop
    {
            public int Accuracy { get; set; }
            public List<Brep> AdditionalBreps { get; set; }
            public List<List<Point3d>> EnteranceList { get; set; }
            public List<House> Houses { get; set; }
            public List<string> LightConditions { get; set; }
            public List<Surface> LightSurfaces { get; set; }
            public List<Vector3d> NormalList { get; set; }
            public List<double> PrivecyOptions { get; set; }
            public List<List<int>> TempNumbers { get; set; }
            public List<int> TedadVahed { get; set; }
            public List<Point3d> UnWantedPts { get; set; }

            private readonly GenLight _GeneralLight;
            public bool PrivecyButton { get; set; }
            public bool LightButton { get; set; }
            public MainLoop(int accuracy, List<Brep> additionalBreps, List<List<Point3d>> enteranceList, List<House> houses,
                                bool lightButton, List<string> lightConditions, List<Surface> lightSurfaces, List<Vector3d> normalList,
                                bool privecyButton, List<double> privecyOptions, List<List<int>> tempNumbers,List<int> tedadvahed, List<Point3d> unWantedPts)
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
                this.TedadVahed = tedadvahed;
                this.UnWantedPts = unWantedPts;
                this._GeneralLight = new GenLight(Accuracy, LightConditions, normalList, LightSurfaces);

            }

        public List<AllHouses> Main_Loop(int floor)
        {
            var positionclass = new PositionsCalculator(EnteranceList, TempNumbers);
            List<AllPositions> allpositionslist = positionclass.AllPositions(floor, TedadVahed);
            var finallist = new List<AllHouses>();
            for (int i= 0; i< allpositionslist.Count ; i++)
            {   if (allpositionslist[i].Check)
                {
                    int floorcount;
                    AllHouses temphouse = Main_Loop_Calculator(allpositionslist[i].Positions, out floorcount);
                    if (temphouse != null)
                    {
                        finallist.Add(temphouse);
                    }
                    else
                    {
                        MoveCodes tempmodeclass = allpositionslist[i].Positions[floorcount - 1];
                        for (int j = i; j < allpositionslist.Count; j++)
                        {   if (allpositionslist[j].Check != false)
                            {
                                AllPositions.MainChecker(allpositionslist[i], allpositionslist[j], floorcount - 1);
                            }
                        }
                    }
                }
                
            }
            return finallist;
        }
        public AllHouses Main_Loop_Calculator( List<MoveCodes> positionList, out int floorcount)
        {
            List<List<House>> HousesList = new List<List<House>>();
            //copying PointCheckList to make sure the main list remains intact
            var PointCheckCopy = new List<Point3d>();
            foreach (var pt in UnWantedPts)
            {
                PointCheckCopy.Add(pt);
            }
            var AllHouseTemp = new AllHouses(PointCheckCopy);
            //iterate through poositions of floors
            floorcount = 0;
            for (int i = 0; i < positionList.Count; i++)
            {
                floorcount = i+1;
                //adding chosen houses to a group
                var listhouses = new List<House>();                
                var houseindextemp = positionList[i].HouseCodes;
                for(int j = 0; j< houseindextemp.Count; j++)
                { 
                    listhouses.Add(Houses[houseindextemp[j]]);
                }


                var positionindextemp = positionList[i].PlaceCode;
                var VecList = new List<Vector3d>();
                for (int j = 0; j < positionindextemp.Count; j++)
                {

                    //////////////////////////////////////////////////////
                    //building move vectors and checking if they overlap
                    //////////////////////////////////////////////////////
                    ///
                    //enterance module's center point
                    Point3d EnteranceCenter = listhouses[j].GetEnteranceCenter();
                    //entrance module's position
                    Point3d destPoint = EnteranceList[i][positionindextemp[j]];
                    // now we create the vec for move
                    Vector3d vec = new Vector3d(destPoint - EnteranceCenter);
                    VecList.Add(vec);

                    //getting all center points to check
                    List<Point3d> Check = listhouses[j].GetAllCenters();
                    List<Point3d> CkeckList = Generals.AddPointWithVector(vec, Check);

                    //check if they are in pointcheck list
                    bool CHECKDup = Generals.CheckForDuplicatePoints(CkeckList, PointCheckCopy);
                    if (CHECKDup == true)
                    {
                        foreach (var pt in CkeckList)
                        {
                            PointCheckCopy.Add(pt);
                        }
                    }
                    else
                    {
                        
                        return null;
                    }
                }  
                    ////////////////////////////////////////////
                    //moving
                    ////////////////////////////////////////////
                        //moving houses with vectors                
                        var MovedList = new List<House>();
                        for (int t = 0; t < listhouses.Count; t++)
                        {
                            MovedList.Add((House.HouseMove(VecList[t], listhouses[t])));
                        }
                        //adding the floor list of houses to the main list
                        HousesList.Add(MovedList);

                        /////////////////////////////////////
                        ///Conditions
                        /////////////////////////////////////
                        List<Brep> AllBreps = AllHouses.GetAllHouseBreps(HousesList, LightSurfaces, AdditionalBreps);
                        //inorder for privacy condition to work light condition must be executed first
                        if (LightButton == false && PrivecyButton == true)
                        {
                            throw new ArgumentException("if you want the privecy condition , light condition must be on too ");
                        }
                        try
                        {
                            var templight = new Light(LightButton, HousesList, _GeneralLight, AllBreps, LightConditions);
                            var tempprivecy = new Privecy(PrivecyButton, HousesList, PrivecyOptions);
                            var ListConditions = new List<Condition>() { templight, tempprivecy };
                            var conditionCheck = true;
                            foreach (var condition in ListConditions)
                            {
                                var tempcheck = condition.ExecuteCondition();
                                if (tempcheck == false)
                                {
                                    conditionCheck = false;
                                    
                                    return null;
                                }
                            }
                            if (conditionCheck)
                            {                               
                                AllHouseTemp.Floors.Add(MovedList);
                            }
                        }
                        catch (Exception)
                        {
                            RhinoApp.WriteLine("problem with Conditions part of Tabaghat class");
                        }

                    
                
            }

            return AllHouseTemp;

        }

    }
}
