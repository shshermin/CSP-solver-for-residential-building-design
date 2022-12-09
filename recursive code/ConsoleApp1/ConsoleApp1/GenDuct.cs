using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Geometry;
using System.Linq;

namespace ConsoleApp1
{
    
    class GenDuct
    {
        public double ModuleX { get; set; }
        public double ModuleY { get; set; }
        public double ModuleZ { get; set; }
        public GenDuct(double x,double y,double z)
        {
            this.ModuleX = x;
            this.ModuleY = y;
            this.ModuleZ = z;
        }
        /// <summary>
        /// sets the first condition , if a module has a duct or not
        /// </summary>
        /// <param name="ListToOrder">the sampepoint list</param>
        /// <param name="DuctPoints">the list of points set as duct points</param>
        public void FirstCondition(List<SamePoints> ListToOrder,List<Point3d> DuctPoints)
        {
            try
            {
                var dist = new List<SamePoints>();
                for (int j = 0; j < ListToOrder.Count; j++)
                {
                    for (int i = 0; i < DuctPoints.Count; i++)
                    {
                        if (Generals.DistanceBetweenPoints2D(ListToOrder[j].pts[0], DuctPoints[i]) <= Math.Sqrt(2)*ModuleX + 0.01)
                        {
                            ListToOrder[j].CheckDiagonally = true;
                        }
                            
                    }
                }
                
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with the FirstCondition method");
            }

            }
        /// <summary>
        /// sets the second condition , which module has the most priority considernig its adjacents
        /// </summary>
        /// <param name="ListToOrder">the sampepoint list</param>
        /// <param name="DuctPoints">the list of points set as duct points</param>
        public void SecondCondition(List<SamePoints> ListToOrder, List<Point3d> DuctPoints)
        {
            try
            {
                for (int j = 0; j < ListToOrder.Count; j++)
                {
                    for (int k = 0; k < ListToOrder.Count; k++)
                    {
                        if (Math.Abs(Generals.DistanceBetweenPoints2D(ListToOrder[j].pts[0], ListToOrder[k].pts[0]) - ModuleX) < 0.01)
                        {
                            ListToOrder.ElementAt(j).Countpriority++;
                        }
                    }
                }

            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with the SecondCondition method");
            }
                           

        }
        /// <summary>
        /// apply priorities
        /// </summary>
        /// <param name="ListToOrder"></param>
        /// <param name="DuctPoints"></param>
        public void SetPriorities(List<SamePoints> ListToOrder,List<Point3d> DuctPoints)
        {
            try
            {
                FirstCondition(ListToOrder, DuctPoints);
                SecondCondition(ListToOrder, DuctPoints);
                   
             
                foreach (var sp in ListToOrder)
                {
                    sp.MeasurePriority();
                }
                ListToOrder = ListToOrder.OrderBy(b => b._Priority).ToList();
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with the SetPriorities method");
            }
            

        }
        /// <summary>
        /// add points to ducts and delete unwanted points
        /// </summary>
        /// <param name="ListToOrder"></param>
        /// <param name="DuctPoints"></param>
        /// <param name="samepoints"></param>
        /// <returns></returns>
        public List<SamePoints> SetDucts(List<SamePoints> ListToOrder, List<Point3d> DuctPoints,List<SamePoints> samepoints)
        {
            try
            {
                for (int i = ListToOrder.Count - 1; i > -1; i--)
                {
                    var temppt = ListToOrder[i].pts[0];
                    if (samepoints.Any(b => Generals.DistanceBetweenPoints2D(temppt, b.pts[0]) < Math.Sqrt(2) * ModuleX+0.01 && Generals.DistanceBetweenPoints2D(temppt, b.pts[0]) > 0.01)
                        && !ListToOrder[i].Duct)
                    {
                        DuctPoints.Add(temppt);
                        samepoints =FindAndDelete(temppt, samepoints);
                        FindAndSet(temppt, ListToOrder);
                        samepoints.Remove(ListToOrder[i]);
                    }
                }
                
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with the SetDucts method");
            }
            
            return samepoints;

        }
        public List<SamePoints> LoopPriorities(List<SamePoints> priority, List<Point3d> DuctPoints, List<SamePoints> samepoints)
        {
            try
            {
                if (priority.Count() != 0)
                {
                    if (DuctPoints.Count != 0)
                    {
                        SetPriorities(priority, DuctPoints);
                    }
                    samepoints = SetDucts(priority, DuctPoints, samepoints);
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with the LoopPriorities method");
            }
           
            return samepoints;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="samepoints"></param>
        /// <returns></returns>
        public List<Point3d> DuctPositioning(List<SamePoints> samepoints)
        {
            List<Point3d> DuctPoints = new List<Point3d>();
            try
            {
                List<SamePoints> Priority_4 = samepoints.Where(b => b.Countpt() == 4).ToList();
                samepoints = LoopPriorities(Priority_4, DuctPoints, samepoints);
                List<SamePoints> Priority_3 = samepoints.Where(b => b.Countpt() == 3).ToList();
                samepoints = LoopPriorities(Priority_3, DuctPoints, samepoints);
                List<SamePoints> Priority_2 = samepoints.Where(b => b.Countpt() == 2).ToList();
                samepoints = LoopPriorities(Priority_2, DuctPoints, samepoints);
                List<SamePoints> Priority_1 = samepoints.Where(b => b.Countpt() == 1).ToList();
                FirstCondition(Priority_1, DuctPoints);
                foreach(var sp in Priority_1)
                {
                    if(!sp.CheckDiagonally)
                    {   var check = false;
                        for(int i = 0; i< DuctPoints.Count;i++)
                        {
                            if(Generals.DistanceBetweenPoints2D(sp.pts[0],DuctPoints[i])< Math.Sqrt(2)*ModuleX+0.1)
                            {
                                check = true;
                            }
                        }
                        if(!check)
                        {
                            DuctPoints.Add(sp.pts[0]);
                        }
                        
                    }
                }
                


                }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with the DuctPositioning method");
                
            }
            

            return DuctPoints;

        }
        /// <summary>
        /// finding the adjacant points to the duct point and deleting them from the list
        /// </summary>
        /// <param name="pt">central point</param>
        /// <param name="ModuleY">module size y</param>
        /// <param name="ModuleX">module size x</param>
        /// <param name="sp">list of points with same type</param>
        /// <returns></returns>
        public List<SamePoints> FindAndDelete(Point3d pt, List<SamePoints> sp)
        {
            try
            {
                var pt1 = new Point3d(pt.X, pt.Y + ModuleY, pt.Z);
                var pt2 = new Point3d(pt.X, pt.Y - ModuleY, pt.Z);
                var pt3 = new Point3d(pt.X + ModuleX, pt.Y, pt.Z);
                var pt4 = new Point3d(pt.X - ModuleX, pt.Y, pt.Z);
                var listpt = new List<Point3d>() { pt1, pt2, pt3, pt4 };
                var listcopy = new List<SamePoints>();
                foreach (var point in sp)
                {
                    listcopy.Add(point);
                }
                foreach (var point in listcopy)
                {
                    for (int j =0;j<listpt.Count;j++)
                    {
                        if (Generals.DistanceBetweenPoints2D(point.pts[0], listpt[j]) < 0.1)
                        {
                            sp.Remove(point);
                        }
                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with the FindAndDelete method");
            }
            return sp;

        }
        public void FindAndSet(Point3d pt, List<SamePoints> sp)
        {
            try
            {
                var pt1 = new Point3d(pt.X, pt.Y + ModuleY, pt.Z);
                var pt2 = new Point3d(pt.X, pt.Y - ModuleY, pt.Z);
                var pt3 = new Point3d(pt.X + ModuleX, pt.Y, pt.Z);
                var pt4 = new Point3d(pt.X - ModuleX, pt.Y, pt.Z);
                var listpt = new List<Point3d>() { pt1, pt2, pt3, pt4 };
                
                foreach (var point in sp)
                {
                    for (int j = 0; j < listpt.Count; j++)
                    {
                        if (Generals.DistanceBetweenPoints2D(point.pts[0], listpt[j]) < 0.1)
                        {
                            point.Duct = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                RhinoApp.WriteLine("problem with the FindAndDelete method");
            }
            

        }


        /// <summary>
        /// makes the joints around the center of the mesh
        /// </summary>
        /// <param name="pt">list of points</param>
        /// <param name="ModuleX">x of modules</param>
        /// <param name="ModuleY">y of modules</param>
        /// <param name="ModuleZ">z of modules</param>
        /// <returns>joints</returns>
        public List<Point3d> JointMaker(Point3d pt)
        {
            var ptlist = new List<Point3d>();
            //try
            //{ 
            var vectorLenght = Math.Sqrt(((ModuleX * ModuleX) + (ModuleY * ModuleY)) / 4);
            var vectorDirection1 = (Vector3d.XAxis + Vector3d.YAxis);
            vectorDirection1.Unitize();
            var vectorDirection2 = (Vector3d.XAxis + (-1 * Vector3d.YAxis));
            vectorDirection2.Unitize();
            var movevector1 = (vectorLenght * vectorDirection1) + new Vector3d(0, 0, ModuleZ / 2);
            var movevector2 = vectorLenght * vectorDirection2 + new Vector3d(0, 0, ModuleZ / 2);
            var movevector3 = -1 * vectorLenght * vectorDirection1 + new Vector3d(0, 0, ModuleZ / 2);
            var movevector4 = -1 * vectorLenght * vectorDirection2 + new Vector3d(0, 0, ModuleZ / 2);
            ///////////////
            var pt1 = pt + movevector1;
            var point1 = new Point3d(pt1);
            var pt2 = pt + movevector2;
            var point2 = new Point3d(pt2);
            var pt3 = pt + movevector3;
            var point3 = new Point3d(pt3);
            var pt4 = pt + movevector4;
            var point4 = new Point3d(pt4);
               
            ptlist = new List<Point3d>() { point1, point2, point3, point4 };
            //}
            //catch(Exception)
            //{
            //    Console.WriteLine("problem with JointMaker method");
            //}
            
            return ptlist;
        }
        /// <summary>
        /// adding points to the samepoint class (points that are duplicate)
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        public static List<SamePoints> AddDuplicate(List<Point3d> pts)
        {
            var list = new List<SamePoints>();
            try
            {
                for (int i = 0; i < pts.Count; i++)
                {
                    var sp = new SamePoints();
                    sp.Add(pts[i]);
                    for (int j = pts.Count - 1; j > i; j--)
                    {
                        if (Generals.DistanceBetweenPoints2D(pts[i], pts[j]) < 0.1)
                        {
                            sp.Add(pts[j]);
                            pts.RemoveAt(j);
                        }
                    }
                    list.Add(sp);
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with AddDuplicate Method in GenDuct class");
            }
            return list;

        }
                
    }
}
