using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Geometry;


namespace ConsoleApp1
{
    public class House
    {
        public List<Space> Each_House { get; set; }
        public List<double> Rotations { get; set; }
        
        
        public House()
        {
            Each_House = new List<Space>();
            Rotations = new List<double>();
        }
        /// <summary>
        /// add spaces to a House 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="light"></param>
        /// <param name="area"></param>
        /// <param name="mesh"></param>
        public void Add(int type, int light, double area, Mesh mesh)
        {           
            Each_House.Add(new Space(type, light, area, mesh));
        }

        /// <summary>
        /// gets all meshes of spaces in the house
        /// </summary>
        /// <returns>meshes</returns>
        public List<Mesh> GetMeshes()
        {
            var meshes = new List<Mesh>();
            foreach (var space in Each_House)
            {
 
                meshes.Add(space.mesh);
            }
            return meshes;
        }

        /// <summary>
        /// gets all the types of the spaces of the house
        /// </summary>
        /// <returns></returns>
        public List<int> GetTypes()
        {
            List<Space> ZeroTypeCheck = Each_House.FindAll(space => space.Type == 0);
            var types = new List<int>();
            foreach (var space in Each_House)
            { 
                types.Add(space.Type);               
            }
           
            if(ZeroTypeCheck.Count != 1)
            {
                throw new ArgumentException("each house should have only one space with Type 0 as Enterance ");
            }
            return types;
        }

        /// <summary>
        /// gets all the areas of the spaces of the house
        /// </summary>
        /// <returns>areas</returns>
        public List<double> GetAreas()
        {
            var areas = new List<double>();
            foreach (var space in Each_House)
                areas.Add(space.Area);
            return areas;
        }

        /// <summary>
        /// gets all the lights of the spaces of the house
        /// </summary>
        /// <returns>lights</returns>
        public List<int> GetLights()
        {
            var lights = new List<int>();
            foreach (var space in Each_House)
                lights.Add(space.Light);
            return lights;
        }

        /// <summary>
        /// gets the center of the type 0 mesh
        /// </summary>
        /// <returns>center point</returns>
        public Point3d GetEnteranceCenter()
        {
            var Center = new Point3d();
            foreach (var space in Each_House)
            {
                if (space.Type == 0)
                {
                    BoundingBox bo = space.mesh.GetBoundingBox(false);
                    Center = bo.Center;
                }
                else
                    continue;
            }
            return Center;
        }
        /// <summary>
        /// gets all center points of the meshes of a house
        /// </summary>
        /// <returns>centers</returns>
        public List<Point3d> GetAllCenters()
        {
            var CentersAll = new List<Point3d>();
            try
            {
                foreach (var space in Each_House)
                {

                    BoundingBox bo = space.mesh.GetBoundingBox(false);
                    var Centertemp = bo.Center;
                    CentersAll.Add(Centertemp);
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with GetAllCenters method ");
            }
            return CentersAll;

        }
        /// <summary>
        /// moves a copy of a house with a vector
        /// </summary>
        /// <param name="vec">move vector/param>
        /// <param name="house">the House</param>
        /// <returns></returns>

        public static House HouseMove(Vector3d vec, House house)
        {
            if(!vec.IsValid || vec.IsZero)
            {
                throw new ArgumentNullException("vector", "zero or null vector detected");
            }
            var test = new House();
            try
            {
                for (int k = 0; k < house.GetMeshes().Count; k++)
                {
                    var copymesh = house.Each_House[k].mesh.Duplicate();
                    bool Move = copymesh.Translate(vec.X, vec.Y, vec.Z);
                    test.Add(house.Each_House[k].Type, house.Each_House[k].Light, house.Each_House[k].Area, (Mesh)copymesh);
                    test.AddConnectedFace();

                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with HouseMove method");
            }

            return test;

        }
        /// <summary>
        /// adds all rotations allowed for a House instance
        /// </summary>
        /// <param name="inputrotation"></param>
        public void RotationAdd(List<double> inputrotation)
        {
            
            foreach (var rot in inputrotation)
            {
                if(rot > 6.29)
                {
                    throw new ArgumentOutOfRangeException("rotation angle", "angles must be in radian");
                }
                Rotations.Add(rot);
            }
        }

        /// <summary>
        /// adds connected faces to a House object
        /// </summary>
        public void AddConnectedFace()
        {
            for (int i = 0;i<Each_House.Count-1;i++)
            {
                for(int j = i+1;j< Each_House.Count;j++)
                {
                    AddConnectedFaces(Each_House[i], Each_House[j]);
                }

            }
        }
        /// <summary>
        /// adds connected faces to to space objects
        /// </summary>
        /// <param name="space_1"></param>
        /// <param name="space_2"></param>
        private void AddConnectedFaces(Space space_1,Space space_2)
        {
            try
            {
                for (int i = 1; i < 7; i++)
                {
                    var face_1Center = Generals.AvaragePointCalculator(GenRays.GetFaceCorners(i, space_1.mesh));
                    for (int j = 1; j < 7; j++)
                    {
                        var face_2center = Generals.AvaragePointCalculator(GenRays.GetFaceCorners(j, space_2.mesh));
                        if (Generals.DistanceBetweenPoints(face_1Center, face_2center) < 0.1)
                        {
                            space_1.AddConnectedFace(i);
                            space_2.AddConnectedFace(j);
                        }
                    }
                }
            }
            catch(Exception)
            {
                RhinoApp.WriteLine("problem with AddConnectedFaces method");
            }

        }
    }
   
  }


