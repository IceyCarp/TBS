using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Net.Http;
using System.Reflection.Emit;

namespace Game.Class
{
    public class Program
    {
        private const string CURRENT_VERSION = "1.1.1"; // Update this with each release
        private const string GITHUB_API_URL = "https://api.github.com/repos/EKDixen/TBS/releases/latest";

        static bool stopMultibleLoad = false;
        public static Player? player = null;
        public static PlayerDatabase db = new PlayerDatabase();
        static JourneyManager journeyManager = new JourneyManager();
        static AttackManager atkManager;
        static CancellationTokenSource? idleCheckCancellation = null;

        public static Player? pendingDeadPlayerUpdate = null;
        public static Enemy? pendingSpiritEnemy = null;

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("\nDisconnecting...");
                DisconnectPlayer();
                Environment.Exit(0);
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                DisconnectPlayer();
            };

            // Check for updates before starting the game
            CheckForUpdates();

            // Lock console window at startup (prevents resizing, which fucks up the UI)
            var ui = new CombatUI();
            ui.InitializeConsole();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=====================================");
            Console.WriteLine("         WELCOME TO TBS!!!           ");
            Console.WriteLine("=====================================");
            Console.ResetColor();
            while (true)
            {
                Console.WriteLine("\nPress enter to continue...");
                Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Welcome! Do you want to:");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Create a new character");

                string choice = Console.ReadKey(true).KeyChar.ToString();

                if (choice == "1")
                {
                    Console.Write("\nEnter username: ");
                    string username = Console.ReadLine();

                    if (username == "panel69420admin")
                    {
                        AdminTools.ShowAdminMenu();
                        continue;
                    }

                    Console.Write("Enter password: ");
                    string password = Console.ReadLine();
                    if (username == null) continue;
                    player = db.LoadPlayer(username, password);

                    if (player != null)
                    {
                        if (player.isDead)
                        {
                            ShowDeadCharacterScreen(player);
                            continue;
                        }

                        Console.WriteLine($"Welcome back, {player.name} (Level {player.level})!");
                        Inventory.MakeInv();
                        
                        StartIdleTimeoutMonitoring();
                        break;
                    }
                    else
                    {
                        var deadPlayer = db.LoadDeadPlayer(username);
                        if (deadPlayer != null && deadPlayer.password == password)
                        {
                            ShowDeadCharacterScreen(deadPlayer);
                            continue;
                        }

                        Console.WriteLine("\nInvalid username or password.");
                        continue;
                    }
                }
                else if (choice == "2")
                {
                    const int MaxUsernameLength = 12;
                    string name;
                    bool isValidUsername;

                    Console.WriteLine("\n--- Character Creation ---");
                    Console.WriteLine($"Username Rules: Max {MaxUsernameLength} characters, no spaces.");

                    do
                    {
                        Console.WriteLine("\nName your character (needed to login):");
                        Console.WriteLine("0 : cancel\n");
                        name = Console.ReadLine();

                        if (name == "0")break;

                        isValidUsername = (name.Length <= MaxUsernameLength && !name.Contains(' ') && name != null);

                        if (!isValidUsername)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            if (name.Length > MaxUsernameLength)
                            {
                                Console.WriteLine($"Error: Username is too long. Must be {MaxUsernameLength} characters or less.");
                            }
                            else if (name.Contains(' '))
                            {
                                Console.WriteLine("Error: Username cannot contain spaces.");
                            }
                            else
                            {
                                Console.WriteLine("Error: Invalid username format. Please try again.");
                            }
                            Console.ResetColor();
                        }
                        else if (db.PlayerExists(name))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("A player with that name already exists, please try again");
                            Console.ResetColor();
                            isValidUsername = false;
                        }

                    } while (!isValidUsername);
                    if (name == "0") continue;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Username accepted!");
                    Console.ResetColor();

                    PlayerCreator creator = new PlayerCreator();
                    player = creator.PlayerCreatorFunction(db, name);
                    db.SavePlayer(player);
                    Inventory.MakeInv();
                    Inventory.AddItem(ItemLibrary.rock, 1);
                    Console.WriteLine("New character created and saved!");
                    
                    StartIdleTimeoutMonitoring();
                    break;
                }
                else
                {
                    Console.WriteLine("\nInvalid choice, write 1 or 2 please.");
                    continue;
                }
            }
            atkManager = new AttackManager(player);
            MainUI.InitializeConsole();
            CheckPlayerLevel();
            if (player.GetStat("isInCombat") == 1)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║                                        ║");
                Console.WriteLine("║      COMBAT LOG DETECTED!              ║");
                Console.WriteLine("║                                        ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("You disconnected during combat!");
                Console.WriteLine("This counts as a death...");
                Console.WriteLine();
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();

                player.HP = 0;
                player.SetStat("isInCombat", 0);
                db.SavePlayer(player).GetAwaiter().GetResult();
                CheckPlayerDeath();
            }
            MainMenu();
        }

        private static void CheckForUpdates()
        {
            try
            {
                Console.WriteLine("Checking for updates...");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "TBS-Game");
                    client.Timeout = TimeSpan.FromSeconds(5);

                    var response = client.GetStringAsync(GITHUB_API_URL).Result;
                    JObject json = JObject.Parse(response);
                    string latestVersion = json["tag_name"]?.ToString()?.TrimStart('v') ?? "";

                    if (!string.IsNullOrEmpty(latestVersion) && latestVersion != CURRENT_VERSION)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("╔════════════════════════════════════════╗");
                        Console.WriteLine("║                                        ║");
                        Console.WriteLine("║          OUTDATED VERSION!             ║");
                        Console.WriteLine("║                                        ║");
                        Console.WriteLine("╚════════════════════════════════════════╝");
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine($"Current version: {CURRENT_VERSION}");
                        Console.WriteLine($"Latest version:  {latestVersion}");
                        Console.WriteLine();
                        Console.WriteLine("This version is outdated and cannot be used.");
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Please use TBSLauncher.exe to download the latest version.");
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.WriteLine("Press any key to close...");
                        Console.ReadKey(true);

                        // Close the game
                        Environment.Exit(0);
                    }
                    else if (!string.IsNullOrEmpty(latestVersion))
                    {
                        Console.WriteLine($"Game is up to date (v{CURRENT_VERSION})");
                        Thread.Sleep(800);
                        Console.Clear();
                    }
                }
            }
            catch
            {
                // Silently fail if can't check for updates (no internet, etc.)
                // Game will continue normally
            }
        }

        public static void MainMenu()
        {
            player?.UpdateActivity();

            MainUI.ShowMovesInPlayerPanel = false;

            Console.Clear(); //do not remove
            MainUI.ClearMainArea();

            MainUI.RenderMainMenuScreen(player);
            if (!stopMultibleLoad)
            {
                MainUI.LoopRenderMain();
                stopMultibleLoad = true;
            }


            // Save in background without blocking UI
            _ = Task.Run(() => db.SavePlayer(player));

            MainUI.WriteInMainArea("What do you wish to do? (type the number next to it)");
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea("Go somewhere : 0");
            MainUI.WriteInMainArea("Open Inventory : 1");
            MainUI.WriteInMainArea("Manage Moves : 2");
            MainUI.WriteInMainArea($"Do something at {player.currentLocation} : 3");
            MainUI.WriteInMainArea("Check stats : 4");
            MainUI.WriteInMainArea("Read Encyclopedia or Map : 5");

            if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input) == false || input > 5 || input < 0)
            {
                MainUI.WriteInMainArea("\nyou gotta type 0, 1, 2, 3, 4, or 5");
                MainUI.WriteInMainArea("Press enter to continue...");
                Console.ReadLine();
                player?.UpdateActivity();
                MainMenu();
                return;
            }
            

            player?.UpdateActivity();
            
            if (input == 0) journeyManager.ChoseTravelDestination();
            else if (input == 1)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("Inventory : 0");
                MainUI.WriteInMainArea("MaterialBag : 1");

                if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int input2) == false || input2 > 1 || input2 < 0)
                {
                    MainUI.WriteInMainArea("\nyou gotta type 0 or 1");
                    MainUI.WriteInMainArea("Press enter to continue...");
                    Console.ReadLine();
                    MainMenu();
                    return;
                }
                else if (input2 == 0)
                {
                    Inventory.ShowInventory();
                }
                else if (input2 == 1)
                {
                    Inventory.ShowMaterialBag();
                }
            }
            else if (input == 2) atkManager.ShowMovesMenu();
            else if (input == 3)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea("all establisments in your current location\n");
                int i = 0;
                foreach (var subLocation in LocationLibrary.Get(player.currentLocation).subLocationsHere)
                {
                    i++;
                    MainUI.WriteInMainArea($"{subLocation.name} : {i}");
                }
                if (i == 0)
                {
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("there arent any establisments in your current location sorry");
                    MainMenu();
                    return;
                }
                MainUI.WriteInMainArea("Cancel : 0");
                MainUI.WriteInMainArea("\ntype out the number next to the location you want to go to\n");

                int targetDes;
                if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out targetDes))
                {
                    if (targetDes == 0)
                    {
                        MainUI.ClearMainArea();
                        MainMenu();
                        return;
                    }
                    else if (targetDes > LocationLibrary.Get(player.currentLocation).subLocationsHere.Count || targetDes < 0)
                    {
                        MainUI.WriteInMainArea("that number is wrong mate");
                        MainMenu();
                        return;
                    }
                    LocationLibrary.Get(player.currentLocation).subLocationsHere[targetDes - 1].DoSubLocation();
                }
                else
                {
                    MainUI.WriteInMainArea("write a number dumb dumb");
                    MainMenu();
                    return;
                }
            }
            else if (input == 4) ShowPlayerStats();
            else if (input == 5) 
            {
                MainUI.WriteInMainArea("");
                MainUI.WriteInMainArea("Read map : 1");
                MainUI.WriteInMainArea("Read Encyclopedia : 2");
                MainUI.WriteInMainArea("Cancel : 0");
                if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int inpu2t) == false || inpu2t > 2 || inpu2t < 0)
                {
                    MainUI.WriteInMainArea("\nyou gotta type 0, 1, 2, 3, 4, or 5");
                    MainUI.WriteInMainArea("Press enter to continue...");
                    Console.ReadLine();
                    MainMenu();
                    return;
                }
                else if (inpu2t == 1) Minimap.DisplayMainmap(4, 7, 75);
                else if (inpu2t == 2) Encyclopedia.EncyclopediaLogic();
                else if (inpu2t == 0) MainMenu();



            }


                // Save in background without blocking UI
                _ = Task.Run(() => db.SavePlayer(player));
        }

        public static void ShowPlayerStats()
        {
            MainUI.ClearMainArea();

            MainUI.WriteInMainArea($"\nAccount Name: {player.name} \n\n1 : Level: {player.level} \n2 : Class: {player.playerClass.name} \n3 : HP: {player.HP}/{player.maxHP} \n4 : Speed: {player.speed} \n5 : armor: {player.armor}" +
                $"\n6 : Dodge: {player.dodge}% \n7 : DodgeNegation: {player.dodgeNegation}% \n8 : Crit-chance: {player.critChance}% \n9 : Crit-Damage: {player.critDamage}% \n 10 : Stun: {player.stun}%" +
                $"\n11 : StunNegation: {player.stunNegation}%\n12 : Companions\n\n");

            Thread.Sleep(400);
            MainUI.WriteInMainArea("0 : Cancel");
            MainUI.WriteInMainArea("Press any stat's corresponding number for details about it");
            string st = Console.ReadLine();

            if (int.TryParse(st, out int input) == false || input > 12 || input < 0)
            {
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea(" \nyou gotta type a real number:)");

                MainUI.WriteInMainArea(" \npress enter to continue...");
                Console.ReadLine();
                ShowPlayerStats();
                return;
            }
            else
            {
                MainUI.ClearMainArea();
                switch (input)
                {
                    case 0:
                        MainMenu();
                        return;
                    case 1:
                        MainUI.WriteInMainArea("For each levelup you gain:");
                        MainUI.WriteInMainArea($"    {player.playerClass.TmaxHP} maxHP");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tspeed} speed");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tarmor} armor");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tdodge} dodge");
                        MainUI.WriteInMainArea($"    {player.playerClass.TdodgeNegation} dodgeNegation");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tcritchance} critChance");
                        MainUI.WriteInMainArea($"    {player.playerClass.TcritDamage} critDamage");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tstun} stun");
                        MainUI.WriteInMainArea($"    {player.playerClass.TstunNegation} stunNegationn\n");

                        int levelsAbove1 = Math.Max(player.level - 1, 0);
                        MainUI.WriteInMainArea("your level gives you a total of:");
                        MainUI.WriteInMainArea($"    {player.playerClass.TmaxHP * levelsAbove1} maxHP");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tspeed * levelsAbove1} speed");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tarmor * levelsAbove1} armor");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tdodge * levelsAbove1} dodge");
                        MainUI.WriteInMainArea($"    {player.playerClass.TdodgeNegation * levelsAbove1} dodgeNegation");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tcritchance * levelsAbove1} critChance");
                        MainUI.WriteInMainArea($"    {player.playerClass.TcritDamage * levelsAbove1} critDamage");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tstun * levelsAbove1} stun");
                        MainUI.WriteInMainArea($"    {player.playerClass.TstunNegation * levelsAbove1} stunNegation");
                        
                        break;
                    case 2:
                        MainUI.WriteInMainArea($"your class is {player.playerClass.name}");
                        MainUI.WriteInMainArea($"{player.playerClass.description}");
                        MainUI.WriteInMainArea("Cause of your class you gain: (pr level)");
                        MainUI.WriteInMainArea($"    {player.playerClass.TmaxHP} maxHP");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tspeed} speed");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tarmor} armor");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tdodge} dodge");
                        MainUI.WriteInMainArea($"    {player.playerClass.TdodgeNegation} dodgeNegation");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tcritchance} critChance");
                        MainUI.WriteInMainArea($"    {player.playerClass.TcritDamage} critDamage");
                        MainUI.WriteInMainArea($"    {player.playerClass.Tstun} stun");
                        MainUI.WriteInMainArea($"    {player.playerClass.TstunNegation} stunNegation");
                        break;
                    case 3:
                        MainUI.WriteInMainArea("Your hp stat is how much you can get hit before you die");
                        break;
                    case 4:
                        MainUI.WriteInMainArea("Your speed stat determines how often you get to take actions in combat");
                        break;
                    case 5:
                        MainUI.WriteInMainArea("Your armor stat acts as a flat decrease to damage taken during combat");
                        break;
                    case 6:
                        MainUI.WriteInMainArea("Your dodge stat gives you a chance to avoid enemy attacks entirely");
                        break;
                    case 7:
                        MainUI.WriteInMainArea("Your dodgeNegation stat makes it harder for your enemies to dodge your \n attacks");
                        break;
                    case 8:
                        MainUI.WriteInMainArea("Your critChance stat gives you a chance to deal bonus damage on your \n attacks");
                        break;
                    case 9:
                        MainUI.WriteInMainArea("Your critDamage stat determines how much extra damage you deal when you crit");
                        break;
                    case 10:
                        MainUI.WriteInMainArea("Your stun stat gives you a chance to stun your opponents when you hit them \n with an attack");
                        break;
                    case 11:
                        MainUI.WriteInMainArea("Your stunNegation stat makes it harder for your opponent to stun you");
                        break;
                    case 12:
                        ShowCompanionsMenu();
                        return;
                }

                MainUI.WriteInMainArea(" \nPress enter to continue...");
                Console.ReadLine();
                ShowPlayerStats();
            }
        }
        
        public static void ShowCompanionsMenu()
        {
            while (true)
            {
                MainUI.ClearMainArea();
                
                int maxCompanions = CompanionSystem.GetMaxCompanions(player);
                var companions = CompanionSystem.GetCompanions(player);
                
                MainUI.WriteInMainArea($"=== Your Companions ({companions.Count}/{maxCompanions}) ===\n");
                
                if (companions.Count == 0)
                {
                    MainUI.WriteInMainArea("You don't have any companions.\n");
                }
                else
                {
                    for (int i = 0; i < companions.Count; i++)
                    {
                        var c = companions[i];
                        MainUI.WriteInMainArea($"{i + 1}. {c.name} - Lvl {c.level} - HP: {c.HP}/{c.maxHP}");
                    }
                    MainUI.WriteInMainArea("");
                }
                
                MainUI.WriteInMainArea("1. Dismiss a companion");
                MainUI.WriteInMainArea("2. Dismiss all companions");
                MainUI.WriteInMainArea("0. Back\n");
                
                if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out int choice) == false || choice > 2 || choice < 0)
                {
                    MainUI.WriteInMainArea("\nInvalid choice. Press enter to continue...");
                    Console.ReadLine();
                    continue;
                }
                
                if (choice == 0)
                {
                    ShowPlayerStats();
                    return;
                }
                else if (choice == 1)
                {
                    if (companions.Count == 0)
                    {
                        MainUI.WriteInMainArea("\nYou don't have any companions to dismiss.");
                        MainUI.WriteInMainArea("Press enter to continue...");
                        Console.ReadLine();
                        continue;
                    }
                    
                    MainUI.WriteInMainArea("\nEnter the number of the companion to dismiss (0 to cancel): ");
                    if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= companions.Count)
                    {
                        string companionName = companions[index - 1].name;
                        CompanionSystem.RemoveCompanion(player, index - 1);
                        MainUI.WriteInMainArea($"\n{companionName} has left your party.");
                        MainUI.WriteInMainArea("Press enter to continue...");
                        Console.ReadLine();
                    }
                    else if (index != 0)
                    {
                        MainUI.WriteInMainArea("\nInvalid selection.");
                        MainUI.WriteInMainArea("Press enter to continue...");
                        Console.ReadLine();
                    }
                }
                else if (choice == 2)
                {
                    if (companions.Count == 0)
                    {
                        MainUI.WriteInMainArea("\nYou don't have any companions to dismiss.");
                        MainUI.WriteInMainArea("Press enter to continue...");
                        Console.ReadLine();
                        continue;
                    }
                    
                    MainUI.WriteInMainArea("\nAre you sure you want to dismiss all companions? (type YES): ");
                    string confirm = Console.ReadLine();
                    if (confirm == "YES")
                    {
                        CompanionSystem.DismissAllCompanions(player);
                        MainUI.WriteInMainArea("\nAll companions have been dismissed.");
                        MainUI.WriteInMainArea("Press enter to continue...");
                        Console.ReadLine();
                    }
                }
            }
        }

        public static async Task SavePlayer()
        {
            db.SavePlayer(player);
        }
        public static async Task CheckPlayerLevel()
        {
            while (true)
            {
                if (player.exp >= player.level * 100)
                {
                    player.exp -= player.level * 100;
                    player.level++;

                    // Recalculate all derived stats based on current class & level
                    player.RecalculateStats();
                }

                await Task.Delay(3200);
            }
        }

        public static void CheckPlayerDeath()
        {
            if (player != null && player.HP <= 0)
            {
                if (pendingDeadPlayerUpdate != null && pendingSpiritEnemy != null)
                {
                    try
                    {
                        pendingDeadPlayerUpdate.HP = pendingSpiritEnemy.HP;
                        db.UpdateDeadPlayer(pendingDeadPlayerUpdate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating spirit HP: {ex.Message}");
                    }
                    finally
                    {
                        pendingDeadPlayerUpdate = null;
                        pendingSpiritEnemy = null;
                    }
                }

                ShowDeathScreen();
            }
        }

        private static void ShowDeathScreen()
        {
            try
            {
                player.SetStat("isInCombat", 0);
                player.isDead = true;
                player.SetOnline(false);
                db.MarkPlayerAsDead(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving dead player: {ex.Message}");
                try { db.SavePlayer(player).GetAwaiter().GetResult(); } catch { }
            }

            idleCheckCancellation?.Cancel();

            Console.Clear();
            Console.CursorVisible = true;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("║          YOU HAVE DIED                 ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("");
            Console.WriteLine($"Character: {player.name}");
            Console.WriteLine($"Level: {player.level}");
            Console.WriteLine($"Class: {player.playerClass.name}");
            Console.WriteLine("");
            Console.WriteLine("Your character has died...");
            Console.WriteLine("They have been moved to the realm of the dead.");
            Console.WriteLine("");
            Console.WriteLine("Press Enter to close the game...");

            try
            {
                Console.ReadLine();
            }
            catch { }

            player = null;

            Console.Clear();
            Console.WriteLine("Game closing...");
            Thread.Sleep(1000);

            // Close the game
            Environment.Exit(0);
        }

        private static void ShowDeadCharacterScreen(Player deadPlayer)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("║       THIS CHARACTER IS DEAD           ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("");
            Console.WriteLine($"Character: {deadPlayer.name}");
            Console.WriteLine($"Level: {deadPlayer.level}");
            Console.WriteLine($"Class: {deadPlayer.playerClass.name}");
            Console.WriteLine("");
            Console.WriteLine("This character died and can no longer be played.");
            Console.WriteLine("Their spirit lingers in the realm of the dead...");
            Console.WriteLine("");
            Console.WriteLine("Press Enter to return to login...");

            Console.ReadLine();
        }

        private static void StartIdleTimeoutMonitoring()
        {
            idleCheckCancellation?.Cancel();
            idleCheckCancellation = new CancellationTokenSource();

            Task.Run(async () =>
            {
                try
                {
                    while (!idleCheckCancellation.Token.IsCancellationRequested)
                    {
                        await Task.Delay(30000, idleCheckCancellation.Token);

                        if (player != null && player.IsIdle(10))
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("╔════════════════════════════════════════╗");
                            Console.WriteLine("║                                        ║");
                            Console.WriteLine("║      AUTO-DISCONNECT: IDLE TIMEOUT     ║");
                            Console.WriteLine("║                                        ║");
                            Console.WriteLine("╚════════════════════════════════════════╝");
                            Console.ResetColor();
                            Console.WriteLine();
                            Console.WriteLine("You have been disconnected due to 10 minutes of inactivity.");
                            Console.WriteLine();
                            Console.WriteLine("Press Enter to close the game...");

                            player.SetOnline(false);
                            await db.SavePlayer(player);

                            try
                            {
                                Console.ReadLine();
                            }
                            catch { }

                            Environment.Exit(0);
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in idle monitoring: {ex.Message}");
                }
            }, idleCheckCancellation.Token);
        }

        public static void DisconnectPlayer()
        {
            if (player != null)
            {
                player.SetOnline(false);
                db.SavePlayer(player).GetAwaiter().GetResult();
            }
            
            idleCheckCancellation?.Cancel();
        }
    }
}