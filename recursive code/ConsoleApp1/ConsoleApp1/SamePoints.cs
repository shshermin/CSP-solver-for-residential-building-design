using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Geometry;

namespace ConsoleApp1
{
    class SamePoints
    {      
            public  List<Point3d> pts { get; set; }
        
            public bool CheckDiagonally { get; set; }
            public int Countpriority { get; set; }
            public int _Priority { get; set; }
            public bool Duct { get; set; }

            public SamePoints()
            {
                this.pts = new List<Point3d>();
            }
            public void Add(Point3d pt)
            {
                pts.Add(pt);
            }
            public int Countpt()
            {
                return pts.Count;
            }
            public void MeasurePriority()
            {
            if (CheckDiagonally)
            {
                _Priority++;
            }
            _Priority += Countpriority;

            }

        
    }
}
