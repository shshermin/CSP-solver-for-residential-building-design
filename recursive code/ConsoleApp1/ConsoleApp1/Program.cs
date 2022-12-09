using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
        {
			var test = new List<Point3d>() {    new Point3d(12.5, 12.5, 1.5),
												new Point3d(12.5, 7.5, 1.5),
												new Point3d(7.5, 7.5, 1.5),
												new Point3d(7.5, 12.5, 1.5),
												new Point3d(17.5, 12.5, 1.5),
												new Point3d(17.5, 7.5, 1.5),
												new Point3d(17.5, 17.5, 1.5),
												new Point3d(12.5, 17.5, 1.5),
												new Point3d(2.5, 7.5, 7.5),
												new Point3d(2.5, 12.5, 7.5),
												new Point3d(17.5, 2.5, 4.5),
												new Point3d(17.5, -2.5, 4.5),
												new Point3d(12.5, -2.5, 4.5),
												new Point3d(12.5, 2.5, 4.5),
												new Point3d(7.5, 17.5, 10.5),
												new Point3d(7.5, 2.5, 13.5)
												};
			


			//teeeeeeeeeeeeeeest
			var tempnumber_1 = new List<int>() {5, 6 };
			var tempnumber_2 = new List<int>() { 7, 9 };
			var tempnumber_3 = new List<int>() { 10, 8 };
			var temnumber = new List<List<int>>() { tempnumber_1, tempnumber_2, tempnumber_3 };
			var ptlist_1 = new List<Point3d>() {new Point3d(12.5, 12.5, 1.5),
												new Point3d(12.5, 7.5, 1.5),
												new Point3d(7.5, 7.5, 1.5)};
			var ptlist_2 = new List<Point3d>() {new Point3d(7.5, 12.5, 1.5),
												new Point3d(17.5, 12.5, 1.5),
												new Point3d(17.5, 7.5, 1.5)};
			var ptlist_3 = new List<Point3d>() {new Point3d(7.5, 17.5, 10.5),
												new Point3d(7.5, 2.5, 13.5)};
			var enterancelist = new List<List<Point3d>>() { ptlist_1 , ptlist_2 , ptlist_3 };

			var testclass = new PositionsCalculator(enterancelist, temnumber);
			var testfinal_floor1 = testclass.HousePositionCalculator(2, 1);
			var testfinal_floor2 = testclass.HousePositionCalculator(2, 2);
			var testfinal_floor3 = testclass.HousePositionCalculator(2, 3);
			var testfinalkoli = new List<List<MoveCodes>>() { testfinal_floor1, testfinal_floor2 , testfinal_floor3 };
			var testfinal_loop = new List<List<MoveCodes>>();
			var arraytest = new MoveCodes[3];
			PositionsCalculator.Set(arraytest, 0, 3, testfinalkoli, ref testfinal_loop);
			var n = 0;
			foreach (var t in testfinal_loop)
            {
                Console.WriteLine("floor" + n.ToString());
				n++;
				Console.WriteLine("[");
				foreach (var x in t)
                {
                    
					Console.Write("housecode ={");
					foreach (var m in x.HouseCodes)
			        {
						Console.Write(m.ToString() + ",");
					}
					Console.WriteLine("}");
					Console.Write("placecode ={");
					foreach (var y in x.PlaceCode)
					{
						Console.Write(y.ToString() + ",");
					}
					Console.WriteLine("}");


				}
			}



			//var n = 1;
			//foreach(var t in testfinal_floor1)
   //         {
   //             Console.WriteLine(t.FloorId.ToString()+ "_" +n.ToString());
			//	n++;
			//	Console.Write("housecode ={");
			//	foreach (var x in t.HouseCodes)
   //             {
			//		Console.Write(x.ToString() + ",");
			//	}
			//	Console.WriteLine("}");
			//	Console.Write("placecode ={");
			//	foreach (var y in t.PlaceCode)
   //             {
			//		Console.Write(y.ToString() + ",");
			//	}
			//	Console.WriteLine("}");
			//}


		}






	}
}
