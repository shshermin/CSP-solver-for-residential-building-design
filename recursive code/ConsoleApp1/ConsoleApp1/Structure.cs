using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace ConsoleApp1
{
    static class Structure
    {
        public static List<Point3d> RemoveDuplicate(List<Point3d> pts)
        {
            for (int i = 0; i < pts.Count; i++)
            {
                for (int j = pts.Count - 1; j > i; j--)
                {
                    if (DistanceBetweenPoints(pts[i], pts[j]) < 0.1)
                    {
                        pts.RemoveAt(j);
                    }
                }
            }
            return pts;

        }
        public static double DistanceBetweenPoints(Point3d a, Point3d b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            var dz = a.Z - b.Z;
            // calculate the distance
            var distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return distance;
        }
        public static List<Point3d> Flatten(List<List<Point3d>> list)
        {
            var listflatten = new List<Point3d>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    listflatten.Add(list[i][j]);
                }
            }
            return listflatten;
        }
        public static List<Line> makeAllColumnsLines(List<List<Point3d>> joints, double z)
        {
            var ptstop = joints[joints.Count - 1];
            var lines = new List<Line>();
            foreach (var p in ptstop)
            {
                var temp = new Point3d(p.X, p.Y, -1 * z / 2);
                lines.Add(new Line(p, temp));
            }
            return lines;
        }
        public static List<List<Point3d>> makeAllColumnsJoints(List<Point3d> pts, double x, double y, double z)
        {
            var list = new List<List<Point3d>>();
            foreach (var pt in pts)
            {
                list.Add(JointMaker(pt, x, y, z));
            }
            return list;
        }
        public static List<Point3d> JointMaker(Point3d pt, double x, double y, double z)
        {
            var vectorLenght = Math.Sqrt(((x * x) + (y * y)) / 4);
            var vectorDirection1 = (Vector3d.XAxis + Vector3d.YAxis);
            vectorDirection1.Unitize();
            var vectorDirection2 = (Vector3d.XAxis + (-1 * Vector3d.YAxis));
            vectorDirection2.Unitize();
            var movevector1 = (vectorLenght * vectorDirection1) + new Vector3d(0, 0, z / 2);
            var movevector2 = vectorLenght * vectorDirection2 + new Vector3d(0, 0, z / 2);
            var movevector3 = -1 * vectorLenght * vectorDirection1 + new Vector3d(0, 0, z / 2);
            var movevector4 = -1 * vectorLenght * vectorDirection2 + new Vector3d(0, 0, z / 2);
            ///////////////
            var pt1 = pt + movevector1;
            var point1 = new Point3d(pt1);
            var pt2 = pt + movevector2;
            var point2 = new Point3d(pt2);
            var pt3 = pt + movevector3;
            var point3 = new Point3d(pt3);
            var pt4 = pt + movevector4;
            var point4 = new Point3d(pt4);
            /////
            var ptlist = new List<Point3d>() { point1, point2, point3, point4 };
            return ptlist;
        }


        public static bool intcheck(List<Point3d> intlist, Point3d check, string direction)
        {
            switch (direction)
            {
                case ("z"):
                    for (int i = 0; i < intlist.Count; i++)
                    {
                        if (vertcheck(intlist[i], check))
                        {
                            return true;
                        }
                    }
                    return false;

                case ("x"):
                    for (int i = 0; i < intlist.Count; i++)
                    {
                        if (horizcheckX(intlist[i], check))
                        {
                            return true;
                        }
                    }
                    return false;

                case ("y"):
                    for (int i = 0; i < intlist.Count; i++)
                    {
                        if (horizcheckY(intlist[i], check))
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;

            }
            //return switch;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static bool vertcheck(Point3d pt1, Point3d pt2)
        {
            if (Math.Abs(pt1.X - pt2.X) < 0.2 && Math.Abs(pt1.Y - pt2.Y) < 0.2)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static bool horizcheckX(Point3d pt1, Point3d pt2)
        {
            if (Math.Abs(pt1.Z - pt2.Z) < 0.2 && Math.Abs(pt1.Y - pt2.Y) < 0.2)
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static bool horizcheckY(Point3d pt1, Point3d pt2)
        {
            if (Math.Abs(pt1.Z - pt2.Z) < 0.2 && Math.Abs(pt1.X - pt2.X) < 0.2)
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// 
        /// </summary>

        public class VerticalPoints
        {
            public List<Point3d> vertpts { get; set; }
            public double Modulez { get; set; }

            public VerticalPoints(List<Point3d> pts, double z)
            {
                this.vertpts = pts;
                this.Sort();
                this.CreateColumns(z);
            }
            /// <summary>
            /// 
            /// </summary>
            public void Sort()
            {
                List<Point3d> newList = vertpts.OrderBy(o => o.Z).ToList();
                vertpts = newList;

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="z"></param>
            public void CreateColumns(double z)
            {
                var listcolumn = new List<Line>();
                var earth = vertpts[vertpts.Count - 1];
                var point = new Point3d(earth.X, earth.Y, -z / 2);
                if (DistanceBetweenPoints(point, vertpts[0]) > 0.01)
                {
                    vertpts.Insert(0, point);
                }

            }

        }
        /// <summary>
        /// 
        /// </summary>

        public class HorizentalPointsx
        {
            public List<Point3d> horizpts { get; set; }

            public HorizentalPointsx(List<Point3d> pts)
            {
                this.horizpts = pts;
                this.Sort();
            }
            public void Sort()
            {
                List<Point3d> newList = horizpts.OrderBy(o => o.X).ToList();
                horizpts = newList;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class HorizentalPointsy
        {
            public List<Point3d> horizpts { get; set; }

            public HorizentalPointsy(List<Point3d> pts)
            {
                this.horizpts = pts;
                this.Sort();
            }
            public void Sort()
            {
                List<Point3d> newList = horizpts.OrderBy(o => o.Y).ToList();
                horizpts = newList;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class Floor
        {
            public List<HorizentalPointsy> YAxis { get; set; }
            public List<HorizentalPointsx> XAxis { get; set; }
            public int floorindex { get; set; }
            public Floor(int floornum)
            {
                this.floorindex = floornum;
                YAxis = new List<HorizentalPointsy>();
                XAxis = new List<HorizentalPointsx>();

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            public void Add(HorizentalPointsx x)
            {
                XAxis.Add(x);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="y"></param>
            public void Add(HorizentalPointsy y)
            {
                YAxis.Add(y);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public class Floors
        {
            public List<Floor> allfloors { get; set; }

            public Floors()
            {
                this.allfloors = new List<Floor>();
            }
            public void Add(Floor f)
            {
                allfloors.Add(f);
            }
            public double MaxX()
            {
                double max = 0;
                foreach (var flr in allfloors)
                {
                    var temp = flr.XAxis;
                    foreach (var r in temp)
                    {
                        var temp1 = r.horizpts;
                        foreach (var pt in temp1)
                        {
                            if (pt.X > max)
                            {
                                max = pt.X;
                            }
                        }
                    }
                }
                return max;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public double MinX()
            {
                double min = 500000;
                foreach (var flr in allfloors)
                {
                    var temp = flr.XAxis;
                    foreach (var r in temp)
                    {
                        var temp1 = r.horizpts;
                        foreach (var pt in temp1)
                        {
                            if (pt.X < min)
                            {
                                min = pt.X;
                            }
                        }
                    }
                }
                return min;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public double MaxY()
            {
                double max = 0;
                foreach (var flr in allfloors)
                {
                    var temp = flr.YAxis;
                    foreach (var r in temp)
                    {
                        var temp1 = r.horizpts;
                        var pt = temp1[temp1.Count - 1];
                        if (pt.Y > max)
                        {
                            max = pt.Y;
                        }
                    }
                }
                return max;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public double MinY()
            {
                double min = 500000;
                foreach (var flr in allfloors)
                {
                    var temp = flr.YAxis;
                    foreach (var r in temp)
                    {
                        var temp1 = r.horizpts;
                        var pt = temp1[0];
                        if (pt.Y < min)
                        {
                            min = pt.Y;
                        }
                    }
                }
                return min;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public List<Point3d> XAxisPts()
            {
                var listptx = new List<Point3d>();
                foreach (var flr in allfloors)
                {
                    var temp = flr.YAxis;
                    foreach (var r in temp)
                    {
                        var temp1 = r.horizpts;
                        var pt = temp1[0];
                        listptx.Add(new Point3d(pt.X, MinY(), 0));

                    }
                }
                listptx = RemoveDuplicate(listptx);
                listptx = listptx.OrderBy(o => o.X).ToList();
                return listptx;


            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public List<Point3d> YAxisPts()
            {
                var listpty = new List<Point3d>();
                var min = MinX();
                foreach (var flr in allfloors)
                {
                    var temp = flr.XAxis;
                    foreach (var r in temp)
                    {
                        var temp1 = r.horizpts;
                        var pt = temp1[0];
                        listpty.Add(new Point3d(min, pt.Y, 0));

                    }
                }
                listpty = RemoveDuplicate(listpty);
                listpty = listpty.OrderBy(o => o.Y).ToList();
                return listpty;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="extend"></param>
            /// <returns></returns>
            public List<Line> XAxisLines(double extend)
            {
                var listline = new List<Line>();
                var start = XAxisPts();
                //start = start.OrderBy(o => o.X).ToList();
                foreach (var pt in start)
                {
                    var ptstart = pt + new Point3d(0, -1 * extend, 0);
                    var ln = new Line(ptstart, Vector3d.YAxis, Math.Abs(MaxY() - MinY()) + 2 * extend);
                    listline.Add(ln);
                }
                return listline;

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="extend"></param>
            /// <returns></returns>
            public List<Line> YAxisLines(double extend)
            {
                var listline = new List<Line>();
                var start = YAxisPts();
                //start = start.OrderBy(o => o.Y).ToList();
                foreach (var pt in start)
                {
                    var ptstart = pt + new Point3d(-1 * extend, 0, 0);
                    var ln = new Line(ptstart, Vector3d.XAxis, Math.Abs(MaxX() - MinX()) + 2 * extend);
                    listline.Add(ln);
                }
                return listline;

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="Modulez"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public List<Circle> AnnotationCrclx(double Modulez, double radius)
            {
                var circles = new List<Circle>();
                var lns = XAxisLines(Modulez);
                var ptc = new Point3d(0, -1 * radius, 0);
                foreach (var l in lns)
                {
                    var pts = l.PointAt(0);
                    Point3d ptrscenter = pts + ptc;
                    circles.Add(new Circle(ptrscenter, radius));
                }
                return circles;

            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="Modulez"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public List<Circle> AnnotationCrcly(double Modulez, double radius)
            {
                var circles = new List<Circle>();
                var lns = YAxisLines(Modulez);
                var ptc = new Point3d(-1 * radius, 0, 0);
                foreach (var l in lns)
                {
                    var pts = l.PointAt(0);
                    Point3d ptrscenter = pts + ptc;
                    circles.Add(new Circle(ptrscenter, radius));
                }
                return circles;

            }

        }
    }
}
