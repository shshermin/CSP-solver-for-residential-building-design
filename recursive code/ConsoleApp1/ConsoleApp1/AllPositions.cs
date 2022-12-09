using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class AllPositions
    {
        public List<MoveCodes> Positions {get;set;}
        public bool Check { get; set; }
        public AllPositions()
        {
            this.Positions = new List<MoveCodes>();
            this.Check = true;
        }

        public void Add(MoveCodes movecodes)
        {
            Positions.Add(movecodes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="moveposition"></param>
        /// <returns>true if they are identical</returns>
        private static bool Checker( MoveCodes moveposition_1, MoveCodes moveposition_2)
        {
            if(object.Equals(moveposition_1,moveposition_2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void MainChecker(AllPositions a , AllPositions b , int falseindex)
        {
            var check = true;
            for(int i = 0; i<falseindex;i++)
            {
                if (Checker(a.Positions[i],b.Positions[i]))
                {
                    continue;
                }
                else
                {
                    check = false;
                }
            }
            if (check)
            {
                b.Check = false;
            }
            
        }

        
    }
}
