using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ClassLab
{
    /*Artificial Intelligence class.
     */
    public class AI
    {
        private GameBoard playersBoard;
        public List<int[]> moves;
        public List<int[]> recomended;
        private List<int[]> goodHits;
        private Settings Set;
        private TargetEnum tn;

        public AI(GameBoard gb, Settings set)
        {
            Set = set;
            playersBoard = gb;
            moves = new List<int[]>();
            recomended = new List<int[]>();
            goodHits = new List<int[]>();
            tn = TargetEnum.None;
            for (int i = 0; i < playersBoard.Size; i++)
            {
                for (int j = 0; j < playersBoard.Size; j++)
                {
                    moves.Add(new[] {i, j});
                }
            }
        }

        /* Ai_Turn. Method simulate second players turn.
         AI choose the position randomly if there no recommended moves
        */
        public Tuple<String, bool> AI_turn()
        {
            Random rnd = new Random();
            bool secondTurn;
            if (recomended == null || recomended.Count == 0)
            {
                // no recommended moves
                int r = rnd.Next(moves.Count - 1);
                secondTurn = AI_shoot(moves[r][0], moves[r][1]);
                return Tuple.Create(moves[r][0] + "-" + (moves[r][1] - 1) + " hit: ", secondTurn);
            }

            //with recommended moves
            int r1 = rnd.Next(recomended.Count - 1);
            int x = recomended[r1][0];
            int y = recomended[r1][1];
            recomended.RemoveAt(r1);
            secondTurn = AI_shoot(x, y);
            return Tuple.Create(x + "-" + y + " hit: ", secondTurn);
        }

        public bool AI_shoot(int x, int y)
        {
            //add move to DB
            playersBoard.MoveString.Append(x + ":" + y + "|");
            //delete move from move list
            deleteMove(x, y);
            if (playersBoard.Board[x][y] == BoardSquareState.Ship)
            {
                playersBoard.Board[x][y] = BoardSquareState.Hit;
                goodHits.Add(new[] {x, y});
                // generate new recommended moves
                recomended = GetRecomended(x, y);

                foreach (var ship in playersBoard.Ships)
                {
                    foreach (var body in ship.Body)
                    {
                        if (body[0] == x && body[1] == y)
                        {
                            ship.Health--;
                            if (ship.Health == 0) AI_kill(ship);
                            return true;
                        }
                    }
                }
            }
            else playersBoard.Board[x][y] = BoardSquareState.Miss;

            return false;
        }

        public void AI_kill(Ship ship)
        {
            //turning ship into dead station
            for (int i = 0; i < ship.Len; i++)
            {
                int x, y;
                if (ship.Horizontal)
                {
                    x = ship.x;
                    y = ship.y + i;
                }
                else
                {
                    x = ship.x + i;
                    y = ship.y;
                }

                playersBoard.Board[x][y] = BoardSquareState.Dead;
            }

            tn = TargetEnum.None;
            goodHits = new List<int[]>();
            recomended = new List<int[]>();
            if (Set.CanTouch == false) AI_cover(ship);
        }

        //method draw borders near ship if needed
        public void AI_cover(Ship ship)
        {
           
            if (!ship.Horizontal)
            {
                for (int i = 0; i < ship.Len + 2; i++)
                {
                    int pos0 = ship.x - 1 + i;
                    int pos1 = ship.y - 1;
                    if (pos0 <= playersBoard.Size - 1 && pos0 >= 0 && pos1 >= 0)
                    {
                        deleteMove(pos0, pos1);
                    }

                    pos1 = ship.y + 1;
                    if (pos0 <= playersBoard.Size - 1 && pos0 >= 0 && pos1 < playersBoard.Size)
                        deleteMove(pos0, pos1);
                }

                if (ship.x - 1 >= 0) deleteMove(ship.x - 1, ship.y);
                if (ship.x + ship.Len < playersBoard.Size)
                    deleteMove(ship.x + ship.Len, ship.y);
            }
            else
            {
                for (int i = 0; i < ship.Len + 2; i++)
                {
                    int pos0 = ship.x - 1;
                    int pos1 = ship.y - 1 + i;
                    if (pos0 >= 0 && pos1 >= 0 && pos1 <= playersBoard.Size - 1)
                    {
                        deleteMove(pos0, pos1);
                    }

                    pos0 = ship.x + 1;
                    if (pos0 <= playersBoard.Size - 1 && pos1 >= 0 && pos1 < playersBoard.Size)
                        deleteMove(pos0, pos1);
                }

                if (ship.y - 1 >= 0) deleteMove(ship.x, ship.y - 1);
                if (ship.y + ship.Len < playersBoard.Size)
                    deleteMove(ship.x, ship.y + +ship.Len);
            }
        }

        //Method marks moves as recommended
        private List<int[]> GetRecomended(int x, int y)
        {
            if (goodHits.Count == 2 && tn == TargetEnum.None)
            {
                if (goodHits[0][0] != goodHits[1][0] && goodHits[0][1] == goodHits[1][1]) tn = TargetEnum.Vertical;
                else if (goodHits[0][0] == goodHits[1][0] && goodHits[0][1] != goodHits[1][1])
                    tn = TargetEnum.Horizontal;
            }


            List<int[]> lst = new List<int[]>();
            lst.AddRange(recomended);
            //Recognize if ship is horizontal or vertical
            if (tn == TargetEnum.None && (recomended == null || recomended.Count == 0))
            {
               
                if (goodHits[0][0] + 1 < playersBoard.Size) lst.Add(new[] {goodHits[0][0] + 1, goodHits[0][1]});
                if (goodHits[0][0] - 1 >= 0) lst.Add(new[] {goodHits[0][0] - 1, goodHits[0][1]});
                if (goodHits[0][1] + 1 < playersBoard.Size) lst.Add(new[] {goodHits[0][0], goodHits[0][1] + 1});
                if (goodHits[0][1] - 1 >= 0) lst.Add(new[] {goodHits[0][0], goodHits[0][1] - 1});
            }
            //add new moves based on ship direction
            else if (tn == TargetEnum.Horizontal)
            {
                var minMax = GetMinAndMax(1);
                lst.Add(new[] {x, minMax[1] + 1});
                lst.Add(new[] {x, minMax[0] - 1});

                List<int[]> copy = new List<int[]>();
                copy.AddRange(lst.Distinct().ToList());

                foreach (int[] element in lst)
                {
                    if (element[0] != x || !inMoves(element) || goodHits.Contains(element)) copy.Remove(element);
                }

                return copy;
            }
            else if (tn == TargetEnum.Vertical)
            {
                var minMax = GetMinAndMax(0);
                lst.Add(new[] {minMax[1] + 1, y});
                lst.Add(new[] {minMax[0] - 1, y});

                List<int[]> copy = new List<int[]>();
                copy.AddRange(lst.Distinct().ToList());

                foreach (int[] element in lst)
                {
                    if (element[1] != y || !inMoves(element) || goodHits.Contains(element)) copy.Remove(element);
                }

                return copy;
            }

            return lst;
        }

        //Method gaves moves to fallow direction
        private int[] GetMinAndMax(int index)
        {
            int max = 0;
            int min = 9;
            foreach (var hit in goodHits)
            {
                if (hit[index] > max) max = hit[index];
                if (hit[index] < min) min = hit[index];
            }

            return new[] {min, max};
        }

        //check if move in possible moves
        private bool inMoves(int[] e)
        {
            foreach (var move in moves)
            {
                if (move[0] == e[0] && move[1] == e[1]) return true;
            }

            return false;
        }

        //makes move impossible
        private void deleteMove(int x, int y)
        {
            List<int[]> nList = new List<int[]>();
            nList.AddRange(moves);
            foreach (var move in moves)
            {
                if (move[0] == x && move[1] == y) nList.Remove(move);
            }

            moves = nList;
        }
    }
}