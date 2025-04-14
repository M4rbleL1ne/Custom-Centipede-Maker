global using static CustomCentisMod.CustomCentiBreedParams;
using Fisobs.Creatures;
using static PathCost.Legality;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using RWCustom;
using System.IO;
using System.Runtime.InteropServices;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace CustomCentisMod;

[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public class CustomCentiBreedParams(CreatureTemplate.Type type, CreatureTemplate.Type? parent = null) : BreedParameters, ICloneable
{
    public enum ChunkRadType
    {
        Normal,
        Small,
        Centiwing
    }

    public enum HeadMoveType
    {
        Normal,
        Small
    }

    public enum SizeGenerationType
    {
        RandomRangePow,
        RandomRange,
        StaticMin,
        StaticMax,
        FromWorldString
    }

    public enum ShockDamageType
    {
        Normal,
        Small,
        AquaCenti
    }

    public enum ViolenceResistanceType
    {
        Normal,
        Red,
        AquaCenti,
        Centiwing
    }

    [Flags]
    public enum IdleScoreTypes
    {
        Normal = 0b_0000,
        Centiwing = 0b_0001,
        AquaCenti = 0b_0010,
        AquaCentiAndCentiwing = Centiwing | AquaCenti
    }

    public enum RelationshipChangeType
    {
        Normal,
        Centiwing,
        Red
    }

    [Flags]
    public enum VisualScoreChangeTypes
    {
        Normal = 0b_0000,
        Centiwing = 0b_0001,
        Red = 0b_0010,
        RedAndCentiwing = Centiwing | Red
    }

    public enum PreyTrackerType
    {
        Normal,
        Red
    }

    public enum ExcitementTrackerType
    {
        Normal,
        Centiwing,
        Red
    }

    public enum WingType
    {
        StaticLengthFactor,
        Centiwing,
        AquaCenti
    }

    public enum PaletteColorType
    {
        Custom,
        BlackColor,
        WaterColor1,
        WaterColor2,
        WaterSurfaceColor1,
        WaterSurfaceColor2,
        WaterShineColor,
        FogColor,
        ShortCutSymbol,
        SkyColor,
        ShortcutColor1,
        ShortcutColor2,
        ShortcutColor3
    }

    public enum FlashColorNoiseType
    {
        None,
        Red,
        Green,
        Blue
    }

    public enum SegmentType
    {
        Normal,
        Centiwing,
        AquaCenti
    }

    public static CustomRelation NullRelation = default;
    [AllowNull] internal Dictionary<RelationKey, RelationValue> _realRelationships;
    public CustomRelation[]? Relationships;
    public string? SegmentSprite, AdditionalShellTextureShader, LegASprite, LegBSprite, WingSprite, WingShader, ShellSpikeSprite1, ShellSpikeSprite2,
    BellyShellSprite, BackShellSprite, StandardIconSprite, BigIconSprite, SmallIconSprite, DevName, Path;
    internal string? _tempPath;
    public CreatureTemplate.Type CreatureType = type;
    public CreatureTemplate.Type? ParentType = parent;
    public MultiplayerUnlocks.SandboxUnlockID? UnlockID, SmallUnlockID, BigUnlockID, ParentUnlock, BigParentUnlock, SmallParentUnlock;
    [AllowNull] internal CreatureTemplate _template;
    [AllowNull] internal CustomCentiCritob _critob;
    internal RelationValue _defaultRel;
    public FlagArray32 Flags, Flags2;
    public const Bits32 Swimming = B0,
        AutomaticPickUp = B1,
        SmallFood = B2,
        ShellParticles = B3,
        Flying = B4,
        DetectsAnnoyingCollisions = B5,
        WeakToStun = B6,
        GlowingHead = B7,
        ShocksWhenGrabbed = B8,
        Shields = B9,
        NoiseTracker = B10,
        AdditionalShellTexture = B11,
        Wings = B12,
        ShellSpikes = B13,
        BodySizeDependantWhisker = B14,
        SmallTube = B15,
        SlowLegs = B16,
        KillScoreHidden = B17,
        CountsAsAKill = B18,
        WaterOnly = B19,
        IgnoresCommunity = B20,
        TriesToStayInRoom = B21,
        EasyKill = B22,
        UsesUnlockData = B23,
        EdibleByOmnivores = B24,
        WormGrassImmune = B25,
        ForbidStandardShortcutEntry = B26,
        BlizzardWanderer = B27,
        MajorCreature = B28,
        NoViolenceStun = B29,
        TooBigForShelter = B30,
        WantsToShock = B31,
        Throwable = B0,
        Throwable_HasValue = B1,
        NightOnly = B2,
        NightOnly_HasValue = B3,
        IgnoresCycle = B4,
        IgnoresCycle_HasValue = B5,
        PreCycle = B6,
        PreCycle_HasValue = B7,
        LavaImmune = B8,
        LavaImmune_HasValue = B9,
        TentacleImmune = B10,
        TentacleImmune_HasValue = B11,
        HypothermiaImmune = B12,
        HypothermiaImmune_HasValue = B13,
        Grabability_HasValue = B14,
        ShellColorType_HasValue = B15,
        SecondaryShellColorType_HasValue = B16,
        SporeCloudImmune = B17,
        DaddyCorruptionImmune = B18,
        DoesNotUseDens = B19,
        CannotBeHitByWeapons = B20,
        SandstormImmune = B21,
        CannotBeBlinded = B22,
        CannotBeDeafened = B23;
    public int FoodPoints, Bites, MinChunkAmount, MaxChunkAmount, WingVariations, StandardKillScore, AbstractedLaziness, ShortCutSegments, BigKillScore,
        SmallKillScore, PatherStepsPerFrame, ExpeditionScore;
    public float BodyChunkRadBonus, BodyChunkMassBonus, ConnectionElasticityReduction, VelocityFactor, HeadVelocityFactor, MaxSize, MinSize, PreyTrackerWeight,
        WingLengthFactor, LegLengthFactor, WhiskerLengthFactor, SmallWhiskerLength, BigWhiskerLength, LegScaleXFactor, ShellScaleYFactor, WhiskerShapeFactor,
        HueMax, HueMin, SaturationMax, SaturationMin, LinearSandboxCost, ExponentialSandboxCost, RoomPerformanceCost, BaseDamageResistance, ExplosionResistance,
        BodySizeEstimate, VisualRadius, WaterVision, ThroughSurfaceVision, MovementBasedVision, DangerousToPlayer, LungCapacity, CommunityInfluence, MeatMin, MeatMax,
        Scaryness, MinBuoyancy, MaxBuoyancy, SureToGetPreyDistance, HeadGlobalVelocityFactor, BaseStunResistance, ExplosionStunResistance, SpikeScaleFactor,
        ShockResistanceReductionFactor, SecondaryShellColorBonus, GlobalVelocityFactor, GlobalFlyingVelocityFactor, DamageReductionFactor, DeadShellChance,
        GrabbedShockChargeReduction, WaterPathingResistance = 1f, BluntDamageResistance, BluntStunResistance, WaterDamageResistance, WaterStunResistance,
        StabDamageResistance, StabStunResistance, BiteDamageResistance, BiteStunResistance, ElectricDamageResistance = 102f, ElectricStunResistance = 102f, OffScreenSpeed = .3f,
        SurfaceFriction = .4f, Bounce = .1f, WaterRetardationImmunity, WaterFriction = .96f, AirFriction = .999f, ImpactThreshold = 1f;// default values for backwards compat
    public ChunkRadType BodyChunkRadType;
    public HeadMoveType HeadVelocityType;
    public SizeGenerationType BodySizeGenerationType;
    public ShockDamageType ShockType;
    public ViolenceResistanceType ResistanceType;
    public IdleScoreTypes AllowedIdleTypes;
    public RelationshipChangeType DynamicRelationshipType;
    public VisualScoreChangeTypes VisualScoreTypes;
    public PreyTrackerType PreyTrackType;
    public ExcitementTrackerType ExcitementType;
    public WingType WingSizeType;
    public PaletteColorType BodyBlackColorType, HeadGlowMinColorType, HeadGlowMaxColorType, LightFlashColorType, WingColorType, MainColorType, ShockColorType,
        ShellColorType, SecondaryShellColorType;
    public FlashColorNoiseType LightFlashNoiseType;
    public SegmentType BodySegmentType;
    public Player.ObjectGrabability Grabability;
    public Color ShockColor, ShortCutColor, HeadGlowMinColor, HeadGlowMaxColor, BodyBlackColor, LightFlashColor, WingColor,
        StandardIconColor, BigIconColor, SmallIconColor, DevColor, ShellColor, SecondaryShellColor;
    internal bool _requiresReloading, _nonMSCAqua, _propsChanged, _relationsChanged, _alreadySetRels;
    //Sparks, ScutigeraDynamicRelationship, ScutigeraHearingSkill, ShieldChance
    internal const int K_Swimming = 2118152008,
        K_AutomaticPickUp = 1285153176,
        K_SmallFood = 1204495808,
        K_ShellParticles = 771709952,
        K_Flying = -736401634,
        K_DetectsAnnoyingCollisions = 944786831,
        K_WeakToStun = -1431764016,
        K_GlowingHead = -1839645858,
        K_ShocksWhenGrabbed = -1905036421,
        K_Shields = -142945639,
        K_NoiseTracker = -2030877217,
        K_AdditionalShellTexture = -1046099439,
        K_Wings = -1095379349,
        K_ShellSpikes = 274335574,
        K_BodySizeDependantWhisker = -2146850650,
        K_SmallTube = -401426802,
        K_SlowLegs = -757330043,
        K_KillScoreHidden = -153185995,
        K_CountsAsAKill = -555963692,
        K_WaterOnly = 25593602,
        K_IgnoresCommunity = -1007105387,
        K_TriesToStayInRoom = 1904842098,
        K_EasyKill = -445026469,
        K_UsesUnlockData = -558365707,
        K_EdibleByOmnivores = -677277881,
        K_WormGrassImmune = 191983381,
        K_ForbidStandardShortcutEntry = -856522600,
        K_Throwable = 217195003,
        K_NightOnly = 886587451,
        K_IgnoresCycle = -1778104146,
        K_PreCycle = -149042072,
        K_LavaImmune = -1408095850,
        K_TentacleImmune = 1374120126,
        K_HypothermiaImmune = 1513703348,
        K_FoodPoints = 1156053698,
        K_Bites = 1802176148,
        K_MinChunkAmount = 1696864944,
        K_MaxChunkAmount = 127538642,
        K_WingVariations = -1670695992,
        K_StandardKillScore = 2122818746,
        K_AbstractedLaziness = -1177082621,
        K_ShortCutSegments = 1390855645,
        K_BigKillScore = -843152643,
        K_SmallKillScore = 1136034022,
        K_BodyChunkRadBonus = -496700652,
        K_BodyChunkMassBonus = -654234507,
        K_ConnectionElasticityReduction = -1234018007,
        K_VelocityFactor = -1204336043,
        K_HeadVelocityFactor = 2053822495,
        K_MaxSize = -2026988664,
        K_MinSize = -245344274,
        K_PreyTrackerWeight = -15790001,
        K_WingLengthFactor = -885030119,
        K_LegLengthFactor = 677669528,
        K_WhiskerLengthFactor = -57191925,
        K_SmallWhiskerLength = 2015469587,
        K_BigWhiskerLength = -1157156724,
        K_LegScaleXFactor = -615113170,
        K_ShellScaleYFactor = -1547427409,
        K_WhiskerShapeFactor = 1869654546,
        K_HueMax = 12466413,
        K_HueMin = -358024301,
        K_SaturationMax = 362838301,
        K_SaturationMin = 529334563,
        K_LinearSandboxCost = 1904586710,
        K_ExponentialSandboxCost = 845414062,
        K_RoomPerformanceCost = -1585550111,
        K_BaseDamageResistance = 317889060,
        K_ExplosionResistance = -587998459,
        K_BodySizeEstimate = 1539280314,
        K_VisualRadius = -321610713,
        K_WaterVision = 1908573784,
        K_ThroughSurfaceVision = 906931173,
        K_MovementBasedVision = -113143649,
        K_DangerousToPlayer = 1683518695,
        K_LungCapacity = -917082659,
        K_CommunityInfluence = -1472271263,
        K_MeatMin = -264773018,
        K_MeatMax = 38504052,
        K_Scaryness = -1364177616,
        K_MinBuoyancy = -1543313995,
        K_MaxBuoyancy = -84254273,
        K_BodyChunkRadType = 1887308189,
        K_HeadVelocityType = 1008451708,
        K_BodySizeGenerationType = -919062838,
        K_ShockType = 1948621529,
        K_ResistanceType = -751250072,
        K_AllowedIdleTypes = 1391274598,
        K_DynamicRelationshipType = 272117236,
        K_VisualScoreTypes = -521400626,
        K_PreyTrackType = -1168778420,
        K_ExcitementType = 313756511,
        K_WingSizeType = 470837211,
        K_MainColorType = 807023175,
        K_LightFlashNoiseType = -434475789,
        K_BodySegmentType = -141605786,
        K_Grabability = 1767849091,
        K_ShockColor = 339545108,
        K_ShortCutColor = 785116210,
        K_HeadGlowMinColor = -905624067,
        K_HeadGlowMaxColor = 1346531319,
        K_BodyBlackColor = 285426427,
        K_LightFlashColor = 296493038,
        K_WingColor = -1796395027,
        K_StandardIconColor = 173278524,
        K_BigIconColor = -171298533,
        K_SmallIconColor = 938977424,
        K_DevColor = 45592067,
        K_ShellColor = 1046977764,
        K_SecondaryShellColor = -246596336,
        K_SegmentSprite = 975665111,
        K_AdditionalShellTextureShader = 1032677444,
        K_LegASprite = -259867181,
        K_LegBSprite = -839401242,
        K_WingSprite = 1360448635,
        K_WingShader = 368108979,
        K_ShellSpikeSprite1 = 1744407307,
        K_ShellSpikeSprite2 = 1761184926,
        K_BellyShellSprite = -335230386,
        K_BackShellSprite = 2112247021,
        K_StandardIconSprite = 1876985984,
        K_BigIconSprite = 1605654317,
        K_SmallIconSprite = -1524166044,
        K_DevName = -1529767817,
        K_Inherit = -513730944,
        K_UnlockID = -144832880,
        K_ParentUnlock = -133434355,
        K_Relationships = 1484277496,
        K_SureToGetPreyDistance = -541990784,
        K_PatherStepsPerFrame = 1631906916,
        K_HeadGlobalVelocityFactor = -2009323420,
        K_BaseStunResistance = -1092190059,
        K_ExplosionStunResistance = -1848258291,
        K_BlizzardWanderer = -1292739883,
        K_SmallParentUnlock = 996705344,
        K_BigParentUnlock = -1849395927,
        K_SmallUnlockID = 588305077,
        K_BigUnlockID = 554043100,
        K_SpikeScaleFactor = -1148843590,
        K_ShockResistanceReductionFactor = -1245087082,
        K_ExpeditionScore = 1503120336,
        K_SecondaryShellColorBonus = -376422457,
        K_GlobalVelocityFactor = -681372402,
        K_GlobalFlyingVelocityFactor = 203029097,
        K_DamageReductionFactor = -1532530614,
        K_DeadShellChance = -1656030645,
        K_GrabbedShockChargeReduction = -41496031,
        K_MajorCreature = 685949595,
        K_NoViolenceStun = 1945472463,
        K_TooBigForShelter = -1724817827,
        K_WantsToShock = -1606634007,
        K_Name = 266367750,
        K_Path = -345578410,
        K_SporeCloudImmune = 2031708194,
        K_WaterPathingResistance = -81949140,
        K_BluntDamageResistance = -1786216348,
        K_BluntStunResistance = -251157035,
        K_WaterDamageResistance = 491430956,
        K_WaterStunResistance = 427852573,
        K_StabDamageResistance = 130930369,
        K_StabStunResistance = -1332118976,
        K_BiteDamageResistance = 3242063,
        K_BiteStunResistance = 308168434,
        K_ElectricDamageResistance = 1209884906,
        K_ElectricStunResistance = -1154932625,
        K_OffScreenSpeed = 1835580041,
        K_SurfaceFriction = 1231787150,
        K_Bounce = 590941277,
        K_WaterRetardationImmunity = -910160173,
        K_WaterFriction = -641427070,
        K_AirFriction = 1470716041,
        K_ImpactThreshold = 146532386,
        K_DaddyCorruptionImmune = 129972033,
        K_DoesNotUseDens = -1993820324,
        K_CannotBeHitByWeapons = 1195975270,
        K_SandstormImmune = -625428655,
        K_CannotBeBlinded = -2004324421,
        K_CannotBeDeafened = -311106403;
    internal static Dictionary<string, int> s_fieldHashDict = new()
    {
        { nameof(Swimming), K_Swimming },
        { nameof(AutomaticPickUp), K_AutomaticPickUp },
        { nameof(SmallFood), K_SmallFood },
        { nameof(ShellParticles), K_ShellParticles },
        { nameof(Flying), K_Flying },
        { nameof(DetectsAnnoyingCollisions), K_DetectsAnnoyingCollisions },
        { nameof(WeakToStun), K_WeakToStun },
        { nameof(GlowingHead), K_GlowingHead },
        { nameof(ShocksWhenGrabbed), K_ShocksWhenGrabbed },
        { nameof(Shields), K_Shields },
        { nameof(NoiseTracker), K_NoiseTracker },
        { nameof(AdditionalShellTexture), K_AdditionalShellTexture },
        { nameof(Wings), K_Wings },
        { nameof(ShellSpikes), K_ShellSpikes },
        { nameof(BodySizeDependantWhisker), K_BodySizeDependantWhisker },
        { nameof(SmallTube), K_SmallTube },
        { nameof(SlowLegs), K_SlowLegs },
        { nameof(KillScoreHidden), K_KillScoreHidden },
        { nameof(CountsAsAKill), K_CountsAsAKill },
        { nameof(WaterOnly), K_WaterOnly },
        { nameof(IgnoresCommunity), K_IgnoresCommunity },
        { nameof(TriesToStayInRoom), K_TriesToStayInRoom },
        { nameof(EasyKill), K_EasyKill },
        { nameof(UsesUnlockData), K_UsesUnlockData },
        { nameof(EdibleByOmnivores), K_EdibleByOmnivores },
        { nameof(WormGrassImmune), K_WormGrassImmune },
        { nameof(ForbidStandardShortcutEntry), K_ForbidStandardShortcutEntry },
        { nameof(Throwable), K_Throwable },
        { nameof(NightOnly), K_NightOnly },
        { nameof(IgnoresCycle), K_IgnoresCycle },
        { nameof(PreCycle), K_PreCycle },
        { nameof(LavaImmune), K_LavaImmune },
        { nameof(TentacleImmune), K_TentacleImmune },
        { nameof(HypothermiaImmune), K_HypothermiaImmune },
        { nameof(FoodPoints), K_FoodPoints },
        { nameof(Bites), K_Bites },
        { nameof(MinChunkAmount), K_MinChunkAmount },
        { nameof(MaxChunkAmount), K_MaxChunkAmount },
        { nameof(WingVariations), K_WingVariations },
        { nameof(StandardKillScore), K_StandardKillScore },
        { nameof(AbstractedLaziness), K_AbstractedLaziness },
        { nameof(ShortCutSegments), K_ShortCutSegments },
        { nameof(BigKillScore), K_BigKillScore },
        { nameof(SmallKillScore), K_SmallKillScore },
        { nameof(BodyChunkRadBonus), K_BodyChunkRadBonus },
        { nameof(BodyChunkMassBonus), K_BodyChunkMassBonus },
        { nameof(ConnectionElasticityReduction), K_ConnectionElasticityReduction },
        { nameof(VelocityFactor), K_VelocityFactor },
        { nameof(HeadVelocityFactor), K_HeadVelocityFactor },
        { nameof(MaxSize), K_MaxSize },
        { nameof(MinSize), K_MinSize },
        { nameof(PreyTrackerWeight), K_PreyTrackerWeight },
        { nameof(WingLengthFactor), K_WingLengthFactor },
        { nameof(LegLengthFactor), K_LegLengthFactor },
        { nameof(WhiskerLengthFactor), K_WhiskerLengthFactor },
        { nameof(SmallWhiskerLength), K_SmallWhiskerLength },
        { nameof(BigWhiskerLength), K_BigWhiskerLength },
        { nameof(LegScaleXFactor), K_LegScaleXFactor },
        { nameof(ShellScaleYFactor), K_ShellScaleYFactor },
        { nameof(WhiskerShapeFactor), K_WhiskerShapeFactor },
        { nameof(HueMax), K_HueMax },
        { nameof(HueMin), K_HueMin },
        { nameof(SaturationMax), K_SaturationMax },
        { nameof(SaturationMin), K_SaturationMin },
        { nameof(LinearSandboxCost), K_LinearSandboxCost },
        { nameof(ExponentialSandboxCost), K_ExponentialSandboxCost },
        { nameof(RoomPerformanceCost), K_RoomPerformanceCost },
        { nameof(BaseDamageResistance), K_BaseDamageResistance },
        { nameof(ExplosionResistance), K_ExplosionResistance },
        { nameof(BodySizeEstimate), K_BodySizeEstimate },
        { nameof(VisualRadius), K_VisualRadius },
        { nameof(WaterVision), K_WaterVision },
        { nameof(ThroughSurfaceVision), K_ThroughSurfaceVision },
        { nameof(MovementBasedVision), K_MovementBasedVision },
        { nameof(DangerousToPlayer), K_DangerousToPlayer },
        { nameof(LungCapacity), K_LungCapacity },
        { nameof(CommunityInfluence), K_CommunityInfluence },
        { nameof(MeatMin), K_MeatMin },
        { nameof(MeatMax), K_MeatMax },
        { nameof(Scaryness), K_Scaryness },
        { nameof(MinBuoyancy), K_MinBuoyancy },
        { nameof(MaxBuoyancy), K_MaxBuoyancy },
        { nameof(BodyChunkRadType), K_BodyChunkRadType },
        { nameof(HeadVelocityType), K_HeadVelocityType },
        { nameof(BodySizeGenerationType), K_BodySizeGenerationType },
        { nameof(ShockType), K_ShockType },
        { nameof(ResistanceType), K_ResistanceType },
        { nameof(AllowedIdleTypes), K_AllowedIdleTypes },
        { nameof(DynamicRelationshipType), K_DynamicRelationshipType },
        { nameof(VisualScoreTypes), K_VisualScoreTypes },
        { nameof(PreyTrackType), K_PreyTrackType },
        { nameof(ExcitementType), K_ExcitementType },
        { nameof(WingSizeType), K_WingSizeType },
        { nameof(MainColorType), K_MainColorType },
        { nameof(LightFlashNoiseType), K_LightFlashNoiseType },
        { nameof(BodySegmentType), K_BodySegmentType },
        { nameof(Grabability), K_Grabability },
        { nameof(ShockColor), K_ShockColor },
        { nameof(ShortCutColor), K_ShortCutColor },
        { nameof(HeadGlowMinColor), K_HeadGlowMinColor },
        { nameof(HeadGlowMaxColor), K_HeadGlowMaxColor },
        { nameof(BodyBlackColor), K_BodyBlackColor },
        { nameof(LightFlashColor), K_LightFlashColor },
        { nameof(WingColor), K_WingColor },
        { nameof(StandardIconColor), K_StandardIconColor },
        { nameof(BigIconColor), K_BigIconColor },
        { nameof(SmallIconColor), K_SmallIconColor },
        { nameof(DevColor), K_DevColor },
        { nameof(ShellColor), K_ShellColor },
        { nameof(SecondaryShellColor), K_SecondaryShellColor },
        { nameof(SegmentSprite), K_SegmentSprite },
        { nameof(AdditionalShellTextureShader), K_AdditionalShellTextureShader },
        { nameof(LegASprite), K_LegASprite },
        { nameof(LegBSprite), K_LegBSprite },
        { nameof(WingSprite), K_WingSprite },
        { nameof(WingShader), K_WingShader },
        { nameof(ShellSpikeSprite1), K_ShellSpikeSprite1 },
        { nameof(ShellSpikeSprite2), K_ShellSpikeSprite2 },
        { nameof(BellyShellSprite), K_BellyShellSprite },
        { nameof(BackShellSprite), K_BackShellSprite },
        { nameof(StandardIconSprite), K_StandardIconSprite },
        { nameof(BigIconSprite), K_BigIconSprite },
        { nameof(SmallIconSprite), K_SmallIconSprite },
        { nameof(DevName), K_DevName },
        { "Inherit", K_Inherit },
        { nameof(UnlockID), K_UnlockID },
        { nameof(ParentUnlock), K_ParentUnlock },
        { nameof(Relationships), K_Relationships },
        { nameof(SureToGetPreyDistance), K_SureToGetPreyDistance },
        { nameof(PatherStepsPerFrame), K_PatherStepsPerFrame },
        { nameof(HeadGlobalVelocityFactor), K_HeadGlobalVelocityFactor },
        { nameof(BaseStunResistance), K_BaseStunResistance },
        { nameof(ExplosionStunResistance), K_ExplosionStunResistance },
        { nameof(BlizzardWanderer), K_BlizzardWanderer },
        { nameof(SmallParentUnlock), K_SmallParentUnlock },
        { nameof(BigParentUnlock), K_BigParentUnlock },
        { nameof(SmallUnlockID), K_SmallUnlockID },
        { nameof(BigUnlockID), K_BigUnlockID },
        { nameof(SpikeScaleFactor), K_SpikeScaleFactor },
        { nameof(ShockResistanceReductionFactor), K_ShockResistanceReductionFactor },
        { nameof(ExpeditionScore), K_ExpeditionScore },
        { nameof(SecondaryShellColorBonus), K_SecondaryShellColorBonus },
        { nameof(GlobalVelocityFactor), K_GlobalVelocityFactor },
        { nameof(GlobalFlyingVelocityFactor), K_GlobalFlyingVelocityFactor },
        { nameof(DamageReductionFactor), K_DamageReductionFactor },
        { nameof(DeadShellChance), K_DeadShellChance },
        { nameof(GrabbedShockChargeReduction), K_GrabbedShockChargeReduction },
        { nameof(MajorCreature), K_MajorCreature },
        { nameof(NoViolenceStun), K_NoViolenceStun },
        { nameof(TooBigForShelter), K_TooBigForShelter },
        { nameof(WantsToShock), K_WantsToShock },
        { "Name", K_Name },
        { nameof(Path), K_Path },
        { nameof(SporeCloudImmune), K_SporeCloudImmune },
        { nameof(WaterPathingResistance), K_WaterPathingResistance },
        { nameof(BluntDamageResistance), K_BluntDamageResistance },
        { nameof(BluntStunResistance), K_BluntStunResistance },
        { nameof(WaterDamageResistance), K_WaterDamageResistance },
        { nameof(WaterStunResistance), K_WaterStunResistance },
        { nameof(StabDamageResistance), K_StabDamageResistance },
        { nameof(StabStunResistance), K_StabStunResistance },
        { nameof(BiteDamageResistance), K_BiteDamageResistance },
        { nameof(BiteStunResistance), K_BiteStunResistance },
        { nameof(ElectricDamageResistance), K_ElectricDamageResistance },
        { nameof(ElectricStunResistance), K_ElectricStunResistance },
        { nameof(OffScreenSpeed), K_OffScreenSpeed },
        { nameof(SurfaceFriction), K_SurfaceFriction },
        { nameof(Bounce), K_Bounce },
        { nameof(WaterRetardationImmunity), K_WaterRetardationImmunity },
        { nameof(WaterFriction), K_WaterFriction },
        { nameof(AirFriction), K_AirFriction },
        { nameof(ImpactThreshold), K_ImpactThreshold },
        { nameof(DaddyCorruptionImmune), K_DaddyCorruptionImmune },
        { nameof(DoesNotUseDens), K_DoesNotUseDens },
        { nameof(CannotBeHitByWeapons), K_CannotBeHitByWeapons },
        { nameof(SandstormImmune), K_SandstormImmune },
        { nameof(CannotBeBlinded), K_CannotBeBlinded },
        { nameof(CannotBeDeafened), K_CannotBeDeafened }
    };

    public CreatureTemplate.Relationship DefaultRelationship
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new() { type = _defaultRel._type, intensity = _defaultRel._intensity };
    }

    internal string this[int hashKey]
    {
        set
        {
            (PaletteColorType Type, Color Clr) paletteRes;
            switch (hashKey)
            {
                case K_Swimming:
                    Flags.Set(Swimming, ParseBool(value));
                    break;
                case K_AutomaticPickUp:
                    Flags.Set(AutomaticPickUp, ParseBool(value));
                    break;
                case K_SmallFood:
                    Flags.Set(SmallFood, ParseBool(value));
                    break;
                case K_ShellParticles:
                    Flags.Set(ShellParticles, ParseBool(value));
                    break;
                case K_Flying:
                    Flags.Set(Flying, ParseBool(value));
                    break;
                case K_DetectsAnnoyingCollisions:
                    Flags.Set(DetectsAnnoyingCollisions, ParseBool(value));
                    break;
                case K_WeakToStun:
                    Flags.Set(WeakToStun, ParseBool(value));
                    break;
                case K_GlowingHead:
                    Flags.Set(GlowingHead, ParseBool(value));
                    break;
                case K_ShocksWhenGrabbed:
                    Flags.Set(ShocksWhenGrabbed, ParseBool(value));
                    break;
                case K_Shields:
                    Flags.Set(Shields, ParseBool(value));
                    break;
                case K_NoiseTracker:
                    Flags.Set(NoiseTracker, ParseBool(value));
                    break;
                case K_AdditionalShellTexture:
                    Flags.Set(AdditionalShellTexture, ParseBool(value));
                    break;
                case K_Wings:
                    Flags.Set(Wings, ParseBool(value));
                    break;
                case K_ShellSpikes:
                    Flags.Set(ShellSpikes, ParseBool(value));
                    break;
                case K_BodySizeDependantWhisker:
                    Flags.Set(BodySizeDependantWhisker, ParseBool(value));
                    break;
                case K_SmallTube:
                    Flags.Set(SmallTube, ParseBool(value));
                    break;
                case K_SlowLegs:
                    Flags.Set(SlowLegs, ParseBool(value));
                    break;
                case K_KillScoreHidden:
                    Flags.Set(KillScoreHidden, ParseBool(value));
                    break;
                case K_CountsAsAKill:
                    Flags.Set(CountsAsAKill, ParseBool(value));
                    break;
                case K_WaterOnly:
                    Flags.Set(WaterOnly, ParseBool(value));
                    break;
                case K_IgnoresCommunity:
                    Flags.Set(IgnoresCommunity, ParseBool(value));
                    break;
                case K_TriesToStayInRoom:
                    Flags.Set(TriesToStayInRoom, ParseBool(value));
                    break;
                case K_EasyKill:
                    Flags.Set(EasyKill, ParseBool(value));
                    break;
                case K_UsesUnlockData:
                    Flags.Set(UsesUnlockData, ParseBool(value));
                    break;
                case K_EdibleByOmnivores:
                    Flags.Set(EdibleByOmnivores, ParseBool(value));
                    break;
                case K_WormGrassImmune:
                    Flags.Set(WormGrassImmune, ParseBool(value));
                    break;
                case K_ForbidStandardShortcutEntry:
                    Flags.Set(ForbidStandardShortcutEntry, ParseBool(value));
                    break;
                case K_BlizzardWanderer:
                    Flags.Set(BlizzardWanderer, ParseBool(value));
                    break;
                case K_NoViolenceStun:
                    Flags.Set(NoViolenceStun, ParseBool(value));
                    break;
                case K_MajorCreature:
                    Flags.Set(MajorCreature, ParseBool(value));
                    break;
                case K_TooBigForShelter:
                    Flags.Set(TooBigForShelter, ParseBool(value));
                    break;
                case K_WantsToShock:
                    Flags.Set(WantsToShock, ParseBool(value));
                    break;
                case K_SporeCloudImmune:
                    Flags2.Set(SporeCloudImmune, ParseBool(value));
                    break;
                case K_DaddyCorruptionImmune:
                    Flags2.Set(DaddyCorruptionImmune, ParseBool(value));
                    break;
                case K_DoesNotUseDens:
                    Flags2.Set(DoesNotUseDens, ParseBool(value));
                    break;
                case K_CannotBeHitByWeapons:
                    Flags2.Set(CannotBeHitByWeapons, ParseBool(value));
                    break;
                case K_SandstormImmune:
                    Flags2.Set(SandstormImmune, ParseBool(value));
                    break;
                case K_CannotBeBlinded:
                    Flags2.Set(CannotBeBlinded, ParseBool(value));
                    break;
                case K_CannotBeDeafened:
                    Flags2.Set(CannotBeDeafened, ParseBool(value));
                    break;
                case K_Throwable:
                    Flags2.Set(Throwable, ParseBool(value));
                    Flags2.Set(Throwable_HasValue, true);
                    break;
                case K_NightOnly:
                    Flags2.Set(NightOnly, ParseBool(value));
                    Flags2.Set(NightOnly_HasValue, true);
                    break;
                case K_IgnoresCycle:
                    Flags2.Set(IgnoresCycle, ParseBool(value));
                    Flags2.Set(IgnoresCycle_HasValue, true);
                    break;
                case K_PreCycle:
                    Flags2.Set(PreCycle, ParseBool(value));
                    Flags2.Set(PreCycle_HasValue, true);
                    break;
                case K_LavaImmune:
                    Flags2.Set(LavaImmune, ParseBool(value));
                    Flags2.Set(LavaImmune_HasValue, true);
                    break;
                case K_TentacleImmune:
                    Flags2.Set(TentacleImmune, ParseBool(value));
                    Flags2.Set(TentacleImmune_HasValue, true);
                    break;
                case K_HypothermiaImmune:
                    Flags2.Set(HypothermiaImmune, ParseBool(value));
                    Flags2.Set(HypothermiaImmune_HasValue, true);
                    break;
                case K_FoodPoints:
                    FoodPoints = ParseInt(value);
                    break;
                case K_Bites:
                    Bites = ParseInt(value);
                    break;
                case K_MinChunkAmount:
                    MinChunkAmount = ParseInt(value);
                    break;
                case K_MaxChunkAmount:
                    MaxChunkAmount = ParseInt(value);
                    break;
                case K_WingVariations:
                    WingVariations = ParseInt(value);
                    break;
                case K_StandardKillScore:
                    StandardKillScore = ParseInt(value);
                    break;
                case K_AbstractedLaziness:
                    AbstractedLaziness = ParseInt(value);
                    break;
                case K_ShortCutSegments:
                    ShortCutSegments = ParseInt(value);
                    break;
                case K_BigKillScore:
                    BigKillScore = ParseInt(value);
                    break;
                case K_SmallKillScore:
                    SmallKillScore = ParseInt(value);
                    break;
                case K_PatherStepsPerFrame:
                    PatherStepsPerFrame = ParseInt(value);
                    break;
                case K_ExpeditionScore:
                    ExpeditionScore = ParseInt(value);
                    break;
                case K_BodyChunkRadBonus:
                    BodyChunkRadBonus = ParseFloat(value);
                    break;
                case K_BodyChunkMassBonus:
                    BodyChunkMassBonus = ParseFloat(value);
                    break;
                case K_ConnectionElasticityReduction:
                    ConnectionElasticityReduction = ParseFloat(value);
                    break;
                case K_VelocityFactor:
                    VelocityFactor = ParseFloat(value);
                    break;
                case K_HeadVelocityFactor:
                    HeadVelocityFactor = ParseFloat(value);
                    break;
                case K_MaxSize:
                    MaxSize = ParseFloat(value);
                    break;
                case K_MinSize:
                    MinSize = ParseFloat(value);
                    break;
                case K_PreyTrackerWeight:
                    PreyTrackerWeight = ParseFloat(value);
                    break;
                case K_WingLengthFactor:
                    WingLengthFactor = ParseFloat(value);
                    break;
                case K_LegLengthFactor:
                    LegLengthFactor = ParseFloat(value);
                    break;
                case K_WhiskerLengthFactor:
                    WhiskerLengthFactor = ParseFloat(value);
                    break;
                case K_SmallWhiskerLength:
                    SmallWhiskerLength = ParseFloat(value);
                    break;
                case K_BigWhiskerLength:
                    BigWhiskerLength = ParseFloat(value);
                    break;
                case K_LegScaleXFactor:
                    LegScaleXFactor = ParseFloat(value);
                    break;
                case K_ShellScaleYFactor:
                    ShellScaleYFactor = ParseFloat(value);
                    break;
                case K_WhiskerShapeFactor:
                    WhiskerShapeFactor = ParseFloat(value);
                    break;
                case K_HueMax:
                    HueMax = ParseFloat(value);
                    break;
                case K_HueMin:
                    HueMin = ParseFloat(value);
                    break;
                case K_SaturationMax:
                    SaturationMax = ParseFloat(value);
                    break;
                case K_SaturationMin:
                    SaturationMin = ParseFloat(value);
                    break;
                case K_LinearSandboxCost:
                    LinearSandboxCost = ParseFloat(value);
                    break;
                case K_ExponentialSandboxCost:
                    ExponentialSandboxCost = ParseFloat(value);
                    break;
                case K_RoomPerformanceCost:
                    RoomPerformanceCost = ParseFloat(value);
                    break;
                case K_BaseDamageResistance:
                    BaseDamageResistance = ParseFloat(value);
                    break;
                case K_ExplosionResistance:
                    ExplosionResistance = ParseFloat(value);
                    break;
                case K_BodySizeEstimate:
                    BodySizeEstimate = ParseFloat(value);
                    break;
                case K_VisualRadius:
                    VisualRadius = ParseFloat(value);
                    break;
                case K_WaterVision:
                    WaterVision = ParseFloat(value);
                    break;
                case K_ThroughSurfaceVision:
                    ThroughSurfaceVision = ParseFloat(value);
                    break;
                case K_MovementBasedVision:
                    MovementBasedVision = ParseFloat(value);
                    break;
                case K_DangerousToPlayer:
                    DangerousToPlayer = ParseFloat(value);
                    break;
                case K_LungCapacity:
                    LungCapacity = ParseFloat(value);
                    break;
                case K_CommunityInfluence:
                    CommunityInfluence = ParseFloat(value);
                    break;
                case K_MeatMin:
                    MeatMin = ParseFloat(value);
                    break;
                case K_MeatMax:
                    MeatMax = ParseFloat(value);
                    break;
                case K_Scaryness:
                    Scaryness = ParseFloat(value);
                    break;
                case K_MinBuoyancy:
                    MinBuoyancy = ParseFloat(value);
                    break;
                case K_MaxBuoyancy:
                    MaxBuoyancy = ParseFloat(value);
                    break;
                case K_SureToGetPreyDistance:
                    SureToGetPreyDistance = ParseFloat(value);
                    break;
                case K_HeadGlobalVelocityFactor:
                    HeadGlobalVelocityFactor = ParseFloat(value);
                    break;
                case K_BaseStunResistance:
                    BaseStunResistance = ParseFloat(value);
                    break;
                case K_ExplosionStunResistance:
                    ExplosionStunResistance = ParseFloat(value);
                    break;
                case K_SpikeScaleFactor:
                    SpikeScaleFactor = ParseFloat(value);
                    break;
                case K_ShockResistanceReductionFactor:
                    ShockResistanceReductionFactor = ParseFloat(value);
                    break;
                case K_SecondaryShellColorBonus:
                    SecondaryShellColorBonus = ParseFloat(value);
                    break;
                case K_GlobalVelocityFactor:
                    GlobalVelocityFactor = ParseFloat(value);
                    break;
                case K_GlobalFlyingVelocityFactor:
                    GlobalFlyingVelocityFactor = ParseFloat(value);
                    break;
                case K_DamageReductionFactor:
                    DamageReductionFactor = ParseFloat(value);
                    break;
                case K_DeadShellChance:
                    DeadShellChance = ParseFloat(value);
                    break;
                case K_GrabbedShockChargeReduction:
                    GrabbedShockChargeReduction = ParseFloat(value);
                    break;
                case K_WaterPathingResistance:
                    WaterPathingResistance = ParseFloat(value);
                    break;
                case K_BluntDamageResistance:
                    BluntDamageResistance = ParseFloat(value);
                    break;
                case K_BluntStunResistance:
                    BluntStunResistance = ParseFloat(value);
                    break;
                case K_WaterDamageResistance:
                    WaterDamageResistance = ParseFloat(value);
                    break;
                case K_WaterStunResistance:
                    WaterStunResistance = ParseFloat(value);
                    break;
                case K_StabDamageResistance:
                    StabDamageResistance = ParseFloat(value);
                    break;
                case K_StabStunResistance:
                    StabStunResistance = ParseFloat(value);
                    break;
                case K_BiteDamageResistance:
                    BiteDamageResistance = ParseFloat(value);
                    break;
                case K_BiteStunResistance:
                    BiteStunResistance = ParseFloat(value);
                    break;
                case K_ElectricDamageResistance:
                    ElectricDamageResistance = ParseFloat(value);
                    break;
                case K_ElectricStunResistance:
                    ElectricStunResistance = ParseFloat(value);
                    break;
                case K_OffScreenSpeed:
                    OffScreenSpeed = ParseFloat(value);
                    break;
                case K_SurfaceFriction:
                    SurfaceFriction = ParseFloat(value);
                    break;
                case K_Bounce:
                    Bounce = ParseFloat(value);
                    break;
                case K_WaterRetardationImmunity:
                    WaterRetardationImmunity = ParseFloat(value);
                    break;
                case K_WaterFriction:
                    WaterFriction = ParseFloat(value);
                    break;
                case K_AirFriction:
                    AirFriction = ParseFloat(value);
                    break;
                case K_ImpactThreshold:
                    ImpactThreshold = ParseFloat(value);
                    break;
                case K_BodyChunkRadType:
                    BodyChunkRadType = ParseEnum<ChunkRadType>(value);
                    break;
                case K_HeadVelocityType:
                    HeadVelocityType = ParseEnum<HeadMoveType>(value);
                    break;
                case K_BodySizeGenerationType:
                    BodySizeGenerationType = ParseEnum<SizeGenerationType>(value);
                    break;
                case K_ShockType:
                    ShockType = ParseEnum<ShockDamageType>(value);
                    break;
                case K_ResistanceType:
                    ResistanceType = ParseEnum<ViolenceResistanceType>(value);
                    break;
                case K_AllowedIdleTypes:
                    AllowedIdleTypes = ParseEnum<IdleScoreTypes>(value);
                    break;
                case K_DynamicRelationshipType:
                    DynamicRelationshipType = ParseEnum<RelationshipChangeType>(value);
                    break;
                case K_VisualScoreTypes:
                    VisualScoreTypes = ParseEnum<VisualScoreChangeTypes>(value);
                    break;
                case K_PreyTrackType:
                    PreyTrackType = ParseEnum<PreyTrackerType>(value);
                    break;
                case K_ExcitementType:
                    ExcitementType = ParseEnum<ExcitementTrackerType>(value);
                    break;
                case K_WingSizeType:
                    WingSizeType = ParseEnum<WingType>(value);
                    break;
                case K_LightFlashNoiseType:
                    LightFlashNoiseType = ParseEnum<FlashColorNoiseType>(value);
                    break;
                case K_BodySegmentType:
                    BodySegmentType = ParseEnum<SegmentType>(value);
                    break;
                case K_Grabability:
                    Grabability = ParseEnum<Player.ObjectGrabability>(value);
                    Flags2.Set(Grabability_HasValue, true);
                    break;
                case K_MainColorType:
                    MainColorType = ParseEnum<PaletteColorType>(value);
                    break;
                case K_BodyBlackColor:
                    paletteRes = ParsePaletteColor(value);
                    BodyBlackColor = paletteRes.Clr;
                    BodyBlackColorType = paletteRes.Type;
                    break;
                case K_HeadGlowMinColor:
                    paletteRes = ParsePaletteColor(value);
                    HeadGlowMinColor = paletteRes.Clr;
                    HeadGlowMinColorType = paletteRes.Type;
                    break;
                case K_HeadGlowMaxColor:
                    paletteRes = ParsePaletteColor(value);
                    HeadGlowMaxColor = paletteRes.Clr;
                    HeadGlowMaxColorType = paletteRes.Type;
                    break;
                case K_LightFlashColor:
                    paletteRes = ParsePaletteColor(value);
                    LightFlashColor = paletteRes.Clr;
                    LightFlashColorType = paletteRes.Type;
                    break;
                case K_WingColor:
                    paletteRes = ParsePaletteColor(value);
                    WingColor = paletteRes.Clr;
                    WingColorType = paletteRes.Type;
                    break;
                case K_ShockColor:
                    paletteRes = ParsePaletteColor(value);
                    ShockColor = paletteRes.Clr;
                    ShockColorType = paletteRes.Type;
                    break;
                case K_ShellColor:
                    paletteRes = ParsePaletteColor(value);
                    ShellColor = paletteRes.Clr;
                    ShellColorType = paletteRes.Type;
                    Flags2.Set(ShellColorType_HasValue, true);
                    break;
                case K_SecondaryShellColor:
                    paletteRes = ParsePaletteColor(value);
                    SecondaryShellColor = paletteRes.Clr;
                    SecondaryShellColorType = paletteRes.Type;
                    Flags2.Set(SecondaryShellColorType_HasValue, true);
                    break;
                case K_ShortCutColor:
                    ShortCutColor = ParseColor(value);
                    break;
                case K_StandardIconColor:
                    StandardIconColor = ParseColor(value);
                    break;
                case K_BigIconColor:
                    BigIconColor = ParseColor(value);
                    break;
                case K_SmallIconColor:
                    SmallIconColor = ParseColor(value);
                    break;
                case K_DevColor:
                    DevColor = ParseColor(value);
                    break;
                case K_SegmentSprite:
                    SegmentSprite = value;
                    break;
                case K_AdditionalShellTextureShader:
                    AdditionalShellTextureShader = value;
                    break;
                case K_LegASprite:
                    LegASprite = value;
                    break;
                case K_LegBSprite:
                    LegBSprite = value;
                    break;
                case K_WingSprite:
                    WingSprite = value;
                    break;
                case K_WingShader:
                    WingShader = value;
                    break;
                case K_ShellSpikeSprite1:
                    ShellSpikeSprite1 = value;
                    break;
                case K_ShellSpikeSprite2:
                    ShellSpikeSprite2 = value;
                    break;
                case K_BellyShellSprite:
                    BellyShellSprite = value;
                    break;
                case K_BackShellSprite:
                    BackShellSprite = value;
                    break;
                case K_StandardIconSprite:
                    StandardIconSprite = value;
                    break;
                case K_BigIconSprite:
                    BigIconSprite = value;
                    break;
                case K_SmallIconSprite:
                    SmallIconSprite = value;
                    break;
                case K_DevName:
                    DevName = value;
                    break;
                case K_SmallUnlockID:
                    SmallUnlockID = new(value, true);
                    break;
                case K_UnlockID:
                    UnlockID = new(value, true);
                    break;
                case K_BigUnlockID:
                    BigUnlockID = new(value, true);
                    break;
                case K_SmallParentUnlock:
                    SmallParentUnlock = new(value);
                    if (SmallParentUnlock.Index == -1)
                        SmallParentUnlock = null;
                    break;
                case K_ParentUnlock:
                    ParentUnlock = new(value);
                    if (ParentUnlock.Index == -1)
                        ParentUnlock = null;
                    break;
                case K_BigParentUnlock:
                    BigParentUnlock = new(value);
                    if (BigParentUnlock.Index == -1)
                        BigParentUnlock = null;
                    break;
                case K_Relationships:
                    Relationships = ParseCustomRelationArray(value);
                    _alreadySetRels = Relationships is not null;
                    break;
            }
        }
    }

    public void Set(Configurable<int> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        int ival;
        switch (strval)
        {
            case K_FoodPoints:
                FoodPoints = config.Value;
                break;
            case K_Bites:
                Bites = config.Value;
                break;
            case K_MinChunkAmount:
                MinChunkAmount = config.Value;
                break;
            case K_MaxChunkAmount:
                MaxChunkAmount = config.Value;
                break;
            case K_WingVariations:
                WingVariations = config.Value;
                break;
            case K_StandardKillScore:///
                StandardKillScore = config.Value;
                /*ival = config.Value;
                StandardKillScore = ival;
                {
                    if (_critob._standardUnlock is SandboxUnlock u)
                        s_KillScoreValue.SetValue(s_KillScore.GetValue(u), ival);
                }*/
                break;
            case K_AbstractedLaziness:
                ival = config.Value;
                AbstractedLaziness = ival;
                _template.abstractedLaziness = ival;
                break;
            case K_ShortCutSegments:
                ival = config.Value;
                ShortCutSegments = ival;
                _template.shortcutSegments = ival;
                break;
            case K_BigKillScore:///
                BigKillScore = config.Value;
                /*ival = config.Value;
                BigKillScore = ival;
                {
                    if (_critob._bigUnlock is SandboxUnlock u)
                        s_KillScoreValue.SetValue(s_KillScore.GetValue(u), ival);
                }*/
                break;
            case K_SmallKillScore:///
                SmallKillScore = config.Value;
                /*ival = config.Value;
                SmallKillScore = ival;
                {
                    if (_critob._smallUnlock is SandboxUnlock u)
                        s_KillScoreValue.SetValue(s_KillScore.GetValue(u), ival);
                }*/
                break;
            case K_PatherStepsPerFrame:
                PatherStepsPerFrame = config.Value;
                break;
            case K_ExpeditionScore:///
                ExpeditionScore = config.Value;
                break;
            /*default:
                throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetConfig(Configurable<int> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        config.Value = strval switch
        {
            K_FoodPoints => FoodPoints,
            K_Bites => Bites,
            K_MinChunkAmount => MinChunkAmount,
            K_MaxChunkAmount => MaxChunkAmount,
            K_WingVariations => WingVariations,
            K_StandardKillScore => StandardKillScore,
            K_AbstractedLaziness => AbstractedLaziness,
            K_ShortCutSegments => ShortCutSegments,
            K_BigKillScore => BigKillScore,
            K_SmallKillScore => SmallKillScore,
            K_PatherStepsPerFrame => PatherStepsPerFrame,
            K_ExpeditionScore => ExpeditionScore,
            _ => 0//throw new ArgumentException("Invalid key: " + s),
        };
    }

    public void Set(Configurable<bool> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        bool flag;
        CreatureTemplate tpl;
        switch (strval)
        {
            case K_Swimming:
                flag = config.Value;
                Flags.Set(Swimming, flag);
                tpl = _template;
                tpl.pathingPreferencesConnections[(int)MovementConnection.MovementType.DropToWater] = flag ? new(1f, Allowed) : new(100f, IllegalConnection);
                tpl.damageRestistances[(int)Creature.DamageType.Water, 0] = flag ? 102f : 0f;
                tpl.damageRestistances[(int)Creature.DamageType.Water, 1] = flag ? 102f : 0f;
                tpl.waterRelationship = Flags.Get(WaterOnly) ? CreatureTemplate.WaterRelationship.WaterOnly : (flag ? CreatureTemplate.WaterRelationship.Amphibious : CreatureTemplate.WaterRelationship.AirAndSurface);
                tpl.preBakedPathingAncestor = StaticWorld.GetCreatureTemplate(Flags.Get(Flying) ? (flag ? CreatureTemplate.Type.BigEel : CreatureTemplate.Type.Fly) : (flag ? CreatureTemplate.Type.Leech : CreatureTemplate.Type.BlueLizard));
                break;
            case K_AutomaticPickUp:
                Flags.Set(AutomaticPickUp, config.Value);
                break;
            case K_SmallFood:
                flag = config.Value;
                Flags.Set(SmallFood, flag);
                _template.meatPoints = flag ? 0 : 1;
                _critob.ShelterDanger = flag ? ShelterDanger.Safe : ShelterDanger.Hostile;
                break;
            case K_ShellParticles:
                Flags.Set(ShellParticles, config.Value);
                break;
            case K_Flying:
                flag = config.Value;
                Flags.Set(Flying, flag);
                tpl = _template;
                tpl.canFly = flag;
                tpl.pathingPreferencesTiles[(int)AItile.Accessibility.Air] = flag ? new(1f, Allowed) : new(60f, IllegalTile);
                tpl.pathingPreferencesConnections[(int)MovementConnection.MovementType.NPCTransportation].resistance = flag ? 10f : 25f;
                tpl.preBakedPathingAncestor = StaticWorld.GetCreatureTemplate(flag ? (Flags.Get(Swimming) ? CreatureTemplate.Type.BigEel : CreatureTemplate.Type.Fly) : (Flags.Get(Swimming) ? CreatureTemplate.Type.Leech : CreatureTemplate.Type.BlueLizard));
                break;
            case K_DetectsAnnoyingCollisions:
                Flags.Set(DetectsAnnoyingCollisions, config.Value);
                break;
            case K_WeakToStun:
                Flags.Set(WeakToStun, config.Value);
                break;
            case K_DaddyCorruptionImmune:
                flag = config.Value;
                _template.daddyCorruptionImmune = flag;
                Flags2.Set(DaddyCorruptionImmune, flag);
                break;
            case K_DoesNotUseDens:
                flag = config.Value;
                _template.doesNotUseDens = flag;
                Flags2.Set(DoesNotUseDens, flag);
                break;
            case K_CannotBeHitByWeapons:
                Flags2.Set(CannotBeHitByWeapons, config.Value);
                break;
            case K_SandstormImmune:
                Flags2.Set(SandstormImmune, config.Value);
                break;
            case K_CannotBeBlinded:
                Flags2.Set(CannotBeBlinded, config.Value);
                break;
            case K_CannotBeDeafened:
                Flags2.Set(CannotBeDeafened, config.Value);
                break;
            case K_GlowingHead:
                Flags.Set(GlowingHead, config.Value);
                break;
            case K_ShocksWhenGrabbed:
                Flags.Set(ShocksWhenGrabbed, config.Value);
                break;
            case K_Shields:
                Flags.Set(Shields, config.Value);
                break;
            case K_NoiseTracker:
                Flags.Set(NoiseTracker, config.Value);
                break;
            case K_AdditionalShellTexture:
                Flags.Set(AdditionalShellTexture, config.Value);
                break;
            case K_Wings:
                Flags.Set(Wings, config.Value);
                break;
            case K_ShellSpikes:
                Flags.Set(ShellSpikes, config.Value);
                break;
            case K_BodySizeDependantWhisker:
                Flags.Set(BodySizeDependantWhisker, config.Value);
                break;
            case K_SmallTube:
                Flags.Set(SmallTube, config.Value);
                break;
            case K_SlowLegs:
                Flags.Set(SlowLegs, config.Value);
                break;
            case K_KillScoreHidden:///
                Flags.Set(KillScoreHidden, config.Value);
                /*flag = config.Value;
                Flags.Set(KillScoreHidden, flag);
                {
                    if (_critob._smallUnlock is SandboxUnlock u)
                        s_KillScoreIsConfigurable.SetValue(s_KillScore.GetValue(u), !flag);
                    if (_critob._standardUnlock is SandboxUnlock u2)
                        s_KillScoreIsConfigurable.SetValue(s_KillScore.GetValue(u2), !flag);
                    if (_critob._bigUnlock is SandboxUnlock u3)
                        s_KillScoreIsConfigurable.SetValue(s_KillScore.GetValue(u3), !flag);
                }*/
                break;
            case K_CountsAsAKill:
                flag = config.Value;
                Flags.Set(CountsAsAKill, flag);
                _template.countsAsAKill = flag ? 2 : 0;
                break;
            case K_WaterOnly:
                flag = config.Value;
                Flags.Set(WaterOnly, flag);
                tpl = _template;
                tpl.waterRelationship = flag ? CreatureTemplate.WaterRelationship.WaterOnly : (Flags.Get(Swimming) ? CreatureTemplate.WaterRelationship.Amphibious : CreatureTemplate.WaterRelationship.AirAndSurface);
                tpl.pathingPreferencesTiles[(int)AItile.Accessibility.Floor].legality = flag ? Unwanted : Allowed;
                tpl.pathingPreferencesTiles[(int)AItile.Accessibility.Corridor].legality = flag ? Unwanted : Allowed;
                tpl.pathingPreferencesTiles[(int)AItile.Accessibility.Climb] = flag ? new(10f, Unwanted) : new(1f, Allowed);
                tpl.pathingPreferencesTiles[(int)AItile.Accessibility.Wall] = flag ? new(100f, Unwanted) : new(1f, Allowed);
                tpl.pathingPreferencesTiles[(int)AItile.Accessibility.Ceiling] = flag ? new(100f, Unwanted) : new(1f, Allowed);
                break;
            case K_IgnoresCommunity:
                flag = config.Value;
                Flags.Set(IgnoresCommunity, flag);
                _template.communityID = flag ? CreatureCommunities.CommunityID.None : CreatureCommunities.CommunityID.All;
                break;
            case K_TriesToStayInRoom:
                flag = config.Value;
                Flags.Set(TriesToStayInRoom, flag);
                _template.roamBetweenRoomsChance = flag ? -1f : .1f;
                break;
            case K_EasyKill:
                flag = config.Value;
                Flags.Set(EasyKill, flag);
                _template.instantDeathDamageLimit = flag ? 1f : float.MaxValue;
                break;
            case K_UsesUnlockData:///
                Flags.Set(UsesUnlockData, config.Value);
                break;
            case K_EdibleByOmnivores:
                Flags.Set(EdibleByOmnivores, config.Value);
                break;
            case K_WormGrassImmune:
                flag = config.Value;
                Flags.Set(WormGrassImmune, flag);
                tpl = _template;
                tpl.wormgrassTilesIgnored = flag;
                tpl.wormGrassImmune = flag;
                break;
            case K_ForbidStandardShortcutEntry:
                flag = config.Value;
                Flags.Set(ForbidStandardShortcutEntry, flag);
                _template.forbidStandardShortcutEntry = flag;
                break;
            case K_BlizzardWanderer:
                flag = config.Value;
                Flags.Set(BlizzardWanderer, flag);
                _template.BlizzardWanderer = flag;
                break;
            case K_NoViolenceStun:
                Flags.Set(NoViolenceStun, config.Value);
                break;
            case K_MajorCreature:
                flag = config.Value;
                Flags.Set(MajorCreature, flag);
                if (s_majorCreatures is HashSet<CreatureTemplate.Type> set)
                {
                    if (!set.Contains(CreatureType) && flag)
                        set.Add(CreatureType);
                    else if (set.Contains(CreatureType) && !flag)
                        set.Remove(CreatureType);
                }
                break;
            case K_TooBigForShelter:
                Flags.Set(TooBigForShelter, config.Value);
                break;
            case K_WantsToShock:
                Flags.Set(WantsToShock, config.Value);
                break;
            case K_SporeCloudImmune:
                Flags2.Set(SporeCloudImmune, config.Value);
                break;
                /*default:
                    throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetConfig(Configurable<bool> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        config.Value = strval switch
        {
            K_Swimming => Flags.Get(Swimming),
            K_AutomaticPickUp => Flags.Get(AutomaticPickUp),
            K_SmallFood => Flags.Get(SmallFood),
            K_ShellParticles => Flags.Get(ShellParticles),
            K_Flying => Flags.Get(Flying),
            K_DetectsAnnoyingCollisions => Flags.Get(DetectsAnnoyingCollisions),
            K_WeakToStun => Flags.Get(WeakToStun),
            K_GlowingHead => Flags.Get(GlowingHead),
            K_ShocksWhenGrabbed => Flags.Get(ShocksWhenGrabbed),
            K_Shields => Flags.Get(Shields),
            K_NoiseTracker => Flags.Get(NoiseTracker),
            K_AdditionalShellTexture => Flags.Get(AdditionalShellTexture),
            K_Wings => Flags.Get(Wings),
            K_ShellSpikes => Flags.Get(ShellSpikes),
            K_BodySizeDependantWhisker => Flags.Get(BodySizeDependantWhisker),
            K_SmallTube => Flags.Get(SmallTube),
            K_SlowLegs => Flags.Get(SlowLegs),
            K_KillScoreHidden => Flags.Get(KillScoreHidden),
            K_CountsAsAKill => Flags.Get(CountsAsAKill),
            K_WaterOnly => Flags.Get(WaterOnly),
            K_IgnoresCommunity => Flags.Get(IgnoresCommunity),
            K_TriesToStayInRoom => Flags.Get(TriesToStayInRoom),
            K_EasyKill => Flags.Get(EasyKill),
            K_UsesUnlockData => Flags.Get(UsesUnlockData),
            K_EdibleByOmnivores => Flags.Get(EdibleByOmnivores),
            K_WormGrassImmune => Flags.Get(WormGrassImmune),
            K_ForbidStandardShortcutEntry => Flags.Get(ForbidStandardShortcutEntry),
            K_BlizzardWanderer => Flags.Get(BlizzardWanderer),
            K_NoViolenceStun => Flags.Get(NoViolenceStun),
            K_MajorCreature => Flags.Get(MajorCreature),
            K_TooBigForShelter => Flags.Get(TooBigForShelter),
            K_WantsToShock => Flags.Get(WantsToShock),
            K_SporeCloudImmune => Flags2.Get(SporeCloudImmune),
            K_DaddyCorruptionImmune => Flags2.Get(DaddyCorruptionImmune),
            K_DoesNotUseDens => Flags2.Get(DoesNotUseDens),
            K_CannotBeHitByWeapons => Flags2.Get(CannotBeHitByWeapons),
            K_SandstormImmune => Flags2.Get(SandstormImmune),
            K_CannotBeBlinded => Flags2.Get(CannotBeBlinded),
            K_CannotBeDeafened => Flags2.Get(CannotBeDeafened),
            _ => false//throw new ArgumentException("Invalid key: " + s),
        };
    }

    public void Set(Configurable<string> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        var val = config.Value;
        switch (strval)
        {
            case K_Throwable:
                if (string.IsNullOrEmpty(val))
                {
                    Flags2.Set(Throwable, false);
                    Flags2.Set(Throwable_HasValue, false);
                }
                else if (val == "No")
                {
                    Flags2.Set(Throwable, false);
                    Flags2.Set(Throwable_HasValue, true);
                }
                else
                {
                    Flags2.Set(Throwable, true);
                    Flags2.Set(Throwable_HasValue, true);
                }
                break;
            case K_NightOnly:
                if (string.IsNullOrEmpty(val))
                {
                    Flags2.Set(NightOnly, false);
                    Flags2.Set(NightOnly_HasValue, false);
                }
                else if (val == "No")
                {
                    Flags2.Set(NightOnly, false);
                    Flags2.Set(NightOnly_HasValue, true);
                }
                else
                {
                    Flags2.Set(NightOnly, true);
                    Flags2.Set(NightOnly_HasValue, true);
                }
                break;
            case K_IgnoresCycle:
                if (string.IsNullOrEmpty(val))
                {
                    Flags2.Set(IgnoresCycle, false);
                    Flags2.Set(IgnoresCycle_HasValue, false);
                }
                else if (val == "No")
                {
                    Flags2.Set(IgnoresCycle, false);
                    Flags2.Set(IgnoresCycle_HasValue, true);
                }
                else
                {
                    Flags2.Set(IgnoresCycle, true);
                    Flags2.Set(IgnoresCycle_HasValue, true);
                }
                break;
            case K_PreCycle:
                if (string.IsNullOrEmpty(val))
                {
                    Flags2.Set(PreCycle, false);
                    Flags2.Set(PreCycle_HasValue, false);
                }
                else if (val == "No")
                {
                    Flags2.Set(PreCycle, false);
                    Flags2.Set(PreCycle_HasValue, true);
                }
                else
                {
                    Flags2.Set(PreCycle, true);
                    Flags2.Set(PreCycle_HasValue, true);
                }
                break;
            case K_LavaImmune:
                if (string.IsNullOrEmpty(val))
                {
                    Flags2.Set(LavaImmune, false);
                    Flags2.Set(LavaImmune_HasValue, false);
                }
                else if (val == "No")
                {
                    Flags2.Set(LavaImmune, false);
                    Flags2.Set(LavaImmune_HasValue, true);
                }
                else
                {
                    Flags2.Set(LavaImmune, true);
                    Flags2.Set(LavaImmune_HasValue, true);
                }
                break;
            case K_TentacleImmune:
                if (string.IsNullOrEmpty(val))
                {
                    Flags2.Set(TentacleImmune, false);
                    Flags2.Set(TentacleImmune_HasValue, false);
                }
                else if (val == "No")
                {
                    Flags2.Set(TentacleImmune, false);
                    Flags2.Set(TentacleImmune_HasValue, true);
                }
                else
                {
                    Flags2.Set(TentacleImmune, true);
                    Flags2.Set(TentacleImmune_HasValue, true);
                }
                break;
            case K_HypothermiaImmune:
                if (string.IsNullOrEmpty(val))
                {
                    Flags2.Set(HypothermiaImmune, false);
                    Flags2.Set(HypothermiaImmune_HasValue, false);
                    _template.BlizzardAdapted = false;
                }
                else if (val == "No")
                {
                    Flags2.Set(HypothermiaImmune, false);
                    Flags2.Set(HypothermiaImmune_HasValue, true);
                    _template.BlizzardAdapted = false;
                }
                else
                {
                    Flags2.Set(HypothermiaImmune, true);
                    Flags2.Set(HypothermiaImmune_HasValue, true);
                    _template.BlizzardAdapted = true;
                }
                break;
            case K_BodyChunkRadType:
                BodyChunkRadType = ParseEnum<ChunkRadType>(val);
                break;
            case K_HeadVelocityType:
                HeadVelocityType = ParseEnum<HeadMoveType>(val);
                break;
            case K_BodySizeGenerationType:
                BodySizeGenerationType = ParseEnum<SizeGenerationType>(val);
                break;
            case K_ShockType:
                ShockType = ParseEnum<ShockDamageType>(val);
                break;
            case K_ResistanceType:
                ResistanceType = ParseEnum<ViolenceResistanceType>(val);
                break;
            case K_AllowedIdleTypes:
                AllowedIdleTypes = ParseEnum<IdleScoreTypes>(val);
                break;
            case K_DynamicRelationshipType:
                DynamicRelationshipType = ParseEnum<RelationshipChangeType>(val);
                break;
            case K_VisualScoreTypes:
                VisualScoreTypes = ParseEnum<VisualScoreChangeTypes>(val);
                break;
            case K_PreyTrackType:
                PreyTrackType = ParseEnum<PreyTrackerType>(val);
                break;
            case K_ExcitementType:
                ExcitementType = ParseEnum<ExcitementTrackerType>(val);
                break;
            case K_WingSizeType:
                WingSizeType = ParseEnum<WingType>(val);
                break;
            case K_LightFlashNoiseType:
                LightFlashNoiseType = ParseEnum<FlashColorNoiseType>(val);
                break;
            case K_BodySegmentType:
                BodySegmentType = ParseEnum<SegmentType>(val);
                break;
            case K_Grabability:
                if (string.IsNullOrEmpty(val))
                {
                    Grabability = default;
                    Flags2.Set(Grabability_HasValue, false);
                }
                else
                {
                    Grabability = ParseEnum<Player.ObjectGrabability>(val);
                    Flags2.Set(Grabability_HasValue, true);
                }
                break;
            case K_MainColorType:
                MainColorType = ParseEnum<PaletteColorType>(val);
                break;
            case K_SegmentSprite:
                SegmentSprite = val;
                break;
            case K_AdditionalShellTextureShader:
                AdditionalShellTextureShader = val;
                break;
            case K_LegASprite:
                LegASprite = val;
                break;
            case K_LegBSprite:
                LegBSprite = val;
                break;
            case K_WingSprite:
                WingSprite = val;
                break;
            case K_WingShader:
                WingShader = val;
                break;
            case K_ShellSpikeSprite1:
                ShellSpikeSprite1 = val;
                break;
            case K_ShellSpikeSprite2:
                ShellSpikeSprite2 = val;
                break;
            case K_BellyShellSprite:
                BellyShellSprite = val;
                break;
            case K_BackShellSprite:
                BackShellSprite = val;
                break;
            case K_StandardIconSprite:
                StandardIconSprite = val;
                break;
            case K_BigIconSprite:
                BigIconSprite = val;
                break;
            case K_SmallIconSprite:
                SmallIconSprite = val;
                break;
            case K_DevName:
                DevName = val;
                break;
            case K_SmallUnlockID:///
                SmallUnlockID = string.IsNullOrEmpty(val) ? null : new(val);
                break;
            case K_UnlockID:///
                UnlockID = string.IsNullOrEmpty(val) ? null : new(val);
                break;
            case K_BigUnlockID:///
                BigUnlockID = string.IsNullOrEmpty(val) ? null : new(val);
                break;
            case K_SmallParentUnlock:///
                SmallParentUnlock = string.IsNullOrEmpty(val) ? null : new(val);
                break;
            case K_ParentUnlock:///
                ParentUnlock = string.IsNullOrEmpty(val) ? null : new(val);
                break;
            case K_BigParentUnlock:///
                BigParentUnlock = string.IsNullOrEmpty(val) ? null : new(val);
                break;
            case K_Inherit:
                ParentType = string.IsNullOrEmpty(val) ? null : new(val);
                var temp = _template;
                if (ParentType is not CreatureTemplate.Type tp)
                {
                    _nonMSCAqua = false;
                    temp.ancestor = null;
                }
                else if (tp.value == nameof(DLCSharedEnums.CreatureTemplateType.AquaCenti))
                {
                    _nonMSCAqua = true;
                    temp.ancestor = StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Centipede);
                }
                else if (tp.Index == -1)
                {
                    _nonMSCAqua = false;
                    temp.ancestor = null;
                }
                else
                {
                    _nonMSCAqua = false;
                    temp.ancestor = StaticWorld.GetCreatureTemplate(tp);
                }
                _critob.ResetRelationships(temp.ancestor);
                _critob.SetCustomRelationships();
                break;
            case K_Path:
                _tempPath = val;
                break;
            /*default:
                throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetConfig(Configurable<string> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        switch (strval)
        {
            case K_Throwable:
                if (Flags2.Get(Throwable_HasValue))
                    config.Value = Flags2.Get(Throwable) ? "Yes" : "No";
                else
                    config.Value = string.Empty;
                break;
            case K_NightOnly:
                if (Flags2.Get(NightOnly_HasValue))
                    config.Value = Flags2.Get(NightOnly) ? "Yes" : "No";
                else
                    config.Value = string.Empty;
                break;
            case K_IgnoresCycle:
                if (Flags2.Get(IgnoresCycle_HasValue))
                    config.Value = Flags2.Get(IgnoresCycle) ? "Yes" : "No";
                else
                    config.Value = string.Empty;
                break;
            case K_PreCycle:
                if (Flags2.Get(PreCycle_HasValue))
                    config.Value = Flags2.Get(PreCycle) ? "Yes" : "No";
                else
                    config.Value = string.Empty;
                break;
            case K_LavaImmune:
                if (Flags2.Get(LavaImmune_HasValue))
                    config.Value = Flags2.Get(LavaImmune) ? "Yes" : "No";
                else
                    config.Value = string.Empty;
                break;
            case K_TentacleImmune:
                if (Flags2.Get(TentacleImmune_HasValue))
                    config.Value = Flags2.Get(TentacleImmune) ? "Yes" : "No";
                else
                    config.Value = string.Empty;
                break;
            case K_HypothermiaImmune:
                if (Flags2.Get(HypothermiaImmune_HasValue))
                    config.Value = Flags2.Get(HypothermiaImmune) ? "Yes" : "No";
                else
                    config.Value = string.Empty;
                break;
            case K_BodyChunkRadType:
                config.Value = BodyChunkRadType.ToString();
                break;
            case K_HeadVelocityType:
                config.Value = HeadVelocityType.ToString();
                break;
            case K_BodySizeGenerationType:
                config.Value = BodySizeGenerationType.ToString();
                break;
            case K_ShockType:
                config.Value = ShockType.ToString();
                break;
            case K_ResistanceType:
                config.Value = ResistanceType.ToString();
                break;
            case K_AllowedIdleTypes:
                config.Value = AllowedIdleTypes.ToString();
                break;
            case K_DynamicRelationshipType:
                config.Value = DynamicRelationshipType.ToString();
                break;
            case K_VisualScoreTypes:
                config.Value = VisualScoreTypes.ToString();
                break;
            case K_PreyTrackType:
                config.Value = PreyTrackType.ToString();
                break;
            case K_ExcitementType:
                config.Value = ExcitementType.ToString();
                break;
            case K_WingSizeType:
                config.Value = WingSizeType.ToString();
                break;
            case K_LightFlashNoiseType:
                config.Value = LightFlashNoiseType.ToString();
                break;
            case K_BodySegmentType:
                config.Value = BodySegmentType.ToString();
                break;
            case K_Grabability:
                if (Flags2.Get(Grabability_HasValue))
                    config.Value = Grabability.ToString();
                else
                    config.Value = string.Empty;
                break;
            case K_MainColorType:
                config.Value = MainColorType.ToString();
                break;
            case K_SegmentSprite:
                config.Value = SegmentSprite ?? string.Empty;
                break;
            case K_AdditionalShellTextureShader:
                config.Value = AdditionalShellTextureShader ?? string.Empty;
                break;
            case K_LegASprite:
                config.Value = LegASprite ?? string.Empty;
                break;
            case K_LegBSprite:
                config.Value = LegBSprite ?? string.Empty;
                break;
            case K_WingSprite:
                config.Value = WingSprite ?? string.Empty;
                break;
            case K_WingShader:
                config.Value = WingShader ?? string.Empty;
                break;
            case K_ShellSpikeSprite1:
                config.Value = ShellSpikeSprite1 ?? string.Empty;
                break;
            case K_ShellSpikeSprite2:
                config.Value = ShellSpikeSprite2 ?? string.Empty;
                break;
            case K_BellyShellSprite:
                config.Value = BellyShellSprite ?? string.Empty;
                break;
            case K_BackShellSprite:
                config.Value = BackShellSprite ?? string.Empty;
                break;
            case K_StandardIconSprite:
                config.Value = StandardIconSprite ?? string.Empty;
                break;
            case K_BigIconSprite:
                config.Value = BigIconSprite ?? string.Empty;
                break;
            case K_SmallIconSprite:
                config.Value = SmallIconSprite ?? string.Empty;
                break;
            case K_DevName:
                config.Value = DevName ?? string.Empty;
                break;
            case K_SmallUnlockID:
                config.Value = SmallUnlockID?.value ?? string.Empty;
                break;
            case K_UnlockID:
                config.Value = UnlockID?.value ?? string.Empty;
                break;
            case K_BigUnlockID:
                config.Value = BigUnlockID?.value ?? string.Empty;
                break;
            case K_SmallParentUnlock:
                config.Value = SmallParentUnlock?.value ?? string.Empty;
                break;
            case K_ParentUnlock:
                config.Value = ParentUnlock?.value ?? string.Empty;
                break;
            case K_BigParentUnlock:
                config.Value = BigParentUnlock?.value ?? string.Empty;
                break;
            case K_Inherit:
                config.Value = ParentType?.value ?? string.Empty;
                break;
            case K_Path:
                config.Value = _tempPath ?? string.Empty;
                break;
            /*default:
                throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetColorType(Configurable<string> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s.Replace("Type", string.Empty), out var strval))
            return;
        var val = config.Value;
        switch (strval)
        {
            case K_BodyBlackColor:
                BodyBlackColorType = ParseEnum<PaletteColorType>(val);
                break;
            case K_HeadGlowMinColor:
                HeadGlowMinColorType = ParseEnum<PaletteColorType>(val);
                break;
            case K_HeadGlowMaxColor:
                HeadGlowMaxColorType = ParseEnum<PaletteColorType>(val);
                break;
            case K_LightFlashColor:
                LightFlashColorType = ParseEnum<PaletteColorType>(val);
                break;
            case K_WingColor:
                WingColorType = ParseEnum<PaletteColorType>(val);
                break;
            case K_ShockColor:
                ShockColorType = ParseEnum<PaletteColorType>(val);
                break;
            case K_ShellColor:
                if (string.IsNullOrEmpty(val))
                {
                    ShellColorType = default;
                    Flags2.Set(ShellColorType_HasValue, false);
                }
                else
                {
                    ShellColorType = ParseEnum<PaletteColorType>(val);
                    Flags2.Set(ShellColorType_HasValue, true);
                }
                break;
            case K_SecondaryShellColor:
                if (string.IsNullOrEmpty(val))
                {
                    SecondaryShellColorType = default;
                    Flags2.Set(SecondaryShellColorType_HasValue, false);
                }
                else
                {
                    SecondaryShellColorType = ParseEnum<PaletteColorType>(val);
                    Flags2.Set(SecondaryShellColorType_HasValue, true);
                }
                break;
            /*default:
                throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetColorTypeConfig(Configurable<string> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s.Replace("Type", string.Empty), out var strval))
            return;
        switch (strval)
        {
            case K_BodyBlackColor:
                config.Value = BodyBlackColorType.ToString();
                break;
            case K_HeadGlowMinColor:
                config.Value = HeadGlowMinColorType.ToString();
                break;
            case K_HeadGlowMaxColor:
                config.Value = HeadGlowMaxColorType.ToString();
                break;
            case K_LightFlashColor:
                config.Value = LightFlashColorType.ToString();
                break;
            case K_WingColor:
                config.Value = WingColorType.ToString();
                break;
            case K_ShockColor:
                config.Value = ShockColorType.ToString();
                break;
            case K_ShellColor:
                if (Flags2.Get(ShellColorType_HasValue))
                    config.Value = ShellColorType.ToString();
                else
                    config.Value = string.Empty;
                break;
            case K_SecondaryShellColor:
                if (Flags2.Get(SecondaryShellColorType_HasValue))
                    config.Value = SecondaryShellColorType.ToString();
                else
                    config.Value = string.Empty;
                break;
            /*default:
                throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetAlpha(Configurable<float> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s.Replace(s_Alpha, string.Empty), out var strval))
            return;
        var val = config.Value;
        switch (strval)
        {
            case K_BodyBlackColor:
                BodyBlackColor.a = val;
                break;
            case K_HeadGlowMinColor:
                HeadGlowMinColor.a = val;
                break;
            case K_HeadGlowMaxColor:
                HeadGlowMaxColor.a = val;
                break;
            case K_LightFlashColor:
                LightFlashColor.a = val;
                break;
            case K_WingColor:
                WingColor.a = val;
                break;
            case K_ShockColor:
                ShockColor.a = val;
                break;
            case K_ShellColor:
                ShellColor.a = val;
                break;
            case K_SecondaryShellColor:
                SecondaryShellColor.a = val;
                break;
            case K_ShortCutColor:
                ShortCutColor.a = val;
                _template.shortcutColor.a = val;
                break;
            case K_StandardIconColor:
                StandardIconColor.a = val;
                break;
            case K_BigIconColor:
                BigIconColor.a = val;
                break;
            case K_SmallIconColor:
                SmallIconColor.a = val;
                break;
            case K_DevColor:
                DevColor.a = val;
                break;
            /*default:
                throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetAlphaConfig(Configurable<float> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s.Replace(s_Alpha, string.Empty), out var strval))
            return;
        config.Value = strval switch
        {
            K_BodyBlackColor => BodyBlackColor.a,
            K_HeadGlowMinColor => HeadGlowMinColor.a,
            K_HeadGlowMaxColor => HeadGlowMaxColor.a,
            K_LightFlashColor => LightFlashColor.a,
            K_WingColor => WingColor.a,
            K_ShockColor => ShockColor.a,
            K_ShellColor => ShellColor.a,
            K_SecondaryShellColor => SecondaryShellColor.a,
            K_ShortCutColor => ShortCutColor.a,
            K_StandardIconColor => StandardIconColor.a,
            K_BigIconColor => BigIconColor.a,
            K_SmallIconColor => SmallIconColor.a,
            K_DevColor => DevColor.a,
            _ => 0f//throw new ArgumentException("Invalid key: " + s),
        };
    }

    public void Set(Configurable<Color> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        var val = config.Value;
        CreatureTemplate tpl;
        switch (strval)
        {
            case K_BodyBlackColor:
                BodyBlackColor.r = val.r;
                BodyBlackColor.g = val.g;
                BodyBlackColor.b = val.b;
                break;
            case K_HeadGlowMinColor:
                HeadGlowMinColor.r = val.r;
                HeadGlowMinColor.g = val.g;
                HeadGlowMinColor.b = val.b;
                break;
            case K_HeadGlowMaxColor:
                HeadGlowMaxColor.r = val.r;
                HeadGlowMaxColor.g = val.g;
                HeadGlowMaxColor.b = val.b;
                break;
            case K_LightFlashColor:
                LightFlashColor.r = val.r;
                LightFlashColor.g = val.g;
                LightFlashColor.b = val.b;
                break;
            case K_WingColor:
                WingColor.r = val.r;
                WingColor.g = val.g;
                WingColor.b = val.b;
                break;
            case K_ShockColor:
                ShockColor.r = val.r;
                ShockColor.g = val.g;
                ShockColor.b = val.b;
                break;
            case K_ShellColor:
                ShellColor.r = val.r;
                ShellColor.g = val.g;
                ShellColor.b = val.b;
                break;
            case K_SecondaryShellColor:
                SecondaryShellColor.r = val.r;
                SecondaryShellColor.g = val.g;
                SecondaryShellColor.b = val.b;
                break;
            case K_ShortCutColor:
                tpl = _template;
                ShortCutColor.r = val.r;
                ShortCutColor.g = val.g;
                ShortCutColor.b = val.b;
                tpl.shortcutColor.r = val.r;
                tpl.shortcutColor.g = val.g;
                tpl.shortcutColor.b = val.b;
                break;
            case K_StandardIconColor:
                StandardIconColor.r = val.r;
                StandardIconColor.g = val.g;
                StandardIconColor.b = val.b;
                break;
            case K_BigIconColor:
                BigIconColor.r = val.r;
                BigIconColor.g = val.g;
                BigIconColor.b = val.b;
                break;
            case K_SmallIconColor:
                SmallIconColor.r = val.r;
                SmallIconColor.g = val.g;
                SmallIconColor.b = val.b;
                break;
            case K_DevColor:
                DevColor.r = val.r;
                DevColor.g = val.g;
                DevColor.b = val.b;
                break;
            /*default:
                throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetConfig(Configurable<Color> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        config.Value = strval switch
        {
            K_BodyBlackColor => BodyBlackColor,
            K_HeadGlowMinColor => HeadGlowMinColor,
            K_HeadGlowMaxColor => HeadGlowMaxColor,
            K_LightFlashColor => LightFlashColor,
            K_WingColor => WingColor,
            K_ShockColor => ShockColor,
            K_ShellColor => ShellColor,
            K_SecondaryShellColor => SecondaryShellColor,
            K_ShortCutColor => ShortCutColor,
            K_StandardIconColor => StandardIconColor,
            K_BigIconColor => BigIconColor,
            K_SmallIconColor => SmallIconColor,
            K_DevColor => DevColor,
            _ => default//throw new ArgumentException("Invalid key: " + s),
        }
        with { a = 1f };
    }

    public void Set(Configurable<float> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        var val = config.Value;
        switch (strval)
        {
            case K_BodyChunkRadBonus:
                BodyChunkRadBonus = val;
                break;
            case K_BodyChunkMassBonus:
                BodyChunkMassBonus = val;
                break;
            case K_ConnectionElasticityReduction:
                ConnectionElasticityReduction = val;
                break;
            case K_VelocityFactor:
                VelocityFactor = val;
                break;
            case K_HeadVelocityFactor:
                HeadVelocityFactor = val;
                break;
            case K_MaxSize:
                MaxSize = val;
                break;
            case K_MinSize:
                MinSize = val;
                break;
            case K_PreyTrackerWeight:
                PreyTrackerWeight = val;
                break;
            case K_WingLengthFactor:
                WingLengthFactor = val;
                break;
            case K_LegLengthFactor:
                LegLengthFactor = val;
                break;
            case K_WhiskerLengthFactor:
                WhiskerLengthFactor = val;
                break;
            case K_SmallWhiskerLength:
                SmallWhiskerLength = val;
                break;
            case K_BigWhiskerLength:
                BigWhiskerLength = val;
                break;
            case K_LegScaleXFactor:
                LegScaleXFactor = val;
                break;
            case K_ShellScaleYFactor:
                ShellScaleYFactor = val;
                break;
            case K_WhiskerShapeFactor:
                WhiskerShapeFactor = val;
                break;
            case K_HueMax:
                HueMax = val;
                break;
            case K_HueMin:
                HueMin = val;
                break;
            case K_SaturationMax:
                SaturationMax = val;
                break;
            case K_SaturationMin:
                SaturationMin = val;
                break;
            case K_LinearSandboxCost:
                LinearSandboxCost = val;
                _critob.SandboxPerformanceCost = new(val, _critob.SandboxPerformanceCost.Exponential);
                break;
            case K_ExponentialSandboxCost:
                ExponentialSandboxCost = val;
                _critob.SandboxPerformanceCost = new(_critob.SandboxPerformanceCost.Linear, val);
                break;
            case K_RoomPerformanceCost:
                RoomPerformanceCost = val;
                _critob.LoadedPerformanceCost = val;
                break;
            case K_BaseDamageResistance:
                BaseDamageResistance = val;
                _template.baseDamageResistance = val;
                break;
            case K_ExplosionResistance:
                ExplosionResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Explosion, 0] = val;
                break;
            case K_BodySizeEstimate:
                BodySizeEstimate = val;
                _template.bodySize = val;
                break;
            case K_VisualRadius:
                VisualRadius = val;
                _template.visualRadius = val;
                break;
            case K_WaterVision:
                WaterVision = val;
                _template.waterVision = val;
                break;
            case K_ThroughSurfaceVision:
                ThroughSurfaceVision = val;
                _template.throughSurfaceVision = val;
                break;
            case K_MovementBasedVision:
                MovementBasedVision = val;
                _template.movementBasedVision = val;
                break;
            case K_DangerousToPlayer:
                DangerousToPlayer = val;
                _template.dangerousToPlayer = val;
                break;
            case K_LungCapacity:
                LungCapacity = val;
                _template.lungCapacity = val;
                break;
            case K_CommunityInfluence:
                CommunityInfluence = val;
                _template.communityInfluence = val;
                break;
            case K_MeatMin:
                MeatMin = val;
                break;
            case K_MeatMax:
                MeatMax = val;
                break;
            case K_Scaryness:
                Scaryness = val;
                _template.scaryness = val;
                break;
            case K_MinBuoyancy:
                MinBuoyancy = val;
                break;
            case K_MaxBuoyancy:
                MaxBuoyancy = val;
                break;
            case K_SureToGetPreyDistance:
                SureToGetPreyDistance = val;
                break;
            case K_HeadGlobalVelocityFactor:
                HeadGlobalVelocityFactor = val;
                break;
            case K_BaseStunResistance:
                BaseStunResistance = val;
                _template.baseStunResistance = val;
                break;
            case K_ExplosionStunResistance:
                ExplosionStunResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Explosion, 1] = val;
                break;
            case K_SpikeScaleFactor:
                SpikeScaleFactor = val;
                break;
            case K_ShockResistanceReductionFactor:
                ShockResistanceReductionFactor = val;
                break;
            case K_SecondaryShellColorBonus:
                SecondaryShellColorBonus = val;
                break;
            case K_GlobalVelocityFactor:
                GlobalVelocityFactor = val;
                break;
            case K_GlobalFlyingVelocityFactor:
                GlobalFlyingVelocityFactor = val;
                break;
            case K_DamageReductionFactor:
                DamageReductionFactor = val;
                break;
            case K_DeadShellChance:
                DeadShellChance = val;
                break;
            case K_GrabbedShockChargeReduction:
                GrabbedShockChargeReduction = val;
                break;
            case K_WaterPathingResistance:
                WaterPathingResistance = val;
                _template.waterPathingResistance = val;
                break;
            case K_BluntDamageResistance:
                BluntDamageResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Blunt, 0] = val;
                break;
            case K_BluntStunResistance:
                BluntStunResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Blunt, 1] = val;
                break;
            case K_WaterDamageResistance:
                WaterDamageResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Water, 0] = val;
                break;
            case K_WaterStunResistance:
                WaterStunResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Water, 1] = val;
                break;
            case K_StabDamageResistance:
                StabDamageResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Stab, 0] = val;
                break;
            case K_StabStunResistance:
                StabStunResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Stab, 1] = val;
                break;
            case K_BiteDamageResistance:
                BiteDamageResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Bite, 0] = val;
                break;
            case K_BiteStunResistance:
                BiteStunResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Bite, 1] = val;
                break;
            case K_ElectricDamageResistance:
                ElectricDamageResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Electric, 0] = val;
                break;
            case K_ElectricStunResistance:
                ElectricStunResistance = val;
                _template.damageRestistances[(int)Creature.DamageType.Electric, 1] = val;
                break;
            case K_OffScreenSpeed:
                OffScreenSpeed = val;
                _template.offScreenSpeed = val;
                break;
            case K_SurfaceFriction:
                SurfaceFriction = val;
                break;
            case K_Bounce:
                Bounce = val;
                break;
            case K_WaterRetardationImmunity:
                WaterRetardationImmunity = val;
                break;
            case K_WaterFriction:
                WaterFriction = val;
                break;
            case K_AirFriction:
                AirFriction = val;
                break;
            case K_ImpactThreshold:
                ImpactThreshold = val;
                break;
                /*default:
                    throw new ArgumentException("Invalid key: " + s);*/
        }
    }

    public void SetConfig(Configurable<float> config)
    {
        if (config?.key is not string s || !s_fieldHashDict.TryGetValue(s, out var strval))
            return;
        config.Value = strval switch
        {
            K_BodyChunkRadBonus => BodyChunkRadBonus,
            K_BodyChunkMassBonus => BodyChunkMassBonus,
            K_ConnectionElasticityReduction => ConnectionElasticityReduction,
            K_VelocityFactor => VelocityFactor,
            K_HeadVelocityFactor => HeadVelocityFactor,
            K_MaxSize => MaxSize,
            K_MinSize => MinSize,
            K_PreyTrackerWeight => PreyTrackerWeight,
            K_WingLengthFactor => WingLengthFactor,
            K_LegLengthFactor => LegLengthFactor,
            K_WhiskerLengthFactor => WhiskerLengthFactor,
            K_SmallWhiskerLength => SmallWhiskerLength,
            K_BigWhiskerLength => BigWhiskerLength,
            K_LegScaleXFactor => LegScaleXFactor,
            K_ShellScaleYFactor => ShellScaleYFactor,
            K_WhiskerShapeFactor => WhiskerShapeFactor,
            K_HueMax => HueMax,
            K_HueMin => HueMin,
            K_SaturationMax => SaturationMax,
            K_SaturationMin => SaturationMin,
            K_LinearSandboxCost => LinearSandboxCost,
            K_ExponentialSandboxCost => ExponentialSandboxCost,
            K_RoomPerformanceCost => RoomPerformanceCost,
            K_BaseDamageResistance => BaseDamageResistance,
            K_ExplosionResistance => ExplosionResistance,
            K_BodySizeEstimate => BodySizeEstimate,
            K_VisualRadius => VisualRadius,
            K_WaterVision => WaterVision,
            K_ThroughSurfaceVision => ThroughSurfaceVision,
            K_MovementBasedVision => MovementBasedVision,
            K_DangerousToPlayer => DangerousToPlayer,
            K_LungCapacity => LungCapacity,
            K_CommunityInfluence => CommunityInfluence,
            K_MeatMin => MeatMin,
            K_MeatMax => MeatMax,
            K_Scaryness => Scaryness,
            K_MinBuoyancy => MinBuoyancy,
            K_MaxBuoyancy => MaxBuoyancy,
            K_SureToGetPreyDistance => SureToGetPreyDistance,
            K_HeadGlobalVelocityFactor => HeadGlobalVelocityFactor,
            K_BaseStunResistance => BaseStunResistance,
            K_ExplosionStunResistance => ExplosionStunResistance,
            K_SpikeScaleFactor => SpikeScaleFactor,
            K_ShockResistanceReductionFactor => ShockResistanceReductionFactor,
            K_SecondaryShellColorBonus => SecondaryShellColorBonus,
            K_GlobalVelocityFactor => GlobalVelocityFactor,
            K_GlobalFlyingVelocityFactor => GlobalFlyingVelocityFactor,
            K_DamageReductionFactor => DamageReductionFactor,
            K_DeadShellChance => DeadShellChance,
            K_GrabbedShockChargeReduction => GrabbedShockChargeReduction,
            K_WaterPathingResistance => WaterPathingResistance,
            K_BluntDamageResistance => BluntDamageResistance,
            K_BluntStunResistance => BluntStunResistance,
            K_WaterDamageResistance => WaterDamageResistance,
            K_WaterStunResistance => WaterStunResistance,
            K_StabDamageResistance => StabDamageResistance,
            K_StabStunResistance => StabStunResistance,
            K_BiteDamageResistance => BiteDamageResistance,
            K_BiteStunResistance => BiteStunResistance,
            K_ElectricDamageResistance => ElectricDamageResistance,
            K_ElectricStunResistance => ElectricStunResistance,
            K_OffScreenSpeed => OffScreenSpeed,
            K_SurfaceFriction => SurfaceFriction,
            K_Bounce => Bounce,
            K_WaterRetardationImmunity => WaterRetardationImmunity,
            K_WaterFriction => WaterFriction,
            K_AirFriction => AirFriction,
            K_ImpactThreshold => ImpactThreshold,
            _ => 0f//throw new ArgumentException("Invalid key: " + s),
        };
    }

    public static bool RequiresReloading(string fieldName)
    {
        return s_fieldHashDict.TryGetValue(fieldName, out var strval) && strval is K_KillScoreHidden
            or K_UsesUnlockData
            or K_StandardKillScore
            or K_BigKillScore
            or K_SmallKillScore
            //or K_Inherit
            or K_UnlockID
            or K_ParentUnlock
            or K_SmallParentUnlock
            or K_BigParentUnlock
            or K_SmallUnlockID
            or K_BigUnlockID
            or K_ExpeditionScore
            or K_BodySizeGenerationType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteFile()
    {
        if (File.Exists(Path))
            File.Delete(Path);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SaveFile()
    {
        var strm = new StreamWriter(Path, false);
        strm.Write(ToString());
        strm.Close();
    }

    public static Color GetColorFromType(PaletteColorType type, in RoomPalette pal, in Color def) => type switch
    {
        PaletteColorType.BlackColor => pal.blackColor,
        PaletteColorType.WaterColor1 => pal.waterColor1,
        PaletteColorType.WaterColor2 => pal.waterColor2,
        PaletteColorType.WaterSurfaceColor1 => pal.waterSurfaceColor1,
        PaletteColorType.WaterSurfaceColor2 => pal.waterSurfaceColor2,
        PaletteColorType.WaterShineColor => pal.waterShineColor,
        PaletteColorType.FogColor => pal.fogColor,
        PaletteColorType.ShortCutSymbol => pal.shortCutSymbol,
        PaletteColorType.SkyColor => pal.skyColor,
        PaletteColorType.ShortcutColor1 => pal.shortcutColors[1],
        PaletteColorType.ShortcutColor2 => pal.shortcutColors[2],
        PaletteColorType.ShortcutColor3 => pal.shortcutColors[3],
        _ => Clamp01(def)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FAtlasElement TryGetSprite(string? name, string def)
    {
        var elems = Futile.atlasManager._allElementsByName;
        return name is not null && elems.ContainsKey(name) ? elems[name] : elems[def];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SpriteExists(string? name) => name is not null && Futile.atlasManager._allElementsByName.ContainsKey(name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ShaderExists(string? name) => name is not null && Custom.rainWorld.Shaders.ContainsKey(name);

    public virtual CustomCentiBreedParams Duplicate()
    {
        var res = (CustomCentiBreedParams)MemberwiseClone();
        res.Relationships = Relationships?.Clone() as CustomRelation[];
        return res;
    }

    public ref readonly CustomRelation GetRelation(string name, bool active)
    {
        var rels = Relationships;
        if (rels is not null)
        {
            for (var i = 0; i < rels.Length; i++)
            {
                ref readonly var rel = ref rels[i];
                if (rel.Target?.value == name && rel.ActivePosition == active)
                    return ref rel;
            }
        }
        return ref NullRelation;
    }

    public override string ToString()
    {
        return new StringBuilder()
        .Feed(s_Inherit, ParentType)
        .Feed($"{nameof(Swimming)}", Flags.Get(Swimming))
        .Feed($"{nameof(AutomaticPickUp)}", Flags.Get(AutomaticPickUp))
        .Feed($"{nameof(SmallFood)}", Flags.Get(SmallFood))
        .Feed($"{nameof(ShellParticles)}", Flags.Get(ShellParticles))
        .Feed($"{nameof(Flying)}", Flags.Get(Flying))
        .Feed($"{nameof(DetectsAnnoyingCollisions)}", Flags.Get(DetectsAnnoyingCollisions))
        .Feed($"{nameof(WeakToStun)}", Flags.Get(WeakToStun))
        .Feed($"{nameof(DaddyCorruptionImmune)}", Flags2.Get(DaddyCorruptionImmune))
        .Feed($"{nameof(DoesNotUseDens)}", Flags2.Get(DoesNotUseDens))
        .Feed($"{nameof(CannotBeHitByWeapons)}", Flags2.Get(CannotBeHitByWeapons))
        .Feed($"{nameof(SandstormImmune)}", Flags2.Get(SandstormImmune))
        .Feed($"{nameof(CannotBeBlinded)}", Flags2.Get(CannotBeBlinded))
        .Feed($"{nameof(CannotBeDeafened)}", Flags2.Get(CannotBeDeafened))
        .Feed($"{nameof(GlowingHead)}", Flags.Get(GlowingHead))
        .Feed($"{nameof(ShocksWhenGrabbed)}", Flags.Get(ShocksWhenGrabbed))
        .Feed($"{nameof(Shields)}", Flags.Get(Shields))
        .Feed($"{nameof(NoiseTracker)}", Flags.Get(NoiseTracker))
        .Feed($"{nameof(AdditionalShellTexture)}", Flags.Get(AdditionalShellTexture))
        .Feed($"{nameof(Wings)}", Flags.Get(Wings))
        .Feed($"{nameof(ShellSpikes)}", Flags.Get(ShellSpikes))
        .Feed($"{nameof(BodySizeDependantWhisker)}", Flags.Get(BodySizeDependantWhisker))
        .Feed($"{nameof(SmallTube)}", Flags.Get(SmallTube))
        .Feed($"{nameof(SlowLegs)}", Flags.Get(SlowLegs))
        .Feed($"{nameof(KillScoreHidden)}", Flags.Get(KillScoreHidden))
        .Feed($"{nameof(CountsAsAKill)}", Flags.Get(CountsAsAKill))
        .Feed($"{nameof(WaterOnly)}", Flags.Get(WaterOnly))
        .Feed($"{nameof(IgnoresCommunity)}", Flags.Get(IgnoresCommunity))
        .Feed($"{nameof(TriesToStayInRoom)}", Flags.Get(TriesToStayInRoom))
        .Feed($"{nameof(EasyKill)}", Flags.Get(EasyKill))
        .Feed($"{nameof(UsesUnlockData)}", Flags.Get(UsesUnlockData))
        .Feed($"{nameof(EdibleByOmnivores)}", Flags.Get(EdibleByOmnivores))
        .Feed($"{nameof(WormGrassImmune)}", Flags.Get(WormGrassImmune))
        .Feed($"{nameof(ForbidStandardShortcutEntry)}", Flags.Get(ForbidStandardShortcutEntry))
        .Feed($"{nameof(BlizzardWanderer)}", Flags.Get(BlizzardWanderer))
        .Feed($"{nameof(MajorCreature)}", Flags.Get(MajorCreature))
        .Feed($"{nameof(NoViolenceStun)}", Flags.Get(NoViolenceStun))
        .Feed($"{nameof(TooBigForShelter)}", Flags.Get(TooBigForShelter))
        .Feed($"{nameof(WantsToShock)}", Flags.Get(WantsToShock))
        .Feed($"{nameof(SporeCloudImmune)}", Flags2.Get(SporeCloudImmune))
        .Feed($"{nameof(Throwable)}", Flags2.Get(Throwable), Flags2.Get(Throwable_HasValue))
        .Feed($"{nameof(NightOnly)}", Flags2.Get(NightOnly), Flags2.Get(NightOnly_HasValue))
        .Feed($"{nameof(IgnoresCycle)}", Flags2.Get(IgnoresCycle), Flags2.Get(IgnoresCycle_HasValue))
        .Feed($"{nameof(PreCycle)}", Flags2.Get(PreCycle), Flags2.Get(PreCycle_HasValue))
        .Feed($"{nameof(LavaImmune)}", Flags2.Get(LavaImmune), Flags2.Get(LavaImmune_HasValue))
        .Feed($"{nameof(TentacleImmune)}", Flags2.Get(TentacleImmune), Flags2.Get(TentacleImmune_HasValue))
        .Feed($"{nameof(HypothermiaImmune)}", Flags2.Get(HypothermiaImmune), Flags2.Get(HypothermiaImmune_HasValue))
        .Feed($"{nameof(FoodPoints)}", FoodPoints)
        .Feed($"{nameof(Bites)}", Bites)
        .Feed($"{nameof(MinChunkAmount)}", MinChunkAmount)
        .Feed($"{nameof(MaxChunkAmount)}", MaxChunkAmount)
        .Feed($"{nameof(WingVariations)}", WingVariations)
        .Feed($"{nameof(StandardKillScore)}", StandardKillScore)
        .Feed($"{nameof(AbstractedLaziness)}", AbstractedLaziness)
        .Feed($"{nameof(ShortCutSegments)}", ShortCutSegments)
        .Feed($"{nameof(BigKillScore)}", BigKillScore)
        .Feed($"{nameof(SmallKillScore)}", SmallKillScore)
        .Feed($"{nameof(PatherStepsPerFrame)}", PatherStepsPerFrame)
        .Feed($"{nameof(ExpeditionScore)}", ExpeditionScore)
        .Feed($"{nameof(BodyChunkRadBonus)}", BodyChunkRadBonus)
        .Feed($"{nameof(BodyChunkMassBonus)}", BodyChunkMassBonus)
        .Feed($"{nameof(ConnectionElasticityReduction)}", ConnectionElasticityReduction)
        .Feed($"{nameof(VelocityFactor)}", VelocityFactor)
        .Feed($"{nameof(HeadVelocityFactor)}", HeadVelocityFactor)
        .Feed($"{nameof(MaxSize)}", MaxSize)
        .Feed($"{nameof(MinSize)}", MinSize)
        .Feed($"{nameof(PreyTrackerWeight)}", PreyTrackerWeight)
        .Feed($"{nameof(WingLengthFactor)}", WingLengthFactor)
        .Feed($"{nameof(LegLengthFactor)}", LegLengthFactor)
        .Feed($"{nameof(WhiskerLengthFactor)}", WhiskerLengthFactor)
        .Feed($"{nameof(SmallWhiskerLength)}", SmallWhiskerLength)
        .Feed($"{nameof(BigWhiskerLength)}", BigWhiskerLength)
        .Feed($"{nameof(LegScaleXFactor)}", LegScaleXFactor)
        .Feed($"{nameof(ShellScaleYFactor)}", ShellScaleYFactor)
        .Feed($"{nameof(WhiskerShapeFactor)}", WhiskerShapeFactor)
        .Feed($"{nameof(HueMax)}", HueMax)
        .Feed($"{nameof(HueMin)}", HueMin)
        .Feed($"{nameof(SaturationMax)}", SaturationMax)
        .Feed($"{nameof(SaturationMin)}", SaturationMin)
        .Feed($"{nameof(LinearSandboxCost)}", LinearSandboxCost)
        .Feed($"{nameof(ExponentialSandboxCost)}", ExponentialSandboxCost)
        .Feed($"{nameof(RoomPerformanceCost)}", RoomPerformanceCost)
        .Feed($"{nameof(BaseDamageResistance)}", BaseDamageResistance)
        .Feed($"{nameof(ExplosionResistance)}", ExplosionResistance)
        .Feed($"{nameof(BodySizeEstimate)}", BodySizeEstimate)
        .Feed($"{nameof(VisualRadius)}", VisualRadius)
        .Feed($"{nameof(WaterVision)}", WaterVision)
        .Feed($"{nameof(ThroughSurfaceVision)}", ThroughSurfaceVision)
        .Feed($"{nameof(MovementBasedVision)}", MovementBasedVision)
        .Feed($"{nameof(DangerousToPlayer)}", DangerousToPlayer)
        .Feed($"{nameof(LungCapacity)}", LungCapacity)
        .Feed($"{nameof(CommunityInfluence)}", CommunityInfluence)
        .Feed($"{nameof(MeatMin)}", MeatMin)
        .Feed($"{nameof(MeatMax)}", MeatMax)
        .Feed($"{nameof(Scaryness)}", Scaryness)
        .Feed($"{nameof(MinBuoyancy)}", MinBuoyancy)
        .Feed($"{nameof(MaxBuoyancy)}", MaxBuoyancy)
        .Feed($"{nameof(SureToGetPreyDistance)}", SureToGetPreyDistance)
        .Feed($"{nameof(HeadGlobalVelocityFactor)}", HeadGlobalVelocityFactor)
        .Feed($"{nameof(BaseStunResistance)}", BaseStunResistance)
        .Feed($"{nameof(ExplosionStunResistance)}", ExplosionStunResistance)
        .Feed($"{nameof(SpikeScaleFactor)}", SpikeScaleFactor)
        .Feed($"{nameof(ShockResistanceReductionFactor)}", ShockResistanceReductionFactor)
        .Feed($"{nameof(SecondaryShellColorBonus)}", SecondaryShellColorBonus)
        .Feed($"{nameof(GlobalVelocityFactor)}", GlobalVelocityFactor)
        .Feed($"{nameof(GlobalFlyingVelocityFactor)}", GlobalFlyingVelocityFactor)
        .Feed($"{nameof(DamageReductionFactor)}", DamageReductionFactor)
        .Feed($"{nameof(DeadShellChance)}", DeadShellChance)
        .Feed($"{nameof(GrabbedShockChargeReduction)}", GrabbedShockChargeReduction)
        .Feed($"{nameof(WaterPathingResistance)}", WaterPathingResistance)
        .Feed($"{nameof(BluntDamageResistance)}", BluntDamageResistance)
        .Feed($"{nameof(BluntStunResistance)}", BluntStunResistance)
        .Feed($"{nameof(WaterDamageResistance)}", WaterDamageResistance)
        .Feed($"{nameof(WaterStunResistance)}", WaterStunResistance)
        .Feed($"{nameof(StabDamageResistance)}", StabDamageResistance)
        .Feed($"{nameof(StabStunResistance)}", StabStunResistance)
        .Feed($"{nameof(BiteDamageResistance)}", BiteDamageResistance)
        .Feed($"{nameof(BiteStunResistance)}", BiteStunResistance)
        .Feed($"{nameof(ElectricDamageResistance)}", ElectricDamageResistance)
        .Feed($"{nameof(ElectricStunResistance)}", ElectricStunResistance)
        .Feed($"{nameof(OffScreenSpeed)}", OffScreenSpeed)
        .Feed($"{nameof(SurfaceFriction)}", SurfaceFriction)
        .Feed($"{nameof(Bounce)}", Bounce)
        .Feed($"{nameof(WaterRetardationImmunity)}", WaterRetardationImmunity)
        .Feed($"{nameof(WaterFriction)}", WaterFriction)
        .Feed($"{nameof(AirFriction)}", AirFriction)
        .Feed($"{nameof(ImpactThreshold)}", ImpactThreshold)
        .Feed($"{nameof(BodyChunkRadType)}", BodyChunkRadType)
        .Feed($"{nameof(HeadVelocityType)}", HeadVelocityType)
        .Feed($"{nameof(BodySizeGenerationType)}", BodySizeGenerationType)
        .Feed($"{nameof(ShockType)}", ShockType)
        .Feed($"{nameof(ResistanceType)}", ResistanceType)
        .Feed($"{nameof(AllowedIdleTypes)}", AllowedIdleTypes)
        .Feed($"{nameof(DynamicRelationshipType)}", DynamicRelationshipType)
        .Feed($"{nameof(VisualScoreTypes)}", VisualScoreTypes)
        .Feed($"{nameof(PreyTrackType)}", PreyTrackType)
        .Feed($"{nameof(ExcitementType)}", ExcitementType)
        .Feed($"{nameof(WingSizeType)}", WingSizeType)
        .Feed($"{nameof(MainColorType)}", MainColorType)
        .Feed($"{nameof(LightFlashNoiseType)}", LightFlashNoiseType)
        .Feed($"{nameof(BodySegmentType)}", BodySegmentType)
        .Feed($"{nameof(Grabability)}", Grabability, Flags2.Get(Grabability_HasValue))
        .Feed($"{nameof(ShockColor)}", ShockColorType, ShockColor)
        .Feed($"{nameof(ShortCutColor)}", ShortCutColor)
        .Feed($"{nameof(HeadGlowMinColor)}", HeadGlowMinColorType, HeadGlowMinColor)
        .Feed($"{nameof(HeadGlowMaxColor)}", HeadGlowMaxColorType, HeadGlowMaxColor)
        .Feed($"{nameof(BodyBlackColor)}", BodyBlackColorType, BodyBlackColor)
        .Feed($"{nameof(LightFlashColor)}", LightFlashColorType, LightFlashColor)
        .Feed($"{nameof(WingColor)}", WingColorType, WingColor)
        .Feed($"{nameof(StandardIconColor)}", StandardIconColor)
        .Feed($"{nameof(BigIconColor)}", BigIconColor)
        .Feed($"{nameof(SmallIconColor)}", SmallIconColor)
        .Feed($"{nameof(DevColor)}", DevColor)
        .Feed($"{nameof(ShellColor)}", ShellColorType, ShellColor, Flags2.Get(ShellColorType_HasValue))
        .Feed($"{nameof(SecondaryShellColor)}", SecondaryShellColorType, SecondaryShellColor, Flags2.Get(SecondaryShellColorType_HasValue))
        .Feed($"{nameof(SegmentSprite)}", SegmentSprite)
        .Feed($"{nameof(AdditionalShellTextureShader)}", AdditionalShellTextureShader)
        .Feed($"{nameof(LegASprite)}", LegASprite)
        .Feed($"{nameof(LegBSprite)}", LegBSprite)
        .Feed($"{nameof(WingSprite)}", WingSprite)
        .Feed($"{nameof(WingShader)}", WingShader)
        .Feed($"{nameof(ShellSpikeSprite1)}", ShellSpikeSprite1)
        .Feed($"{nameof(ShellSpikeSprite2)}", ShellSpikeSprite2)
        .Feed($"{nameof(BellyShellSprite)}", BellyShellSprite)
        .Feed($"{nameof(BackShellSprite)}", BackShellSprite)
        .Feed($"{nameof(StandardIconSprite)}", StandardIconSprite)
        .Feed($"{nameof(BigIconSprite)}", BigIconSprite)
        .Feed($"{nameof(SmallIconSprite)}", SmallIconSprite)
        .Feed($"{nameof(DevName)}", DevName)
        .Feed($"{nameof(SmallUnlockID)}", SmallUnlockID)
        .Feed($"{nameof(UnlockID)}", UnlockID)
        .Feed($"{nameof(BigUnlockID)}", BigUnlockID)
        .Feed($"{nameof(SmallParentUnlock)}", SmallParentUnlock)
        .Feed($"{nameof(ParentUnlock)}", ParentUnlock)
        .Feed($"{nameof(BigParentUnlock)}", BigParentUnlock)
        .Feed($"{nameof(Relationships)}", Relationships).ToString();
    }

    internal static string GenerateExample()
    {
        string flagS = "~Y (Yes) / N (No); N (No) by default",
            flagNS = "~Y (Yes) / N (No); nothing by default",
            intS = "~Integer >= 0; 0 by default",
            int1S = "~Integer >= 1; 1 by default",
            int2S = "~Integer >= 2; 2 by default",
            floatS = "~Decimal >= 0; 0 by default",
            floatHS = "~Decimal (+ or -); 0 by default",
            paletteS = "~BLACKCOLOR / WATERCOLOR1 / WATERCOLOR2 / WATERSURFACECOLOR1 / WATERSURFACECOLOR2 / WATERSHINECOLOR / FOGCOLOR / SHORTCUTSYMBOL / SKYCOLOR / SHORTCUTCOLOR1 / SHORTCUTCOLOR2 / SHORTCUTCOLOR3 or HSLA:Decimal (0-1)~Decimal (0-1)~Decimal (0-1)~Decimal (0-1) or RGBA:Decimal (0-1)~Decimal (0-1)~Decimal (0-1)~Decimal (0-1); RGBA:0~0~0~0 by default; IGNORES CASE",
            paletteNS = "~BLACKCOLOR / WATERCOLOR1 / WATERCOLOR2 / WATERSURFACECOLOR1 / WATERSURFACECOLOR2 / WATERSHINECOLOR / FOGCOLOR / SHORTCUTSYMBOL / SKYCOLOR / SHORTCUTCOLOR1 / SHORTCUTCOLOR2 / SHORTCUTCOLOR3 or HSLA:Decimal (0-1)~Decimal (0-1)~Decimal (0-1)~Decimal (0-1) or RGBA:Decimal (0-1)~Decimal (0-1)~Decimal (0-1)~Decimal (0-1); nothing by default; IGNORES CASE",
            colorS = "~HSLA:Decimal (0-1)~Decimal (0-1)~Decimal (0-1)~Decimal (0-1) or RGBA:Decimal (0-1)~Decimal (0-1)~Decimal (0-1)~Decimal (0-1); RGBA:0~0~0~0 by default; IGNORES CASE",
            enumSNRC = "~NORMAL / CENTIWING / RED; NORMAL by default; IGNORES CASE",
            centiSegS = "~Sprite name; CentipedeSegment by default; CASE SENSITIVE",
            pUnlockS = "~MultiplayerUnlocks.SandboxUnlockID name; nothing by default; CASE SENSITIVE",
            unlockS = "~Name; nothing by default; CASE SENSITIVE";
        return new StringBuilder()
        .AppendLine("#The name of this file is the name of your creature. Multiple TP files can exist in the CustomCentis folder. PNG files in the CustomCentis folder will be automatically loaded (atlas text is optional). Remove / Add # to uncomment / comment. Spaces and empty lines are ignored. Decimal numbers use dots as points. Any invalid entry will be ignored. Define at most one field per line. Field names are CASE SENSITIVE.")
        .Tell(s_Inherit, "~SmallCentipede / Centipede / Centiwing / RedCentipede / AquaCenti; nothing by default; CASE SENSITIVE; If the Inherit field is present, it should always be the first line of the file.")
        .Tell($"{nameof(Swimming)}", flagS)
        .Tell($"{nameof(AutomaticPickUp)}", flagS)
        .Tell($"{nameof(SmallFood)}", flagS)
        .Tell($"{nameof(ShellParticles)}", flagS)
        .Tell($"{nameof(Flying)}", flagS)
        .Tell($"{nameof(DetectsAnnoyingCollisions)}", flagS)
        .Tell($"{nameof(WeakToStun)}", flagS)
        .Tell($"{nameof(DaddyCorruptionImmune)}", flagS)
        .Tell($"{nameof(DoesNotUseDens)}", flagS)
        .Tell($"{nameof(CannotBeHitByWeapons)}", flagS)
        .Tell($"{nameof(SandstormImmune)}", flagS)
        .Tell($"{nameof(CannotBeBlinded)}", flagS)
        .Tell($"{nameof(CannotBeDeafened)}", flagS)
        .Tell($"{nameof(GlowingHead)}", flagS)
        .Tell($"{nameof(ShocksWhenGrabbed)}", flagS)
        .Tell($"{nameof(Shields)}", flagS)
        .Tell($"{nameof(NoiseTracker)}", flagS)
        .Tell($"{nameof(AdditionalShellTexture)}", flagS)
        .Tell($"{nameof(Wings)}", flagS)
        .Tell($"{nameof(ShellSpikes)}", flagS)
        .Tell($"{nameof(BodySizeDependantWhisker)}", flagS)
        .Tell($"{nameof(SmallTube)}", flagS)
        .Tell($"{nameof(SlowLegs)}", flagS)
        .Tell($"{nameof(KillScoreHidden)}", flagS)
        .Tell($"{nameof(CountsAsAKill)}", flagS)
        .Tell($"{nameof(WaterOnly)}", flagS)
        .Tell($"{nameof(IgnoresCommunity)}", flagS)
        .Tell($"{nameof(TriesToStayInRoom)}", flagS)
        .Tell($"{nameof(EasyKill)}", flagS)
        .Tell($"{nameof(UsesUnlockData)}", flagS)
        .Tell($"{nameof(EdibleByOmnivores)}", flagS)
        .Tell($"{nameof(WormGrassImmune)}", flagS)
        .Tell($"{nameof(ForbidStandardShortcutEntry)}", flagS)
        .Tell($"{nameof(BlizzardWanderer)}", flagS)
        .Tell($"{nameof(MajorCreature)}", flagS)
        .Tell($"{nameof(NoViolenceStun)}", flagS)
        .Tell($"{nameof(TooBigForShelter)}", flagS)
        .Tell($"{nameof(WantsToShock)}", flagS)
        .Tell($"{nameof(SporeCloudImmune)}", flagS)
        .Tell($"{nameof(Throwable)}", flagNS)
        .Tell($"{nameof(NightOnly)}", flagNS)
        .Tell($"{nameof(IgnoresCycle)}", flagNS)
        .Tell($"{nameof(PreCycle)}", flagNS)
        .Tell($"{nameof(LavaImmune)}", flagNS)
        .Tell($"{nameof(TentacleImmune)}", flagNS)
        .Tell($"{nameof(HypothermiaImmune)}", flagNS)
        .Tell($"{nameof(FoodPoints)}", intS)
        .Tell($"{nameof(Bites)}", int1S)
        .Tell($"{nameof(MinChunkAmount)}", int2S)
        .Tell($"{nameof(MaxChunkAmount)}", int2S)
        .Tell($"{nameof(WingVariations)}", int1S)
        .Tell($"{nameof(StandardKillScore)}", intS)
        .Tell($"{nameof(AbstractedLaziness)}", intS)
        .Tell($"{nameof(ShortCutSegments)}", int1S)
        .Tell($"{nameof(BigKillScore)}", intS)
        .Tell($"{nameof(SmallKillScore)}", intS)
        .Tell($"{nameof(PatherStepsPerFrame)}", intS)
        .Tell($"{nameof(ExpeditionScore)}", intS)
        .Tell($"{nameof(BodyChunkRadBonus)}", floatHS)
        .Tell($"{nameof(BodyChunkMassBonus)}", floatHS)
        .Tell($"{nameof(ConnectionElasticityReduction)}", floatS)
        .Tell($"{nameof(VelocityFactor)}", floatS)
        .Tell($"{nameof(HeadVelocityFactor)}", floatS)
        .Tell($"{nameof(MaxSize)}", floatS)
        .Tell($"{nameof(MinSize)}", floatS)
        .Tell($"{nameof(PreyTrackerWeight)}", floatS)
        .Tell($"{nameof(WingLengthFactor)}", floatS)
        .Tell($"{nameof(LegLengthFactor)}", floatS)
        .Tell($"{nameof(WhiskerLengthFactor)}", floatS)
        .Tell($"{nameof(SmallWhiskerLength)}", floatS)
        .Tell($"{nameof(BigWhiskerLength)}", floatS)
        .Tell($"{nameof(LegScaleXFactor)}", floatS)
        .Tell($"{nameof(ShellScaleYFactor)}", floatS)
        .Tell($"{nameof(WhiskerShapeFactor)}", floatS)
        .Tell($"{nameof(HueMax)}", floatHS)
        .Tell($"{nameof(HueMin)}", floatHS)
        .Tell($"{nameof(SaturationMax)}", floatS)
        .Tell($"{nameof(SaturationMin)}", floatS)
        .Tell($"{nameof(LinearSandboxCost)}", floatS)
        .Tell($"{nameof(ExponentialSandboxCost)}", floatS)
        .Tell($"{nameof(RoomPerformanceCost)}", floatS)
        .Tell($"{nameof(BaseDamageResistance)}", floatS)
        .Tell($"{nameof(ExplosionResistance)}", floatS)
        .Tell($"{nameof(BodySizeEstimate)}", floatS)
        .Tell($"{nameof(VisualRadius)}", floatS)
        .Tell($"{nameof(WaterVision)}", floatS)
        .Tell($"{nameof(ThroughSurfaceVision)}", floatS)
        .Tell($"{nameof(MovementBasedVision)}", floatS)
        .Tell($"{nameof(DangerousToPlayer)}", floatS)
        .Tell($"{nameof(LungCapacity)}", floatS)
        .Tell($"{nameof(CommunityInfluence)}", floatS)
        .Tell($"{nameof(MeatMin)}", floatS)
        .Tell($"{nameof(MeatMax)}", floatS)
        .Tell($"{nameof(Scaryness)}", floatS)
        .Tell($"{nameof(MinBuoyancy)}", floatS)
        .Tell($"{nameof(MaxBuoyancy)}", floatS)
        .Tell($"{nameof(SureToGetPreyDistance)}", floatS)
        .Tell($"{nameof(HeadGlobalVelocityFactor)}", floatS)
        .Tell($"{nameof(BaseStunResistance)}", floatS)
        .Tell($"{nameof(ExplosionStunResistance)}", floatS)
        .Tell($"{nameof(SpikeScaleFactor)}", floatS)
        .Tell($"{nameof(ShockResistanceReductionFactor)}", floatS)
        .Tell($"{nameof(SecondaryShellColorBonus)}", floatS)
        .Tell($"{nameof(GlobalVelocityFactor)}", floatS)
        .Tell($"{nameof(GlobalFlyingVelocityFactor)}", floatS)
        .Tell($"{nameof(DamageReductionFactor)}", floatS)
        .Tell($"{nameof(DeadShellChance)}", floatS)
        .Tell($"{nameof(GrabbedShockChargeReduction)}", floatHS)
        .Tell($"{nameof(WaterPathingResistance)}", floatS)
        .Tell($"{nameof(BluntDamageResistance)}", floatS)
        .Tell($"{nameof(BluntStunResistance)}", floatS)
        .Tell($"{nameof(WaterDamageResistance)}", floatS)
        .Tell($"{nameof(WaterStunResistance)}", floatS)
        .Tell($"{nameof(StabDamageResistance)}", floatS)
        .Tell($"{nameof(StabStunResistance)}", floatS)
        .Tell($"{nameof(BiteDamageResistance)}", floatS)
        .Tell($"{nameof(BiteStunResistance)}", floatS)
        .Tell($"{nameof(ElectricDamageResistance)}", floatS)
        .Tell($"{nameof(ElectricStunResistance)}", floatS)
        .Tell($"{nameof(OffScreenSpeed)}", floatS)
        .Tell($"{nameof(SurfaceFriction)}", floatS)
        .Tell($"{nameof(Bounce)}", floatS)
        .Tell($"{nameof(WaterRetardationImmunity)}", floatS)
        .Tell($"{nameof(WaterFriction)}", floatS)
        .Tell($"{nameof(AirFriction)}", floatS)
        .Tell($"{nameof(ImpactThreshold)}", floatS)
        .Tell($"{nameof(BodyChunkRadType)}", "~NORMAL / SMALL / CENTIWING; NORMAL by default; IGNORES CASE")
        .Tell($"{nameof(HeadVelocityType)}", "~NORMAL / SMALL; NORMAL by default; IGNORES CASE")
        .Tell($"{nameof(BodySizeGenerationType)}", "~RANDOMRANGEPOW / RANDOMRANGE / STATICMIN / STATICMAX / FROMWORLDSTRING; RANDOMRANGEPOW by default; IGNORES CASE")
        .Tell($"{nameof(ShockType)}", "~NORMAL / SMALL / AQUACENTI; NORMAL by default; IGNORES CASE")
        .Tell($"{nameof(ResistanceType)}", "~NORMAL / RED / AQUACENTI / CENTIWING; NORMAL by default; IGNORES CASE")
        .Tell($"{nameof(AllowedIdleTypes)}", "~NORMAL / CENTIWING / AQUACENTI / AQUACENTIANDCENTIWING; NORMAL by default; IGNORES CASE")
        .Tell($"{nameof(DynamicRelationshipType)}", enumSNRC)
        .Tell($"{nameof(VisualScoreTypes)}", "~NORMAL / CENTIWING / RED / REDANDCENTIWING; NORMAL by default; IGNORES CASE")
        .Tell($"{nameof(PreyTrackType)}", "~NORMAL / RED; NORMAL by default; IGNORES CASE")
        .Tell($"{nameof(ExcitementType)}", enumSNRC)
        .Tell($"{nameof(WingSizeType)}", "~STATICLENGTHFACTOR / CENTIWING / AQUACENTI; STATICLENGTHFACTOR by default; IGNORES CASE")
        .Tell($"{nameof(MainColorType)}", "~CUSTOM / BLACKCOLOR / WATERCOLOR1 / WATERCOLOR2 / WATERSURFACECOLOR1 / WATERSURFACECOLOR2 / WATERSHINECOLOR / FOGCOLOR / SHORTCUTSYMBOL / SKYCOLOR / SHORTCUTCOLOR1 / SHORTCUTCOLOR2 / SHORTCUTCOLOR3; CUSTOM (uses HueMin, HueMax, SaturationMin and SaturationMax) by default; IGNORES CASE")
        .Tell($"{nameof(LightFlashNoiseType)}", "~NONE / RED / GREEN / BLUE; NONE by default; IGNORES CASE")
        .Tell($"{nameof(BodySegmentType)}", "~NORMAL / CENTIWING / AQUACENTI; NORMAL by default; IGNORES CASE")
        .Tell($"{nameof(Grabability)}", "~CANTGRAB / ONEHAND / BIGONEHAND / TWOHANDS / DRAG; nothing by default; IGNORES CASE")
        .Tell($"{nameof(ShockColor)}", paletteS)
        .Tell($"{nameof(ShortCutColor)}", colorS)
        .Tell($"{nameof(HeadGlowMinColor)}", paletteS)
        .Tell($"{nameof(HeadGlowMaxColor)}", paletteS)
        .Tell($"{nameof(BodyBlackColor)}", paletteS)
        .Tell($"{nameof(LightFlashColor)}", paletteS)
        .Tell($"{nameof(WingColor)}", paletteS)
        .Tell($"{nameof(StandardIconColor)}", colorS)
        .Tell($"{nameof(BigIconColor)}", colorS)
        .Tell($"{nameof(SmallIconColor)}", colorS)
        .Tell($"{nameof(DevColor)}", colorS)
        .Tell($"{nameof(ShellColor)}", paletteNS)
        .Tell($"{nameof(SecondaryShellColor)}", paletteNS)
        .Tell($"{nameof(SegmentSprite)}", centiSegS)
        .Tell($"{nameof(AdditionalShellTextureShader)}", "~Shader name; AquapedeBody by default; CASE SENSITIVE")
        .Tell($"{nameof(LegASprite)}", "~Sprite name; CentipedeLegA by default; CASE SENSITIVE")
        .Tell($"{nameof(LegBSprite)}", "~Sprite name; CentipedeLegB by default; CASE SENSITIVE")
        .Tell($"{nameof(WingSprite)}", "~Sprite name; CentipedeWing by default; CASE SENSITIVE")
        .Tell($"{nameof(WingShader)}", "~Shader name; CicadaWing by default; CASE SENSITIVE")
        .Tell($"{nameof(ShellSpikeSprite1)}", "~Sprite name; Cicada8body by default; CASE SENSITIVE")
        .Tell($"{nameof(ShellSpikeSprite2)}", centiSegS)
        .Tell($"{nameof(BellyShellSprite)}", "~Sprite name; CentipedeBellyShell by default; CASE SENSITIVE")
        .Tell($"{nameof(BackShellSprite)}", "~Sprite name; CentipedeBackShell by default; CASE SENSITIVE")
        .Tell($"{nameof(StandardIconSprite)}", "~Sprite name; Kill_Centipede2 by default; CASE SENSITIVE")
        .Tell($"{nameof(BigIconSprite)}", "~Sprite name; Kill_Centipede3 by default; CASE SENSITIVE")
        .Tell($"{nameof(SmallIconSprite)}", "~Sprite name; Kill_Centipede1 by default; CASE SENSITIVE")
        .Tell($"{nameof(DevName)}", "~Name; the creature type name by default; CASE SENSITIVE")
        .Tell($"{nameof(SmallUnlockID)}", unlockS)
        .Tell($"{nameof(UnlockID)}", unlockS)
        .Tell($"{nameof(BigUnlockID)}", unlockS)
        .Tell($"{nameof(SmallParentUnlock)}", pUnlockS)
        .Tell($"{nameof(ParentUnlock)}", pUnlockS)
        .Tell($"{nameof(BigParentUnlock)}", pUnlockS)
        .Tell($"{nameof(Relationships)}", "~Multiple custom relations, separated by \";\", each relation should be of the form: active position?(Y (Yes) / N (No))~target name(CreatureTemplate.Type name)~relation type(DoesntTrack / Ignores / Eats / Afraid / StayOutOfWay / AgressiveRival / Attacks / Uncomfortable / Antagonizes / PlaysWith / SocialDependent / Pack)~intensity(Decimal (0-1)); CASE SENSITIVE; Example: Y~Vulture~Afraid~0.75;N~BigEel~Eats~0.2").ToString();
    }

    object ICloneable.Clone() => Duplicate();
}