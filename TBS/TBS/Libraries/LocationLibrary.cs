using System.Collections.Generic;

public static class LocationLibrary
{
    #region "Coastal Alliance"
    public static Location Maplecross = new Location("Maplecross", new System.Numerics.Vector2(0, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.smallHealthPotion),
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.baseballCap),
                (ItemLibrary.sandals),
                (ItemLibrary.speedPotion),
                (ItemLibrary.BasicBackpack),
                (ItemLibrary.sword)
            }
        },
        new SubLocation("Blacksmith", SubLocationType.blacksmith)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Bank", SubLocationType.bank)
        {

        },
        new SubLocation("Market", SubLocationType.marketplace)
        {

        }

    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 40},
        { EncounterLibrary.LostCoins, 30},
        { EncounterLibrary.BanditFight, 20},
        { EncounterLibrary.WanderingMerchant, 15},
        { EncounterLibrary.MysteriousShrine, 5},
        { EncounterLibrary.LostChild, 5}
    }, "Coastal Alliance");

    public static Location Mistport = new Location("Mistport", new System.Numerics.Vector2(0, -1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("pond", SubLocationType.pond)
        {

        },
        new SubLocation("Bank", SubLocationType.bank)
        {

        },
        new SubLocation("Port", SubLocationType.port)
        {

        },
        new SubLocation("Market", SubLocationType.marketplace)
        {

        }
    },
   new Dictionary<Encounter, int>
   {
        { EncounterLibrary.FoundCoins, 35},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.StrangeMushrooms, 20},
        { EncounterLibrary.WanderingMerchant, 15},
        { EncounterLibrary.FallenIntoTrap, 10},
        { EncounterLibrary.FallingFish, 10},
        { EncounterLibrary.LostChild, 5}
   }, "Coastal Alliance");

    public static Location SaltmarshShore = new Location("Saltmarsh Shore", new System.Numerics.Vector2(-1, -1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("pond", SubLocationType.pond)
        {

        },        
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.smallHealthPotion),
                (ItemLibrary.RoughTrousers),
                (ItemLibrary.FoulHook),
                (ItemLibrary.CheapAle)
            }
        },
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 35},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.FallingFish, 20},
        { EncounterLibrary.SeaBanditRaid, 15},
        { EncounterLibrary.WanderingMerchant, 10}
    }, "Coastal Alliance");

    #endregion

    #region "Greenwood Territories"
    public static Location Greenhollow = new Location("Greenhollow", new System.Numerics.Vector2(1, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.smallHealthPotion),
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.speedBoots),
                (ItemLibrary.camoPants)
            }
        },
        new SubLocation("Blacksmith", SubLocationType.blacksmith)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.WildGoblin, 35},
        { EncounterLibrary.GoblinPack, 1},
        { EncounterLibrary.StrangeMushrooms, 30},
        { EncounterLibrary.FoundCoins, 20 },
        { EncounterLibrary.FallenIntoTrap, 20},
        { EncounterLibrary.FoundTreasure, 10},
        { EncounterLibrary.RoadGambling, 10},
        { EncounterLibrary.LearnFirstAid, 5},
        { EncounterLibrary.WeakRopeBridge, 5}
    }, "Greenwood Territories");

    public static Location WhisperWood = new Location("WhisperWood", new System.Numerics.Vector2(2, 1), 3, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Market", SubLocationType.marketplace)
        {

        },        
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.CrudePoisonSpear),
                (ItemLibrary.ExpeditionVest),
                (ItemLibrary.WildBerries)

            }
        },
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.ForestSpiderNest, 30},
        { EncounterLibrary.DireWolfHunt, 15},
        { EncounterLibrary.Wolfsensei, 15},
        { EncounterLibrary.DireWolfPack, 10},
        { EncounterLibrary.StrangeMushrooms, 20},
        { EncounterLibrary.FoundTreasure, 15},
        { EncounterLibrary.AbandonedBackpack, 5},
        { EncounterLibrary.InjuredWolf, 2},
        { EncounterLibrary.WeakRopeBridge, 5}
    }, "Greenwood Territories");

    public static Location MossGate = new Location("MossGate", new System.Numerics.Vector2(2, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Bank", SubLocationType.bank)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 35},
        { EncounterLibrary.StrangeMushrooms, 40},
        { EncounterLibrary.FallenIntoTrap, 10},
        { EncounterLibrary.GoblinPack, 15},
        { EncounterLibrary.RoadGambling, 10},
        { EncounterLibrary.LearnFirstAid, 15},
        { EncounterLibrary.RogueUnlock, 5},
        { EncounterLibrary.WeakRopeBridge, 5}
    }, "Greenwood Territories");

    public static Location Fernshade = new Location("Fernshade", new System.Numerics.Vector2(3, 1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.WildBerries),
                (ItemLibrary.bigHealthPotion)
            }
        },
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.GoblinPack, 50},
        { EncounterLibrary.StrangeMushrooms, 30},
        { EncounterLibrary.FoundCoins, 20 },
        { EncounterLibrary.FallenIntoTrap, 40},
        { EncounterLibrary.FoundTreasure, 10},
        { EncounterLibrary.RoadGambling, 10},
        { EncounterLibrary.DireWolfPack, 20},
        { EncounterLibrary.AbandonedBackpack, 5},
        { EncounterLibrary.InjuredWolf, 5},
        { EncounterLibrary.WeakRopeBridge, 10}
    }, "Greenwood Territories");
    #endregion

    #region "Fallen Kingdom"
    public static Location Ironpeak = new Location("Ironpeak", new System.Numerics.Vector2(-1, 0), 1, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.speedBoots),
                (ItemLibrary.knightHelmet),
                (ItemLibrary.constructionVest),
                (ItemLibrary.StoneMaul)

            }
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Casino",SubLocationType.casino)
        {
            casinoMaxBet = 20
        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 40},
        { EncounterLibrary.FoundTreasure, 30},
        { EncounterLibrary.MysteriousShrine, 25},
        { EncounterLibrary.WanderingMerchant, 20},
        { EncounterLibrary.BanditFight, 50},
        { EncounterLibrary.BanditAmbush, 3},
        { EncounterLibrary.WeakRopeBridge, 3}
    }, "Fallen Kingdom");

    public static Location ShattershoreCliffs = new Location("Shattershore Cliffs", new System.Numerics.Vector2(-2, 0), 3, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Market", SubLocationType.marketplace)
        {

        },
        new SubLocation("Training Grounds", SubLocationType.TrainingGrounds)
        {
            
        },
        new SubLocation("Blacksmith", SubLocationType.blacksmith)
        {
        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.KingdomGuardPatrol, 30},
        { EncounterLibrary.SmugglerAmbush, 25},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.BanditAmbush, 15},
        { EncounterLibrary.FallenIntoTrap, 10},
        { EncounterLibrary.WeakRopeBridge, 15}
    }, "Fallen Kingdom");

    public static Location WitheredRuins = new Location("Withered Ruins", new System.Numerics.Vector2(-2, 2), 3, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Mine",SubLocationType.mine)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.RuinedGolemAwakens, 25},
        { EncounterLibrary.CorruptedKnightBattle, 25},
        { EncounterLibrary.GhostlyApparition, 30},
        { EncounterLibrary.SkeletonWarriors, 30},
        { EncounterLibrary.FoundTreasure, 20},
        { EncounterLibrary.AbandonedBackpack, 2}
    }, "Fallen Kingdom");

    public static Location Nightreach = new Location("Nightreach", new System.Numerics.Vector2(-1, 1), 1, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.VampireRing),
                (ItemLibrary.CloakofDusk),
                (ItemLibrary.NightStalkerGreaves),
                (ItemLibrary.VampireMask)
            }
        },
        new SubLocation("Market", SubLocationType.marketplace)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundTreasure, 40},
        { EncounterLibrary.VampireAttack, 60}
    }, "Fallen Kingdom");

    public static Location SilverfallRuins = new Location("Silverfall Ruins", new System.Numerics.Vector2(-2, 1), 3, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.FallenGuardHelmet),
                (ItemLibrary.SilverfallAmulet)
            }
        },        
        new SubLocation("Mine",SubLocationType.mine)
        {

        }
    },
new Dictionary<Encounter, int>
{
        { EncounterLibrary.FoundTreasure, 20},
        { EncounterLibrary.GhostlyApparition, 60},
        { EncounterLibrary.SkeletonWarriors, 60},
        { EncounterLibrary.AbandonedBackpack, 2}
}, "Fallen Kingdom");
    #endregion

    #region "wilderness"
    #region "Tofrozen" 
    public static Location FrozenWastes = new Location("Frozen Wastes", new System.Numerics.Vector2(-3, 1), 20, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 40},
        { EncounterLibrary.FrostTrollAmbush, 30},
        { EncounterLibrary.IceMageEncounter, 25},
        { EncounterLibrary.FrozenHorde, 15},
        { EncounterLibrary.LostChild, 1}
    }, null);

    public static Location TundraMarch = new Location("Tundra March", new System.Numerics.Vector2(-4, 1), 20, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 45},
        { EncounterLibrary.FrostTrollAmbush, 35},
        { EncounterLibrary.IceMageEncounter, 20},
        { EncounterLibrary.SnowWraithAttack, 10}
    }, null);
    #endregion
    #region "Elves"
    public static Location SvalbardWastes = new Location("Svalbard Wastes", new System.Numerics.Vector2(3, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
            new Dictionary<Encounter, int>
    {
        { EncounterLibrary.DireWolfPack, 50},
        { EncounterLibrary.GoblinPack, 20}
    }, null);
    #endregion
    #region "ToAria"

    public static Location ColdshadeForest = new Location("Coldshade forest", new System.Numerics.Vector2(-7, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.SnowWolfPack, 50},
        { EncounterLibrary.SnowElkAndWolf, 30},
        { EncounterLibrary.ColdStalker, 10},
        { EncounterLibrary.FrozenElk, 50},
        { EncounterLibrary.AbandonedBackpack, 20 },
        { EncounterLibrary.StrangeMushrooms, 15 },
        { EncounterLibrary.InjuredWolf, 2 },
        { EncounterLibrary.AriaPropagandaBoard, 15 },
        { EncounterLibrary.ChaosSignpost, 8 }

    }, null);
    public static Location QuietClearing = new Location("Quiet clearing", new System.Numerics.Vector2(-8, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Clearing",SubLocationType.swordInStone)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.DyingTraveler, 30 },
        { EncounterLibrary.AncientLibrary, 20 },
        { EncounterLibrary.MysteriousShrine, 25 },
        { EncounterLibrary.FoundTreasure, 15 },
        { EncounterLibrary.AriaPropagandaBoard, 10 },
        { EncounterLibrary.ChaosSignpost, 5 },
    }, null);

    public static Location TheWithering = new Location("The Withering", new System.Numerics.Vector2(-7, -1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.DireWolfPack, 50},
        { EncounterLibrary.SkeletonWarriors, 25 },
        { EncounterLibrary.VampireAttack, 20 },
        { EncounterLibrary.MysteriousShrine, 15 },
        { EncounterLibrary.SuspiciousChest, 20 },
        { EncounterLibrary.DyingTraveler, 25 },
        { EncounterLibrary.AncientLibrary, 10 },
        { EncounterLibrary.ChaosSignpost, 15 },

    }, null);

    public static Location NomadsRest = new Location("Nomad's rest", new System.Numerics.Vector2(-8, -1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    }, 
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.DireWolfPack, 50},
        { EncounterLibrary.WanderingMerchant, 5 },
        { EncounterLibrary.RoadGambling, 20 },
        { EncounterLibrary.DyingTraveler, 30 },
        { EncounterLibrary.SuspiciousChest, 15 },
        { EncounterLibrary.AncientLibrary, 5 },
        { EncounterLibrary.FoundCoins, 20 },
        { EncounterLibrary.ChaosSignpost, 25 },
        { EncounterLibrary.AriaPropagandaBoard, 25 }

    }, null);

    public static Location Wraithwood = new Location("Wraithwood", new System.Numerics.Vector2(-9, -1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    }, 

    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.DireWolfPack, 50},
        { EncounterLibrary.BanditAmbush, 20 },
        { EncounterLibrary.MysteriousShrine, 20 },
        { EncounterLibrary.SuspiciousChest, 25 },
        { EncounterLibrary.DyingTraveler, 20 },
        { EncounterLibrary.AriaPropagandaBoard, 20 },
        { EncounterLibrary.ChaosSignpost, 8 },
        { EncounterLibrary.AncientLibrary, 10 },

    }, null);
    #endregion
    #endregion

    #region"Frostborn Dominion"
    public static Location SnowfallRidge = new Location("Snowfall Ridge", new System.Numerics.Vector2(-5, 1), 5, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 30},
        { EncounterLibrary.SnowWraithAttack, 25},
        { EncounterLibrary.IceMageEncounter, 20},
        { EncounterLibrary.FrozenGolemHealer, 2},
        { EncounterLibrary.FoundTreasure, 20},
        { EncounterLibrary.MysteriousShrine, 10}
    }, "Frostborn Dominion");

    public static Location EternalIcefall = new Location("Eternal Icefall", new System.Numerics.Vector2(-6, 1), 15, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Market", SubLocationType.marketplace)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.SnowWraithAttack, 35},
        { EncounterLibrary.FrozenHorde, 30},
        { EncounterLibrary.FrostTrollAmbush, 25},
        { EncounterLibrary.FrozenGolemHealer, 5},
        { EncounterLibrary.Snowman, 15 },
        { EncounterLibrary.MysteriousShrine, 10}
    }, "Frostborn Dominion");

    public static Location FrostfangCrag = new Location("Frostfang Crag", new System.Numerics.Vector2(-6, 0), 15, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Mine",SubLocationType.mine)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.SnowWraithAttack, 40},
        { EncounterLibrary.FrostTrollAmbush, 30},
        { EncounterLibrary.IceWolfPack, 25},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.FrozenGolemHealer, 5},
        { EncounterLibrary.Snowman, 15 },
        { EncounterLibrary.FallenIntoTrap, 10}
    }, "Frostborn Dominion");

    public static Location Everwinter = new Location("Everwinter", new System.Numerics.Vector2(-5, 0), 10, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.frozenChestplate),
                (ItemLibrary.iceSkates)
            }
        },
        new SubLocation("Blacksmith", SubLocationType.blacksmith)
        {
        },
        new SubLocation("Casino", SubLocationType.casino)
        {
            casinoMaxBet = 50,
        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 30},
        { EncounterLibrary.IceMageEncounter, 25},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.FoundCoins, 20},
        { EncounterLibrary.WanderingMerchant, 10}
    }, "Frostborn Dominion");

    public static Location IceboundPort = new Location("Icebound Port", new System.Numerics.Vector2(-5, -1), 13, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("pond", SubLocationType.pond)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                (ItemLibrary.bigHealthPotion),
                (ItemLibrary.speedBoots)
            }
        },
        new SubLocation("Port", SubLocationType.port)
        {

        },
        new SubLocation("Market", SubLocationType.marketplace)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.IceWolfPack, 25},
        { EncounterLibrary.IceMageEncounter, 20},
        { EncounterLibrary.FoundCoins, 25},
        { EncounterLibrary.FoundTreasure, 25},
        { EncounterLibrary.FallingFish, 10},
        { EncounterLibrary.WanderingMerchant, 10}
    }, "Frostborn Dominion");
    #endregion

    #region"Rootbound Empire"
    public static Location SylvanVeil = new Location("Sylvan Veil", new System.Numerics.Vector2(2, 2), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Bank",SubLocationType.bank)
        {

        }
    },
        new Dictionary<Encounter, int>
        {
        { EncounterLibrary.FoundCoins, 30},
        { EncounterLibrary.ElvenScout, 20},
        { EncounterLibrary.ElvenPatrol, 20},
        { EncounterLibrary.LynxAmbush, 20},
        { EncounterLibrary.ElfWarband, 20},
        { EncounterLibrary.HunterAndBeast, 20}
        }, "Rootbound Empire");

    public static Location SerenityPass = new Location("Serenity Pass", new System.Numerics.Vector2(3, 2), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Store", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                ItemLibrary.MoonleafGreaves,
                ItemLibrary.MoonveilSigil,
                ItemLibrary.ElixirOfEvasion,
                ItemLibrary.LeafsongSolution,
                ItemLibrary.WillowTea
            }
        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 30},
        { EncounterLibrary.ElvenScout, 20},
        { EncounterLibrary.ElvenPatrol, 20},
        { EncounterLibrary.LynxAmbush, 20},
        { EncounterLibrary.ElfWarband, 20},
        { EncounterLibrary.HunterAndBeast, 20},
        { EncounterLibrary.AbandonedBackpack, 5}
    }, "Rootbound Empire");

    public static Location FaelandGlen = new Location("Faeland Glen", new System.Numerics.Vector2(4, 2), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Marketplace",SubLocationType.marketplace)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 30},
        { EncounterLibrary.ElvenScout, 20},
        { EncounterLibrary.ElvenPatrol, 20},
        { EncounterLibrary.LynxAmbush, 20},
        { EncounterLibrary.ElfWarband, 20},
        { EncounterLibrary.HunterAndBeast, 20}
    }, "Rootbound Empire");

    public static Location WillowWeave = new Location("WillowWeave", new System.Numerics.Vector2(4, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Shop", SubLocationType.shop)
        {
            shopItems = new List<Item>
            {
                ItemLibrary.WillowChargebow,
                ItemLibrary.WillowwovenCloak,
                ItemLibrary.ThornlaceCirclet,
                ItemLibrary.WindspiritCharm,
                ItemLibrary.WardenStone
            }
        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 30},
        { EncounterLibrary.ElvenScout, 20},
        { EncounterLibrary.ElvenPatrol, 20},
        { EncounterLibrary.LynxAmbush, 20},
        { EncounterLibrary.ElfWarband, 20},
        { EncounterLibrary.HunterAndBeast, 20}
    }, "Rootbound Empire");

    public static Location ElderwoodVigil = new Location("Elderwood Vigil", new System.Numerics.Vector2(4, 1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {
        },
        new SubLocation("Wilderness",SubLocationType.wilderness)
        {

        },
        new SubLocation("Blacksmith", SubLocationType.blacksmith)
        {

        }

    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.FoundCoins, 30},
        { EncounterLibrary.ElvenScout, 20},
        { EncounterLibrary.ElvenPatrol, 20},
        { EncounterLibrary.LynxAmbush, 20},
        { EncounterLibrary.ElfWarband, 20},
        { EncounterLibrary.HunterAndBeast, 20}
    }, "Rootbound Empire");


    #endregion

    #region "Aria"
    public static Location NorthAriaBridge = new Location("North Aria Bridge", new System.Numerics.Vector2(-8, -2), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    public static Location NorthAriaGate = new Location("North Aria Gate", new System.Numerics.Vector2(-8, -3), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    public static Location AriaMarket = new Location("Aria Market", new System.Numerics.Vector2(-9, -5), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        },
        new SubLocation("marketplace", SubLocationType.marketplace)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    public static Location AriaCore = new Location("Aria Core", new System.Numerics.Vector2(-8, -4), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    public static Location AriaPort = new Location("Aria Port", new System.Numerics.Vector2(-7, -4), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        },
        new SubLocation("Port", SubLocationType.port)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    public static Location AriaTower = new Location("Aria Tower", new System.Numerics.Vector2(-8, -5), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    public static Location SouthAriaGate = new Location("South Aria Gate", new System.Numerics.Vector2(-8, -6), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    public static Location VaultAria = new Location("Vault Aria", new System.Numerics.Vector2(-9, -4), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        },
        new SubLocation("Bank", SubLocationType.bank)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    public static Location WestAriaGate = new Location("West Aria Gate", new System.Numerics.Vector2(-10, -4), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        }
    },
    new Dictionary<Encounter, int>
    {

    }, "Aria");

    #endregion

    #region "Deep Dark"

    public static Location OssuaryCrypt = new Location("Ossuary Crypt", new System.Numerics.Vector2(-10, -1), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.AncientLibrary, 25 },
        { EncounterLibrary.ChaosSignpost, 8 },
    }, "Deep Dark");

    public static Location NecroticCatacombs = new Location("Necrotic Catacombs", new System.Numerics.Vector2(-10, 0), 0, new List<SubLocation>
    {
        new SubLocation("Graveyard", SubLocationType.graveyard)
        {

        },
        new SubLocation("Crypt",SubLocationType.crypt)
        {

        }
    },
    new Dictionary<Encounter, int>
    {
        { EncounterLibrary.AncientLibrary, 25 },
        { EncounterLibrary.ChaosSignpost, 8 },
    }, "Deep Dark");

    #endregion

    public static List<Location> locations = new List<Location>
    {
        Maplecross, Greenhollow, Ironpeak, Mistport, MossGate, Nightreach, SilverfallRuins,
        WhisperWood, SaltmarshShore, ShattershoreCliffs, WitheredRuins, FrozenWastes,
        TundraMarch, SnowfallRidge, EternalIcefall, FrostfangCrag, Everwinter, IceboundPort,
        SylvanVeil, SerenityPass, FaelandGlen, ElderwoodVigil, SvalbardWastes, WillowWeave,Fernshade,

        //new-
        ColdshadeForest, QuietClearing, TheWithering, NomadsRest, Wraithwood, NorthAriaBridge, NorthAriaGate,
        AriaMarket, AriaCore, AriaPort, AriaTower, SouthAriaGate, VaultAria, WestAriaGate, NecroticCatacombs,
        OssuaryCrypt
    };

    public static Dictionary<string, Location> locationMap = locations.ToDictionary(l => l.name);


    public static Location Get(string name)
    {
        locationMap.TryGetValue(name, out var loc);
        return loc;
    }

}