using System;
using System.Collections.Generic;
using System.Text;
using Rhino;
using Rhino.Geometry;

namespace ConsoleApp1
{
    class Privecy : Condition
    {
        public List<double> PrivecyInfo { get; set; }
        public Privecy(bool button,List<List<House>> PreviousHousesList, List<double> privecy)
            :base(PreviousHousesList)
        {
            this.Button = button;
            this.PrivecyInfo = privecy;
        }
       
        public  override bool ExecuteCondition( )
        {
            if(Button == false)
            {
                return true;
            }
            else
            {
                List<Space> spacelist = AddAllSpaces();
                var conditionfinal = Conditions(spacelist, PrivecyInfo);
                return conditionfinal;
            }
               
               
        }
        private  List<Space> AddAllSpaces()
        {
            var tempspacelist = new List<Space>();
            try
            {
                for (int i = 0; i < PreviousHousesList.Count; i++)
                {
                    for (int ja = 0; ja < PreviousHousesList[i].Count; ja++)
                    {
                        for (int k = 0; k < PreviousHousesList[i][ja].Each_House.Count; k++)
                        {
                            if (PreviousHousesList[i][ja].Each_House[k].Light != 0)
                            {
                                tempspacelist.Add(PreviousHousesList[i][ja].Each_House[k]);
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with AddAllSpaces method in Privecy class");
            }
            return tempspacelist;

        }
        private bool Conditions(List<Space> tempspacelist,List<double> privecy)
        {
            bool ShartPrivecy = true;
            try
            {
                for (int i = 0; i < tempspacelist.Count; i++)
                {
                    for (int ja = i + 1; ja < tempspacelist.Count; ja++)
                    {
                        bool shartbool = PrivecyCheck(tempspacelist[i], tempspacelist[ja], privecy[0], privecy[1]);
                        if (shartbool == false)
                        {
                            ShartPrivecy = false;
                        }

                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with Conditions method in privecy class");
            }
                return ShartPrivecy;
        }

        /// <summary>
        /// checks if privecy condition works for 2 spaces
        /// </summary>
        /// <param name="a">first space</param>
        /// <param name="b">second space</param>
        /// <param name="angle">limit angle</param>
        /// <param name="distance">limit distance</param>
        /// <returns>true if the condition has no problem</returns>
        public static bool PrivecyCheck(Space a, Space b, double angle, double distance)
        {
            if (angle >= 6.29)
            {
                throw new ArgumentOutOfRangeException("angle", "angle is not within the range of 0 to 2*pi are you sure you are not using angle in degree?");
            }
            if (!a.WinFacePlane.IsValid || !b.WinFacePlane.IsValid)
            {
                RhinoApp.WriteLine("one or both plains were null");
                return true;
            }
            Vector3d normal_a = a.WinFacePlane.Normal;
            Vector3d normal_b = b.WinFacePlane.Normal;
            Point3d center_a = a.WinFacePlane.Origin;
            Point3d center_b = b.WinFacePlane.Origin;
            Vector3d vector = center_b - center_a;
            if (vector.Length > distance)
            {
                return true;
            }
            var angle_1 = Vector3d.VectorAngle(vector, normal_a);
            var angle_2 = Vector3d.VectorAngle(-1 * vector, normal_b);
            if (angle_2 >= angle || angle_1 >= angle)
            {
                return true;
            }
            return false;
        }


    }
}
