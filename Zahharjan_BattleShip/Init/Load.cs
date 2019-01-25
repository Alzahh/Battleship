using System;
using System.Linq;
using System.Threading;
using ClassLab;
using Zahharjan_BattleShip.GameUI;

namespace Zahharjan_BattleShip.Init
{
    public class Load
    {
        private bool Replay;

        public Load(bool replay)
        {
            Replay = replay;
        }

        public string LoadGame(string command)
        {
            using (var ctx = new CodeFirstContext())
            {
                var users = ctx.Saves;
                var lines = users.ToList();
                int i = 1;
                foreach (var item in lines)
                {
                    if (Replay && item.Ended == 1)
                    {
                        Console.WriteLine(i + ")" + " " + item.Time);
                        i++;
                    }
                    else if (!Replay && item.Ended == 0)
                    {
                        Console.WriteLine(i + ")" + " " + item.Time);
                        i++;
                    }
                }

                Console.WriteLine("Choose Save file");
                var ans = Console.ReadLine()?.ToLower();
                int val;
                while (true)
                {
                    if (Int32.TryParse(ans, out val))
                    {
                        if (val >= 1 && val < lines.Count) break;
                    }
                    else if (ans == "q" || ans == "x")
                    {
                        return "";
                    }

                    Console.WriteLine("Put right index");
                    ans = Console.ReadLine();
                }

                int counter = 0;
                Save line = new Save();

                foreach (var element in lines)
                {
                    if (Replay && element.Ended == 1)
                    {
                        counter++;
                        if (counter == val)
                        {
                            line = element;
                        }
                    }
                    else if (!Replay && element.Ended == 0)
                    {
                        counter++;
                        if (counter == val)
                        {
                            line = element;
                        }
                    }
                }

                GameBoard gb = new GameBoard(line.Board_size);
                GameBoard hidden = new GameBoard(line.Board_size);
                GameBoard shown = new GameBoard(line.Board_size);
                Rules gbRules = new Rules(gb, line.CanTouch);
                Rules hiddRules = new Rules(hidden, line.CanTouch);
                var playerShips = line.Player_Board.Substring(0, line.Player_Board.Length - 1).Split('|');
                var aiShips = line.AI_Board.Substring(0, line.AI_Board.Length - 1).Split('|');
                putShips(gbRules, playerShips);
                putShips(hiddRules, aiShips);

                Play play = new Play(gb, hidden, shown, line.CanTouch, line.Board_size);
                var moves = line.Moves.Substring(0, line.Moves.Length - 1).Split('|');
                Simulate(moves, play, gb, shown);
            }


            return "";
        }


        private void Simulate(string[] moves, Play play, GameBoard gb, GameBoard shown)
        {
            bool playersTurn = true;

            foreach (var move in moves)
            {
                var pos = move.Split(':');
                if (playersTurn)
                {
                    playersTurn = play.pt.Shoot(int.Parse(pos[0]), int.Parse(pos[1]));
                }
                else
                {
                    playersTurn = !play.enemy.AI_shoot(int.Parse(pos[0]), int.Parse(pos[1]));
                }

                if (Replay)
                {
                    Console.Clear();
                    Console.WriteLine(gb.DisplayTables(shown));
                    Thread.Sleep(300);
                }
            }

            if (!Replay) play.Game();
            else
            {
                var winner = "Player";
                if (gb.GetHp() == 0) winner = "AI";
                Console.WriteLine(winner + " won");
            }
        }

        private void putShips(Rules rules, string[] ships)
        {
            foreach (var ship in ships)
            {
                var values = ship.Split(':');
                bool horizontal = values[3] == "True";
                Ship newShip = new Ship(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]),
                    horizontal);
                rules.toSave(newShip);
            }
        }
    }
}