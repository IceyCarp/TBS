using Game.Class;
using System.Net.Http.Headers;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

public static class EncounterLibrary
{
    private static Random rng = new Random();

    #region Treasure Encounters

    public static Encounter FoundCoins = new Encounter(
        "FoundCoins",                                    // Unique identifier for this encounter
        false,                                           // IsEnemyEncounter - false means no combat
        "You found some Rai on the ground!",           // Description shown to player
        null,                                            // Enemies list - null for non-combat encounters
        (player) => {                                    // OnEncounter action - custom logic executed when encounter triggers
            int coins = rng.Next(1, 4);                 // Generate random gold amount between 1-2
            player.money += coins;                       // Add gold to player's money
            MainUI.WriteInMainArea($"You gained {coins} Rai!");  // Display result to player
            Program.SavePlayer();                        // Save player state to persist changes
        },
        EncounterType.Treasure                           // Type categorization for organization
    );

    public static Encounter FoundTreasure = new Encounter(
        "FoundTreasure",
        false,
        "You discover a hidden treasure chest!",
        null,
        (player) => {
            int coins = rng.Next(3, 8);
            int exp = rng.Next(5, 15);
            player.money += coins;
            player.exp += exp;
            MainUI.WriteInMainArea($"You gained {coins} Rai and {exp} experience!");
            Program.SavePlayer();
        },
        EncounterType.Treasure
    );

    public static Encounter AbandonedBackpack = new Encounter(
    "AbandonedBackpack",
    false,
    "An old backpack lies partially buried in dirt.",
    null,
    (player) => {

        // Get ALL materials :)
        var materialItems = typeof(ItemLibrary)
            .GetFields(System.Reflection.BindingFlags.Public |
                       System.Reflection.BindingFlags.Static |
                       System.Reflection.BindingFlags.FlattenHierarchy)
                    .Where(f => f.FieldType == typeof(Item))
                    .Select(f => (Item)f.GetValue(null))
                    .Where(i => i.type == ItemType.Material)
                    .ToList();

        if (materialItems.Count == 0)
        {
            MainUI.WriteInMainArea("The backpack is empty...");
            return;
        }

        Item selected = materialItems[rng.Next(materialItems.Count)];

        Inventory.AddItem(selected, 1);

        MainUI.WriteInMainArea($"You found a {selected.name} inside the backpack!");
    },
    EncounterType.Treasure
);


    #endregion

    #region Trap/Hazard Encounters
    public static Encounter LostCoins = new Encounter(
        "LostCoins",
        false,
        "You realize your Rai are missing! A pickpocket must have struck!",
        null,
        (player) => {
            int lostCoins = Math.Min(rng.Next(2, 6), player.money);
            player.money -= lostCoins;
            MainUI.WriteInMainArea($"You lost {lostCoins} Rai!");
            Program.SavePlayer();
        },
        EncounterType.Trap
    );

    public static Encounter FallenIntoTrap = new Encounter(
        "FallenIntoTrap",
        false,
        "You step on a hidden trap! Spikes shoot up from the ground!",
        null,
        (player) => {
            int damage = rng.Next(5, 15);
            player.HP -= damage;
            MainUI.WriteInMainArea($"You took {damage} damage! HP: {Math.Max(0, player.HP)}/{player.maxHP}");
            Program.SavePlayer();
            Program.CheckPlayerDeath();
        },
        EncounterType.Trap
    );

    public static Encounter WeakRopeBridge = new Encounter(
    "WeakRopeBridge",
    false,
    "You come across a very old and suspicious rope bridge dangling over\n a shallow ravine.",
    null,
    (player) => {
        MainUI.WriteInMainArea("");
        MainUI.WriteInMainArea("Do you want to cross the bridge or go around? \ncross : 1 \naround : 2\n");
        string choice = Console.ReadKey(true).KeyChar.ToString();

        if (choice =="1")
        {
            int outcome = rng.Next(1, 101);

            if (outcome <= 30)
            {
                MainUI.WriteInMainArea("You cross the bridge carefully... \nIt holds. Nothing happens.");
            }
            else if (outcome <= 65)
            {
                int dmg = rng.Next(2, 6);
                player.HP -= dmg;
                MainUI.WriteInMainArea($"One of the planks snaps! \nYou fall halfway through and take {dmg} damage.");
                Program.CheckPlayerDeath();
            }
            else if (outcome <= 85)
            {
                int coins = rng.Next(3, 5);
                player.money += coins;
                MainUI.WriteInMainArea($"You notice a pouch stuck between two loose ropes! \nYou gain {coins} Rai!");
            }
            else
            {
                int dmg = rng.Next(8, 18);
                player.HP -= dmg;
                MainUI.WriteInMainArea($"The entire bridge collapses beneath you! \nYou crash into the ravine and take {dmg} damage!");
                Program.CheckPlayerDeath();
            }

            
        }
        else if (choice == "2")
        {
            MainUI.WriteInMainArea("You decide to take the longer route around the ravine...\n");
            MainUI.WriteInMainArea("Press enter to continue...");
            Console.ReadLine();

            int totalSteps = 100;
            int barWidth = 50;

            for (int i = 0; i <= totalSteps; i++)
            {
                double progress = (double)i / totalSteps;
                int filled = (int)(progress * barWidth);
                int empty = barWidth - filled;

                string bar = new string('█', filled) + new string('░', empty);
                int percent = (int)(progress * 100);
                MainUI.ClearMainArea();
                MainUI.WriteInMainArea($"Finding a safe path: {bar} {percent}%");

                Thread.Sleep(400);
            }
            MainUI.ClearMainArea();
            MainUI.WriteInMainArea("You found a safe path across!!!");
        }
        else
        {
            int coins = rng.Next(0, player.money/2);
            coins = (int)MathF.Floor(coins);
            player.money -= coins;
            MainUI.WriteInMainArea("");
            MainUI.WriteInMainArea($"You stand there unsure what to do... eventually you decide to move on.\n" +
                $"However during your moment of thought, a pickpocket snatches {coins} of your Rai!");
        }
        Program.SavePlayer();
    },
    EncounterType.Event
);


    #endregion

    #region Combat Encounters - Basic/Starter Areas
    public static Encounter BanditFight = new Encounter(
        "BanditFight",
        true,
        "A solo bandit jumps out from behind the trees!",
        new List<Enemy> { EnemyLibrary.Thug},
        null,
        EncounterType.Combat
    );

    public static Encounter BanditAmbush = new Encounter(
        "BanditAmbush",
        true,
        "Bandits jump out from behind the trees!",
        new List<Enemy> { EnemyLibrary.Thug, EnemyLibrary.Thug, EnemyLibrary.Healer},
        null,
        EncounterType.Combat
    );

    public static Encounter WildGoblin = new Encounter(
        "WildGoblin",
        true,
        "A wild Goblin appears from the bushes!",
        new List<Enemy> { EnemyLibrary.Goblin },
        null,
        EncounterType.Combat
    );

    public static Encounter GoblinPack = new Encounter(
        "GoblinPack",
        true,
        "A pack of Goblins surrounds you!",
        new List<Enemy> { EnemyLibrary.Goblin, EnemyLibrary.Goblin, EnemyLibrary.Goblin },
        null,
        EncounterType.Combat
    );
    #endregion

    #region Combat Encounters - Forest Regions
    public static Encounter ForestSpiderNest = new Encounter(
        "ForestSpiderNest",
        true,
        "You stumble into a Forest Spider nest!",
        new List<Enemy> { EnemyLibrary.ForestSpider, EnemyLibrary.ForestSpider, EnemyLibrary.ForestSpider },
        null,
        EncounterType.Combat
    );

    public static Encounter DireWolfHunt = new Encounter(
        "DireWolfHunt",
        true,
        "A Dire Wolf stalks you through the trees!",
        new List<Enemy> { EnemyLibrary.DireWolf },
        null,
        EncounterType.Combat
    );

    public static Encounter DireWolfPack = new Encounter(
        "DireWolfPack",
        true,
        "A pack of Dire Wolves surrounds you!",
        new List<Enemy> { EnemyLibrary.DireWolf, EnemyLibrary.DireWolf },
        null,
        EncounterType.Combat
    );
    #endregion

    #region Combat Encounters - Coastal Regions
    public static Encounter SeaBanditRaid = new Encounter(
        "SeaBanditRaid",
        true,
        "Sea Bandits attack from the shore!",
        new List<Enemy> { EnemyLibrary.SeaBandit, EnemyLibrary.SeaBandit },
        null,
        EncounterType.Combat
    );

    public static Encounter SmugglerAmbush = new Encounter(
        "SmugglerAmbush",
        true,
        "Smugglers emerge from the shadows!",
        new List<Enemy> { EnemyLibrary.Smuggler, EnemyLibrary.Smuggler },
        null,
        EncounterType.Combat
    );
    #endregion

    #region Combat Encounters - Fallen Kingdom
    public static Encounter VampireAttack = new Encounter(
        "VampireAttack",
        true,
        "A Vampire Spawn emerges from the shadows!",
        new List<Enemy> { EnemyLibrary.VampireSpawn },
        null,
        EncounterType.Combat
    );

    public static Encounter GhostlyApparition = new Encounter(
        "GhostlyApparition",
        true,
        "A Ghostly Apparition appears!",
        new List<Enemy> { EnemyLibrary.GhostlyApparition },
        null,
        EncounterType.Combat
    );

    public static Encounter SkeletonWarriors = new Encounter(
        "SkeletonWarriors",
        true,
        "A couple Skeleton Warrior appears!",
        new List<Enemy> { EnemyLibrary.SkeletonWarrior, EnemyLibrary.SkeletonWarrior, EnemyLibrary.SkeletonWarrior },
        null,
        EncounterType.Combat
    );

    public static Encounter KingdomGuardPatrol = new Encounter(
        "KingdomGuardPatrol",
        true,
        "Kingdom Guards spot you as an intruder!",
        new List<Enemy> { EnemyLibrary.KingdomGuard, EnemyLibrary.KingdomGuard },
        null,
        EncounterType.Combat
    );

    public static Encounter CorruptedKnightBattle = new Encounter(
        "CorruptedKnightBattle",
        true,
        "A Corrupted Knight challenges you!",
        new List<Enemy> { EnemyLibrary.CorruptedKnight },
        null,
        EncounterType.Combat
    );

    public static Encounter RuinedGolemAwakens = new Encounter(
        "RuinedGolemAwakens",
        true,
        "An ancient Ruined Golem awakens!",
        new List<Enemy> { EnemyLibrary.RuinedGolem },
        null,
        EncounterType.Combat
    );
    #endregion

    #region Combat Encounters - Frozen Regions
    public static Encounter IceWolfPack = new Encounter(
        "IceWolfPack",
        true,
        "A pack of Ice Wolves emerges from the blizzard!",
        new List<Enemy> { EnemyLibrary.IceWolf, EnemyLibrary.IceWolf, EnemyLibrary.IceWolf },
        null,
        EncounterType.Combat
    );

    public static Encounter FrostTrollAmbush = new Encounter(
        "FrostTrollAmbush",
        true,
        "A massive Frost Troll blocks your path!",
        new List<Enemy> { EnemyLibrary.FrostTroll },
        null,
        EncounterType.Combat
    );

    public static Encounter IceMageEncounter = new Encounter(
        "IceMageEncounter",
        true,
        "An Ice Mage materializes from the frozen mist!",
        new List<Enemy> { EnemyLibrary.IceMage, EnemyLibrary.IceWolf },
        null,
        EncounterType.Combat
    );

    public static Encounter SnowWraithAttack = new Encounter(
        "SnowWraithAttack",
        true,
        "A Snow Wraith descends upon you!",
        new List<Enemy> { EnemyLibrary.SnowWraith },
        null,
        EncounterType.Combat
    );

    public static Encounter FrozenHorde = new Encounter(
        "FrozenHorde",
        true,
        "You're surrounded by frozen creatures!",
        new List<Enemy> { EnemyLibrary.IceWolf, EnemyLibrary.IceWolf, EnemyLibrary.IceMage },
        null,
        EncounterType.Combat
    );
    public static Encounter FrozenGolemHealer = new Encounter(
    "FrozenGolem Healer",
    true,
    "You're surrounded by frozen creatures!",
    new List<Enemy> { EnemyLibrary.IceMage, EnemyLibrary.IceMage, EnemyLibrary.GlacierGolem },
    null,
    EncounterType.Combat
);
    #endregion

    #region Combat Encounters - Rootbound Empire

    public static Encounter ElvenScout = new Encounter(
        "ElvenScout",
        true,
        "An Elf Hunter steps out from the dense foliage, nocking an arrow.",
        new List<Enemy> { EnemyLibrary.ElfHunter },
        null,
        EncounterType.Combat
    );

    public static Encounter LynxAmbush = new Encounter(
        "LynxAmbush",
        true,
        "A pair of Shadow Lynx drop silently from the branches above!",
        new List<Enemy> { EnemyLibrary.ShadowLynx, EnemyLibrary.ShadowLynx },
        null,
        EncounterType.Combat
    );

    public static Encounter ElvenPatrol = new Encounter(
        "ElvenPatrol",
        true,
        "An elven patrol spots you! \nA hunter draws their bow as a mystic begins to chant.",
        new List<Enemy> { EnemyLibrary.ElfHunter, EnemyLibrary.ElfMystic },
        null,
        EncounterType.Combat
    );

    public static Encounter HunterAndBeast = new Encounter(
        "HunterAndBeast",
        true,
        "You hear a low growl as a Shadow Lynx emerges, \nan Elf Hunter right behind it.",
        new List<Enemy> { EnemyLibrary.ElfHunter, EnemyLibrary.ShadowLynx },
        null,
        EncounterType.Combat
    );

    public static Encounter ElfWarband = new Encounter(
        "ElfWarband",
        true,
        "A full elven warband blocks your path, supported by their beast!",
        new List<Enemy> { EnemyLibrary.ElfHunter, EnemyLibrary.ElfMystic, EnemyLibrary.ShadowLynx, EnemyLibrary.ElfHunter },
        null,
        EncounterType.Combat
    );

    #endregion

    #region Event Encounters
    public static Encounter StrangeMushrooms = new Encounter(
        "StrangeMushrooms",
        false,
        "You stumble upon some strange mushrooms.",
        null,
        (player) => {
            MainUI.WriteInMainArea("Eat the mushrooms? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                int roll = rng.Next(1, 101);
                if (roll <= 50)
                {
                    int heal = rng.Next(10, 25);
                    player.HP = Math.Min(player.maxHP, player.HP + heal);
                    MainUI.WriteInMainArea($"The mushrooms were healing! You recovered {heal} HP!");
                }
                else
                {
                    int damage = rng.Next(5, 15);
                    player.HP -= damage;
                    MainUI.WriteInMainArea($"The mushrooms were poisonous! You took {damage} damage!");
                }
                Program.SavePlayer();
                Program.CheckPlayerDeath();
            }
            else
            {
                MainUI.WriteInMainArea("You decide not to risk it and move on.");
            }
        },
        EncounterType.Event
    );

    public static Encounter MysteriousShrine = new Encounter(
        "MysteriousShrine",
        false,
        "You find a mysterious shrine",
        null,
        (player) => {
            MainUI.WriteInMainArea("Pray at the shrine? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                int roll = rng.Next(1, 101);
                if (roll <= 70)
                {
                    int exp = rng.Next(1, 50);
                    player.exp += exp;
                    MainUI.WriteInMainArea($"The shrine blesses you! You gained {exp} experience!");
                }
                else if (roll == 89 || roll == 90)
                {
                    player.HP -= player.HP-1;
                    MainUI.WriteInMainArea("The mysterious shrine rips the strength from your veins, \nleaving only the thinnest thread of life");
                }
                else
                {
                    MainUI.WriteInMainArea("Nothing happens...");
                }
                Program.SavePlayer();
            }
            else
            {
                MainUI.WriteInMainArea("You respectfully pass by the shrine.");
            }
        },
        EncounterType.Mystery
    );

    public static Encounter WanderingMerchant = new Encounter(
        "WanderingMerchant",
        false,
        "A wandering merchant offers you a mysterious potion",
        null,
        (player) => {
            MainUI.WriteInMainArea("Buy the potion for 15 Rai? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                if (player.money >= 15)
                {
                    player.money -= 15;
                    int heal = rng.Next(20, 40);
                    player.HP = Math.Min(player.maxHP, player.HP + heal);
                    MainUI.WriteInMainArea($"You bought the potion and recovered {heal} HP!");
                    Program.SavePlayer();
                }
                else
                {
                    MainUI.WriteInMainArea("You don't have enough Rai!");
                }
            }
            else
            {
                MainUI.WriteInMainArea("You decline the merchant's offer.");
            }
        },
        EncounterType.Merchant
    );

    public static Encounter RoadGambling = new Encounter(
        "RoadGambling",
        false,
        "An old man offers to gamble with you on a coin flip",
        null,
        (player) => {
            MainUI.WriteInMainArea("Accept his offer? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                MainUI.WriteInMainArea($"how much do you want to bet? current Rai: {Program.player.money} \nYou're betting on heads");
                int.TryParse(Console.ReadLine(), out int bet);
                if (bet == null || bet < 0)
                {
                    MainUI.ClearMainArea();
                    MainUI.WriteInMainArea("sweetie you gotta type a number that we can use\n ");
                    return;
                }
                else if (bet > Program.player.money)
                {
                    MainUI.WriteInMainArea("\nyou dont have that much Rai\n ");
                    return;
                }
                int roll = rng.Next(1, 20);
                if (roll == 9)
                {
                    MainUI.WriteInMainArea($"The man takes your bet of {bet} and runs off");
                    Program.player.money -= bet;
                    
                }
                else
                {
                    MainUI.WriteInMainArea($"you bet: {bet}");
                    Program.player.money -= bet;
                    Program.SavePlayer();

                    Random rand = new Random();
                    int coin = rand.Next(1, 3);
                    if (coin == 1)
                    {
                        MainUI.WriteInMainArea($"The coin landed on heads, you win {bet} Rai!");
                        Program.player.money += bet * 2;
                    }
                    else
                    {
                        MainUI.WriteInMainArea("The coin landed on tails, you lost...");
                    }
                }
            }
            else
            {
                MainUI.WriteInMainArea("You respectfully decline his offer.");
            }
        },
        EncounterType.Event
    );

    public static Encounter FallingFish = new Encounter(
        "FallingFish",
        false,
        "A fish slings out of the pond and lands in front of you",
        null,
        (player) => {
            MainUI.WriteInMainArea("Would you like to pick it up? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                
                Random rand = new Random();
                int slap = rand.Next(1, 6);
                if (slap == 5)
                {
                    player.HP -= 1;
                    MainUI.WriteInMainArea($"The fish slaps your hand away with its fins! You took 1 damage!");
                }
                else
                {
                    MainUI.WriteInMainArea("You successfully pick up the fish without issues!");
                    Inventory.AddItem(ItemLibrary.fish,1);
                }


            }
            else
            {
                MainUI.WriteInMainArea("You don't pick the fish up out of respect");
            }
        },
        EncounterType.Mystery
    );

    public static Encounter LearnFirstAid = new Encounter(
        "LearnFirstAid",
        false,
        "You see a doctor on the side of the road",
        null,
        (player) => {
            if (player.HP < player.maxHP * 0.7 && !player.ownedAttacks.Contains(AttackLibrary.FirstAid))
            {
                {
                    MainUI.WriteInMainArea("The doctor sees you're wounded and offers to teach you first aid\n");
                    MainUI.WriteInMainArea("Learn the move first aid? (y/n): ");
                    string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
                    if (choice == "y" || choice == "yes")
                    {
                        MainUI.WriteInMainArea("The doctor teaches you first aid!");
                        AttackManager atkmanager = new AttackManager(Program.player);
                        atkmanager.LearnAttack(AttackLibrary.FirstAid);
                    }
                    else
                    {
                        MainUI.WriteInMainArea("You respectfully decline the doctors offer.");
                    }
                }
            }
        },
        EncounterType.Mystery
    );



    public static Encounter Wolfsensei = new Encounter(
        "Wolfsensei",
        false,
        "You stumble into a.. Friendly wolf perhaps..",
        null,
        (player) => {
            MainUI.WriteInMainArea("Do you talk to him? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                int damage = rng.Next(5, 15);
                player.HP -= damage;
                MainUI.WriteInMainArea($"The wolf bites you and deals {damage} damage!");
                MainUI.WriteInMainArea($"The wolf says you could learn their form of combat!");
                Program.SavePlayer();
                Program.CheckPlayerDeath();
                MainUI.WriteInMainArea("Do you accept his offer? (y/n): ");
                string choice2 = Console.ReadKey(true).KeyChar.ToString().ToLower();
                if (choice2 == "y" || choice2 == "yes")
                {
                    AttackManager atkmanager = new AttackManager(Program.player);
                    atkmanager.LearnAttack(AttackLibrary.Bite);
                }
                else
                {
                    MainUI.WriteInMainArea("You respectfully decline the wolf's offer.");
                }
            }
            else
            {
                MainUI.WriteInMainArea("You decide not to risk it and move on.");
            }
        },
        EncounterType.Event
    );
    public static Encounter Snowman = new Encounter(
     "Snowman",
     false,
     "You see a snowman in the distance..",
     null,
     (player) => {
         MainUI.WriteInMainArea("Do you approach it? (y/n): ");
         string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
         if (choice == "y" || choice == "yes")
         {
             int number = rng.Next(1, 3);
             if (number == 1)
             {
                 MainUI.WriteInMainArea($"The snowman begins moving, and asks if u wanna see something funny, and he begins grinning..");
                 MainUI.WriteInMainArea("Do you accept his offer? (y/n): ");
                 string choice2 = Console.ReadKey(true).KeyChar.ToString().ToLower();
                 if (choice2 == "y" || choice2 == "yes")
                 {
                     int number2 = rng.Next(1, 3);
                     if (number2 == 1)
                     {
                         int damage = rng.Next(15, 50);
                         player.HP -= damage;
                         MainUI.WriteInMainArea($"The snowman throws a snowball at you and deals {damage} damage!");
                         Program.SavePlayer();
                         Program.CheckPlayerDeath();
                     }
                     else
                     {
                         AttackManager atkmanager = new AttackManager(Program.player);
                         MainUI.WriteInMainArea($"The snowman makes a snowball and teaches you how to do it aswell!");
                         atkmanager.LearnAttack(AttackLibrary.Snowball);
                     }
                 }
                 else
                 {
                     MainUI.WriteInMainArea("You respectfully decline the snowman's offer.");
                 }
             }
             else
             {
                 MainUI.WriteInMainArea($"You walk up to the snowman, just to see its a normal snowman, like any other.");
             }
         }
         else
         {
             MainUI.WriteInMainArea("You decide not to risk it and move on.");
         }
     },
     EncounterType.Event
 );

    public static Encounter LostChild = new Encounter(
    "LostChild",
    false,
    "A small child is crying alone on the roadside.",
    null,
    (player) => {
        MainUI.WriteInMainArea("Help the child? (y/n): ");
        string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();

        if (choice == "y" || choice == "yes")
        {
            int outcome = rng.Next(1, 111); 

            if (outcome <= 50)
            {
                
                int reward = rng.Next(1, 11);
                player.money += reward;
                MainUI.WriteInMainArea(
                    $"The child's parent arrives running and gratefully rewards you {reward} Rai!");
            }
            else if (outcome <= 80)
            {
                
                int stolen = Math.Min(player.money, rng.Next(3, 11));
                player.money -= stolen;
                MainUI.WriteInMainArea(
                    $"When you turn around, the child is gone... and so are {stolen} of your Rai!");
            }
            else if (outcome <= 95)
            {
                
                int damage = rng.Next(5, 16);
                player.HP -= damage;
                MainUI.WriteInMainArea(
                    $"As you approach, the child panics and swings a stick! You take {damage} damage!");
                Program.CheckPlayerDeath();
            }
            else if (outcome <= 105)
            {
                MainUI.WriteInMainArea($"\nThe child calms down, but no one shows up.");
                MainUI.WriteInMainArea($"Do you wish to bring it with you (y/n)");

                string choice2 = Console.ReadKey(true).KeyChar.ToString().ToLower();

                if (choice2 == "y" || choice2 == "yes")
                {
                    int maxCompanions = CompanionSystem.GetMaxCompanions(player);
                    var currentCompanions = CompanionSystem.GetCompanions(player);

                    if (maxCompanions == 0)
                    {
                        MainUI.WriteInMainArea($"\nYour class ({player.playerClass.name}) cannot have companions!");
                        MainUI.WriteInMainArea("You leave the child to fend for itself");
                    }
                    else if (currentCompanions.Count <= maxCompanions)
                    {
                        MainUI.WriteInMainArea("\nyou cannot bring any more companions with you");
                        MainUI.WriteInMainArea("You leave the child to fend for itself");
                    }
                    else
                    {
                        MainUI.WriteInMainArea("you bring the child under your care \nit'll be of some help down the road");
                        bool success = CompanionSystem.RecruitByName(player, "lost child");

                        if (success)
                        {
                            MainUI.WriteInMainArea("\nThe Lost Child has joined your party as a loyal companion!");
                        }
                    }

                }
                else
                {
                    MainUI.WriteInMainArea("You leave the child to fend for itself");
                }
            }
            else
            {

                MainUI.WriteInMainArea("The child calms down, but no one shows up. You move on.");
            }

            Program.SavePlayer();
        }
        else
        {
            MainUI.WriteInMainArea("You walk past the crying child.");
        }
    },
    EncounterType.Event
);
    public static Encounter RogueUnlock = new Encounter(
 "RogueUnlock",
 false,
 "You see a small cave entrance, lit by a torch in the distance",
 null,
 (player) => {
     int flawlessVictories = player.GetStat("totalFlawlessVictories");

     if (flawlessVictories < 1)
     {
         MainUI.WriteInMainArea("You approach the cave, but see what looks like a trap near the entrance. You move on.");
         return;
     }

     MainUI.WriteInMainArea("Do you approach it? (y/n): ");
     string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();

     if (choice == "y" || choice == "yes")
     {
         MainUI.WriteInMainArea("\nAs you enter the cave, a shadowy figure emerges from the darkness...");
         MainUI.WriteInMainArea("The figure studies you silently for a moment.");

         if (flawlessVictories >= 5)
         {
             if (player.playerClass == ClassLibrary.Rogue)
             {
                 MainUI.WriteInMainArea("Welcome back, shadow walker, the figure says with a nod.");
                 MainUI.WriteInMainArea("I see you continue to walk the path of stealth and precision.");

                 if (!player.ownedAttacks.Contains(AttackLibrary.Lunge))
                 {
                     MainUI.WriteInMainArea("Let me teach you a new technique...");
                     AttackManager atkmanager = new AttackManager(player);
                     atkmanager.LearnAttack(AttackLibrary.Lunge);
                 }
                 else
                 {
                     if (player.level >= 10 && flawlessVictories >= 25)
                     {
                         if (!player.ownedAttacks.Contains(AttackLibrary.Bloodieddagger))
                         {
                             MainUI.WriteInMainArea("\n\"Let me teach you a new technique...\"");
                             AttackManager atkmanager = new AttackManager(player);
                             atkmanager.LearnAttack(AttackLibrary.Bloodieddagger);
                         }
                         else
                         {
                             MainUI.WriteInMainArea("You have grown strong, shadow walker, the figure says.");
                         }
                     }
                     else
                     {
                         MainUI.WriteInMainArea("You have learned what I can teach you for now.");
                         MainUI.WriteInMainArea($"Return when you reach level 10 and have 25 flawless victories.");
                         MainUI.WriteInMainArea($"Then I will teach you further.");
                         MainUI.WriteInMainArea($"(Current: Level {player.level}/10, Flawless Victories: {flawlessVictories}/25)");
                     }
                 }

                 MainUI.WriteInMainArea("The figure fades back into the shadows.");
             }
             else
             {
                 MainUI.WriteInMainArea("Impressive, the figure says in a low voice.");
                 MainUI.WriteInMainArea("I've been watching you. Your technique is flawless.");
                 MainUI.WriteInMainArea("You strike without giving your enemies a chance to retaliate.");
                 MainUI.WriteInMainArea("I am a master of the shadows. Would you like to learn my ways?");
                 MainUI.WriteInMainArea("Become a Rogue? (y/n): ");

                 string classChoice = Console.ReadKey(true).KeyChar.ToString().ToLower();
                 if (classChoice == "y" || classChoice == "yes")
                 {
                     player.playerClass = ClassLibrary.Rogue;
                     player.RecalculateStats();
                     MainUI.WriteInMainArea("The figure nods approvingly.");
                     MainUI.WriteInMainArea("Welcome to the shadows. You are now a Rogue.");
                     MainUI.WriteInMainArea("The figure vanishes into the darkness.");
                     Program.SavePlayer();
                 }
                 else
                 {
                     MainUI.WriteInMainArea("Perhaps another time, the figure says, fading into the shadows.");
                 }
             }
         }
         else
         {
             // Has 1-4 flawless victories
             MainUI.WriteInMainArea("You show promise, the figure says.");
             MainUI.WriteInMainArea("But you are not yet strong enough to be my disciple.");
             MainUI.WriteInMainArea("Get more fighting experience before returning.");
             MainUI.WriteInMainArea($"(Flawless Victories: {flawlessVictories}/5)");
             MainUI.WriteInMainArea("The figure disappears back into the shadows.");
         }
     }
     else
     {
         MainUI.WriteInMainArea("You decide not to risk it and move on.");
     }
 },
 EncounterType.Event
);



    public static Encounter InjuredWolf = new Encounter(
        "InjuredWolf",
        false,
        "You find a Dire Wolf lying on the ground, badly injured and bleeding.",
        null,
        (player) => {
            MainUI.WriteInMainArea("The wolf looks at you with desperate, pleading eyes.");
            MainUI.WriteInMainArea("It's losing blood fast and needs immediate help.");
            MainUI.WriteInMainArea("\nOffer a Big Health Potion to heal it? (y/n): ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            
            if (choice == "y" || choice == "yes")
            {
                var bigHealingPotion = player.ownedItems.FirstOrDefault(i => i.name == ItemLibrary.bigHealthPotion.name && i.amount > 0);
                
                if (bigHealingPotion == null)
                {
                    MainUI.WriteInMainArea("\nYou don't have a Big Health Potion!");
                    MainUI.WriteInMainArea("The wolf whimpers as you walk away empty-handed.");
                    return;
                }
                
                int attackRoll = rng.Next(1, 4);
                if (attackRoll == 1)
                {
                    MainUI.WriteInMainArea("\nAs you approach with the potion, the wolf panics!");
                    MainUI.WriteInMainArea("In its pain and fear, it sees you as a threat!");
                    MainUI.WriteInMainArea("The Dire Wolf attacks!");
                    MainUI.WriteInMainArea("\nPress Enter to continue...");
                    Console.ReadLine();
                    
                    var combat = new CombatManager(player, new List<Enemy> { EnemyLibrary.DireWolf }, true, null);
                    combat.StartCombat();
                    return;
                }
                
                int maxCompanions = CompanionSystem.GetMaxCompanions(player);
                var currentCompanions = CompanionSystem.GetCompanions(player);
                
                if (maxCompanions == 0)
                {
                    MainUI.WriteInMainArea($"\nYour class ({player.playerClass.name}) cannot have companions!");
                    MainUI.WriteInMainArea("You use the potion anyway out of kindness.");
                    bigHealingPotion.amount--;
                    if (bigHealingPotion.amount <= 0)
                    {
                        player.ownedItems.Remove(bigHealingPotion);
                    }
                    MainUI.WriteInMainArea("The wolf recovers and runs off into the forest.");
                }
                else if (currentCompanions.Count >= maxCompanions)
                {
                    MainUI.WriteInMainArea($"\nYou already have the maximum number of companions ({maxCompanions}).");
                    MainUI.WriteInMainArea("You use the potion anyway out of kindness.");
                    bigHealingPotion.amount--;
                    if (bigHealingPotion.amount <= 0)
                    {
                        player.ownedItems.Remove(bigHealingPotion);
                    }
                    MainUI.WriteInMainArea("The wolf recovers but sadly cannot join you.");
                    MainUI.WriteInMainArea("It howls gratefully before disappearing into the woods.");
                }
                else
                {
                    bigHealingPotion.amount--;
                    if (bigHealingPotion.amount <= 0)
                    {
                        player.ownedItems.Remove(bigHealingPotion);
                    }
                    
                    MainUI.WriteInMainArea("\nYou carefully pour the Big Health Potion over the wolf's wounds.");
                    MainUI.WriteInMainArea("The wolf's injuries begin to close and its breathing steadies.");
                    MainUI.WriteInMainArea("It stands up, fully healed, and looks at you with deep gratitude.");
                    MainUI.WriteInMainArea("The wolf nuzzles your hand and refuses to leave your side!");
                    
                    bool success = CompanionSystem.RecruitByName(player, "dire wolf");
                    
                    if (success)
                    {
                        MainUI.WriteInMainArea("\nThe Dire Wolf has joined your party as a loyal companion!");
                    }
                }
                
                Program.SavePlayer();
            }
            else
            {
                MainUI.WriteInMainArea("\nYou decide not to use your potion.");
                MainUI.WriteInMainArea("The wolf's breathing grows weaker as you walk away...");
            }
        },
        EncounterType.Event
    );

    #endregion
}
