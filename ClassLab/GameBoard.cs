using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassLab
{
    public class GameBoard
    {
        public List<List<BoardSquareState>> Board { get; set; } = new List<List<BoardSquareState>>();
        public int Size;
        public List<Ship> Ships { get; set; } = new List<Ship>();
        public StringBuilder MoveString = new StringBuilder();


        public GameBoard(int boardRows = 10)

        {
            Size = boardRows;
            for (int i = 0; i < boardRows; i++)
            {
                Board.Add(new List<BoardSquareState>());
                for (int j = 0; j < boardRows; j++)
                {
                    Board[i].Add(BoardSquareState.NotUsed);
                }
            }
        }


        // FIXME: UI methods don't belong in Domain!!!!!

        // ┼───┼───┼───┼    +───+───+───+
        // │ X │ O │   │    │ X │ O │   │
        // ┼───┼───┼───┼    +───+───+───+

        public string GetBoardString()
        {
            var sb = new StringBuilder(" ");
            for (int x = 0; x < Size; x++)
            {
                if (x.ToString().Length == 1) sb.Append("   " + x);
                else sb.Append("  " + x);
            }

            sb.Append("\n");

            int i = 0;
            foreach (var boardRow in Board)
            {
                sb.Append("  " + GetRowSeparator(boardRow.Count) + "\n");
                if (i < 10) sb.Append(i + " " + GetRowWithData(boardRow) + "\n");
                else sb.Append(i + GetRowWithData(boardRow) + "\n");
                i++;
            }

            sb.Append("  " + GetRowSeparator(Board[0].Count));
            return sb.ToString();
        }

        public string DisplayTables(GameBoard gb)
        {
            string[] first = Regex.Split(GetBoardString(), "[\r\n]+");
            string[] second = Regex.Split(gb.GetBoardString(), "[\r\n]+");
            var sb = new StringBuilder();
            for (int i = 0; i < first.Length; i++)
            {
                sb.Append(first[i] + "\t" + second[i] + "\n");
            }

            return sb.ToString();
        }


        public string GetRowSeparator(int elemCountInRow)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < elemCountInRow; i++)
            {
                sb.Append("┼───");
            }

            sb.Append("┼");
            return sb.ToString();
        }

        public string GetRowWithData(List<BoardSquareState> boardRow)
        {
            var sb = new StringBuilder();
            foreach (var boardSquareState in boardRow)
            {
                sb.Append("│ " + GetBoardSquareStateSymbol(boardSquareState) + " ");
            }

            sb.Append("│");
            return sb.ToString();
        }

        public string GetBoardSquareStateSymbol(BoardSquareState state)
        {
            switch (state)
            {
                case BoardSquareState.NotUsed: return " ";
                case BoardSquareState.Hit: return "X";
                case BoardSquareState.Dead: return "D";
                case BoardSquareState.Miss: return "-";
                case BoardSquareState.Border: return "^";
                case BoardSquareState.Ship: return "■";
                case BoardSquareState.Target: return "O";

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }


        public int GetHp()
        {
            int i = 0;
            foreach (var ship in Ships)
            {
                i += ship.Health;
            }

            return i;
        }

        public string BoardToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ship in Ships)
            {
                sb.Append(ship.x + ":" + ship.y + ":" + ship.Len + ":" + ship.Horizontal + "|");
            }

            return sb.ToString();
        }

        public void clean()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Board[i][j] == BoardSquareState.Target) Board[i][j] = BoardSquareState.NotUsed;
                }
            }
        }

        public void cover(Ship ship)
        {
            if (!ship.Horizontal)
            {
                for (int i = 0; i < ship.Len + 2; i++)
                {
                    int pos0 = ship.x - 1 + i;
                    int pos1 = ship.y - 1;
                    if (pos0 <= Size - 1 && pos0 >= 0 && pos1 >= 0)
                    {
                        Board[pos0][pos1] = BoardSquareState.Border;
                    }

                    pos1 = ship.y + 1;
                    if (pos0 <= Size - 1 && pos0 >= 0 && pos1 < Size)
                        Board[pos0][pos1] = BoardSquareState.Border;
                }

                if (ship.x - 1 >= 0) Board[ship.x - 1][ship.y] = BoardSquareState.Border;
                if (ship.x + ship.Len < Size)
                    Board[ship.x + ship.Len][ship.y] = BoardSquareState.Border;
            }
            else
            {
                for (int i = 0; i < ship.Len + 2; i++)
                {
                    int pos0 = ship.x - 1;
                    int pos1 = ship.y - 1 + i;
                    if (pos0 >= 0 && pos1 >= 0 && pos1 <= Size - 1)
                    {
                        Board[pos0][pos1] = BoardSquareState.Border;
                    }

                    pos0 = ship.x + 1;
                    if (pos0 <= Size - 1 && pos1 >= 0 && pos1 < Size)
                        Board[pos0][pos1] = BoardSquareState.Border;
                }

                if (ship.y - 1 >= 0) Board[ship.x][ship.y - 1] = BoardSquareState.Border;
                if (ship.y + ship.Len < Size)
                    Board[ship.x][ship.y + +ship.Len] = BoardSquareState.Border;
            }
        }
    }
}