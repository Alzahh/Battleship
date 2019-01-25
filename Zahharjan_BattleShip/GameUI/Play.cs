using System;
using System.Threading;
using Zahharjan_BattleShip.Init;
using ClassLab;

namespace Zahharjan_BattleShip.GameUI
{
    class Play
    {
        public GameBoard GameBoard { get; set; }
        public GameBoard hiden;
        public GameBoard Shown;
        public PlayersTurn pt;
        public AI enemy { get; }
        public Settings Set { get; set; }


        public Play(GameBoard gameBoard, Settings set)
        {
            Set = set;
            GameBoard = gameBoard;
            hiden = new GameBoard(set.BoardSize);
            Shown = new GameBoard(set.BoardSize);
            enemy = new AI(gameBoard, set);
            Rules hidRules = new Rules(hiden, Set);
            hidRules.RandomPlace(hidRules.Ships);
            pt = new PlayersTurn(hiden, gameBoard, Shown, set);
        }

        public Play(GameBoard gameBoard, GameBoard hidden, GameBoard shown, bool canTouch, int boardSize)
        {
            Set = new Settings();
            Set.CanTouch = canTouch;
            Set.BoardSize = boardSize;
            GameBoard = gameBoard;
            hiden = hidden;
            Shown = shown;
            enemy = new AI(gameBoard, Set);
            pt = new PlayersTurn(hiden, gameBoard, Shown, Set);
        }

        public void Game()
        {
            Console.Clear();
            int x = 0;
            int y = 0;
            GameRender(x, y);
            do
            {
                ConsoleKeyInfo ckey = Console.ReadKey();
                switch (ckey.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (x == Shown.Board.Count - 1) x = 0;
                        else x++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (x <= 0) x = GameBoard.Board.Count - 1;
                        else x--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (y == GameBoard.Board.Count - 1) y = 0;
                        else y++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (y <= 0) y = GameBoard.Board.Count - 1;
                        else y--;
                        break;
                    case ConsoleKey.S:
                        SaveGame(0);
                        break;
                    case ConsoleKey.Spacebar:
                        return;
                    case ConsoleKey.Enter:
                        if (GoodTarget(x, y))
                        {
                            if (!pt.Shoot(x, y))
                            {
                                var tup = enemy.AI_turn();
                                Console.WriteLine(tup.Item1 + " " + tup.Item2);
                                Thread.Sleep(500);
                                while (tup.Item2)
                                {
                                    tup = enemy.AI_turn();
                                    Console.WriteLine(tup.Item1 + " " + tup.Item2);
                                    Thread.Sleep(500);
                                }

                                if (GameBoard.GetHp() == 0)
                                {
                                    GameRender(x, y);
                                    Console.WriteLine("AI WON");
                                    Console.WriteLine("Want to save replay? (y/n)");
                                    if (Console.ReadLine()?.ToLower() == "y") SaveGame(1);
                                    ApplicationMenu.MainMenu.RunMenu();
                                }
                            }

                            if (hiden.GetHp() == 0)
                            {
                                Console.WriteLine("You Won");
                                Console.WriteLine("Want to save replay? (y/n)");
                                if (Console.ReadLine()?.ToLower() == "y") SaveGame(1);
                                ApplicationMenu.MainMenu.RunMenu();
                            }
                        }

                        break;
                }

                GameRender(x, y);
            } while (true);
        }

        public void GameRender(int x, int y)
        {
            BoardSquareState state = Shown.Board[x][y];
            Shown.Board[x][y] = BoardSquareState.Target;

            Console.Clear();
            Console.WriteLine(GameBoard.DisplayTables(Shown));
            Console.WriteLine("Press Spacebar to quit.                            Press S to save");

            Shown.Board[x][y] = state;
        }


        public bool GoodTarget(int x, int y)
        {
            return (Shown.Board[x][y] == BoardSquareState.NotUsed);
        }

        public void SaveGame(int ended)
        {
            Console.WriteLine("Start");

            var dc = new CodeFirstContext();
            var aiBoard = hiden.BoardToString();
            var playerBoard = GameBoard.BoardToString();
            var time = DateTime.Now.ToString();
            var save = new Save(time, playerBoard, aiBoard, GameBoard.Size, GameBoard.MoveString.ToString(), ended,
                Set.CanTouch);
            dc.Saves.Add(save);
            dc.SaveChanges();
            Console.WriteLine("Done");
            Thread.Sleep(300);
        }

    }
}