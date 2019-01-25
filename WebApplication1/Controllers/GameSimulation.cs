using System;
using System.Threading;
using ClassLab;

namespace WebApplication1.Controllers
{
    public class GameSimulation
    {
        public GameSimulation()
        {
        }

        public void PutShips(Rules rules, string[] ships)
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

        public void Simulate(string[] moves, PlayersTurn pt, AI enemy, GameBoard gb, GameBoard shown)
        {
            bool playersTurn = true;

            foreach (var move in moves)
            {
                var pos = move.Split(':');
                if (playersTurn)
                {
                    playersTurn = pt.Shoot(int.Parse(pos[0]), int.Parse(pos[1]));
                }
                else
                {
                    playersTurn = !enemy.AI_shoot(int.Parse(pos[0]), int.Parse(pos[1]));
                }
            }
        }

        public bool SimulateOneTurn(string move, bool playerTurn, PlayersTurn pt, AI enemy)
        {
            var pos = move.Split(':');
            if (playerTurn)
            {
                playerTurn = pt.Shoot(int.Parse(pos[0]), int.Parse(pos[1]));
            }
            else
            {
                playerTurn = !enemy.AI_shoot(int.Parse(pos[0]), int.Parse(pos[1]));
            }

            return playerTurn;
        }
    }
}