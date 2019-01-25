namespace ClassLab
{
//Class process code of players move
    public class PlayersTurn
    {
        public GameBoard Hiden;
        public GameBoard Gb;
        private GameBoard Shown;
        private Settings Set;

        public PlayersTurn(GameBoard hiden, GameBoard gb, GameBoard shown, Settings set)
        {
            Hiden = hiden;
            Gb = gb;
            Shown = shown;
            Set = set;
        }


        public bool Shoot(int x, int y)
        {
            Gb.MoveString.Append(x + ":" + y + "|");
            if (Hiden.Board[x][y] == BoardSquareState.Ship)
            {
                Shown.Board[x][y] = BoardSquareState.Hit;
                foreach (var ship in Hiden.Ships)
                {
                    foreach (var body in ship.Body)
                    {
                        if (body[0] == x && body[1] == y)
                        {
                            ship.Health--;
                            if (ship.Health == 0)
                            {
                                Dead(ship);
                            }

                            return true;
                        }
                    }
                }
            }
            else Shown.Board[x][y] = BoardSquareState.Miss;


            return false;
        }

        //choose ship state dead
        public void Dead(Ship ship)
        {
            for (int i = 0; i < ship.Len; i++)
            {
                if (ship.Horizontal) Shown.Board[ship.x][ship.y + i] = BoardSquareState.Dead;
                else Shown.Board[ship.x + i][ship.y] = BoardSquareState.Dead;
            }

            if (Set.CanTouch == false) Shown.cover(ship);
        }
    }
}