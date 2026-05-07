public static class ItemLibrary
{

    /*    

    stats:

    Heal;
    maxHP;
    damage;
    speed;
    armor;


    stats in percent:

    dodge;
    dodgeNegation;
    critChance;
    critDamage;
    stun;
    stunNegation;
*/


    #region material
    public static Item rock = new Item("Rock", "A common crafting material.", 3, 0, ItemType.Material)
    {
        detailsLore = "Its just a small pebble. Useful for basic crafting, or throwing at kings.",
    };
    public static Item iron = new Item("Iron", "Raw iron ore used for basic forging.", 3, 0, ItemType.Material)
    {
        detailsLore = "Some nice iron ore. Useful for crafting weapons and armor.",
    };
    public static Item fish = new Item("Fish", "A fresh fish, sometimes used in recipes.", 1, 0, ItemType.Material)
    {
        detailsLore = "It's a fish. You like fish.",
    };

    // Monster-specific materials
    public static Item bone = new Item("Bone", "A piece of bone", 5, 2, ItemType.Material)
    {
        detailsLore = "A piece of skeletal remains, sharp enough to fashion into tools.",
    };
    public static Item goblinEar = new Item("Goblin Ear", "A trophy from a defeated goblin.", 5, 0, ItemType.Material)
    {
        detailsLore = "Goblins have surprisingly large ears. Useful for certain crafts.",
    };

    public static Item spiderSilk = new Item("Spider Silk", "Strong silk from a forest spider.", 8, 0, ItemType.Material)
    {
        detailsLore = "Incredibly strong and flexible. Prized by armorers.",
    };

    public static Item wolfPelt = new Item("Wolf Pelt", "Thick fur from a dire wolf.", 12, 0, ItemType.Material)
    {
        detailsLore = "Warm and durable. Perfect for crafting protective gear.",
    };

    public static Item frostCore = new Item("Frost Core", "A frozen crystal from an ice wolf.", 20, 0, ItemType.Material)
    {
        detailsLore = "Radiates cold. Used in advanced frost-based crafting.",
    };

    public static Item Shadowleaf = new Item("Shadowleaf", "A dark leaf that blends into shadows.", 15, 0, ItemType.Material)
    {
        detailsLore = "",
    };

    public static Item ShadowClaw = new Item("Shadow Claw", "a razor sharp dark claw.", 18, 0, ItemType.Material)
    {
        detailsLore = "",
    };

    public static Item MoonstoneShard = new Item("Moonstone Shard", "A crystal humming with lunar energy.", 25, 0, ItemType.Material)
    {
        detailsLore = "",
    };

    #endregion

    #region craftable equipment (not sold in stores)

    #region head
    public static Item goblinSkullHelm = new Item("Goblin Skull Helm", "+2 armor, +5% dodge", 25, 5, ItemType.Equipment)
    {
        stats = { ["armor"] = 2, ["dodge"] = 5 },
        equipmentType = EquipmentType.Head,
        detailsLore = "A crude helmet fashioned from goblin trophies and reinforced cloth.\nLightweight but surprisingly effective."
    };

    public static Item frostforgedHelm = new Item("Frostforged Helm", "+4 armor, +10 maxHP", 80, 12, ItemType.Equipment)
    {
        stats = { ["armor"] = 4, ["maxHP"] = 10 },
        equipmentType = EquipmentType.Head,
        detailsLore = "A helmet infused with frost magic. The cold never bothered you anyway."
    };
    #endregion

    #region torso
    public static Item spidersilkVest = new Item("Spidersilk Vest", "+3 armor, +15% dodge", 60, 7, ItemType.Equipment)
    {
        stats = { ["armor"] = 3, ["dodge"] = 15 },
        equipmentType = EquipmentType.Torso,
        detailsLore = "Woven from spider silk and reinforced with wolf pelt.\nFlexible yet protective."
    };

    public static Item reinforcedIronPlate = new Item("Reinforced Iron Plate", "+6 armor, +15 maxHP, -1 speed", 70, 15, ItemType.Equipment)
    {
        stats = { ["armor"] = 6, ["maxHP"] = 15, ["speed"] = -1 },
        equipmentType = EquipmentType.Torso,
        detailsLore = "Heavy iron plating reinforced with quality materials.\nProvides excellent protection at the cost of mobility."
    };
    #endregion

    #region legs
    public static Item spidersilkLeggings = new Item("Spidersilk Leggings", "+1 armor, +25% dodge, +1 speed", 55, 4, ItemType.Equipment)
    {
        stats = { ["armor"] = 1, ["dodge"] = 25, ["speed"] = 1 },
        equipmentType = EquipmentType.Legs,
        detailsLore = "Lightweight leggings woven from spider silk.\nEnhances agility significantly."
    };
    #endregion

    #region feet
    public static Item wolfhideBoots = new Item("Wolfhide Boots", "+2 speed, +2 armor", 40, 3, ItemType.Equipment)
    {
        stats = { ["speed"] = 2, ["armor"] = 2 },
        equipmentType = EquipmentType.Feet,
        detailsLore = "Sturdy boots lined with wolf pelt.\nProvides both protection and mobility."
    };
    #endregion

    #region weapon
    public static Item IronWarhammer = new Item("Iron Warhammer", "Heavy Slam", 50, 18, ItemType.Equipment)
    {
        detailsLore = "A massive iron hammer, Hits with incredible force and \nshattering enemy defenses.",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.HeavySlam
    };

    public static Item FishSword = new Item("Fish Sword", "Heavy Slam", 15, 6, ItemType.Equipment)
    {
        detailsLore = "a sword coated in fish intestines",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.FishCut
    };

    public static Item GlacierReaver = new Item("Glacier Reaver", "Glacial Sweep", 150, 25, ItemType.Equipment)
    {
        detailsLore = "A massive sharp blade carved from a single big shard of ice",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.GlacialSweep
    };

    public static Item WinterfangGauntlet = new Item("Winterfang Gauntlet", "Frost Barrier", 500, 30, ItemType.Equipment)
    {
        detailsLore = "A reinforced gauntlet using it to strike an enemy instantly coats the wearer in a brittle layer of protective frost.",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.FrostBarrier
    };

    public static Item SpineBreaker = new Item("Spine Breaker", "Bone Shatter", 40, 12, ItemType.Equipment)
    {
        detailsLore = "A jagged, heavy bone club fashioned from a large spine, \nMakes the target extremely vulnerable to future stuns",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.BoneShatter
    };
    #endregion

    #endregion

    #region artifacts
    public static Item VampireRing = new Item("Vampire Ring", "+4% CritChance", 11, 1, ItemType.Artifact)
    {
        detailsLore = "Silver ring rumored to strengthen \nits wearer’s blood using magic.",
        stats = { ["critChance"] = 4 },
    };

    public static Item BasicBackpack = new Item("Basic Backpack", "A simple backpack", 50, 2, ItemType.Artifact)
    {
        stats = { ["materialCapacity"] = 50 },
        detailsLore = "A sturdy canvas backpack. Increases how many materials you can carry without weighing you down."
    };
    public static Item SilverfallAmulet = new Item("Silverfall Amulet", "+5 maxHP, +3% critChance", 12, 1, ItemType.Artifact)
    {
        stats = { ["maxHP"] = 5, ["critChance"] = 3 },
        detailsLore = "Old amulet with a tiny silver waterfall engraved\n hums faintly with magic"
    };
    public static Item MoonveilSigil = new Item("Moonveil Sigil",
    "+10% dodge", 120, 3, ItemType.Artifact)
    {
        stats = { ["dodge"] = 10 },
        detailsLore = "An etched sigil that shimmers with soft moonlight, \nshifting the bearer away from danger."
    };
    public static Item WindspiritCharm = new Item("Windspirit Charm",
    "+5% dodge, +10 dodgeNegation", 100, 3, ItemType.Artifact)
    {
        stats = { ["dodge"] = 5, ["dodgeNegation"] = 10 },
        detailsLore = "A charm woven of gentle wind spirits, \nguides the bearer while disrupting swift opponents"
    };
    public static Item WardenStone = new Item("Warden Stone",
    "+25 dodgeNegation, +1 armor", 300, 5, ItemType.Artifact)
    {
        stats = { ["dodgeNegation"] = 25, ["armor"] = 1 },
        detailsLore = "A carved stone humming with deep-forest strength, making enemy evasions falter."
    };
    #endregion

    #region equipment

    #region head
    public static Item baseballCap = new Item("Baseball Cap", "+1 armor", 5, 2, ItemType.Equipment)
    {
        stats = { ["armor"] = 1 },
        equipmentType = EquipmentType.Head,
        detailsLore = "cool Cap.. somehow makes it harder to punch your head off"
    };
    public static Item knightHelmet = new Item("Knight Helmet", "+3 armor, -1 speed", 7, 10, ItemType.Equipment)
    {
        stats = { ["armor"] = 3, ["speed"] = -1 },
        equipmentType = EquipmentType.Head,
        detailsLore = "heavy knight helmet.. makes it much harder to cut your head off, \nbut its a little too heavy to move properly in"
    };

    public static Item VampireMask = new Item("Vampire Mask", "+20% stun", 23, 4, ItemType.Equipment)
    {
        stats = { ["stun"] = 20 },
        equipmentType = EquipmentType.Head,
        detailsLore = "Elegant mask that makes all opponents freeze in fear"
    };

    public static Item ThornlaceCirclet = new Item("Thornlace Circlet", "+12 dodgeNegation, +2 armor", 40, 2, ItemType.Equipment)
    {
        stats = { ["dodgeNegation"] = 12, ["armor"] = 2 },
        equipmentType = EquipmentType.Head,
        detailsLore = "A delicate circlet that gives insight into enemy motions, reducing their chance to evade."
    };

    public static Item FallenGuardHelmet = new Item("Fallen Guard's Helmet", "+5 maxHP, +2 Armor...", 14, 9, ItemType.Equipment)
    {
        stats = { ["maxHP"] = 5, ["armor"] = 2, ["stunNegation"] = 1 },
        equipmentType = EquipmentType.Head,
        detailsLore = "Cracked iron helmet of a long dead town guard"
    };
    public static Item TatteredHat = new Item("Tattered Hat", "+1 armor, +1 dodge", 5, 3, ItemType.Equipment)
    {
        stats = { ["armor"] = 1, ["dodge"] = 1 },
        equipmentType = EquipmentType.Head,
        detailsLore = "A weather-worn hat from a long-forgotten villager"
    };
    #endregion

    #region torso
    public static Item constructionVest = new Item("Construction Vest", "+2 armor, -40% dodge", 6, 5, ItemType.Equipment)
    {
        stats = { ["armor"] = 2, ["dodge"] = -40 },
        equipmentType = EquipmentType.Torso,
        detailsLore = "a bright yellow construction vest... \ndefends you a bit, but its hard to dodge in"
    };
    public static Item CloakofDusk = new Item("Cloak of Dusk", "+1 speed, +40% dodge", 35, 6, ItemType.Equipment)
    {
        stats = { ["speed"] = 1, ["dodge"] = 40 },
        equipmentType = EquipmentType.Torso,
        detailsLore = "Black velvet cloak that blends perfectly with shadows"
    };
    public static Item frozenChestplate = new Item("Frozen chestplate", "+50 max hp, +5 armor", 350, 13, ItemType.Equipment)
    {
        stats = { ["maxHP"] = 50, ["armor"] = 5 },
        equipmentType = EquipmentType.Torso,
        detailsLore = "Black velvet cloak that blends perfectly with shadows"
    };
    public static Item ExpeditionVest = new Item("Expedition Vest", "+10 maxHP, +2 speed", 20, 13, ItemType.Equipment)
    {
        stats = { ["maxHP"] = 10, ["speed"] = 2 },
        equipmentType = EquipmentType.Torso,
        detailsLore = "A vest made of tough canvas. \nIt's light enough to enhance movement",
    };
    public static Item WillowwovenCloak = new Item("Willowwoven Cloak", "+25% dodge, +70 dodgeNegation", 120, 10, ItemType.Equipment)
    {
        stats = { ["dodge"] = 25, ["dodgeNegation"] = 70 },
        equipmentType = EquipmentType.Torso,
        detailsLore = "A cloak braided with thorn-silk; it hides steps and tangles evasive tricks."
    };
    #endregion

    #region legs
    public static Item camoPants = new Item("Camo Pants", "+20% dodge", 5, 2, ItemType.Equipment)
    {
        stats = { ["dodge"] = 20 },
        equipmentType = EquipmentType.Legs,
        detailsLore = "military grade camo pants..\n makes it harder to hit you"
    };

    public static Item RoughTrousers = new Item("Rough Trousers", "+1 armor", 4, 2, ItemType.Equipment)
    {
        stats = { ["armor"] = 1 },
        equipmentType = EquipmentType.Legs,
        detailsLore = "Simple, thick trousers reinforced with leather patches \nThey offer a tiny bit of defense",
    };
    #endregion

    #region boots
    public static Item sandals = new Item("Sandals", "+1 speed", 5, 1, ItemType.Equipment)
    {
        stats = { ["speed"] = 1 },
        equipmentType = EquipmentType.Feet,
        detailsLore = "some nice sandals.. decently fine to run in"
    };
    public static Item speedBoots = new Item("Speed Boots", "+3 speed", 8, 2, ItemType.Equipment)
    {
        stats = { ["speed"] = 3 },
        equipmentType = EquipmentType.Feet,
        detailsLore = "some beautiful boots of speed.. really nice to run in"
    };

    public static Item NightStalkerGreaves = new Item("Night Stalker Greaves", "+50% dodgeNegation", 17, 4, ItemType.Equipment)
    {
        stats = { ["dodgeNegation"] = 50 },
        equipmentType = EquipmentType.Feet,
        detailsLore = "Dark greaves that allow for perfect stalking of the pray\n no matter how agile it may be",

    };

    public static Item MoonleafGreaves = new Item("Moonleaf Greaves", "+20% dodge, +40% dodgeNegation", 80, 8, ItemType.Equipment)
    {
        stats = { ["dodge"] = 20, ["dodgeNegation"] = 40 },
        equipmentType = EquipmentType.Legs,
        detailsLore = "Light greaves that allow for very swift movement"
    };

    public static Item iceSkates = new Item("Ice Skates", "+5 speed, -2 armor ", 250, 4, ItemType.Equipment)
    {
        stats = { ["speed"] = 5, ["armor"] = -2 },
        equipmentType = EquipmentType.Feet,
        detailsLore = "Dark greaves that allow for perfect stalking of the pray\n no matter how agile it may be",

    };
    #endregion

    #region weapon

    public static Item sword = new Item("Sword", "Slash", 11, 5, ItemType.Equipment)
    {
        detailsLore = "Just a simple sword, can cut a bit",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.Slash
    };
    public static Item CrudePoisonSpear = new Item("Crude Poison Spear", "Venom Strike", 20, 4, ItemType.Equipment)
    {
        detailsLore = "A simple stone tipped spear coated in a foul smelling venom",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.VenomStrike
    };
    public static Item StoneMaul = new Item("Two-Handed Stone Maul", "Reckless Swing", 15, 30, ItemType.Equipment)
    {
        detailsLore = "BIG STONE ON A STICK:D",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.RecklessSwing
    };
    public static Item FoulHook = new Item("Foul Hook", "Foul Thrust", 15, 6, ItemType.Equipment)
    {
        weaponAttack = AttackLibrary.FoulThrust,
        equipmentType = EquipmentType.Weapon,
        detailsLore = "A rusty, crude hook.. It’s dirty and unbalanced, \nbut the sharp point can easily catch clothing or skin.",
    };
    public static Item WillowChargebow = new Item("WillowCharge bow", "Willowrend Shot", 120, 8, ItemType.Equipment)
    {
        detailsLore = "A bow of braided willow \nusing arrows that make opponents a lot less slippery",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.WillowShot
    };

    #endregion

    #endregion

    #region consumables
    public static Item smallHealthPotion = new Item("Small Health Potion", "+20 Health", 2, 2, ItemType.Consumable)
    {
        effects = new List<AttackEffect>()
        {
            new AttackEffect("heal", 20, 0, "self")
        },
        duration = 0,
        detailsLore = "simple health potion, drink it and you regain some health..\n not much but being alive is nice"
    };
    public static Item bigHealthPotion = new Item("Big Health Potion", "+50 Health", 4, 3, ItemType.Consumable)
    {
        effects = new List<AttackEffect>()
        {
            new AttackEffect("heal", 50, 0, "self")
        },
        duration = 0,
        detailsLore = "advanced health potion, drink it and you regain\n a major amount of health.. great for staying alive"
    };

    public static Item speedPotion = new Item("Speed Potion", "+10 Speed", 4, 3, ItemType.Consumable)
    {
        duration = 2,
        effects = new List<AttackEffect>()
        {
            new AttackEffect("speed", 10, 2, "self")
        },
        detailsLore = "speed potion, drink it to become faster for a bit"
    };

    public static Item ElixirOfEvasion = new Item("Elixir of Evasion", "+30% Dodge", 8, 1, ItemType.Consumable)
    {
        duration = 3,
        effects = new List<AttackEffect>()
        {
            new AttackEffect("dodge", 30, 3, "self")
        },
        detailsLore = "A vial of shimmering brew, \nDrink it to become a blur for a short while."
    };

    public static Item LeafsongSolution = new Item("Leafsong Solution", "+50% DodgeNegation", 11, 1, ItemType.Consumable)
    {
        duration = 2,
        effects = new List<AttackEffect>()
        {
            new AttackEffect("dodgeNegation", 50, 2, "self")
        },
        detailsLore = "Solution of crushed willow bark\n sharpens the mind against swift foes"
    };

    public static Item WillowTea = new Item("Willow Tea", "+20 HP, +10% Dodge", 4, 1, ItemType.Consumable)
    {
        duration = 1,
        effects = new List<AttackEffect>()
        {
            new AttackEffect("heal", 20, 0, "self"),
            new AttackEffect("dodge", 10, 1, "self")
        },
        detailsLore = "A warm brew that steadies the nerves and loosens the limbs"
    };

    public static Item CheapAle = new Item("Cheap Ale", "+15 HP, -15% Dodge", 1, 2, ItemType.Consumable)
    {
        effects = new List<AttackEffect>()
        {
            new AttackEffect("dodge", -15, 1, "self"),
            new AttackEffect("heal", 15, 0, "self")
        },
        duration = 0,
        detailsLore = "A harsh, watery brew common in dockside taverns. \nIt grants a small heal and a brief surge of reckless courage, but makes you stumble."
    };
    public static Item WildBerries = new Item("Wild Berries", "+10 HP, +10% CritDamage", 5, 1, ItemType.Consumable)
    {
        effects = new List<AttackEffect>()
        {
            new AttackEffect("heal", 10, 0, "self"),
            new AttackEffect("critDamage", 10, 1, "self")
        },
        duration = 0,
        detailsLore = "A handful of wild berries found deep in the woods, \nThey offer a burst of energy and sharpen the senses.",
    };


    #endregion

    #region special/exclusive

    public static Item Excalibur = new Item("Excalibur", "King’s Cut", 0, 5, ItemType.Equipment)
    {
        detailsLore = "The legendary sword of myth Excalibur \nknown to give anyone who claims it the rank of king \ntruly a weapon for the greats",
        equipmentType = EquipmentType.Weapon,
        weaponAttack = AttackLibrary.KingsCut
    };

    #endregion





}