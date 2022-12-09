using System;
using System.Collections.Generic;
using System.Text;
using Rhino;
using Rhino.Geometry;

namespace ConsoleApp1
{
    public class Light: Condition
    {
        public GenLight GeneralLight { get; set; }
        public List<Brep> AllBreps { get; set; }
        public List<String> LightConditions { get; set; }
        public Light(bool button ,List<List<House>> PreviousHousesList, GenLight generalLight, List<Brep> allbreps, List<String> lightconditions)
           : base(PreviousHousesList)
        {
            this.Button = button;
            this.GeneralLight = generalLight;
            this.AllBreps = allbreps;
            this.LightConditions = lightconditions;
        }
        public override bool ExecuteCondition( )
        {
            if (Button == false)
            {
                return true;
            }
            else
            {
                AddLightToSpaces(GeneralLight, AllBreps);
                var conditionfinal = Conditions(LightConditions);
                return conditionfinal;

            }
            
                
           
        }
        private void AddLightToSpaces( GenLight generallight, List<Brep> AllBreps)
        {
            foreach (List<House> listhouse in PreviousHousesList)
            {
                generallight.AddLightToListOfHouse(listhouse, AllBreps);
            }

        }
        private bool Conditions(List<String> LightConditions)
        {
            var ShartLight = true;
            try
            {
                for (int i = 0; i < PreviousHousesList.Count; i++)
                {
                    for (int ja = 0; ja < PreviousHousesList[i].Count; ja++)
                    {
                        for (int k = 0; k < PreviousHousesList[i][ja].Each_House.Count; k++)
                        {
                            var shartbool = PreviousHousesList[i][ja].Each_House[k].LightCheck(5, LightConditions);
                            if (shartbool == false)
                            {
                                ShartLight = false;
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with Conditions Method in Light Class");
            }
            return ShartLight;
        }
    }
}
