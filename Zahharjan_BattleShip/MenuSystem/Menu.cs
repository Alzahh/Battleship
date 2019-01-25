using System;
using System.Collections.Generic;
using System.Linq;

namespace Zahharjan_BattleShip.MenuSystem
{
    class Menu
    {
        public string Title { get; set; }

        public Dictionary<string, MenuItem> MenuItems { get; } = new Dictionary<string, MenuItem>()
        {
            {
                "X", new MenuItem()
                {
                    LongDescription = "Go back!",
                    Shortcuts = new HashSet<string>() {"X", "BACK", "GOBACK"},
                    MenuItemType = MenuItemType.GoBackOneLevel
                }
            },

            {
                "Q", new MenuItem()
                {
                    LongDescription = "Quit to main menu!",
                    Shortcuts = new HashSet<string>() {"Q", "QUIT", "HOME"},
                    MenuItemType = MenuItemType.GoBackToMain
                }
            }
        };

        public bool ClearScreenInMenuStart { get; set; } = true;


        public bool DisplayQuitToMainMenu { get; set; } = true;
        public bool IsMainMenu { get; set; } = false;

        private KeyValuePair<string, MenuItem> goBackItem;
        private KeyValuePair<string, MenuItem> quitToMainItem;

        private void PrintMenu()
        {
            var defaultMenuChoice = MenuItems.FirstOrDefault(m => m.Value.IsDefaultChoice == true);

            if (ClearScreenInMenuStart)
            {
                Console.Clear();
            }

            Console.WriteLine("-------- " + Title + "--------");
            foreach (var dictionaryItem in MenuItems.Where(m => m.Value.MenuItemType == MenuItemType.Regular))
            {
                var menuItem = dictionaryItem.Value;

                if (menuItem.IsDefaultChoice)
                {
                    Console.ForegroundColor =
                        ConsoleColor.Red;

                    Console.Write(dictionaryItem.Key);
                    Console.WriteLine(menuItem);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(dictionaryItem.Key);
                    Console.WriteLine(menuItem);
                }
            }

            Console.WriteLine("--------");


            // Go back one level menu item
            if (goBackItem.Value != null)
            {
                Console.WriteLine(goBackItem.Key + goBackItem.Value);
            }

            // Go back to main menu menu item
            if (DisplayQuitToMainMenu)
            {
                if (quitToMainItem.Value != null)
                {
                    Console.WriteLine(quitToMainItem.Key + quitToMainItem.Value);
                }
            }

            Console.Write(
                defaultMenuChoice.Value == null ? ">" : "[" + defaultMenuChoice.Value.Shortcuts.First() + "]>"
            );
        }


        private void WaitForUser()
        {
            Console.Write("Press any key to continue!");
            Console.ReadKey();
        }

        public string RunMenu()
        {
            goBackItem = MenuItems.FirstOrDefault(m => m.Value.MenuItemType == MenuItemType.GoBackOneLevel);
            quitToMainItem = MenuItems.FirstOrDefault(m => m.Value.MenuItemType == MenuItemType.GoBackToMain);

            var done = true;
            string input;
            do
            {
                done = false;

                PrintMenu();
                input = Console.ReadLine().ToUpper().Trim();


                var GoBackItem = MenuItems.FirstOrDefault(m => m.Value.MenuItemType == MenuItemType.GoBackOneLevel);

                // shall we exit from this menu
                if (GoBackItem.Value.Shortcuts.Any(s => s == input))
                {
                    break; // jump out of the loop
                }

                if (DisplayQuitToMainMenu && quitToMainItem.Value.Shortcuts.Any(s => s.ToUpper() == input))
                {
                    break; // jump out of the loop
                }


                // find the correct menu item
                MenuItem item = null;
                item = string.IsNullOrWhiteSpace(input)
                    ? MenuItems.FirstOrDefault(m => m.Value.IsDefaultChoice == true).Value
                    // dig out item, where this input is in its shortcuts
                    : MenuItems.FirstOrDefault(m => m.Value.Shortcuts.Contains(input)).Value;

                if (item == null)
                {
                    Console.WriteLine(input + " was not found in the list of commands!");
                    WaitForUser();
                    continue; // jump back to the start of loop
                }

                // execute the command specified in the menu item
                if (item.CommandToExecute == null)
                {
                    Console.WriteLine(input + " has no command assigned to it!");
                    WaitForUser();
                    continue; // jump back to the start of loop
                }

                // everything should be ok now, lets run it!
                var chosenCommand = item.CommandToExecute(input);
                input = chosenCommand;

                if (IsMainMenu == false && quitToMainItem.Value.Shortcuts.Contains(chosenCommand))
                {
                    break;
                }

                if (!goBackItem.Value.Shortcuts.Contains(chosenCommand) &&
                    !quitToMainItem.Value.Shortcuts.Contains(chosenCommand))
                    WaitForUser();
            } while (done != true);


            return input;
        }
    }
}