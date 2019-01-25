using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ClassLab;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(bool canTouch, bool randomPos, int boardSize, int carrierSize, int carrierNum,
            int battleshipSize, int battleshipNum, int submarineSize, int submarineNum, int cruiserSize, int cruiserNum,
            int patrolSize, int patrolNum = 1)
        {
            var Set = new Settings();
            Set.CanTouch = canTouch;
            Set.RandomPos = randomPos;
            Set.BoardSize = boardSize;
            Set.CarrierSize = carrierSize;
            Set.CarrierNum = carrierNum;
            Set.BattleshipSize = battleshipSize;
            Set.BattleshipNum = battleshipNum;
            Set.SubmarineSize = submarineSize;
            Set.SubmarineNum = submarineNum;
            Set.CruiserSize = cruiserSize;
            Set.CruiserNum = cruiserNum;
            Set.PatrolSize = patrolSize;
            Set.PatrolNum = patrolNum;
            if (!Set.Validate()) return View();
            var Gb = new GameBoard(Set.BoardSize);
            var Rules = new Rules(Gb, Set);
            var enemyRule = new Rules(new GameBoard(Set.BoardSize), Set);
            enemyRule.RandomPlace(enemyRule.Ships);
            Session["Rules"] = Rules;
            Session["index"] = 0;
            Session["Set"] = Set;
            Session["ships"] = Rules.Ships;
            Session["Enemy"] = enemyRule.gameBoard;
            Session["Shown"] = new GameBoard(Set.BoardSize);
            Session["AI"] = new AI(Gb, Set);
            ViewData["Ship"] = Rules.Ships[0];

            if (randomPos)
            {
                Session["index"] = null;
                Rules.RandomPlace(Rules.Ships);
                ViewData["message"] = "Your turn";
                return View("Game");
            }

            return View("SetUp");
        }


        public ActionResult Save()
        {
            var ctx = new CodeFirstContext();
            var data = ctx.Saves.ToList();
            Session["SaveLines"] = data;
            return View();
        }

        [HttpPost]
        public ActionResult Save(int index)
        {
            var lines = (List<Save>) Session["SaveLines"];
            var line = lines[index];
            var Set = new Settings();
            Set.BoardSize = line.Board_size;
            Set.CanTouch = line.CanTouch;
            var Gb = new GameBoard(Set.BoardSize);
            var Rules = new Rules(Gb, Set);
            var Shown = new GameBoard(Set.BoardSize);
            var enemyRule = new Rules(new GameBoard(Set.BoardSize), Set);
            var Pt = new PlayersTurn(enemyRule.gameBoard, Gb, Shown, Set);
            var enemy = new AI(Gb, Set);

            var simulator = new GameSimulation();
            var playerShips = line.Player_Board.Substring(0, line.Player_Board.Length - 1).Split('|');
            var aiShips = line.AI_Board.Substring(0, line.AI_Board.Length - 1).Split('|');
            simulator.PutShips(Rules, playerShips);
            simulator.PutShips(enemyRule, aiShips);
            var moves = line.Moves.Substring(0, line.Moves.Length - 1).Split('|');
            simulator.Simulate(moves, Pt, enemy, Gb, Shown);

            Session["Rules"] = Rules;
            Session["Set"] = Set;
            Session["ships"] = Rules.Ships;
            Session["Enemy"] = enemyRule.gameBoard;
            Session["Shown"] = Shown;
            Session["AI"] = enemy;
            ViewData["message"] = "Your turn";
            return View("Game");
        }

        public ActionResult Replay()
        {
            var ctx = new CodeFirstContext();
            var data = ctx.Saves.ToList();
            Session["SaveLines"] = data;
            Session["Counter"] = 0;
            Session["Turn"] = false;
            return View();
        }

        [HttpPost]
        public ActionResult Replay(int index)
        {
            var lines = (List<Save>) Session["SaveLines"];
            var line = lines[index];
            var simulator = new GameSimulation();


            var Set = new Settings();
            Set.BoardSize = line.Board_size;
            Set.CanTouch = line.CanTouch;
            var Gb = new GameBoard(Set.BoardSize);
            var Rules = new Rules(Gb, Set);
            var Shown = new GameBoard(Set.BoardSize);
            var enemyRule = new Rules(new GameBoard(Set.BoardSize), Set);
            var Pt = new PlayersTurn(enemyRule.gameBoard, Gb, Shown, Set);
            var enemy = new AI(Gb, Set);

            var playerShips = line.Player_Board.Substring(0, line.Player_Board.Length - 1).Split('|');
            var aiShips = line.AI_Board.Substring(0, line.AI_Board.Length - 1).Split('|');
            simulator.PutShips(Rules, playerShips);
            simulator.PutShips(enemyRule, aiShips);

            var moves = line.Moves.Substring(0, line.Moves.Length - 1).Split('|');


            Session["Count"] = 1;
            Session["Moves"] = moves;
            Session["Rules"] = Rules;
            Session["ships"] = Rules.Ships;
            Session["Enemy"] = enemy;
            Session["Pt"] = Pt;
            Session["Shown"] = Shown;
            ViewData["message"] = "Your turn";
            return RedirectPermanent("/Home/Show");
        }


        [HttpPost]
        public ActionResult SetUp(int x, int y, bool toSave)
        {
            var Ships = (List<Ship>) Session["ships"];
            var i = (int) Session["index"];

            var rule = (Rules) Session["Rules"];
            rule.gameBoard.clean();

            if (toSave)
            {
                var lastX = (int) Session["x"];
                var lastY = (int) Session["y"];
                if (x == lastX)
                {
                    Ships[i].Horizontal = true;
                    if (y < lastY)
                    {
                        Ships[i].y = y;
                        rule.toSave(Ships[i]);
                    }
                    else
                    {
                        Ships[i].y = lastY;
                        rule.toSave(Ships[i]);
                    }
                }
                else
                {
                    Ships[i].Horizontal = false;
                    if (x < lastX)
                    {
                        Ships[i].x = x;
                        rule.toSave(Ships[i]);
                    }
                    else
                    {
                        Ships[i].x = lastX;
                        rule.toSave(Ships[i]);
                    }
                }

                i++;
            }


            if (toSave == false)
            {
                Ships[i].x = x;
                Ships[i].y = y;

                Session["x"] = x;
                Session["y"] = y;

                var test = Ships[i];


                test.Horizontal = true;
                test.y = y - (test.Len - 1);

                if (checkPos(rule.gameBoard, Ships[i]) &&
                    rule.goodPos(rule.underMove(test))) rule.gameBoard.Board[test.x][test.y] = BoardSquareState.Target;

                test.y = y;
                if (checkPos(rule.gameBoard, test) &&
                    rule.goodPos(rule.underMove(test)))
                    rule.gameBoard.Board[test.x][test.y + (test.Len - 1)] = BoardSquareState.Target;

                test.Horizontal = false;

                test.x = x - (test.Len - 1);
                if (checkPos(rule.gameBoard, test) &&
                    rule.goodPos(rule.underMove(test))) rule.gameBoard.Board[test.x][test.y] = BoardSquareState.Target;

                test.x = x;
                if (checkPos(rule.gameBoard, test) &&
                    rule.goodPos(rule.underMove(test)))
                    rule.gameBoard.Board[test.x + (test.Len - 1)][test.y] = BoardSquareState.Target;
            }


            if (i < Ships.Count)
            {
                ViewData["Ship"] = Ships[i];
                Session["ships"] = Ships;
                Session["index"] = i;
                return View();
            }

            Session["Ships"] = null;


            ViewData["message"] = "Your turn";
            return View("Game");
        }

        private bool checkPos(GameBoard gb, Ship ship)
        {
            if (ship.x < 0 || ship.y < 0 || !ship.Horizontal && ship.x + ship.Len > gb.Size ||
                ship.Horizontal && ship.y + ship.Len > gb.Size)
            {
                return false;
            }

            return true;
        }


        [HttpPost]
        public ActionResult Game(int x, int y)
        {
            ViewBag.Message = "Your extra turn";
            var Shown = (GameBoard) Session["Shown"];
            var Hiden = (GameBoard) Session["Enemy"];
            var Rules = (Rules) Session["Rules"];
            var Set = (Settings) Session["Set"];
            var AI = (AI) Session["AI"];
            PlayersTurn pt = new PlayersTurn(Hiden, Rules.gameBoard, Shown, Set);
            if (!pt.Shoot(x, y))
            {
                var str = new StringBuilder();
                var tup = AI.AI_turn();
                str.Append(tup.Item1 + " " + tup.Item2 + " ");
                while (tup.Item2)
                {
                    tup = AI.AI_turn();
                    str.Append(tup.Item1 + " " + tup.Item2 + " ");
                }

                ViewData["message"] = str.ToString();
            }

            int playerHp = Rules.gameBoard.GetHp();
            int AiHp = Hiden.GetHp();
            Session["playerHP"] = playerHp;
            Session["AiHP"] = AiHp;
            if (playerHp == 0 || AiHp == 0) return View("Ended");

            return View();
        }


        public ActionResult Ended()
        {
            return View("Index");
        }


        public ActionResult Show()
        {
            Session["number"] = 0;
            Session["Turn"] = true;
            return View();
        }

        [HttpPost]
        public ActionResult Show(String message)
        {
            var counter = (int) Session["number"];
            var simulator = new GameSimulation();
            var turn = (bool) Session["Turn"];
            var moves = Session["Moves"] as string[];
            var Pt = (PlayersTurn) Session["Pt"];
            var enemy = (AI) Session["Enemy"];


            if (Pt.Gb.GetHp() > 0 && Pt.Hiden.GetHp() > 0)
            {
                turn = simulator.SimulateOneTurn(moves[counter], turn, Pt, enemy);
                counter = counter + 1;
                Session["number"] = counter;
                Session["Moves"] = moves;
                Session["Turn"] = turn;
                Session["AI"] = enemy;
                Session["Pt"] = Pt;
            }
            else
            {
                return RedirectPermanent("/Home/Index");
            }


            return View();
        }


        public ActionResult ToSave(int ended)
        {
            var Hiden = (GameBoard) Session["Enemy"];
            var Rules = (Rules) Session["Rules"];
            var Set = (Settings) Session["Set"];
            var gb = Rules.gameBoard;

            var dc = new CodeFirstContext();
            var aiBoard = Hiden.BoardToString();
            var playerBoard = gb.BoardToString();
            var time = DateTime.Now.ToString();
            var save = new Save(time, playerBoard, aiBoard, gb.Size, gb.MoveString.ToString(), ended, Set.CanTouch);
            dc.Saves.Add(save);
            dc.SaveChanges();
            if (ended == 1) return View("Index");
            return View("Game");
        }
    }
}