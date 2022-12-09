using System;
using System.Collections.Generic;
using System.Text;
using Rhino;
using Rhino.Geometry;
namespace ConsoleApp1
{
    /// <summary>
    /// handels the functions regarding all Houses in a block alternative
    /// </summary>
    public class AllHouses
    {
        public List<List<House>> Floors;
        public List<Point3d> pointcheck;
        public AllHouses(List<Point3d> ptcheck )
        {
            this.pointcheck = ptcheck;
            Floors = new List<List<House>>();
            
        }
        /// <summary>
        /// add all Houses belonging to this alternative
        /// </summary>
        /// <param name="floors"></param>
        public void AssignFloor(List<List<House>> floors)
        {
            try
            {
                if (floors.Count == 0 || floors == null)
                {
                    throw new ArgumentException(String.Format("there are no floors to add or {0}is null", floors));
                }
                foreach (var a in floors)
                {
                    var temp = new List<House>();
                    for(int i = 0; i< a.Count;i++)
                    {
                        temp.Add(a[i]);
                    }
                    Floors.Add(temp);
                }
            }
            catch(ArgumentException)
            { }
        }
        /// <summary>
        /// converts all meshes to brep ( for rayshoot method)
        /// </summary>
        /// <param name="inputlist">List of Houses containing meshes</param>
        /// <param name="Lightsurfaces">list of surfaces</param>
        /// <returns></returns>

        public static List<Brep> GetAllHouseBreps(List<List<House>> inputlist, List<Surface> Lightsurfaces, List<Brep> additionalBreps)
        {
            var brps = new List<Brep>();
            try
            {    //flattening the input list       
                var Listmesh = new List<Mesh>();
                foreach (var i in inputlist)
                {
                    foreach (var j in i)
                    {
                        var temp = j.GetMeshes();
                        foreach (var k in temp)
                        {
                            Listmesh.Add(k);
                        }
                    }
                } 
                //adding meshes of flattened list
                foreach (var i in Listmesh)
                {
                    brps.Add(Brep.CreateFromMesh(i, false));
                }
                //adding surfaces;
                foreach (var s in Lightsurfaces)
                {
                    brps.Add(s.ToBrep());
                }
               
            }
            
            catch (Exception)
            {
                RhinoApp.WriteLine("something wrong happened in the GetAllHouseBreps method");
            }
            return brps;
        }
        /// <summary>
        /// converts all meshes to brep ( for rayshoot method) and also add aditional breps
        /// </summary>
        /// <param name="inputlist"></param>
        /// <param name="Lightsurfaces"></param>
        /// <param name="additionalbreps"></param>
        /// <returns></returns>
        public static List<Brep> GetAllHouseBreps(List<House> inputlist, List<Surface> Lightsurfaces, List<Brep> additionalbreps)
        {
            var brps = new List<Brep>();
            try
            {
                var Listmesh = new List<Mesh>();
                
                    foreach (var j in inputlist)
                    {
                        var temp = j.GetMeshes();
                        foreach (var k in temp)
                        {
                            Listmesh.Add(k);
                        }
                    }
                
                foreach (var i in Listmesh)
                {
                    brps.Add(Brep.CreateFromMesh(i, false));
                }
                foreach (var s in Lightsurfaces)
                {
                    brps.Add(s.ToBrep());
                }
                foreach (var i in additionalbreps)
                {
                    brps.Add(i);
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("something wrong happened in the GetAllHouseBreps method");
            }
            return brps;
        }
       
        
    }
}
