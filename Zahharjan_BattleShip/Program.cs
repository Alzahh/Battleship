using System.Linq;
using Zahharjan_BattleShip.GameUI;
using Zahharjan_BattleShip.Init;
using ClassLab;

namespace Zahharjan_BattleShip
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();

            var settMenu = ApplicationMenu.MainMenu.MenuItems["2"];
            settMenu.CommandToExecute = settings.RunSett;

            var loading = new Load(false);
            var replaying = new Load(true);

            var load = ApplicationMenu.MainMenu.MenuItems["3"];
            load.CommandToExecute = loading.LoadGame;

            var replay = ApplicationMenu.MainMenu.MenuItems["4"];
            replay.CommandToExecute = replaying.LoadGame;


            var gameUi = new ConnectUI(settings);
            var menuItemNewGame = ApplicationMenu.MainMenu.MenuItems.First(m => m.Value.LongDescription == "New game");
            menuItemNewGame.Value.CommandToExecute = gameUi.RunGame;


            ApplicationMenu.MainMenu.RunMenu();
        }
    }
}