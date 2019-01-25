using System;
using System.Collections.Generic;


namespace ClassLab
{
    // Class provides interface for ship placement
    public class Rules
    {
        public GameBoard gameBoard;
        public List<Ship> Ships = new List<Ship>();
        public Settings set { get; set; }

        public Rules(GameBoard gb, Settings set)
        {
            this.set = set;
            Ships.AddRange(shipGen("carrier", set.CarrierNum));
            Ships.AddRange(shipGen("battleship", set.BattleshipNum));
            Ships.AddRange(shipGen("submarine", set.SubmarineNum));
            Ships.AddRange(shipGen("cruiser", set.CruiserNum));
            Ships.AddRange(shipGen("patrol", set.PatrolNum));
            gameBoard = gb;
        }

        public Rules(GameBoard gb, bool canTouch)
        {
            set = new Settings();
            set.CanTouch = canTouch;
            gameBoard = gb;
        }


        public List<Ship> shipGen(String type, int num)
        {
            List<Ship> ships = new List<Ship>();
            for (int i = 0; i < num; i++)
            {
                switch (type)
                {
                    case "carrier":
                        ships.Add(new Ship(0, 0, set.CarrierSize, type));
                        break;
                    case "battleship":
                        ships.Add(new Ship(0, 0, set.BattleshipSize, type));
                        break;
                    case "submarine":
                        ships.Add(new Ship(0, 0, set.SubmarineSize, type));
                        break;
                    case "cruiser":
                        ships.Add(new Ship(0, 0, set.CruiserSize, type));
                        break;
                    case "patrol":
                        ships.Add(new Ship(0, 0, set.PatrolSize, type));
                        break;
                }
            }

            return ships;
        }

        public GameBoard Start(bool random)
        {
            if (random) RandomPlace(Ships);
            else
            {
                foreach (var ship in Ships)

                {
                    bool placed = false;
                    do
                    {
                        render(ship);
                        ConsoleKeyInfo ckey = Console.ReadKey();
                        switch (ckey.Key)
                        {
                            case ConsoleKey.DownArrow:
                                if (!ship.Horizontal && ship.x == gameBoard.Size - ship.Len ||
                                    ship.Horizontal && ship.x == gameBoard.Size - 1) ship.x = 0;
                                else ship.x++;
                                break;
                            case ConsoleKey.UpArrow:
                                if (ship.x <= 0) ship.x = gameBoard.Size - ship.Len;
                                else ship.x--;
                                break;
                            case ConsoleKey.RightArrow:
                                if (ship.Horizontal && ship.y == gameBoard.Size - ship.Len ||
                                    !ship.Horizontal && ship.y == gameBoard.Size - 1) ship.y = 0;
                                else ship.y++;
                                break;
                            case ConsoleKey.LeftArrow:
                                if (ship.Horizontal && ship.y <= 0) ship.y = gameBoard.Size - ship.Len;
                                else if (!ship.Horizontal && ship.y <= 0) ship.y = gameBoard.Size - 1;
                                else ship.y--;
                                break;

                            case ConsoleKey.R:
                                if (ship.x < gameBoard.Size - ship.Len + 1 &&
                                    ship.y < gameBoard.Size - ship.Len + 1)
                                    ship.Horizontal = !ship.Horizontal;
                                break;
                            case ConsoleKey.Enter:
                                if (goodPos(underMove(ship)))
                                {
                                    placed = true;
                                    render(ship);
                                    toSave(ship);
                                }

                                break;
                        }
                    } while (!placed);
                }
            }

            return gameBoard;
        }

        public List<BoardSquareState> underMove(Ship ship)
        {
            List<BoardSquareState> states = new List<BoardSquareState>();

            for (int i = 0; i < ship.Len; i++)
            {
                if (ship.Horizontal)
                    states.Add(gameBoard.Board[ship.x][ship.y + i]);
                else states.Add(gameBoard.Board[ship.x + i][ship.y]);
            }

            return states;
        }

        private void render(Ship ship)
        {
            List<BoardSquareState> states = underMove(ship);

            for (int i = 0; i < ship.Len; i++)
            {
                if (ship.Horizontal) gameBoard.Board[ship.x][ship.y + i] = BoardSquareState.Ship;
                else gameBoard.Board[ship.x + i][ship.y] = BoardSquareState.Ship;
            }


            Console.Clear();
            Console.WriteLine(gameBoard.GetBoardString());
            Console.WriteLine("Press r to rotate. " + ship.ShipName
                                                    + " Len = " + ship.Len + " Position suits: " + goodPos(states));


            for (int i = 0; i < ship.Len; i++)
            {
                if (ship.Horizontal) gameBoard.Board[ship.x][ship.y + i] = states[i];
                else gameBoard.Board[ship.x + i][ship.y] = states[i];
            }
        }

        public void toSave(Ship ship)
        {
            gameBoard.Ships.Add(ship);
            for (int i = 0; i < ship.Len; i++)
            {
                if (ship.Horizontal)
                {
                    if (ship.Horizontal) gameBoard.Board[ship.x][ship.y + i] = BoardSquareState.Ship;
                    ship.Body.Add(new[] {ship.x, ship.y + i});
                }
                else
                {
                    gameBoard.Board[ship.x + i][ship.y] = BoardSquareState.Ship;
                    ship.Body.Add(new[] {ship.x + i, ship.y});
                }
            }

            if (set.CanTouch == false) gameBoard.cover(ship);
        }

        public bool goodPos(List<BoardSquareState> states)
        {
            foreach (var state in states)
            {
                if (state != BoardSquareState.NotUsed) return false;
            }

            return true;
        }

        public void RandomPlace(List<Ship> ships)
        {
            for (int i = 0; i < ships.Count; i++)
            {
                Random rnd = new Random();
                int hor = rnd.Next(0, 2);
                int hor2 = 1 - hor;
                ships[i].Horizontal = (hor == 0);
                ships[i].x = rnd.Next(0, gameBoard.Size - (ships[i].Len - 1) * hor);
                ships[i].y = rnd.Next(0, gameBoard.Size - (ships[i].Len - 1) * hor2);
                if (goodPos(underMove(ships[i])))
                {
                    toSave(ships[i]);
                }
                else
                {
                    i--;
                }
            }
        }
    }
}