namespace Zycalipse
{
    public enum GameState
    {
        Default,    //Should Not Happen
        MainMenu,
        InGameMenu,
        Settings,
        InGameSettings,
        Loading,
        PreparingCombat,
        PlayerTurn,
        PlayerAttacking,
        EnemyTurn,
        EnemyAction,
        PowerUpSection,
        PlayerDied,
        GameOver
    }

    public enum LevelName
    {
        LevelOne,
        LevelTwo
    }

    public enum ItemStatus
    {
        Owned,
        Closed,
        Locked,
        Unlocked,
        Pending,
        ComingSoon
    }

    public enum MenuList
    {
        MainMenu,
        BattleMenu,
        IGM,
        DeathMenu,
        AdsMenu,
        PlayerLevelUpMenu
    }

    public enum MainMenuSubMenus
    {
        LevelMenu,
        ShopMenu,
        InventoryMenu,
        CharacterMenu
    }

    public enum CurrencyType
    {
        Gold,
        Gem,
        Money
    }

    public enum CharacterStatus
    {
        Normal,
        Poisoned,
        Paralized,
        Confused,
        Bleed,
        Stunned,
        Freezed
    }

    public enum PlayerCharacterName
    {
        MaleChar1,
        FemaleChar1
    }

    public enum WeaponName
    {
        Pistol,
        Shotgun,
        Sniper,
        GauntletPistol,
        EnergyGun,
        EnergySniper,
        BarrelBomb,
        C4,
        Launcher
    }

    public enum WeaponType
    {
        CloseRange,
        MidRange,
        LongRange
    }

    public enum EquipedWeaponRangeType
    {
        EquipedCloseRangeWeapon,
        EquipedMidRangeWeapon,
        EquipedLongRangeWeapon
    }

    public enum WeaponAttackType
    {
        Instant,
        Delay
    }

    public enum WeaponKnockbackDirection
    {
        Right,
        Left,
        Forward,
        Backward
    }

    public enum WeaponSkills
    {
        Burn,
        Freeze,
        Lightning,
        Poison,
        Blind,
        Bleed,
        Stun,
        MultiShot
    }
    
    public enum MissionNames
    {
        Kill10Enemy,
        Kill20Enemy,
        Kill30Enemy,
        Kill1000Enemy,
        Kill3MiniBoss,
        Kill4Boss,
        Died50Time,
        Kill3IceZombie,
        Kill6IceZombie,
        Kill10IceZombie,
        Kill10MinerZombie,
        Kill20MinersZombie
    }

    public enum Effect
    {
        Firework,
        Burn,
        Freeze,
        Poison,
        Bleed,
        Blind,
        Lightning,
        Stun,
        EnemyHit,
        PlayerHit,
        PlayerRevive,
        PlayerWalk,
        Explosion,
        Heal,
        LevelUp
    }

    public enum LevelUpItems
    {
        SmallHeal,
        MedHeal,
        GreatHeal,
        ExtraLive,
        IncreaseMaxLevel,
        SkillLightning,
        SkillFreeze,
        SkillBurn,
        SkillPoison,
        SkillBleed,
        SkillStun,
        SkillBlind,
        SkillShadowClone,
        Rage,
        FullPower,
        GreedLife,
        Meteor
    }

    public enum LevelUpItemSlotNumber
    {
        Left,
        Middle,
        Right
    }

    public enum EnemyType
    {
        Normal,
        MiniBoss,
        Boss
    }

    public enum EnemySkills
    {
        Stun,
        Freeze,
        Poison,
        Blind,
        Heal
    }

    public enum EnemyMoveDirections
    {
        Forward,
        Backward,
        Left,
        Right,
        Stay
    }

    public enum EnemyAttackRange
    {
        CloseRange,
        MidRange,
        LongRange
    }

    public enum EnemyDropType
    {
        Exp,
        SmallHealthPotion,
        MediumHealthPotion,
        BigHealthPotion
    }
    public enum EnemyLayer
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six
    }

    public enum DamageType
    {
        Normal,
        Burn,
        Lightning,
        Poison,
        Freeze,
        Miss
    }
}