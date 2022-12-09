using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Geometry;

namespace ConsoleApp1
{
    class Grid_2
    {
        
    //variables fromoutside
    public int BlockWidth { get; set; }
        public double HouseWidth { get; set; }
        public int BlockLength { get; set; }
        public double HouseLength { get; set; }
        public double HouseHight { get; set; }
        public List<string> Vertical_Connection { get; set; }
        /// <summary>
        /// constructor for building the grid
        /// </summary>
        /// <param name="BlockWidth">the x length of the block</param>
        /// <param name="HouseWidth">the x length of the house</param>
        /// <param name="BlockLength">the y length of the block</param>
        /// <param name="HouseLength">the y length of the house</param>
        /// <param name="HouseHight">the z length of house</param>
        /// <param name="verticalConnection">the position of the vertical access/s</param>
        public Grid_2(int BlockWidth, int HouseWidth, int BlockLength, int HouseLength, int HouseHight, List<string> verticalConnection)
        {

            if (BlockWidth <= 0 || HouseWidth <= 0 || BlockLength <= 0 || HouseLength <= 0 || HouseHight <= 0)
            {
                throw new ArgumentOutOfRangeException("dimentions", "dimentions should be greater than zero");
            }

            if (BlockWidth < HouseWidth || BlockLength < HouseLength)
            {
                throw new ArgumentOutOfRangeException("dimentions", "block dimentions are lower than house dimentions");
            }


            this.BlockWidth = BlockWidth;
            this.HouseWidth = HouseWidth;
            this.BlockLength = BlockLength;
            this.HouseLength = HouseLength;
            this.HouseHight = HouseHight;
            this.Vertical_Connection = verticalConnection;

        }



        /// <summary>
        ///creates the main grid without the vertical access
        /// </summary>
        /// <param name="ZAxis">the floor number</param>
        /// <returns>The grid 2d</returns>
        public List<Point3d> Construct_Grid_2D(int ZAxis)
        {

            List<Point3d> pt = VerticalAccess(ZAxis);
            var PTGrid2D = new List<Point3d>();

            for (double i = 0; i < BlockWidth * HouseWidth; i += HouseWidth)
            {
                for (double j = 0; j < BlockLength * HouseLength; j += HouseLength)
                {
                    Point3d Pt_Temp = new Point3d(i, j, (ZAxis * HouseHight));
                    PTGrid2D.Add(Pt_Temp);
                }
            }
            for (int i = 0; i < pt.Count; i++)
            {
                int temp = PTGrid2D.IndexOf(pt[i]);
                PTGrid2D.RemoveAt(temp);
            }
            return PTGrid2D;
        }
        /// <summary>
        /// gets the enterance points in each floor
        /// </summary>
        /// <param name="ZAxis">the floor</param>
        /// <param name="str"></param>
        /// <returns></returns>

        public List<Point3d> EnterancePts(int ZAxis)
        {
            var EnterancePTs = new List<Point3d>();
            var verticalaccess = VerticalAccess(ZAxis);
            foreach (var temp in verticalaccess)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (Math.Abs(i) == Math.Abs(j))
                        {
                            continue;
                        }
                        var pttemp = new Point3d(temp.X + i * HouseWidth, temp.Y + j * HouseLength, temp.Z);
                        if (pttemp.X >= 0 && pttemp.X <= (BlockWidth-1) * HouseWidth && pttemp.Y >= 0 && pttemp.Y <= (BlockLength-1) * HouseLength)
                        {
                            var boolpt = true;
                            for (int k = 0; k < verticalaccess.Count; k++)
                            {
                                if (Generals.DistanceBetweenPoints(verticalaccess[k], pttemp) < 0.1)
                                {
                                    boolpt = false;
                                }

                            }
                            if (boolpt)
                            {
                                EnterancePTs.Add(pttemp);
                            }
                        }
                    }
                }
            }
            return EnterancePTs;
        }

        /// <summary>
        /// an overload taking indexes to delete the unwanted enterance points
        /// </summary>
        /// <param name="ZAxis">the floor</param>
        /// <param name="deleteindex">indexes you want to remove</param>
        /// <returns>enterance points</returns>
        public List<Point3d> EnterancePts(int ZAxis, List<int> deleteindex)
        {
            for (int i = 0; i < deleteindex.Count; i++)
            {
                if (deleteindex[i] < 0 || deleteindex[i] > 3)
                {
                    throw new ArgumentOutOfRangeException("deleteindex", "invalid index: index can only be 0,1,2,3");
                }
            }
            var verticalaccess = VerticalAccess(ZAxis);
            var EnterancePTs = new List<Point3d>();
            foreach (var temp in verticalaccess)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (Math.Abs(i) == Math.Abs(j))
                        {
                            continue;
                        }
                        var pttemp = new Point3d(temp.X + i * HouseWidth, temp.Y + j * HouseLength, temp.Z);
                        if (pttemp.X >= 0 && pttemp.X <= BlockWidth * HouseWidth && pttemp.Y >= 0 && pttemp.Y <= BlockLength * HouseLength)
                        {
                            var boolpt = true;
                            for (int k = 0; k < verticalaccess.Count; k++)
                            {
                                if (Generals.DistanceBetweenPoints(verticalaccess[k], pttemp) < 0.1)
                                {
                                    boolpt = false;
                                }

                            }
                            if (boolpt)
                            {
                                EnterancePTs.Add(pttemp);
                            }

                        }
                    }
                }
            }
            for (int i = deleteindex.Count - 1; i >= 0; i--)
            {
                EnterancePTs.RemoveAt(deleteindex[i]);
            }

            return EnterancePTs;
        }
        /// <summary>
        /// creats vertical access points
        /// </summary>
        /// <param name="ZAxis">the floor</param>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<Point3d> VerticalAccess(int ZAxis)
        {
            var ptl = new List<Point3d>();
            foreach (var str in Vertical_Connection)
            {
                var temp = Generals.GetString(str);
                Point3d pt = new Point3d(temp[0] * HouseWidth, temp[1] * HouseLength, ZAxis * HouseHight);
                ptl.Add(pt);
            }
            return ptl;
        }

        public List<Box> MakeVerticalConnection(List<Point3d> vertpts)
        {
            var BoxList = new List<Box>();
            var intervalx = new Interval(-1 * (HouseWidth / 2), HouseWidth / 2);
            var intervaly = new Interval(-1 * (HouseLength / 2), HouseLength / 2);
            var intervalz = new Interval(-1 * (HouseHight / 2), HouseHight / 2);
            for (int i = 0; i < vertpts.Count; i++)
            {
                var PlaneOrigin = new Plane(vertpts[i], Vector3d.ZAxis);
                //varPlaneList.Add(PlaneOrigin);
                var box = new Box(PlaneOrigin, intervalx, intervaly, intervalz);
                BoxList.Add(box);
            }
            return BoxList;

        }



    }
}
