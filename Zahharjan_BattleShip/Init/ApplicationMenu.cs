using System.Collections.Generic;
using Zahharjan_BattleShip.MenuSystem;

namespace Zahharjan_BattleShip.Init
{
    class ApplicationMenu
    {
        public static readonly Menu SettingMenu;
        public static readonly Menu MainMenu;

        static ApplicationMenu()
        {
            SettingMenu = new Menu()
            {
                Title = "Settings",
            };
            SettingMenu.MenuItems.Add(
                "S", new MenuItem()
                {
                    LongDescription = "Back to Settings",
                    Shortcuts = new HashSet<string>() { "S" },
                    CommandToExecute = null,
                    IsDefaultChoice = true,

                }
            );


            MainMenu = new Menu()
            {
                Title = "Main menu: Connect-X",
                IsMainMenu = true,
            };
            MainMenu.MenuItems.Add(
                "1", new MenuItem()
                {
                    LongDescription = "New game",
                    Shortcuts = new HashSet<string>() { "1" },
                    CommandToExecute = null,
                    IsDefaultChoice = true
                }
            );
            MainMenu.MenuItems.Add("2", new MenuItem()
            {
                LongDescription = "Settings",
                Shortcuts = new HashSet<string>() { "2" },
                CommandToExecute = null,
                IsDefaultChoice = true
            });
            MainMenu.MenuItems.Add("3", new MenuItem()
            {
                LongDescription = "Load",
                Shortcuts = new HashSet<string>() { "3" },
                CommandToExecute = null,
                IsDefaultChoice = true
            });
            MainMenu.MenuItems.Add("4", new MenuItem()
            {
                LongDescription = "Replay",
                Shortcuts = new HashSet<string>() { "4" },
                CommandToExecute = null,
                IsDefaultChoice = true
            });
        }
    }
}
