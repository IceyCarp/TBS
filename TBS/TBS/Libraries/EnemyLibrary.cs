using Game.Class;
using System.Collections.Generic;

// Enemy constructor order:
// name, level, exp, HP, speed, armor, dodge, dodgeNegation, critChance, critDamage, stun, stunNegation, money
public static class EnemyLibrary
{
    #region Basic/Starter Enemies
    public static Enemy Thug = new Enemy("Thug", 1, 10, 30,  8, 2, 5, 5, 5, 100, 0, 0, 10)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.ThrowHands, 100 }
        }
    };
    public static Enemy LostChild = new Enemy("Lost Child", 1, 0, 3, 3, 0, 50, 5, 5, 400, 0, 0, 0)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slap
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slap, 100 }
        }
    };
    public static Enemy Goblin = new Enemy("Goblin", 1, 5, 5, 16, 0, 25, 0, 10, 100, 5, 0, 0)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 70 },
            { AttackLibrary.ThrowHands, 30 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.goblinEar, 1, 2, 0.5f),
            new MaterialDrop(ItemLibrary.rock, 0, 1, 0.2f)
        }
    };
    #endregion

    #region Coastal Alliance Enemies
    public static Enemy SeaBandit = new Enemy("Sea Bandit", 2, 15, 35, 9, 2, 6, 7, 6, 130, 5, 3, 15)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 55 },
            { AttackLibrary.ThrowHands, 45 }
        }
    };

    public static Enemy Smuggler = new Enemy("Smuggler",3, 25, 45, 10, 3, 8, 9, 8, 150, 8, 5, 22)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 65 },
            { AttackLibrary.ThrowHands, 35 }
        }
    };
    public static Enemy Healer = new Enemy("Healer", 3, 25, 25, 10, 3, 8, 9, 8, 150, 8, 5, 22)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.GroupHeal
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.GroupHeal, 100 },
        }
    };
    #endregion

    #region Greenwood Territories Enemies
    public static Enemy ForestSpider = new Enemy("Forest Spider", 4, 20, 35, 8, 2, 5, 10, 6, 110, 8, 4, 0)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.Bite
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 75 },
            { AttackLibrary.Bite, 25 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.spiderSilk, 1, 3, 0.3f)
        }
    };

    public static Enemy DireWolf = new Enemy("Dire Wolf", 3, 35, 50, 12, 3, 8, 10, 8, 140, 12, 6, 0)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.Bite
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 85 },
            { AttackLibrary.Bite, 15 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.wolfPelt, 1, 2, 0.3f)
        }
    };
    #endregion

    #region Fallen Kingdom Enemies
    public static Enemy VampireSpawn = new Enemy("Vampire Spawn", 3, 25, 40, 12, 0, 10, 10, 10, 160, 5, 5, 12)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.VampiricSlash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.VampiricSlash, 100 }
        }
    };

    public static Enemy GhostlyApparition = new Enemy("Ghostly Apparition", 6, 30, 25, 8, 0, 20, 10, 5, 0, 200, 0, 15)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.EtherealTouch
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.EtherealTouch, 100 }
        }
    };

    public static Enemy SkeletonWarrior = new Enemy("Skeleton Warrior", 4, 25, 50, 5, 1, 1, 0, 20, 50, 100, 300, 10)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 100 }
        },
        materialDrops = new List<MaterialDrop> 
        {
            new MaterialDrop(ItemLibrary.bone, 1, 2, 0.6f) 
        }
    };

    public static Enemy KingdomGuard = new Enemy("Kingdom Guard", 5, 30, 55, 10, 2, 12, 6, 8, 130, 8, 6, 18)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 65 },
            { AttackLibrary.ThrowHands, 35 }
        }
    };

    public static Enemy CorruptedKnight = new Enemy("Corrupted Knight", 6, 70, 90, 14, 1, 18, 8, 12, 140, 15, 15, 40)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.VampiricSlash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 50 },
            { AttackLibrary.VampiricSlash, 50 }
        }
    };

    public static Enemy RuinedGolem = new Enemy("Ruined Golem", 7, 120, 150, 20, 0, 25, 3, 15, 100, 20, 20, 60)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.ThrowHands
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.ThrowHands, 100 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.rock, 4, 8, 0.33f)
        }
    };
    #endregion

    #region Frostborn Dominion Enemies
    public static Enemy IceWolf = new Enemy("Ice Wolf", 6, 40, 60, 10, 3, 8, 8, 8, 120, 10, 5, 0)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash,
            AttackLibrary.Bite
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 60 },
            { AttackLibrary.Bite, 40 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.frostCore, 1, 1, 0.1f),
            new MaterialDrop(ItemLibrary.wolfPelt, 1, 2, 0.3f)
        }
    };

    public static Enemy FrostTroll = new Enemy("Frost Troll", 9, 70, 120, 15, 1, 15, 5, 12, 100, 15, 10, 35)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.ThrowHands,
            AttackLibrary.GlacialCoating
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.ThrowHands, 60 },
            { AttackLibrary.GlacialCoating, 40 }
        }
    };

    public static Enemy GlacierGolem = new Enemy("Glacier Golem", 12, 150, 250, 5, 5, 15, 50, 10, 200, 75, 50, 50)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.ThrowHands,
            AttackLibrary.GlacialCoating
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.ThrowHands, 50 },
            { AttackLibrary.GlacialCoating, 50 }
        }
    };

    public static Enemy IceMage = new Enemy("Ice Mage", 9, 60, 50, 12, 2, 10, 12, 10, 150, 20, 8, 30)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Hailstorm,
            AttackLibrary.GroupHeal
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Hailstorm, 40 },
            { AttackLibrary.GroupHeal, 60 }
        }
    };

    public static Enemy SnowWraith = new Enemy("Snow Wraith", 14, 200, 80, 14, 4, 12, 15, 15, 50, 20, 12, 50)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.EtherealTouch,
            AttackLibrary.VampiricSlash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.EtherealTouch, 55 },
            { AttackLibrary.VampiricSlash, 45 }
        }
    };
    #endregion

    #region Rootbound Empire
    public static Enemy ElfHunter = new Enemy("Elf Hunter", 5, 30, 35, 11, 4, 20, 70, 20, 140, 10, 5, 14)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.WillowShot 
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.WillowShot, 100 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.Shadowleaf, 1, 2, 0.4f)
        }
    };

    public static Enemy ShadowLynx = new Enemy("Shadow Lynx", 5, 40, 45, 14, 6, 6, 10, 30, 150, 0, 10, 10)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Bite,
            AttackLibrary.Slash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Bite, 60 },
            { AttackLibrary.Slash, 40 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.ShadowClaw, 1, 1, 0.25f)
        }
    };

    public static Enemy ElfMystic = new Enemy("Elf Mystic", 4, 30, 30, 17, 3, 5, 50, 15, 100, 20, 0, 18)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.GroupHeal,
            AttackLibrary.Slash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.GroupHeal, 70 },
            { AttackLibrary.EtherealTouch, 30 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.MoonstoneShard, 1, 1, 0.3f)
        }
    };

    #endregion

    #region wilderness
    public static Enemy SnowWolf = new Enemy("Snow Wolf",11,85,95,20,1,18,18,170,20,10,35,20)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Bite,
            AttackLibrary.PoisonBite
        },

        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Bite, 65 },
            { AttackLibrary.PoisonBite, 35 }
        },

        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.wolfPelt, 1, 4, 0.5f)
        }
    };
    public static Enemy ColdStalker = new Enemy("Cold Stalker", 8, 30, 55, 6, 0, 20, 130, 10, 300, 0, 0, 14)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.WillowShot
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.WillowShot, 100 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.Shadowleaf, 1, 2, 0.4f)
        }
    };
    public static Enemy FrozenElk = new Enemy("Frozen Elk", 13, 70, 200, 5, 0, 0, 0, 10, 100, 70, 0, 14)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Ram
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Ram, 100 }
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.ElkHorn, 1, 2, 0.4f)
        }
    };


    public static Enemy rotWalker = new Enemy("rotWalker", 6, 20, 20, 2, 0, 0, 0, 10, 100, 0, 0, 5)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slap, 
            AttackLibrary.Bite,
            AttackLibrary.PoisonBite
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slap, 30 },
            { AttackLibrary.Bite, 70 },
            { AttackLibrary.PoisonBite, 5 }
        }
    };


    public static Enemy CaravanGuard = new Enemy("Caravan Guard", 17, 100, 300, 1, 15, 0, 0, 0, 0, 0, 0, 14)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.Slash
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.Slash, 100 }
        }
    };

    public static Enemy CrossbowMercenary = new Enemy("Crossbow Mercenary", 12, 80, 80, 7, 3, 10, 0, 20, 100, 0, 0, 10)
    {
        attacks = new List<Attack>
        {
            AttackLibrary.CrossBowShot
        },
        attackWeights = new Dictionary<Attack, int>
        {
            { AttackLibrary.CrossBowShot, 100 }
        }
    };

    #endregion

    #region misc

    public static Enemy BridgeTroll = new Enemy("Bridge Troll", 16, 250, 200, 10, 4, 0, 50, 20, 100, 20, 50, 100)
    {

        attacks = new List<Attack>
        {
            AttackLibrary.Haymaker,
            AttackLibrary.BoneShatter,
            AttackLibrary.HeavySlam
        },
        attackWeights = new Dictionary<Attack, int>
        {
        },
        materialDrops = new List<MaterialDrop>
        {
            new MaterialDrop(ItemLibrary.rock, 1, 5, 0.4f)        }
    };
    #endregion

}