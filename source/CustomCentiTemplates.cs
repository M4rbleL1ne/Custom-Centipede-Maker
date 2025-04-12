using RWCustom;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Fisobs.Creatures;
using static PathCost.Legality;

namespace CustomCentisMod;

public static class CustomCentiTemplates
{
    internal const int K_Centiwing = 1118218201,
        K_Centipede = -615786266,
        K_RedCentipede = 2030104277,
        K_SmallCentipede = 769016933,
        K_AquaCenti = 96824418;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool AlreadyExists(string name) => CreatureTemplate.Type.values.entries.Contains(name);

    public static void SetParentFields(CustomCentiBreedParams props, int hashName)
    {
        switch (hashName)
        {
            case K_Centipede:
                Centipede(props);
                break;
            case K_RedCentipede:
                RedCentipede(props);
                break;
            case K_SmallCentipede:
                SmallCentipede(props);
                break;
            case K_Centiwing:
                Centiwing(props);
                break;
            case K_AquaCenti:
                AquaCenti(props);
                break;
        }
    }

    public static void SetParentRelationships(CustomCentiBreedParams props)
    {
        CreatureTemplate.Relationship rel;
        if (props.ParentType?.Index is int index && index != -1)
        {
            var customRels = new List<CustomRelation>();
            var templates = StaticWorld.creatureTemplates;
            for (var i = 0; i < templates.Length; i++)
            {
                var template = templates[i];
                if (template?.relationships is CreatureTemplate.Relationship[] rels)
                {
                    if (i == index)
                    {
                        for (var j = 0; j < rels.Length; j++)
                        {
                            rel = rels[j];
                            customRels.Add(new()
                            {
                                ActivePosition = true,
                                Target = new(CreatureTemplate.Type.values.entries[j]),
                                Type = rel.type,
                                Intensity = rel.intensity
                            });
                        }
                    }
                    rel = rels[index];
                    customRels.Add(new()
                    {
                        Target = template.type,
                        Type = rel.type,
                        Intensity = rel.intensity
                    });
                }
            }
            if (props._nonMSCAqua)
                customRels.Add(new()
                {
                    ActivePosition = true,
                    Target = CreatureTemplate.Type.BigEel,
                    Type = CreatureTemplate.Relationship.Type.Afraid,
                    Intensity = 1f
                });
            props.Relationships = customRels.ToArray();
        }
    }

    internal static void Centiwing(CustomCentiBreedParams props)
    {
        props.SegmentSprite = "CentipedeSegment";
        props.LegASprite = "CentipedeLegA";
        props.LegBSprite = "CentipedeLegB";
        props.WingSprite = "CentipedeWing";
        props.WingShader = "CicadaWing";
        props.BellyShellSprite = "CentipedeBellyShell";
        props.BackShellSprite = "CentipedeBackShell";
        props.SmallKillScore = 5;
        props.StandardKillScore = 5;
        props.BigKillScore = 5;
        props.ExpeditionScore = 5;
        props.DevColor = new() { g = 1f, b = .2f, a = 1f };
        props.BigIconColor = props.StandardIconColor = props.SmallIconColor = new() { r = .05490196f, g = .698039234f, b = .235294119f, a = 1f };
        props.BodyBlackColorType = PaletteColorType.BlackColor;
        props.HueMin = .28f;
        props.HueMax = .38f;
        props.SaturationMin = .5f;
        props.SaturationMax = .5f;
        props.AbstractedLaziness = 150;
        props.AllowedIdleTypes = IdleScoreTypes.Centiwing;
        props.ResistanceType = ViolenceResistanceType.Centiwing;
        props.Flags.Set(CountsAsAKill | DetectsAnnoyingCollisions | Wings | EdibleByOmnivores | WeakToStun | Flying | ShellParticles | BodySizeDependantWhisker | BlizzardWanderer | WantsToShock, true);
        props.MinChunkAmount = 7;
        props.MaxChunkAmount = 17;
        props.MinSize = .5f;
        props.MaxSize = .65f;
        props.LegLengthFactor = .65f;
        props.WingColor = HSL2RGB(.99f, 1f, .5f, .5f);
        props.WingLengthFactor = 1f;
        props.PreyTrackerWeight = .12f;
        props.BodySizeGenerationType = SizeGenerationType.RandomRange;
        props.BodyChunkRadType = ChunkRadType.Centiwing;
        props.VisualScoreTypes = VisualScoreChangeTypes.Centiwing;
        props.ExcitementType = ExcitementTrackerType.Centiwing;
        props.Flags2.Set(HypothermiaImmune_HasValue | HypothermiaImmune, true);
        props.WingVariations = 1;
        props.WingSizeType = WingType.Centiwing;
        props.BodySegmentType = SegmentType.Centiwing;
        props.DevName = "cW";
        props.LightFlashNoiseType = FlashColorNoiseType.Blue;
        props.SmallIconSprite = props.BigIconSprite = props.StandardIconSprite = s_Kill_Centiwing;
        props.MeatMin = 3f;
        props.MeatMax = 3f;
        props.BaseDamageResistance = 1f;
        props.BaseStunResistance = .6f;
        props.MovementBasedVision = .25f;
        props.VisualRadius = 300f;
        props.BodySizeEstimate = 1.2f;
        props.WaterVision = .4f;
        props.ThroughSurfaceVision = .85f;
        props.DangerousToPlayer = .4f;
        props.CommunityInfluence = .25f;
        props.ShortCutSegments = 3;
        props.HeadGlobalVelocityFactor = 1f;
        props.HeadVelocityFactor = .7f;
        props.ShortCutColor = Custom.HSL2RGB(Lerp(.28f, .38f, .5f), .5f, .5f);
        props.WhiskerLengthFactor = 1f;
        props.BigWhiskerLength = 43f;
        props.SmallWhiskerLength = 17f;
        props.DynamicRelationshipType = RelationshipChangeType.Centiwing;
        props.ExponentialSandboxCost = .5f;
        props.LinearSandboxCost = .8f;
        props.ShockColor = new() { r = .7f, g = .7f, b = 1f, a = 1f };
        props.ShockResistanceReductionFactor = 1f;
        props.LegScaleXFactor = 1f;
        props.ShellScaleYFactor = 1.5f;
        props.WhiskerShapeFactor = 1f;
        props.LungCapacity = 900f;
        props.LightFlashColor = s_blue;
        props.VelocityFactor = 1f;
        props.RoomPerformanceCost = 10f;
        props.MinBuoyancy = 1.05f;
        props.MaxBuoyancy = 1.05f;
        props.SureToGetPreyDistance = 5f;
        props.PatherStepsPerFrame = 10;
        props.GlobalVelocityFactor = 1f;
        props.GlobalFlyingVelocityFactor = 1f;
        props.DamageReductionFactor = 1f;
        props.DeadShellChance = .015f;
    }

    internal static void RedCentipede(CustomCentiBreedParams props)
    {
        props.SegmentSprite = "CentipedeSegment";
        props.LegASprite = "CentipedeLegA";
        props.LegBSprite = "CentipedeLegB";
        props.ShellSpikeSprite1 = "Cicada8body";
        props.ShellSpikeSprite2 = "CentipedeSegment";
        props.BellyShellSprite = "CentipedeBellyShell";
        props.BackShellSprite = "CentipedeBackShell";
        props.AbstractedLaziness = 150;
        props.BaseDamageResistance = 1f;
        props.BaseStunResistance = .6f;
        props.BigWhiskerLength = 48f;
        props.BodyBlackColorType = PaletteColorType.BlackColor;
        props.BodyChunkMassBonus = .02f;
        props.BodyChunkRadBonus = 1.5f;
        props.BodySizeEstimate = 8.5f;
        props.BodySizeGenerationType = SizeGenerationType.StaticMax;
        props.CommunityInfluence = .25f;
        props.Flags.Set(CountsAsAKill | DetectsAnnoyingCollisions | EdibleByOmnivores | CustomCentiBreedParams.NoiseTracker | ShellParticles | ShellSpikes | Shields | BlizzardWanderer | TooBigForShelter | MajorCreature | WantsToShock, true);
        props.DangerousToPlayer = .9f;
        props.DevName = "RC";
        props.DynamicRelationshipType = RelationshipChangeType.Red;
        props.ExcitementType = ExcitementTrackerType.Red;
        props.ExpeditionScore = 25;
        props.ExponentialSandboxCost = .7f;
        props.HeadGlobalVelocityFactor = 1f;
        props.HeadVelocityFactor = 1f;
        props.HueMax = .01f;
        props.HueMin = -.02f;
        props.Flags2.Set(HypothermiaImmune_HasValue | HypothermiaImmune, true);
        props.LegLengthFactor = 1f;
        props.LegScaleXFactor = 1.3f;
        props.LightFlashColor = s_blue;
        props.LightFlashNoiseType = FlashColorNoiseType.Blue;
        props.LinearSandboxCost = 1f;
        props.LungCapacity = 900f;
        props.MaxBuoyancy = 1.05f;
        props.MinBuoyancy = 1.05f;
        props.MaxChunkAmount = 18;
        props.MaxSize = 1f;
        props.MeatMax = 9f;
        props.MeatMin = 9f;
        props.MinChunkAmount = 18;
        props.MovementBasedVision = .95f;
        props.PatherStepsPerFrame = 15;
        props.PreyTrackerWeight = .9f;
        props.PreyTrackType = PreyTrackerType.Red;
        props.ResistanceType = ViolenceResistanceType.Red;
        props.RoomPerformanceCost = 10f;
        props.SaturationMax = 1f;
        props.SaturationMin = .9f;
        props.ShellScaleYFactor = 1.7f;
        props.ShockColor = new() { r = .7f, g = .7f, b = 1f, a = 1f };
        props.ShockResistanceReductionFactor = 1f;
        props.DevColor = props.ShortCutColor = Color.red;
        props.ShortCutSegments = 5;
        props.SmallWhiskerLength = 44f;
        props.SpikeScaleFactor = 1f;
        props.StandardIconColor = s_coolRed;
        props.StandardIconSprite = s_Kill_Centipede3;
        props.StandardKillScore = ModManager.MSC ? 25 : 19;
        props.SureToGetPreyDistance = 100f;
        props.ThroughSurfaceVision = .85f;
        props.VelocityFactor = 1.25f;
        props.VisualRadius = 1100f;
        props.VisualScoreTypes = VisualScoreChangeTypes.Red;
        props.WaterVision = .4f;
        props.WhiskerLengthFactor = 1f;
        props.WhiskerShapeFactor = 1f;
        props.GlobalVelocityFactor = 1f;
        props.GlobalFlyingVelocityFactor = 1f;
        props.DamageReductionFactor = 1f;
        props.DeadShellChance = .015f;
    }

    internal static void AquaCenti(CustomCentiBreedParams props)
    {
        props.SegmentSprite = "CentipedeSegment";
        props.AdditionalShellTextureShader = "AquapedeBody";
        props.LegASprite = "CentipedeLegA";
        props.LegBSprite = "CentipedeLegB";
        props.WingShader = "CicadaWing";
        props.BellyShellSprite = "CentipedeBellyShell";
        props.BackShellSprite = "CentipedeBackShell";
        props.AbstractedLaziness = 150;
        props.Flags.Set(AdditionalShellTexture | CountsAsAKill | DetectsAnnoyingCollisions | EdibleByOmnivores | GlowingHead | CustomCentiBreedParams.NoiseTracker | ShellParticles | SlowLegs | Swimming | WaterOnly | Wings | BlizzardWanderer | MajorCreature | WantsToShock, true);
        props.AllowedIdleTypes = IdleScoreTypes.AquaCenti;
        props.BaseDamageResistance = .9f;
        props.BaseStunResistance = .6f;
        props.BigWhiskerLength = 72f;
        props.BodyBlackColorType = PaletteColorType.BlackColor;
        props.BodySegmentType = SegmentType.AquaCenti;
        props.BodySizeEstimate = 1f;
        props.BodySizeGenerationType = SizeGenerationType.RandomRange;
        props.CommunityInfluence = .35f;
        props.ConnectionElasticityReduction = .7f;
        props.DangerousToPlayer = .9f;
        props.DevColor = s_blue;
        props.DevName = "aC";
        props.ExpeditionScore = 10;
        props.ExponentialSandboxCost = .5f;
        props.HeadGlowMaxColor = props.ShockColor = new() { r = .7f, g = .7f, b = 1f, a = 1f };
        props.HeadGlowMinColorType = PaletteColorType.WaterColor1;
        props.HeadGlobalVelocityFactor = .2f;
        props.HeadVelocityFactor = 1f;
        props.HueMax = .6f;
        props.HueMin = .5f;
        props.Flags2.Set(HypothermiaImmune_HasValue | HypothermiaImmune, true);
        props.LegLengthFactor = 1f;
        props.LegScaleXFactor = 1f;
        props.LightFlashNoiseType = FlashColorNoiseType.Blue;
        props.LinearSandboxCost = .8f;
        props.LungCapacity = 9900f;
        props.MaxBuoyancy = .78f;
        props.MinBuoyancy = .15f;
        props.MaxChunkAmount = 17;
        props.MaxSize = 1.8f;
        props.MeatMax = 7f;
        props.MeatMin = 2.3f;
        props.MinChunkAmount = 7;
        props.MinSize = .9f;
        props.MovementBasedVision = .95f;
        props.PatherStepsPerFrame = 10;
        props.SureToGetPreyDistance = 5f;
        props.PreyTrackerWeight = .9f;
        props.ResistanceType = ViolenceResistanceType.AquaCenti;
        props.RoomPerformanceCost = 10f;
        props.SaturationMax = 1f;
        props.SaturationMin = .8f;
        props.ShellScaleYFactor = 1.5f;
        props.ShockResistanceReductionFactor = 1f;
        props.ShockType = ShockDamageType.AquaCenti;
        props.ShortCutColor = Custom.HSL2RGB(Lerp(.5f, .6f, .5f), .9f, .5f);
        props.ShortCutSegments = 4;
        props.SmallWhiskerLength = 72f;
        props.LightFlashColor = props.BigIconColor = props.StandardIconColor = props.SmallIconColor = s_blue;
        props.BigIconSprite = props.StandardIconSprite = props.SmallIconSprite = s_Kill_Centiwing;
        props.SmallKillScore = 10;
        props.StandardKillScore = 10;
        props.BigKillScore = 10;
        props.ThroughSurfaceVision = .2f;
        props.VelocityFactor = 1f;
        props.VisualRadius = 1900f;
        props.WaterVision = 2f;
        props.WhiskerLengthFactor = 1f;
        props.WhiskerShapeFactor = 1f;
        props.WingColor = HSL2RGB(.99f, 1f, .5f, .5f);
        props.WingLengthFactor = 1f;
        props.WingSizeType = WingType.AquaCenti;
        props.WingSprite = "AquapedeWing";
        props.WingVariations = 4;
        props.GlobalVelocityFactor = 1f;
        props.GlobalFlyingVelocityFactor = 1f;
        props.DamageReductionFactor = 1f;
        props.DeadShellChance = .015f;
    }

    internal static void SmallCentipede(CustomCentiBreedParams props)
    {
        props.SegmentSprite = "CentipedeSegment";
        props.LegASprite = "CentipedeLegA";
        props.LegBSprite = "CentipedeLegB";
        props.BellyShellSprite = "CentipedeBellyShell";
        props.BackShellSprite = "CentipedeBackShell";
        props.AbstractedLaziness = 1000;
        props.Flags.Set(/*AutomaticPickUp | */BodySizeDependantWhisker | EasyKill | BlizzardWanderer | IgnoresCommunity | KillScoreHidden | CustomCentiBreedParams.NoiseTracker | ShocksWhenGrabbed | SmallFood | SmallTube | TriesToStayInRoom | WantsToShock, true);
        props.BaseDamageResistance = 1f;
        props.BaseStunResistance = .6f;
        props.BigWhiskerLength = 43f;
        props.Bites = 5;
        props.BodyBlackColorType = PaletteColorType.BlackColor;
        props.BodyChunkRadType = ChunkRadType.Small;
        props.BodySizeEstimate = .2f;
        props.BodySizeGenerationType = SizeGenerationType.StaticMin;
        props.CommunityInfluence = .25f;
        props.DevColor = new() { r = 1f, g = .7f, a = 1f };
        props.DevName = "sc";
        props.ExpeditionScore = 4;
        props.ExponentialSandboxCost = .3f;
        props.FoodPoints = 2;
        props.Grabability = Player.ObjectGrabability.OneHand;
        props.Flags2.Set(Grabability_HasValue | HypothermiaImmune | HypothermiaImmune_HasValue, true);
        props.HeadGlobalVelocityFactor = 1f;
        props.HeadVelocityFactor = 1f;
        props.HeadVelocityType = HeadMoveType.Small;
        props.HueMax = .1f;
        props.HueMin = .04f;
        props.LegLengthFactor = 1f;
        props.LegScaleXFactor = 1f;
        props.LightFlashColor = s_blue;
        props.LightFlashNoiseType = FlashColorNoiseType.Blue;
        props.LinearSandboxCost = .3f;
        props.LungCapacity = 900f;
        props.MaxBuoyancy = 1.05f;
        props.MinBuoyancy = 1.05f;
        props.MaxChunkAmount = 5;
        props.MinChunkAmount = 5;
        props.MovementBasedVision = .95f;
        props.PatherStepsPerFrame = 10;
        props.PreyTrackerWeight = .9f;
        props.RoomPerformanceCost = 10f;
        props.SaturationMax = .9f;
        props.SaturationMin = .9f;
        props.ShellScaleYFactor = 1.5f;
        props.ShockColor = new() { r = .7f, g = .7f, b = 1f, a = 1f };
        props.ShockResistanceReductionFactor = 1f;
        props.ShockType = ShockDamageType.Small;
        props.ShortCutColor = Custom.HSL2RGB(Lerp(.04f, .1f, .5f), .9f, .5f);
        props.ShortCutSegments = 3;
        props.SmallWhiskerLength = 17f;
        props.StandardIconColor = new() { r = 1f, g = .6f, a = 1f };
        props.StandardIconSprite = s_Kill_Centipede1;
        props.SureToGetPreyDistance = 5f;
        props.ThroughSurfaceVision = .85f;
        props.VelocityFactor = 1f;
        props.VisualRadius = 900f;
        props.WaterVision = .4f;
        props.WhiskerLengthFactor = .75f;
        props.WhiskerShapeFactor = .5f;
        props.GlobalVelocityFactor = 1f;
        props.GlobalFlyingVelocityFactor = 1f;
        props.DamageReductionFactor = 1f;
    }

    internal static void Centipede(CustomCentiBreedParams props)
    {
        props.SegmentSprite = "CentipedeSegment";
        props.LegASprite = "CentipedeLegA";
        props.LegBSprite = "CentipedeLegB";
        props.BellyShellSprite = "CentipedeBellyShell";
        props.BackShellSprite = "CentipedeBackShell";
        props.StandardIconSprite = s_Kill_Centipede2;
        props.BigIconSprite = s_Kill_Centipede3;
        props.SmallIconSprite = s_Kill_Centipede1;
        props.AbstractedLaziness = 150;
        props.BaseDamageResistance = .9f;
        props.BaseStunResistance = .6f;
        props.StandardIconColor = props.SmallIconColor = props.BigIconColor = new() { r = 1f, g = .6f, a = 1f };
        props.BigKillScore = 7;
        props.BigWhiskerLength = 43f;
        props.BodyBlackColorType = PaletteColorType.BlackColor;
        props.Flags.Set(BodySizeDependantWhisker | CountsAsAKill | DetectsAnnoyingCollisions | EdibleByOmnivores | CustomCentiBreedParams.NoiseTracker | ShellParticles | BlizzardWanderer | UsesUnlockData | WantsToShock, true);
        props.BodySizeEstimate = 1f;
        props.BodySizeGenerationType = SizeGenerationType.FromWorldString;
        props.CommunityInfluence = .25f;
        props.DangerousToPlayer = .3f;
        props.DevColor = new() { r = 1f, g = .7f, a = 1f };
        props.DevName = "c";
        props.ExpeditionScore = 4;
        props.ExponentialSandboxCost = .5f;
        props.HeadGlobalVelocityFactor = 1f;
        props.HeadVelocityFactor = 1f;
        props.HueMax = .1f;
        props.HueMin = .04f;
        props.Flags2.Set(HypothermiaImmune | HypothermiaImmune_HasValue, true);
        props.LegLengthFactor = 1f;
        props.LegScaleXFactor = 1f;
        props.LightFlashColor = s_blue;
        props.LightFlashNoiseType = FlashColorNoiseType.Blue;
        props.LinearSandboxCost = .8f;
        props.LungCapacity = 900f;
        props.MaxBuoyancy = 1.05f;
        props.MinBuoyancy = 1.05f;
        props.MaxChunkAmount = 17;
        props.MaxSize = 1f;
        props.MeatMax = 7f;
        props.MeatMin = 2.3f;
        props.MinChunkAmount = 7;
        // MinSize is 0f by default, no need to set it again
        props.MovementBasedVision = .95f;
        props.PatherStepsPerFrame = 10;
        props.PreyTrackerWeight = .9f;
        props.RoomPerformanceCost = 10f;
        props.SaturationMax = .9f;
        props.SaturationMin = .9f;
        props.ShellScaleYFactor = 1.5f;
        props.ShockColor = new() { r = .7f, g = .7f, b = 1f, a = 1f };
        props.ShockResistanceReductionFactor = 1f;
        props.ShortCutColor = Custom.HSL2RGB(Lerp(.04f, .1f, .5f), .9f, .5f);
        props.ShortCutSegments = 3;
        props.SmallKillScore = 4;
        props.SmallWhiskerLength = 17f;
        props.StandardKillScore = 4;
        props.SureToGetPreyDistance = 5f;
        props.ThroughSurfaceVision = .85f;
        props.VelocityFactor = 1f;
        props.VisualRadius = 900f;
        props.WaterVision = .4f;
        props.WhiskerLengthFactor = 1f;
        props.WhiskerShapeFactor = 1f;
        props.GlobalVelocityFactor = 1f;
        props.GlobalFlyingVelocityFactor = 1f;
        props.DamageReductionFactor = 1f;
        props.DeadShellChance = .015f;
    }

    internal static void UpdateTemplateFields(CustomCentiBreedParams props)
    {
        var template = props._template;
        var critob = props._critob;
        template.abstractedLaziness = props.AbstractedLaziness;
        template.shortcutSegments = props.ShortCutSegments;
        template.pathingPreferencesConnections[(int)MovementConnection.MovementType.DropToWater] = props.Flags.Get(Swimming) ? new(1f, Allowed) : new(100f, IllegalConnection);
        template.pathingPreferencesConnections[(int)MovementConnection.MovementType.NPCTransportation].resistance = props.Flags.Get(Flying) ? 10f : 25f;
        template.pathingPreferencesTiles[(int)AItile.Accessibility.Floor].legality = props.Flags.Get(WaterOnly) ? Unwanted : Allowed;
        template.pathingPreferencesTiles[(int)AItile.Accessibility.Corridor].legality = props.Flags.Get(WaterOnly) ? Unwanted : Allowed;
        template.pathingPreferencesTiles[(int)AItile.Accessibility.Climb] = props.Flags.Get(WaterOnly) ? new(10f, Unwanted) : new(1f, Allowed);
        template.pathingPreferencesTiles[(int)AItile.Accessibility.Wall] = props.Flags.Get(WaterOnly) ? new(100f, Unwanted) : new(1f, Allowed);
        template.pathingPreferencesTiles[(int)AItile.Accessibility.Ceiling] = props.Flags.Get(WaterOnly) ? new(100f, Unwanted) : new(1f, Allowed);
        template.damageRestistances[(int)Creature.DamageType.Water, 0] = props.Flags.Get(Swimming) ? 102f : 0f;
        template.damageRestistances[(int)Creature.DamageType.Water, 1] = props.Flags.Get(Swimming) ? 102f : 0f;
        template.damageRestistances[(int)Creature.DamageType.Explosion, 0] = props.ExplosionResistance;
        template.damageRestistances[(int)Creature.DamageType.Explosion, 1] = props.ExplosionStunResistance;
        template.waterRelationship = props.Flags.Get(WaterOnly) ? CreatureTemplate.WaterRelationship.WaterOnly : (props.Flags.Get(Swimming) ? CreatureTemplate.WaterRelationship.Amphibious : CreatureTemplate.WaterRelationship.AirAndSurface);
        template.preBakedPathingAncestor = StaticWorld.GetCreatureTemplate(props.Flags.Get(Flying) ? (props.Flags.Get(Swimming) ? CreatureTemplate.Type.BigEel : CreatureTemplate.Type.Fly) : (props.Flags.Get(Swimming) ? CreatureTemplate.Type.JetFish : CreatureTemplate.Type.BlueLizard));
        template.meatPoints = props.Flags.Get(SmallFood) ? 0 : 1;
        critob.ShelterDanger = props.Flags.Get(SmallFood) ? ShelterDanger.Safe : ShelterDanger.Hostile;
        template.canFly = props.Flags.Get(Flying);
        template.pathingPreferencesTiles[(int)AItile.Accessibility.Air] = props.Flags.Get(Flying) ? new(1f, Allowed) : new(60f, IllegalTile);
        template.countsAsAKill = props.Flags.Get(CountsAsAKill) ? 2 : 0;
        template.communityID = props.Flags.Get(IgnoresCommunity) ? CreatureCommunities.CommunityID.None : CreatureCommunities.CommunityID.All;
        template.roamBetweenRoomsChance = props.Flags.Get(TriesToStayInRoom) ? -1f : .1f;
        template.instantDeathDamageLimit = props.Flags.Get(EasyKill) ? 1f : float.MaxValue;
        template.wormgrassTilesIgnored = props.Flags.Get(WormGrassImmune);
        template.wormGrassImmune = props.Flags.Get(WormGrassImmune);
        template.forbidStandardShortcutEntry = props.Flags.Get(ForbidStandardShortcutEntry);
        template.BlizzardWanderer = props.Flags.Get(BlizzardWanderer);
        if (s_majorCreatures is HashSet<CreatureTemplate.Type> set)
        {
            if (!set.Contains(props.CreatureType) && props.Flags.Get(MajorCreature))
                set.Add(props.CreatureType);
            else if (set.Contains(props.CreatureType) && !props.Flags.Get(MajorCreature))
                set.Remove(props.CreatureType);
        }
        template.BlizzardAdapted = props.Flags2.Get(HypothermiaImmune | HypothermiaImmune_HasValue);
        critob.SetCustomRelationships();
        template.shortcutColor.a = props.ShortCutColor.a;
        template.shortcutColor.r = props.ShortCutColor.r;
        template.shortcutColor.g = props.ShortCutColor.g;
        template.shortcutColor.b = props.ShortCutColor.b;
        critob.SandboxPerformanceCost = new(props.LinearSandboxCost, props.ExponentialSandboxCost);
        critob.LoadedPerformanceCost = props.RoomPerformanceCost;
        template.baseDamageResistance = props.BaseDamageResistance;
        template.bodySize = props.BodySizeEstimate;
        template.visualRadius = props.VisualRadius;
        template.waterVision = props.WaterVision;
        template.throughSurfaceVision = props.ThroughSurfaceVision;
        template.movementBasedVision = props.MovementBasedVision;
        template.dangerousToPlayer = props.DangerousToPlayer;
        template.lungCapacity = props.LungCapacity;
        template.communityInfluence = props.CommunityInfluence;
        template.scaryness = props.Scaryness;
        template.baseStunResistance = props.BaseStunResistance;
    }
}