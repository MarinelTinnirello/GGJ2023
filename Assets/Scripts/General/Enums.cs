public enum MainScenes
{
    UserInit,
    MainMenu,
    InGame
}
public enum EnviromentAreas
{
    Floor,
    Stage
}

public enum CharacterPowerTypes
{
    A,
    B,
    C,
    D
}

public enum GameOverState
{
    Lose,
    BadScore,
    AwayTimeOut,
    Win
}

public enum FinalScoreState
{
    Fail,
    OneStar,
    TwoStars,
    ThreeStars
}

public enum CharacterList
{
    MainPlayer,
    NPC,
    EnemyType1,
    EnemyType2,
    EnemyType3
}

public enum CharacterType
{
    None,
    MainPlayer,
    NPC,
    Enemy
}
public enum MultiplayerCharacterSettings
{
    Offline,
    OnlineMainPlayer,
    OnlineCoOp,
    OnlineEnemy
}

public enum CharacterState
{
    IsIdle,
    IsAttacking,
    knockedOut
}

public enum CharacterAnimationStates
{
    isIdle,
    isWalking,
    isJumping,
    isDJing,
    isPartying,
    isBeingAttacked
}

public enum FacingDirection
{
    Right,
    Left,
    Random
}

public enum CharacterHairTypes
{
    Bald,
    HairA,
    HairB,
    HairC,
    HairD,
    HairE
}

public enum CharacterSkinOptions
{
    SkinA,
    SkinB
}

public enum CharacterFaceOptions
{
    BlackEyebrowsNoBeard,
    BlackEyebrowsBrownBeard,
    OrangeEyebrowsCurledMustache,
    SmorezzHead
}

public enum CharacterFacialExpressions
{
    Idle,
    Dazed,
    Happy
}

public enum CharacterShirtTypes
{
    None,
    ShirtA,
    ShirtB,
    ShirtC,
    ShirtD
}

public enum CharacterShirtLogoTypes
{
    None,
    SecurityLabel,
    Stripe,
    ButtonCollar,
    Peace,
    Headphones,
    VestStuds,
    Dumbell,
    SmileyFace,
    Record,
    NoSmoking,
    PlusMinus
}

public enum CharacterPantTypes
{
    None,
    PantA,
    PantB,
    PantC,
    PantD
}

public enum CharacterShoeTypes
{
    None,
    ShoeA,
    ShoeB,
    ShoeC
}

public enum CharacterBodyParts
{
    Face,
    Head,
    Hair,
    HairRear,
    Torso,
    Neck,
    LeftUpperArm,
    LeftForearm,
    LeftHand,
    RightUpperArm,
    RightForearm,
    RightHand,
    LeftThigh,
    LeftLowerLeg,
    LeftFoot,
    RightThigh,
    RightLowerLeg,
    RightFoot,
    Pelvis,
    FaceBlink,
    FaceDazed,
    FaceHappy,
    Shirt,
    ShirtLogo,
    LeftUpperShirtSleve,
    RightUpperShirtSleve,
    PantTop,
    LeftPantThigh,
    RightPantThigh
}

public enum ActiveAxis
{
    XY,
    X,
    Y,
    Z
}

public enum AttackType
{
    Low,
    Medium,
    High
}

public enum HypeMeterLevel
{
    Low,
    Medium,
    High
}

public enum ItemTypes
{
    Health,
    SpeedUp,
    PowerUp
}

public enum MovementType
{
    is2D,
    is3D
}

public enum RenderMode
{
    WorldSpace,
    Billboard,
    ScreenSpaceOverlay
}