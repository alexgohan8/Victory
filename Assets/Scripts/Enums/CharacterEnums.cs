namespace Simulation.Enums.Character
{
    public enum Element 
    {
        Fire, 
        Forest, 
        Wind, 
        Mountain
    }

    public enum Position 
    {
        FW, 
        MF, 
        DF, 
        GK 
    }

    public enum Gender 
    { 
        Male, 
        Female 
    }

    public enum TeamSide
    {
        Home,
        Away
    }

    public enum ControlType 
    { 
        LocalHuman, 
        RemoteHuman, 
        AI
    }

    public enum Stat 
    { 
        Gp, 
        Tp, 
        Kick, 
        Control, 
        Technique, 
        Pressure, 
        Physical, 
        Agility,
        Intelligence
    }

    public enum CharacterSize 
    { 
        S, 
        M, 
        L, 
        XL
    }

    public enum PortraitSize 
    { 
        XS, 
        S, 
        SM, 
        M, 
        ML, 
        L, 
        XL    
    }

    public enum BubbleMessage
    {
        Win,
        Lose,
        Dribble,
        Block,
        Pass,
        Shoot,
        Nice,
        Direct
    }

    public enum FatigueState
    {
        Normal,
        Tired,
        Exhausted
    }

    public enum StatusEffect
    {
        None,
        Stunned,
        Tripping
    }

    public enum CharacterState 
    { 
        Idle,
        Move,
        Dash,
        Kick,
        Control,
        Dribble,
        Block
    }

    public enum AIDifficulty 
    { 
        Easy, 
        Normal, 
        Hard 
    }

    public enum AIState 
    { 
        Idle, 
        Kickoff, 
        KickoffPass, 
        ChaseBall, 
        Defend,
        Combo,        
        Dribble,
        Mark, 
        Support,
        Keeper, 
        Pass, 
        Shoot 
    }
}
