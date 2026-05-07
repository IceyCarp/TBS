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

    #region Combat Encounters - Wilderness

    public static Encounter SnowWolfPack = new Encounter(
    "SnowWolfPack",
    true,
    "A pack of Snow Wolves emerges from the trees",
    new List<Enemy> { EnemyLibrary.SnowWolf, EnemyLibrary.SnowWolf },
    null,
    EncounterType.Combat
    );

    public static Encounter SnowElkAndWolf = new Encounter(
    "SnowElkAndWolf",
    true,
    "You come across a Frozen Elk and a snow wolf, they arent friendly",
    new List<Enemy> { EnemyLibrary.FrozenElk, EnemyLibrary.SnowWolf },
    null,
    EncounterType.Combat
    );

    public static Encounter ColdStalker = new Encounter(
    "ColdStalker",
    true,
    "you've been spottet by an ColdStalker",
    new List<Enemy> { EnemyLibrary.ColdStalker },
    null,
    EncounterType.Combat
    );

    public static Encounter FrozenElk = new Encounter(
    "FrozenElk",
    true,
    "You come across a Frozen Elk, it isn't friendly",
    new List<Enemy> { EnemyLibrary.FrozenElk },
    null,
    EncounterType.Combat
    );

    public static Encounter RotWalkerHorde = new Encounter(
    "RotWalkerHorde",
    true,
    "you find yourself in the middle of a Horde of RotWalkers",
    new List<Enemy> { EnemyLibrary.rotWalker, EnemyLibrary.rotWalker, EnemyLibrary.rotWalker, EnemyLibrary.rotWalker,
        EnemyLibrary.rotWalker, EnemyLibrary.rotWalker, EnemyLibrary.rotWalker, EnemyLibrary.rotWalker, EnemyLibrary.rotWalker,EnemyLibrary.rotWalker,EnemyLibrary.rotWalker,EnemyLibrary.rotWalker },
    null,
    EncounterType.Combat
    );

    public static Encounter CaravanAmbush = new Encounter(
    "CaravanAmbush",
    true,
    "You find yourself in the way of a traveling caravan\nthey take offence and launch an attack",
    new List<Enemy> { EnemyLibrary.CaravanGuard, EnemyLibrary.CaravanGuard, EnemyLibrary.CrossbowMercenary },
    null,
    EncounterType.Combat
    );

    public static Encounter CaravanTrap = new Encounter(
    "CaravanTrap",
    true,
    "You see an abandoned caravan, as you approach you are suddenly under attack",
    new List<Enemy> { EnemyLibrary.CaravanGuard, EnemyLibrary.Thug, EnemyLibrary.CrossbowMercenary, EnemyLibrary.CrossbowMercenary },
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

    
    public static Encounter BridgeTroll = new Encounter("BridgeTroll",
        false,
        "You stumble upon a bride, and hear a large creature talking to you below",
        null,
        (player) => {
        MainUI.WriteInMainArea("The creature asks you for rai to pass his bridge.");
        MainUI.WriteInMainArea("He seems a bit aggressive, if u dont pay him, we will likely start a fight.");
        MainUI.WriteInMainArea("\nDo u offer him any rai? ");
            string choice = Console.ReadKey(true).KeyChar.ToString().ToLower();
            if (choice == "y" || choice == "yes")
            {
                MainUI.WriteInMainArea($"how much do you want to offer him? current Rai: {Program.player.money}");
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
                int roll = rng.Next(1, bet+20);
                if (roll >= 19)
                {
                    MainUI.WriteInMainArea($"The troll takes your offered amount of {bet} and lets u pass");
                    Program.player.money -= bet;

                }
                else
                {
                    MainUI.WriteInMainArea($"you offered {bet}, the troll looks at you with anger and starts a fight");
                    Program.player.money -= bet;
                    Program.SavePlayer();
                    var combat = new CombatManager(player, new List<Enemy> { EnemyLibrary.BridgeTroll }, true, null);
                    combat.StartCombat();
                }
            }
            else
            {
                MainUI.WriteInMainArea("The troll, angered, starts a brawl with you.");
                var combat = new CombatManager(player, new List<Enemy> { EnemyLibrary.BridgeTroll }, true, null);
                combat.StartCombat();
            }
        },
        EncounterType.Event
    );
    public static Encounter AncientLibrary = new Encounter(
        "AncientLibrary",
        false,
        "The Librarian's ghost appears. 'How fast is your mind?'",
        null,
        (player) => {
            int correct = 0;
            DateTime end = DateTime.Now.AddSeconds(30);
            MainUI.WriteInMainArea("30 seconds of speed math! Solve as many as you can.");
            MainUI.WriteInMainArea("Press any key to start...");
            Console.ReadKey(true);
            while (DateTime.Now < end)
            {
                int a = rng.Next(2, 13), b = rng.Next(2, 13);
                int secsLeft = (int)(end - DateTime.Now).TotalSeconds;
                MainUI.WriteInMainArea($"[{secsLeft}s] {a} + {b} = ?");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int res) && res == (a + b))
                {
                    correct++;
                    MainUI.WriteInMainArea("Correct!");
                }
                else
                {
                    MainUI.WriteInMainArea("Wrong!");
                }
            }
            int expGained = correct * 15;
            player.exp += expGained;
            MainUI.WriteInMainArea($"Time's up! Solved {correct} problems. +{expGained} EXP.");
            Program.SavePlayer();
        },
        EncounterType.Event
    );

    public static Encounter ChaosSignpost = new Encounter(
    "ChaosSignpost",
    false,
    "You find a splintered signpost with two confusing directions.",
    null,
    (player) => {
        string[] labels = { "FREE LUNCH", "CERTAIN DOOM", "DRAGON'S DEN", "SHORTCUT", "FREE RAI", "THE ABYSS", "SAFETY", "A.. CASINO, TRUST!", "CHINESE RESTAURANT", "MAID CAFE", "MONSTER AHEAD (FIRNDLY)", "MONSTER AHEAD (NOT FIRNDLY)", "DEFINITELY NOT A TRAP", "BOSS FIGHT", "???", "THE 'FUN' ROUTE", "LAST KNOWN LOCATION OF GREG", "Bazinga" };

        string left = "";
        string right = "";

        while (true)
        {
            left = labels[rng.Next(labels.Length)];
            right = labels[rng.Next(labels.Length)];
            if (left != right) break;
        }

        MainUI.WriteInMainArea($"Left: {left} | Right: {right}");
        MainUI.WriteInMainArea("Which way do you go? (1: Left, 2: Right)");
        Console.ReadKey(true);

        int outcome = rng.Next(1, 20);
        switch (outcome)
        {
            case 1:
                int rai = rng.Next(25, 100);
                player.money += rai;
                MainUI.WriteInMainArea($"You found {rai} Rai hidden in a bush! \nWho left this here?");
                break;
            case 2:
                int dmg = rng.Next(5, 15);
                player.HP -= dmg;
                MainUI.WriteInMainArea($"A hidden trap triggers! You take {dmg} damage.");
                Program.CheckPlayerDeath();
                break;
            case 3:
                player.exp += 20;
                MainUI.WriteInMainArea("The path was scenic and peaceful. You gained 20 EXP.");
                break;
            case 4:
                MainUI.WriteInMainArea("The path leads in a circle. You wasted time, but found nothing.");
                break;
            case 5:
                var knownLocs = player.knownLocationnames
                    .Where(n => n != player.currentLocation && n != Encounter.TravelDestination)
                    .ToList();
                if (knownLocs.Count > 0)
                {
                    string dest = knownLocs[rng.Next(knownLocs.Count)];
                    player.currentLocation = dest;
                    MainUI.WriteInMainArea($"A mysterious force yanks you off your feet!");
                    MainUI.WriteInMainArea($"You land in {dest}. The signpost cackles behind you.");
                    Encounter.SkipRemainingEncounters = true;
                }
                else
                {
                    MainUI.WriteInMainArea("A mysterious force tries to move you... but you have nowhere to go.");
                }
                break;
            case 6:
                if (player.money > 0)
                {
                    int stolen = rng.Next(10, Math.Min(31, player.money + 1));
                    player.money -= stolen;
                    MainUI.WriteInMainArea($"The signpost rattles and shakes.");
                    MainUI.WriteInMainArea($"You feel lighter. {stolen} Rai have vanished from your pocket.");
                }
                else
                {
                    MainUI.WriteInMainArea("The signpost eyes your empty pockets and says nothing.");
                }
                break;
            case 7:
            case 8:
                string[] ominous = {
                    "The signpost spins slowly and stops, pointing nowhere.\nYou feel watched.",
                    "One of the signs reads your name.\nWhen you look again, it doesn't.",
                    "The signpost leans toward you as you pass.\nYou walk faster.",
                    "All the signs point the same direction.\nThat direction is down.",
                    "You hear laughter from the signpost.\nThere is no one around.",
                    "The wood is warm to the touch.\nYou did not touch it.",
                    "A sign falls off as you approach.\nIt was blank on both sides."
                };
                MainUI.WriteInMainArea(ominous[rng.Next(ominous.Length)]);
                break;
            case 9:
                MainUI.WriteInMainArea("The signpost wobbles. A painted face appears on the wood.");
                MainUI.WriteInMainArea("\"Hey! You there. How are you doing? Tell me everything- \nHow are you, what have u been up to lately?\"");
                MainUI.WriteInMainArea("(Type your answer and press Enter)");
                Console.ReadLine();
                string[] responses = {
                    "The signpost stares blankly.\n\"Didn't ask.\"",
                    "\"Cool story.\" \nThe signpost turns away.",
                    "\"Wow.\" \n...It does not elaborate.",
                    "\"I don't care even a little bit.\"",
                    "\"Mmhm. Mmhm.\" \nIt was not listening.",
                    "The signpost yawns.\n\"Are you done?\"",
                    "\"That's crazy. Anyway.\"",
                };
                int pick = rng.Next(responses.Length + 2);
                if (pick >= responses.Length)
                {
                    int slap = rng.Next(3, 8);
                    player.HP -= slap;
                    MainUI.WriteInMainArea("The signpost swings and smacks you across the face.");
                    MainUI.WriteInMainArea($"\"Nobody asked.\" -{slap} HP.");
                    MainUI.WriteInMainArea("You stumble away from the signpost hurt after the swing, \nas you hear it jumping away laughing hysterically at you");
                    Program.CheckPlayerDeath();
                }
                else
                {
                    MainUI.WriteInMainArea(responses[pick]);
                }
                break;
            case 10:
                MainUI.WriteInMainArea("The signpost suddenly sprouts legs and arms, and hops around you excitedly.");
                MainUI.WriteInMainArea("It grabs your sleeve. \"Hey. HEY. Lend me your ear for a second.\"");
                MainUI.WriteInMainArea("Press any key...");
                Console.ReadKey(true);
                MainUI.WriteInMainArea("It leans in close and cups its hands around where a mouth would be.");
                MainUI.WriteInMainArea("\"...Nobody will ever believe you.\"");
                MainUI.WriteInMainArea("It sits back down and becomes a normal signpost again.");
                break;
            case 11:
                MainUI.WriteInMainArea("The signpost points aggressively into the bushes.");
                MainUI.WriteInMainArea("Something stirs. Then more somethings.");
                MainUI.WriteInMainArea("Press any key...");
                Console.ReadKey(true);
                var goblinHorde = new List<Enemy> {
                    EnemyLibrary.Goblin, EnemyLibrary.Goblin, EnemyLibrary.Goblin,
                    EnemyLibrary.Goblin, EnemyLibrary.Goblin
                };
                var hordeCombat = new CombatManager(player, goblinHorde, true, null);
                hordeCombat.StartCombat();
                break;

            case 12:
                if (player.ownedItems.Count > 0)
                {
                    Item threatened = player.ownedItems[rng.Next(player.ownedItems.Count)];
                    MainUI.WriteInMainArea("The signpost eyes your belongings.");
                    MainUI.WriteInMainArea($"It points directly at your {threatened.name}.");
                    MainUI.WriteInMainArea("\"Leave it. Or else.\"");
                    MainUI.WriteInMainArea("Press any key...");
                    Console.ReadKey(true);
                    Inventory.DropItem(threatened, 1);
                    MainUI.WriteInMainArea($"You leave your {threatened.name} on the ground and back away slowly.");
                }
                else
                {
                    MainUI.WriteInMainArea("The signpost eyes your belongings.");
                    MainUI.WriteInMainArea("You have nothing. It seems almost disappointed.");
                }
                break;

            case 13:
                var teleportLocs = player.knownLocationnames
                    .Where(n => n != player.currentLocation && n != Encounter.TravelDestination)
                    .ToList();
                if (teleportLocs.Count > 0)
                {
                    string realDest = teleportLocs[rng.Next(teleportLocs.Count)];
                    string fakeDest = teleportLocs[rng.Next(teleportLocs.Count)];
                    MainUI.WriteInMainArea("The signpost glows and grabs you by the collar.");
                    MainUI.WriteInMainArea($"\"Sending you to... {fakeDest}!\"");
                    MainUI.WriteInMainArea("Press any key...");
                    Console.ReadKey(true);
                    if (fakeDest != realDest && rng.Next(0, 2) == 0)
                    {
                        MainUI.WriteInMainArea("\"...Actually.\"");
                        MainUI.WriteInMainArea($"\"It's {realDest}. I misread it.\"");
                        player.currentLocation = realDest;
                    }
                    else
                    {
                        player.currentLocation = fakeDest;
                        MainUI.WriteInMainArea($"You land in {fakeDest}.");
                    }
                    MainUI.WriteInMainArea("The signpost waves goodbye.");
                    Encounter.SkipRemainingEncounters = true;
                }
                else
                {
                    MainUI.WriteInMainArea("The signpost tries to grab you but has nowhere to send you.");
                }
                break;

            case 14:
                MainUI.WriteInMainArea("The signpost begins talking to itself.");
                MainUI.WriteInMainArea("\"Send them left.\" \"No, right.\" \"Left is better.\"");
                MainUI.WriteInMainArea("Press any key...");
                Console.ReadKey(true);
                MainUI.WriteInMainArea("\"Right has more charm.\" \"Left has CHARACTER.\"");
                MainUI.WriteInMainArea("\"Fine. Neither. We send them nowhere.\"");
                MainUI.WriteInMainArea("\"...Agreed.\"");
                MainUI.WriteInMainArea("The signpost goes quiet. You leave while it's distracted.");
                break;

            case 15:
                MainUI.WriteInMainArea("The signpost clears its throat.");
                MainUI.WriteInMainArea("\"A poem. For the traveller.\"");
                MainUI.WriteInMainArea("Press any key...");
                Console.ReadKey(true);
                string[] poems = {
                    "\"Roads go left.\nRoads go right.\nYou will probably be fine.\nGoodnight.\"",
                    "\"There once was a man on a path.\nHe walked it and did some math.\nHe went left, or right,\nit was fine, or not quite.\nThe signpost just stood there and laughed.\"",
                    "\"Left.\nRight.\nYou.\nWhy.\"",
                    "\"The road is long.\nThe road is wide.\nI am a sign.\nYou are outside.\""
                };
                MainUI.WriteInMainArea(poems[rng.Next(poems.Length)]);
                MainUI.WriteInMainArea("The signpost takes a bow.");
                break;

            case 16:
                int bigExp = player.level * 50;
                player.exp += bigExp;
                MainUI.WriteInMainArea("The signpost looks at you for a long moment.");
                MainUI.WriteInMainArea("Then looks away.");
                MainUI.WriteInMainArea($"+{bigExp} EXP.");
                MainUI.WriteInMainArea("It does not explain this.");
                break;

            case 17:
                int healed = player.maxHP - player.HP;
                player.HP = player.maxHP;
                MainUI.WriteInMainArea("The signpost pats you on the head.");
                MainUI.WriteInMainArea($"You feel completely restored. +{healed} HP.");
                MainUI.WriteInMainArea("\"You look terrible. Go on.\"");
                break;

            case 18:
                if (!player.ownedAttacks.Any(a => a.name == "Misdirection"))
                {
                    new AttackManager(player).LearnAttack(AttackLibrary.Misdirection);
                    MainUI.WriteInMainArea("The signpost rips off one of its arms and hands it to you.");
                    MainUI.WriteInMainArea("\"Hit someone with this. Confuse them.\"");
                    MainUI.WriteInMainArea("\"It works on me all the time.\"");
                    MainUI.WriteInMainArea("It grows a new arm immediately. You learned Misdirection.");
                }
                else
                {
                    MainUI.WriteInMainArea("The signpost goes to hand you something, then notices you already have it.");
                    MainUI.WriteInMainArea("\"Oh. You've met me before.\"");
                    int bonusExp = player.level * 25;
                    player.exp += bonusExp;
                    MainUI.WriteInMainArea($"+{bonusExp} EXP as a consolation.");
                }
                break;
            case 19:
                MainUI.WriteInMainArea("The signpost starts shaking violently.");
                MainUI.WriteInMainArea("The ground rumbles. You hear screaming in the distance.");
                MainUI.WriteInMainArea("Press any key...");
                Console.ReadKey(true);
                MainUI.WriteInMainArea("Twenty goblins burst out of the treeline, charging directly at you.");
                MainUI.WriteInMainArea("Press any key...");
                Console.ReadKey(true);
                MainUI.WriteInMainArea("They run straight past you.");
                MainUI.WriteInMainArea("...");
                MainUI.WriteInMainArea("The signpost giggles.");
                MainUI.WriteInMainArea("\"Got you.\"");
                break;
        }
        Program.SavePlayer();
    },
    EncounterType.Trap
);
    public static Encounter AriaPropagandaBoard = new Encounter(
        "AriaPropagandaBoard",
        false,
        "A large board stands by the roadside, plastered with colourful posters.",
        null,
        (player) => {
            string[] posters = {
            "ARIA — THE CITY THAT NEVER SLEEPS (AND NEITHER SHOULD YOU)\nFine print: Sleep deprivation is not the city's responsibility.",
            "THINKING OF LEAVING ARIA?\nDon't.\n- The Aria City Council",
            "ARIA: POPULATION 40,000 HAPPY CITIZENS\n(Happiness is mandatory.)",
            "VISIT ARIA!\nClean streets. Safe walls. No questions asked.\nAsk no questions.",
            "ARIA WELCOMES ALL TRAVELLERS\n(Subject to entry screening, background checks,\nweapon confiscation and a small processing fee.)",
            "THE ROAD TO ARIA IS THE ROAD TO OPPORTUNITY\nAria is not responsible for what happens on the road.",
            };
            MainUI.WriteInMainArea("The board is covered in official-looking posters about Aria.");
            Thread.Sleep(1500);
            MainUI.WriteInMainArea("You stop to read one.");
            Thread.Sleep(2000);
            MainUI.WriteInMainArea(posters[rng.Next(posters.Length)]);
            Thread.Sleep(4000);
            MainUI.WriteInMainArea("\nYou stand there for a moment.");
            Thread.Sleep(1500);
            MainUI.WriteInMainArea("You continue walking.");
            Program.SavePlayer();
        },
        EncounterType.Event
    );


    public static Encounter SuspiciousChest = new Encounter(
        "SuspiciousChest",
        false,
        "A lone chest sits in the middle of the road. It's... breathing.",
        null,
        (player) => {
            MainUI.WriteInMainArea("Open the chest? (y/n)");
            if (Console.ReadKey(true).KeyChar == 'y')
            {
                if (rng.Next(1, 101) <= 40)
                {
                    player.HP -= 20;
                    if (player.HP < 1) player.HP = 1;
                    MainUI.WriteInMainArea("MIMIC! Rows of teeth slam shut. -20 HP. You scramble free.");
                }
                else
                {
                    player.money += 25;
                    MainUI.WriteInMainArea("Just a chest after all. 25 Rai inside — not bad.");
                }
                Program.SavePlayer();
            }
            else
            {
                MainUI.WriteInMainArea("You give it a wide berth. It watches you leave.");
            }
        },
        EncounterType.Treasure
    );
    public static Encounter DyingTraveler = new Encounter(
    "DyingTraveler",
    false,
    "Someone is slumped against a milestone by the road. They're still breathing. Barely.",
    null,
    (player) => {
        MainUI.WriteInMainArea("A traveler, badly wounded, reaches a hand toward you.");
        MainUI.WriteInMainArea("\"Please...\"");
        MainUI.WriteInMainArea("");

        var healingItem = player.ownedItems.FirstOrDefault(i =>
            i.stats.ContainsKey("heal") && i.amount >= 1);

        if (healingItem != null)
        {
            MainUI.WriteInMainArea($"You have a {healingItem.name}. Use it on them? (y/n)");
            if (Console.ReadKey(true).KeyChar == 'y')
            {
                Inventory.DropItem(healingItem, 1);
                MainUI.WriteInMainArea("You kneel down and use it on them.");
                MainUI.WriteInMainArea("The colour slowly returns to their face.");
                MainUI.WriteInMainArea("They grab your arm.");
                MainUI.WriteInMainArea("Press any key...");
                Console.ReadKey(true);
                string[] info = {
                    "\"Don't go to Aria through the main gate.\nThey search everyone. There's a side entrance, east wall.\"",
                    "\"The road north of here... something lives in it now.\nIt isn't an animal.\"",
                    "\"I had coin on me. It's gone.\nWhoever did this came from the direction you're heading.\"",
                    "\"Aria isn't what they say it is.\nJust... remember that when you get there.\"",
                };
                MainUI.WriteInMainArea(info[rng.Next(info.Length)]);
                int expGain = player.level * 10;
                player.exp += expGain;
                MainUI.WriteInMainArea($"\n+{expGain} EXP.");
            }
            else
            {
                MainUI.WriteInMainArea("You step around them and keep walking.");
                MainUI.WriteInMainArea("They don't call after you.");
                MainUI.WriteInMainArea("You don't look back.");
            }
        }
        else
        {
            MainUI.WriteInMainArea("You have nothing to help them with.");
            MainUI.WriteInMainArea("You check your bag twice, just to be sure.");
            MainUI.WriteInMainArea("\"It's alright,\" they say.");
            MainUI.WriteInMainArea("\"Go on.\"");
            MainUI.WriteInMainArea("You do.");
        }
        Program.SavePlayer();
    },
    EncounterType.Event
);
    #endregion
}
