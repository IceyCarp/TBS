using Game.Class;
using System.ComponentModel.Design;
using System.Numerics;
using System.Runtime.CompilerServices;


public class Encyclopedia
{
    private static int itemsearch = 0;
    private static int currentPage = 1;
    private static int itemsPerPage = 9;
    private static string searchTerm = "";
    private static List<Item> filteredItems;

    public static void EncyclopediaLogic()
    {
        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Welcome to the Encyclopedia!");
            MainUI.WriteInMainArea($"What would you like to view?\n");
            MainUI.WriteInMainArea("1 : View Items");
            MainUI.WriteInMainArea($"2 : Bestiary ({Program.player.knownEnemies.Count} discovered)");
            MainUI.WriteInMainArea("0 : Leave");

            if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input) == false || input > 2 || input < 0)
            {
                MainUI.WriteInMainArea(" \nyou gotta type a number from 0-2");
                Thread.Sleep(1000);
                continue;
            }

            switch (input)
            {
                case 0:
                    Program.MainMenu();
                    return;
                case 1:
                    ViewItems();
                    break;
                case 2:
                    ViewEnemies();
                    break;
            }
            Program.SavePlayer();
        }
    }

    public static void ViewEnemies()
    {
        int enemyPage = 1;
        int enemiesPerPage = 9;
        string enemySearch = "";

        while (true)
        {
            var allEnemies = Program.player.knownEnemies;

            var filtered = string.IsNullOrEmpty(enemySearch)
                ? allEnemies
                : allEnemies.Where(e => e.name.ToLower().Contains(enemySearch.ToLower())).ToList();

            filtered = filtered.OrderBy(e => e.name).ToList();

            int totalPages = Math.Max(1, (int)Math.Ceiling((double)filtered.Count / enemiesPerPage));
            if (enemyPage > totalPages) enemyPage = totalPages;
            if (enemyPage < 1) enemyPage = 1;

            var pageEnemies = filtered.Skip((enemyPage - 1) * enemiesPerPage).Take(enemiesPerPage).ToList();

            MainUI.ClearMainArea();

            if (!string.IsNullOrEmpty(enemySearch))
                MainUI.WriteInMainArea($"Showing results for: \"{enemySearch}\"");

            if (allEnemies.Count == 0)
            {
                MainUI.WriteInMainArea("You haven't defeated any enemies yet.");
                MainUI.WriteInMainArea("\nPress Enter to go back...");
                Console.ReadLine();
                return;
            }

            MainUI.WriteInMainArea($"--- Bestiary ({allEnemies.Count} discovered) ---\n");
            MainUI.WriteInMainArea("nr     Name                   Lvl    HP     EXP    Rai");
            MainUI.WriteInMainArea("---------------------------------------------------------");

            for (int i = 0; i < pageEnemies.Count; i++)
            {
                var e = pageEnemies[i];
                MainUI.WriteInMainArea($"{i + 1,-7}{e.name,-23} {e.level,-7}{e.maxHP,-7}{e.exp,-7}{e.money}");
            }

            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"--- Page {enemyPage} of {totalPages} ---");
            MainUI.WriteInMainArea("[N] Next  [P] Prev  [S] Search  [0] Back  or number for details");

            string key = Console.ReadKey(true).KeyChar.ToString().ToLower();

            if (key == "n") { if (enemyPage < totalPages) enemyPage++; continue; }
            if (key == "p") { if (enemyPage > 1) enemyPage--; continue; }
            if (key == "0") return;
            if (key == "s")
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("Search enemy name:");
                enemySearch = Console.ReadLine()?.ToLower() ?? "";
                enemyPage = 1;
                continue;
            }

            if (int.TryParse(key, out int pick) && pick >= 1 && pick <= pageEnemies.Count)
            {
                var selected = pageEnemies[pick - 1];
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea($"=== {selected.name} ===\n");
                MainUI.WriteInMainArea($"Level:         {selected.level}");
                MainUI.WriteInMainArea($"HP:            {selected.maxHP}");
                MainUI.WriteInMainArea($"Speed:         {selected.speed}");
                MainUI.WriteInMainArea($"Armor:         {selected.armor}");
                MainUI.WriteInMainArea($"Dodge:         {selected.dodge}%");
                MainUI.WriteInMainArea($"Crit Chance:   {selected.critChance}%");
                MainUI.WriteInMainArea($"Stun:          {selected.stun}");
                MainUI.WriteInMainArea($"EXP reward:    {selected.exp}");
                MainUI.WriteInMainArea($"Rai reward:    {selected.money}");

                if (selected.materialDrops != null && selected.materialDrops.Count > 0)
                {
                    MainUI.WriteInMainArea("\nDrops:");
                    foreach (var drop in selected.materialDrops)
                    {
                        int pct = (int)(drop.DropChance * 100);
                        MainUI.WriteInMainArea($"  {drop.Material.name}  x{drop.MinQuantity}-{drop.MaxQuantity}  ({pct}%)");
                    }
                }
                else
                {
                    MainUI.WriteInMainArea("\nNo material drops.");
                }

                MainUI.WriteInMainArea("\nPress Enter to go back...");
                Console.ReadLine();
            }
        }
    }

    public static void ViewItems()
    {
        while (true)
        {
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("Welcome to the Encyclopedia!");
            MainUI.WriteInMainArea($"What type of item would you like to view?\n");
            MainUI.WriteInMainArea("1 : View Consumables");
            MainUI.WriteInMainArea("2 : View Equipment");
            MainUI.WriteInMainArea("3 : View Artifacts");
            MainUI.WriteInMainArea("4 : View Materials");
            MainUI.WriteInMainArea("0 : Back");

            if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input) == false || input > 4 || input < 0)
            {
                MainUI.WriteInMainArea("\nyou gotta type 0, 1, 2, 3, or 4");
                MainUI.WriteInMainArea("Press enter to continue...");
                Console.ReadLine();
                EncyclopediaLogic();
                return;
            }
            else if (input == 0) EncyclopediaLogic();
            else if (input == 1) itemsearch = 0;
            else if (input == 2) itemsearch = 1;
            else if (input == 3) itemsearch = 2;
            else if (input == 4) itemsearch = 3;
            break;
        }

        while (true)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                filteredItems = Program.player.knownItems.Where(item => item.type.Equals((ItemType)itemsearch))
                .ToList();
            }
            else
            {
                filteredItems = Program.player.knownItems
                    .Where(item => item.name.ToLower().Contains(searchTerm.ToLower()) &&
                                   item.type.Equals((ItemType)itemsearch) ||
                                   item.description.ToLower().Contains(searchTerm.ToLower()) &&
                                   item.type.Equals((ItemType)itemsearch) ||
                                   item.GetDescription().ToLower().Contains(searchTerm.ToLower()) &&
                                   item.type.Equals((ItemType)itemsearch))
                    .ToList();
            }

            int totalItems = filteredItems.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            if (totalPages == 0) totalPages = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;

            filteredItems.Sort((x, y) => string.Compare(x.name, y.name));

            List<Item> pageItems = filteredItems
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            MainUI.ClearMainArea();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                MainUI.WriteInMainArea($"\nShowing results for: \"{searchTerm}\"");
            }

            MainUI.WriteInMainArea("");
            if (itemsearch == 0)
            {
                MainUI.WriteInMainArea("nr     Name                   Value         Usable         Weight");
            }
            else if (itemsearch == 1)
            {
                MainUI.WriteInMainArea("nr     Name                   Value          Slot         Weight");
            }
            else
            {
                MainUI.WriteInMainArea("nr     Name                   Value          Type         Weight");
            }

            MainUI.WriteInMainArea("-----------------------------------------------------------------");
            int i = 0;
            foreach (var item in pageItems)
            {
                i++;
                if (itemsearch == 0)
                {
                    string usable = "";
                    if (item.duration == 0) { usable = "Anywhere"; }
                    else if (item.duration > 0) { usable = "Battle"; }
                    else { usable = "Overworld"; }
                    MainUI.WriteInMainArea($"{i,-7}{item.name,-24} {item.value,-11} {usable,-16} {item.weight}");
                }
                else if (itemsearch == 1)
                {
                    MainUI.WriteInMainArea($"{i,-7}{item.name,-24} {item.value,-12} {item.equipmentType,-16} {item.weight}");
                }
                else
                {
                    MainUI.WriteInMainArea($"{i,-7}{item.name,-24} {item.value,-10} {item.type,-17} {item.weight}");
                }
            }

            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"--- Page {currentPage} of {totalPages} ---");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Type item number (1-9) to interact, or:");
            MainUI.WriteInMainArea("[N] Next Page  [P] Prev Page  [S] Search  [0] Back to Main Menu");

            string inputString = Console.ReadKey(true).KeyChar.ToString().ToLower() ?? "";

            if (inputString == "n") { if (currentPage < totalPages) currentPage++; continue; }
            if (inputString == "p") { if (currentPage > 1) currentPage--; continue; }
            if (inputString == "s") { HandleSearch(); continue; }

            var n = int.TryParse(inputString, out int inp);

            if (inp == 0) { Program.MainMenu(); return; }

            if (!n || inp < 1 || inp > pageItems.Count)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("sweetie you gotta type a usable number *from this page* ");
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            Item selectedItem = pageItems[inp - 1];

            MainUI.ClearMainArea();
            MainUI.WriteInMainArea($"you've picked {selectedItem.name}");
            MainUI.WriteInMainArea("0 : cancel");
            MainUI.WriteInMainArea("1 : details");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("type out the number next to the action you want to perform");

            var k = int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int ik);
            if (!k || ik < 0 || ik > 1)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("my love would you please type a number this time\n ");
                MainUI.WriteInMainArea("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }
            else if (ik == 0) { continue; }
            else if (ik == 1)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea($"you've picked {selectedItem.name}\n");
                MainUI.WriteInMainArea($"\n{selectedItem.GetDescription()}\n");
                MainUI.WriteInMainArea($"Press Enter to continue...");
                Console.ReadLine();
                continue;
            }

            Program.SavePlayer();
            Program.MainMenu();
            break;
        }
    }

    private static void HandleSearch()
    {
        MainUI.ClearMainArea();
        MainUI.WriteInMainArea("Enter search term (or leave empty to clear):");
        MainUI.WriteInMainArea("> ");
        searchTerm = Console.ReadLine()?.ToLower() ?? "";
        currentPage = 1;
    }
}