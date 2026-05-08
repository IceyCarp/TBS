public static class ClassLibrary
{

    #region magic

    public static readonly Class Necromancer = new Class("Necromancer")
    {
        description = "Can raise undead from the graveyard to join their party. \nyou can command one dead for each of 10 levels",
        roles = ClassRole.Magic,
        extraStat = "CommandDeadSlots",
        extraStatIncrease = 0.1f
    };

    public static readonly Class Zoologist = new Class("Zoologist")
    {
        description = "Can have an exponentially scaling number of pets.",
        roles = ClassRole.Magic
    };

    public static readonly Class Alchemist = new Class("Alchemist")
    {
        description = "Crafts and uses more effective potions and consumables.",
        roles = ClassRole.Magic
    };

    public static readonly Class Mage = new Class("Mage")
    {
        description = "A standard magic-user.",
        roles = ClassRole.Magic
    };
    #endregion

    #region phys

    public static readonly Class Brawler = new Class("Brawler")
    {
        description = "Cannot equip armor, but gains significantly more max HP per level.",
        roles = ClassRole.Physical,
        TmaxHP = 12, 
        Tarmor = 0,  
    };

    public static readonly Class Monk = new Class("Monk")
    {
        description = "Cannot equip weapons, but gains more dodge chance per level.",
        roles = ClassRole.Physical,
        Tdodge = 10, 
    };

    public static readonly Class Pathfinder = new Class("Pathfinder")
    {
        description = "A hardy explorer who gains slightly more HP per level.",
        roles = ClassRole.Physical,
        TmaxHP = 7, 
    };

    public static readonly Class Tank = new Class("Tank")
    {
        description = "Gains more armor & HP per level, but less speed.",
        roles = ClassRole.Physical,
        TmaxHP = 8,  
        Tarmor = 3,  
        Tspeed = 0, 
    };

    public static readonly Class Rogue = new Class("Rogue")
    {
        description = "Gains more speed per level, but less HP and armor.",
        roles = ClassRole.Physical,
        Tspeed = 1,
        TmaxHP = -3,
        Tarmor = -0,
    };

    public static readonly Class Gambler = new Class("Gambler")
    {
        description = "Gains more luck per level.",
        roles = ClassRole.Physical,
        Tluck = 3,
    };

    public static readonly Class Ranger = new Class("Ranger")
    {
        description = "Gains slightly more speed per level. Always starts combat.",
        roles = ClassRole.Physical,
        TmaxHP = 5
    };

    public static readonly Class Merchant = new Class("Merchant")
    {
        description = "Gets better deals when buying and selling items.",
        roles = ClassRole.Physical,
    };

    public static readonly Class Nomad = new Class("Nomad")
    {
        description = "Gains more luck per level. Cannot use carriages.",
        roles = ClassRole.Physical,
        Tluck = 2, 
    };

    public static readonly Class Knight = new Class("Knight")
    {
        description = "Gains more armor & health per level.",
        roles = ClassRole.Physical,
        TmaxHP = 10,  
        Tarmor = 2, 
    };

    public static readonly Class Apprentice = new Class("Apprentice")
    {
        description = "Gains more armor & health per level.",
        roles = ClassRole.Physical,
        TmaxHP = 8,
        Tarmor = 1,
    };

    public static readonly Class Bladeharper = new Class("Bladeharper")
    {
        description = "Ability to dual-wield weapons.",
        roles = ClassRole.Physical,
    };

    public static readonly Class Duelist = new Class("Duelist")
    {
        description = "Increased armor and speed when in 1v1 situations.",
        roles = ClassRole.Physical,
    };

    public static readonly Class Berserker = new Class("Berserker")
    {
        description = "Gets buffs when below a certain HP threshold.",
        roles = ClassRole.Physical,
    };

    public static readonly Class Executioner = new Class("Executioner")
    {
        description = "Gets an extra turn if their turn killed the enemy.",
        roles = ClassRole.Physical,
    };

    public static readonly Class Scribe = new Class("Scribe")
    {
        description = "Gets stronger when fighting enemies already defeated prior.",
        roles = ClassRole.Physical,
    };

    public static readonly Class BloodKnight = new Class("Blood Knight")
    {
        description = "Gains more armor & health. Drains HP from everyone each turn. Cannot have allies.",
        roles = ClassRole.Physical,
        TmaxHP = 7,  
        Tarmor = 2,  
    };
    #endregion

    #region hybrid

    public static readonly Class Templar = new Class("Templar")
    {
        description = "A hybrid of physical and magical, with more HP per level.",
        roles = ClassRole.Physical | ClassRole.Magic,
        TmaxHP = 7,     
    };
    #endregion

}
