using System.Collections.Generic;

namespace ClassLab
{
    public class Ship
    {
        public int x { get; set; }
        public int y { get; set; }
        public int Len { get; set; }
        public int Health { get; set; }
        public string ShipName { get; set; }
        public bool Horizontal { get; set; }
        public List<int[]> Body { get; set; }

        public Ship(int x, int y, int len, string name)
        {
            this.x = x;
            this.y = y;
            Len = len;
            ShipName = name;
            Health = len;
            Horizontal = false;
            Body = new List<int[]>();
        }

        public Ship(int x, int y, int len, bool horizontal)
        {
            this.x = x;
            this.y = y;
            Len = len;
            Health = len;
            Horizontal = horizontal;
            Body = new List<int[]>();
        }
    }
}