using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class MoveCodes
    {
        public List<int> HouseCodes { get; set; }
        public List<int> PlaceCode { get; set; }
        public int FloorId { get; set; }
        public MoveCodes(int floorid)
        {
            this.HouseCodes = new List<int>();
            this.PlaceCode = new List<int>();
            this.FloorId = floorid;
        }
        public void AddHouses(List<int> houses)
        {
            for (int i =0;i< houses.Count;i++)
            {
                HouseCodes.Add(houses[i]);
            }
        }
        public void AddPlaces(List<int> places)
        {
            for (int i = 0; i < places.Count; i++)
            {
                PlaceCode.Add(places[i]);
            }
        }
    }
}
