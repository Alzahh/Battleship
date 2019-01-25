using System;
using System.Linq;

namespace ClassLab
{
    public class Settings
    {

        public bool CanTouch;
        public bool RandomPos;
        public int BoardSize = 10;
        public int CarrierSize = 5;
        public int CarrierNum = 1;
        public int BattleshipSize = 4;
        public int BattleshipNum = 1;
        public int SubmarineSize = 3;
        public int SubmarineNum = 1;
        public int CruiserSize = 2;
        public int CruiserNum = 1;
        public int PatrolSize = 1;
        public int PatrolNum = 1;


        public string RunSett(string command)
        {
            bool exit = false;
            bool valid = true;
            do
            {
                Console.Clear();
                Console.WriteLine("1) board size: " + BoardSize +
                                  "\n2) ships can touch: " + CanTouch +
                                  "\n3) carrier size: " + CarrierSize + "  number: " + CarrierNum +
                                  "\n4) battleship size: " + BattleshipSize + " number: " + BattleshipNum +
                                  "\n5) submarine size: " + SubmarineSize + " number: " + SubmarineNum +
                                  "\n6) cruiser size: " + CruiserSize + " number: " + CruiserNum +
                                  "\n7) patrol size: " + PatrolSize + " number: " + PatrolNum +
                                  "\n8) Player random ship position: " + RandomPos +
                                  "\n press X or Q to quit");
                if (valid) Console.WriteLine("\nSettings are suitable");
                else Console.WriteLine("Settings are unexpected. Problems with ship placement are possible");
                var c = Console.ReadLine();
                Console.Clear();
                switch (c)
                {
                    case "1":
                        Console.WriteLine("Put new Board size");
                        BoardSize = Convert.ToInt32(Console.ReadLine());
                        if (BoardSize < 2) BoardSize = 10;
                        valid = Validate();
                        break;
                    case "2":
                        Console.WriteLine("Can ships touch? (y/n)");
                        if (Console.ReadLine() == "y") CanTouch = true;
                        else CanTouch = false;
                        valid = Validate();
                        break;
                    case "3":
                        Console.WriteLine("Put new Carrier size");
                        CarrierSize = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Put new Carrier number");
                        CarrierNum = Convert.ToInt32(Console.ReadLine());
                        valid = Validate();
                        break;
                    case "4":
                        Console.WriteLine("Put new Battleship size");
                        BattleshipSize = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Put new Battleship number");
                        BattleshipNum = Convert.ToInt32(Console.ReadLine());
                        valid = Validate();
                        break;
                    case "5":
                        Console.WriteLine("Put new Submarine size");
                        SubmarineSize = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Put new Submarine number");
                        SubmarineNum = Convert.ToInt32(Console.ReadLine());
                        valid = Validate();
                        break;
                    case "6":
                        Console.WriteLine("Put new Cruiser size");
                        CruiserSize = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Put new Cruiser number");
                        CruiserNum = Convert.ToInt32(Console.ReadLine());
                        valid = Validate();
                        break;
                    case "7":
                        Console.WriteLine("Put new Patrol size");
                        CruiserSize = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Put new Patrol number");
                        CruiserNum = Convert.ToInt32(Console.ReadLine());
                        valid = Validate();
                        break;
                    case "8":
                        Console.WriteLine("Should be player`s ship position random? (y/n)");
                        if (Console.ReadLine() == "y") RandomPos = true;
                        else RandomPos = false;
                        break;
                    case "x":
                    case "q":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Wrong rule index");
                        break;
                }
            } while (exit == false);

            return "";
        }

        public bool Validate()
        {
            var biggestShip = new[] { CarrierSize, BattleshipSize, SubmarineSize, CruiserSize, PatrolSize }.Max();
            if (BoardSize < biggestShip) return false;
            if (CanTouch && CarrierSize * CarrierNum
                + BattleshipSize * BattleshipNum
                + SubmarineSize * SubmarineNum
                + CruiserSize * CruiserNum
                + PatrolSize * PatrolNum < BoardSize * BoardSize) return true;
            int a = (CarrierSize + 2) * 3 * CarrierNum
                    + (BattleshipSize + 2) * 3 * BattleshipNum
                    + (SubmarineSize + 2) * 3 * SubmarineNum
                    + (CruiserSize + 2) * 3 * CruiserNum
                    + (PatrolSize + 2) * 3 * PatrolNum;
            return !CanTouch && a < BoardSize * BoardSize + BoardSize;
        }
    }
}