using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Geometry;

namespace ConsoleApp1
{
    public abstract class Condition
    {
        public bool Button { get; set; }
        public List<List<House>> PreviousHousesList { get; set; }

        public Condition(List<List<House>> PreviousHousesList)
        {
            this.PreviousHousesList = PreviousHousesList;
        }
        public abstract bool ExecuteCondition();
       

    }
}
