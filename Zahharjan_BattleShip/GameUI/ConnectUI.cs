using System;
using ClassLab;


namespace Zahharjan_BattleShip.GameUI
{
    class ConnectUI
    {
        public GameBoard Player { get; set; }
        public Settings Settings { get; set; }

        public ConnectUI(Settings settings)
        {
            Settings = settings;


            //ApplicationMenu.SettingMenu.ClearScreenInMenuStart = false;
        }

        public string RunGame(string command)
        {
            Player = new GameBoard(Settings.BoardSize);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.WriteLine(Player.GetBoardString());
            Rules rules = new Rules(Player, Settings);
            Player = rules.Start(Settings.RandomPos);
            Play play = new Play(Player, Settings);
            play.Game();
            return "";
        }
    }
}