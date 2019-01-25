using System.ComponentModel.DataAnnotations;

namespace ClassLab
{
    public class Save
    {
        [Key] public int Save_id { get; set; }
        public string Time { get; set; }
        public string Player_Board { get; set; }
        public string AI_Board { get; set; }
        public int Board_size { get; set; }
        public string Moves { get; set; }
        public int Ended { get; set; }
        public bool CanTouch { get; set; }


        public Save(string time, string playerBoard, string aiBoard, int boardSize, string moves, int ended,
            bool canTouch)
        {
            Time = time;
            Player_Board = playerBoard;
            AI_Board = aiBoard;
            Board_size = boardSize;
            Moves = moves;
            Ended = ended;
            CanTouch = canTouch;
        }

        public Save()
        {
        }
    }
}