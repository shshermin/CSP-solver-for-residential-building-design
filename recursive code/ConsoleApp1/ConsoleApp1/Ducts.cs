using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Geometry;

namespace ConsoleApp1
{
    class Ducts
    {
        
        public List<Point3d> pts { get; set; }

        public Ducts()
        {
            this.pts = new List<Point3d>();
        }
        public void Add(Point3d pt)
        {
               pts.Add(pt);
        }
  
    }
}
