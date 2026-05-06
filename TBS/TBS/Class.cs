using Game.Class;

[System.Flags]
public enum ClassRole
{
    Physical = 1,
    Magic    = 2
}

public class Class
{
    public string name;
    public string description; 

    // Role flags: Physical, Magic, or both for hybrids
    public ClassRole roles;       

    public int THP = 0;
    public int TmaxHP = 0;
    public int Tspeed = 0;
    public int Tarmor = 0;
    public int Tdodge = 0;
    public int TdodgeNegation = 0;
    public int Tcritchance = 0;
    public int TcritDamage = 0;
    public int Tstun = 0;
    public int TstunNegation = 0;
    public int Tluck  = 0;

    public string? extraStat = null;
    public float? extraStatIncrease = null;

    public Class() { }//never forget😔
    public Class(string name)
    {
        this.name = name;
    }
    
    public void LevelupStats()//isnt in use anymore
    {
        Program.player.HP += this.THP;
        Program.player.maxHP += this.TmaxHP;
        Program.player.speed += this.Tspeed;
        Program.player.armor += this.Tarmor;
        Program.player.dodge += this.Tdodge;
        Program.player.dodgeNegation += this.TdodgeNegation;
        Program.player.critChance += this.Tcritchance;
        Program.player.critDamage += this.TcritDamage;
        Program.player.stun += this.Tstun;
        Program.player.stunNegation += this.TstunNegation;
        Program.player.luck += this.Tluck; 
    }
}