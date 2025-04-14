using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;
using RWCustom;

namespace CustomCentisMod;

public static class CustomCentiCalls
{
    static Type s_callsType = typeof(CustomCentiCalls);

    public static Type CallsType
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => s_callsType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Dispose() => s_callsType = null!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ILCursor EmitCentiCall(this ILCursor c, string customCentiMethodName) => c.Emit(OpCodes.Call, s_callsType.GetMethod(customCentiMethodName));

    public static Region SetCustomFlagsRegionNullCheck(Region region) => region is not null ? region : s_nullCheckRegion;

    public static void MakeSaveButtonGrey(Menu.Remix.ConfigMenuTab.ButtonManager self)
    {
        if (Menu.Remix.ConfigContainer.ActiveInterface is CustomCentiInterface)
            self.saveButton.greyedOut = true;
    }

    public static int BodyRotationsWhenFlyingFix(int length, CentipedeGraphics self)
    {
        if (self.totSegs != 2)
            return length;
        var rots = self.bodyRotations;
        var l = rots.GetLength(0);
        var chs = self.centipede.bodyChunks;
        for (var i = 0; i < l; i++)
        {
            rots[i, 1] = rots[i, 0];
            var num = i == 0 ? 0 : 1;
            if (!self.centipede.flying || (i < chs.Length && self.centipede.room.aimap.getTerrainProximity(chs[i].pos) < 2))
            {
                rots[i, 0] = Vector3.Slerp(rots[i, 0], self.BestBodyRotatAtChunk(num), self.centipede.moving ? .4f : .01f);
                continue;
            }
            var vector = default(Vector2);
            if (num == 1)
                vector += Custom.DirVec(chs[num].pos, chs[num - 1].pos);
            else if (num < l - 1 && num == 0)
                vector -= Custom.DirVec(chs[num].pos, chs[num + 1].pos);
            vector += new Vector2(0f, (num == self.centipede.HeadIndex) ? .8f : .1f);
            rots[i, 0] = Vector3.Slerp(rots[i, 0], vector.normalized, (num == self.centipede.HeadIndex) ? .5f : .1f);
        }
        return int.MinValue;
    }

    public static InsectoidCreature? SporeCloudInvulnerableCheck(InsectoidCreature? cr) => (cr is null || cr.abstractCreature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.Flags2.Get(SporeCloudImmune)) ? null : cr;

    public static void SetGlobalVelocityFactorsAndShockCharge(Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
        {
            var chunks = self.bodyChunks;
            float b;
            if (props.Flags.Get(Flying) && self.flying)
                b = Above0(props.GlobalFlyingVelocityFactor);
            else
                b = Above0(props.GlobalVelocityFactor);
            for (var i = 0; i < chunks.Length; i++)
                chunks[i].vel *= b;
            if (!self.dead && self.grabbedBy?.Count > 0 && props.Flags.Get(ShocksWhenGrabbed))
                self.shockCharge = Above0(self.shockCharge - props.GrabbedShockChargeReduction);
        }
    }

    public static float GetDamageReductionFactor(float val, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            val *= Above0(props.DamageReductionFactor);
        return val;
    }

    public static float GetDeadShellChance(float val, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            val = Clamp01(1f - Above0(props.DeadShellChance));
        return val;
    }

    public static void UpdateStun(Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(NoViolenceStun))
            self.stun = 0;
    }

    public static bool IsSmallFood(bool flag, Creature crit) => flag || ((crit as Centipede)!.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(SmallFood));

    public static void InitiateMeat(Centipede self)
    {
        var acrit = self.abstractCreature;
        if (acrit.creatureTemplate.breedParameters is CustomCentiBreedParams props)
        {
            var state = (acrit.state as Centipede.CentipedeState)!;
            if (props.Flags.Get(SmallFood))
                self.bites = Above1(props.Bites);
            else
                state.meatLeft = (int)Math.Round(Lerp(Above0(props.MeatMin), Above0(props.MeatMax), self.size));
            state.meatInitated = true;
        }
    }

    public static int InitiateChunks(int length, Centipede self) => self.Template.breedParameters is CustomCentiBreedParams props ? Lerp(Above2(props.MinChunkAmount), Above2(props.MaxChunkAmount), self.size) : length;

    public static float InitiateChunkRads(Centipede self, float num, float num2)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
        {
            if (props.BodyChunkRadType == ChunkRadType.Small)
                num2 = Lerp(1.5f, 3f, (float)Math.Pow(Clamp((float)Math.Sin(Mathf.PI * num), 0f, 1f), .5d));
            else if (props.BodyChunkRadType == ChunkRadType.Centiwing)
                num2 = Lerp(num2, Lerp(2f, 3.5f, self.size), .4f);
            num2 = Above0(num2 + props.BodyChunkRadBonus);
        }
        return num2;
    }

    public static void InitiateChunkMasses(Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
        {
            var chunks = self.bodyChunks;
            var massBonus = props.BodyChunkMassBonus;
            for (var j = 0; j < chunks.Length; j++)
            {
                var ch = chunks[j];
                ch.mass = Above0(ch.mass + massBonus + massBonus * 4f * Clamp01((float)Math.Sin(InverseLerp(0f, chunks.Length - 1, j) * Mathf.PI)));
            }
        }
    }

    public static bool DoesNotHaveShellParticles(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && !props.Flags.Get(ShellParticles));

    public static PhysicalObject.BodyChunkConnection InitiateChunkConnections(PhysicalObject.BodyChunkConnection connec, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            connec.elasticity = Clamp01(connec.elasticity - Above0(props.ConnectionElasticityReduction));
        return connec;
    }

    public static void InitiateLastParams(Centipede self)
    {
        var acrit = self.abstractCreature;
        if (acrit.creatureTemplate.breedParameters is CustomCentiBreedParams props)
        {
            self.flying = props.Flags.Get(Flying);
            self.buoyancy = props.MaxBuoyancy;
            var flags2 = props.Flags2;
            if (flags2.Get(HypothermiaImmune_HasValue))
                acrit.HypothermiaImmune = flags2.Get(HypothermiaImmune);
            if (flags2.Get(LavaImmune_HasValue))
                acrit.lavaImmune = flags2.Get(LavaImmune);
            if (flags2.Get(IgnoresCycle_HasValue))
                acrit.ignoreCycle = flags2.Get(IgnoresCycle) && (!flags2.Get(NightOnly_HasValue) || !flags2.Get(NightOnly));
            if (flags2.Get(NightOnly_HasValue))
                acrit.nightCreature = flags2.Get(NightOnly);
            if (flags2.Get(TentacleImmune_HasValue))
                acrit.tentacleImmune = flags2.Get(TentacleImmune);
            if (flags2.Get(PreCycle_HasValue))
                acrit.preCycle = flags2.Get(PreCycle);
            acrit.voidCreature = false;
            acrit.Winterized = false;
            acrit.superSizeMe = false;
            self.canBeHitByWeapons = !flags2.Get(CannotBeHitByWeapons);
            self.surfaceFriction = Above0(props.SurfaceFriction);
            self.bounce = Above0(props.Bounce);
            self.waterRetardationImmunity = Above0(props.WaterRetardationImmunity);
            self.waterFriction = Above0(props.WaterFriction);
            self.airFriction = Above0(props.AirFriction);
            self.impactTreshhold = Above0(props.ImpactThreshold);
        }
    }

    public static bool IsFlying(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Flying));

    public static bool IsFlyingOrSwimming(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && (props.Flags.Get(Flying) || props.Flags.Get(Swimming)));

    public static bool SetMaxBuoyancy(bool flag, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            self.buoyancy = props.MaxBuoyancy;
        return flag;
    }

    public static bool SetMinBuoyancy(bool flag, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            self.buoyancy = props.MinBuoyancy;
        return flag;
    }

    public static void SetHeadGlobalVelocityFactor(Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            self.HeadChunk.vel *= props.HeadGlobalVelocityFactor;
    }

    public static bool DoesNotDetectAnnoyingCollisions(bool flag, AbstractCreature self) => flag || (self.creatureTemplate.breedParameters is CustomCentiBreedParams props && !props.Flags.Get(DetectsAnnoyingCollisions));

    public static float GetVelocityFactor(float val, Centipede self) => self.Template.breedParameters is CustomCentiBreedParams props ? Above0(props.VelocityFactor) : val;

    public static bool UsesSmallHeadMoveType(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.HeadVelocityType == HeadMoveType.Small);

    public static float GetHeadVelocityFactor(float val, Centipede self) => self.Template.breedParameters is CustomCentiBreedParams props ? Above0(props.HeadVelocityFactor) : val;

    public static bool IsSwimmingAndMovingInWater(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Swimming) && self.Submersion > .5f);

    public static void SetFlyingIfUnderWater(Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Swimming | Flying) && self.Submersion > .5f)
            self.flying = false;
    }

    public static bool UsesAquaCentiShockType(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.ShockType == ShockDamageType.AquaCenti);

    public static bool UsesSmallShockType(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.ShockType == ShockDamageType.Small);

    public static Color GetShockColor(Color clr, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            return self.room?.game?.cameras[0] is RoomCamera cam ? GetColorFromType(props.ShockColorType, cam.currentPalette, props.ShockColor) : Clamp01(props.ShockColor);
        return clr;
    }

    public static float GetShockResistanceReductionFactor(float f, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            return f * Above0(props.ShockResistanceReductionFactor);
        return f;
    }

    public static bool IsWeakToStun(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(WeakToStun));

    public static bool HasGlowingHead(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(GlowingHead));

    public static Color GetHeadGlowMaxColor(Color clr, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            return self.room?.game?.cameras[0] is RoomCamera cam ? GetColorFromType(props.HeadGlowMaxColorType, cam.currentPalette, props.HeadGlowMaxColor) : Clamp01(props.HeadGlowMaxColor);
        return clr;
    }

    public static bool CanShockWhenGrabbed(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(ShocksWhenGrabbed));

    public static bool HasShields(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Shields));

    public static bool HasShieldsAndParticles(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Shields | ShellParticles));

    public static bool UsesRedResistanceType(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.ResistanceType == ViolenceResistanceType.Red);

    public static bool UsesCentiwingResistanceType(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.ResistanceType == ViolenceResistanceType.Centiwing);

    public static bool UsesAquaCentiResistanceType(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.ResistanceType == ViolenceResistanceType.AquaCenti);

    public static bool CanTrackNoise(bool flag, CentipedeAI self) => flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.Flags.Get(CustomCentiBreedParams.NoiseTracker));

    public static float GetPreyTrackerWeight(float val, CentipedeAI self)
    {
        if (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props)
            return Above0(props.PreyTrackerWeight);
        return val;
    }

    public static float GetPreyTrackerPersistance(float val, CentipedeAI self)
    {
        if (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props)
            return Above0(props.SureToGetPreyDistance);
        return val;
    }

    public static void SetStepsPerFrame(CentipedeAI self)
    {
        if (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props)
            self.pathFinder.stepsPerFrame = Above0(props.PatherStepsPerFrame);
    }

    public static bool UsesAquaCentiIdleType(bool flag, CentipedeAI self)
    {
        return flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && (props.AllowedIdleTypes & IdleScoreTypes.AquaCenti) == IdleScoreTypes.AquaCenti);
    }

    public static bool UsesCentiwingIdleType(bool flag, CentipedeAI self)
    {
        return flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && (props.AllowedIdleTypes & IdleScoreTypes.Centiwing) == IdleScoreTypes.Centiwing);
    }

    public static bool UsesCentiwingDynamicRelationship(bool flag, CentipedeAI self)
    {
        return flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.DynamicRelationshipType == RelationshipChangeType.Centiwing);
    }

    public static bool UsesRedDynamicRelationship(bool flag, CentipedeAI self)
    {
        return flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.DynamicRelationshipType == RelationshipChangeType.Red);
    }

    public static bool UsesRedPreyTracker(bool flag, CentipedeAI self) => flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.PreyTrackType == PreyTrackerType.Red);

    public static bool UsesAquaCentiOrCentiwingIdleType(bool flag, CentipedeAI self) => flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && ((props.AllowedIdleTypes & IdleScoreTypes.AquaCenti) == IdleScoreTypes.AquaCenti || (props.AllowedIdleTypes & IdleScoreTypes.Centiwing) == IdleScoreTypes.Centiwing));

    public static bool UsesCentiwingExcitementType(bool flag, CentipedeAI self) => flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.ExcitementType == ExcitementTrackerType.Centiwing);

    public static bool UsesRedExcitementType(bool flag, CentipedeAI self) => flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.ExcitementType == ExcitementTrackerType.Red);

    public static CreatureTemplate.Type GetMyTemplateType(CreatureTemplate.Type tp, CentipedeAI self) => self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams ? self.creature.creatureTemplate.type : tp;

    public static bool UsesRedOrCentiwingVisualScore(bool flag, CentipedeAI self) => flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && ((props.VisualScoreTypes & VisualScoreChangeTypes.Red) == VisualScoreChangeTypes.Red || (props.VisualScoreTypes & VisualScoreChangeTypes.Centiwing) == VisualScoreChangeTypes.Centiwing));

    public static bool IsFlyingAndNotMovingInWater(bool flag, Centipede self) => flag || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Flying) && (!props.Flags.Get(Swimming) || (self.Submersion <= .5f)));

    public static bool IsFlyingAndNotMovingInWaterForPather(bool flag, CentipedePather self) => flag || (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Flying) && (!props.Flags.Get(Swimming) || (self.creature.realizedCreature is Centipede c && c.Submersion <= .5f)));

    public static void InitiateWings(CentipedeGraphics self)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props)
        {
            var chunks = self.centipede.bodyChunks;
            var flag = props.Flags.Get(Wings);
            if (flag && props.Flags.Get(ShellSpikes))
                self.wingPairs = chunks.Length * 2;
            else if (flag || props.Flags.Get(ShellSpikes))
                self.wingPairs = chunks.Length;
            if (flag)
            {
                self.wingLengths = new float[self.totSegs];
                var wl = self.wingLengths;
                float sz = self.centipede.size, fac = Above0(props.WingLengthFactor);
                int k;
                if (props.WingSizeType == WingType.Centiwing)
                {
                    for (k = 0; k < wl.Length; k++)
                    {
                        float num = (float)k / (self.totSegs - 1),
                            num2 = .5f + (float)Math.Sin((float)Math.Pow(InverseLerp(.5f, 0f, num), .75d) * Mathf.PI) * (1f - num) * .5f,
                            num3 = .5f + (float)Math.Sin((float)Math.Pow(InverseLerp(1f, .5f, num), .75d) * Mathf.PI) * num * .5f;
                        wl[k] = Lerp(3f, LerpMap(sz, .5f, 1f, 60f, 80f), Math.Max(num2, num3) - (float)Math.Sin(num * Mathf.PI) * .25f) * fac;
                    }
                }
                else if (props.WingSizeType == WingType.AquaCenti)
                {
                    for (k = 0; k < wl.Length; k++)
                    {
                        float num4 = (float)k / (self.totSegs - 1),
                            num5 = .5f + (float)Math.Sin((float)Math.Pow(InverseLerp(.4f, 0f, num4), .75d) * Mathf.PI) * (1f - num4) * .5f,
                            num6 = .5f + (float)Math.Sin((float)Math.Pow(InverseLerp(.6f, .4f, num4), .75d) * Mathf.PI) * num4 * .5f;
                        wl[k] = Clamp(Lerp(3f, LerpMap(sz, .5f, 1f, 100f, 130f), Math.Max(num5, num6)) * .75f, 60f, 80f) * fac;
                    }
                }
                else
                {
                    for (k = 0; k < wl.Length; k++)
                        wl[k] = fac;
                }
            }
        }
    }

    public static float GetLegLengthFactor(float val, CentipedeGraphics self) => self.centipede.Template.breedParameters is CustomCentiBreedParams props ? Above0(props.LegLengthFactor) : val;

    public static Color GetHeadGlowMinColorWithRCam(Color clr, CentipedeGraphics self, RoomCamera rCam)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props)
            return GetColorFromType(props.HeadGlowMinColorType, rCam.currentPalette, props.HeadGlowMinColor);
        return clr;
    }

    public static Color GetHeadGlowMaxColorWithRCam(Color clr, CentipedeGraphics self, RoomCamera rCam)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props)
            return GetColorFromType(props.HeadGlowMaxColorType, rCam.currentPalette, props.HeadGlowMaxColor);
        return clr;
    }

    public static Color GetBodyBlackColor(Color clr, CentipedeGraphics self, RoomCamera rCam)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props)
            return GetColorFromType(props.BodyBlackColorType, rCam.currentPalette, props.BodyBlackColor);
        return clr;
    }

    public static Color GetFlashColor(Color clr, CentipedeGraphics self)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props)
        {
            var col = self.centipede.room?.game?.cameras[0] is RoomCamera cam ? GetColorFromType(props.LightFlashColorType, cam.currentPalette, props.LightFlashColor) : Clamp01(props.LightFlashColor);
            switch (props.LightFlashNoiseType)
            {
                case FlashColorNoiseType.Red:
                    clr.b = clr.g;
                    clr.r = col.r;
                    break;
                case FlashColorNoiseType.Green:
                    clr.b = clr.g;
                    clr.g = col.g;
                    break;
                case FlashColorNoiseType.Blue:
                    clr.b = col.b;
                    break;
                default:
                    clr = col;
                    break;
            }
        }
        return clr;
    }

    public static bool IsSwimmingAndUnderWater(bool flag, CentipedeGraphics self) => flag || (self.centipede.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Swimming) && self.centipede.Submersion > .5f);

    public static bool HasAdditionalShellSprite(bool flag, CentipedeGraphics self) => flag || (self.centipede.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(AdditionalShellTexture));

    public static bool UsesSmallTube(bool flag, CentipedeGraphics self) => flag || (self.centipede.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(SmallTube));

    public static bool UsesCentiwingSegment(bool flag, CentipedeGraphics self) => flag || (self.centipede.Template.breedParameters is CustomCentiBreedParams props && props.BodySegmentType == SegmentType.Centiwing);

    public static bool UsesAquaCentiSegment(bool flag, CentipedeGraphics self) => flag || (self.centipede.Template.breedParameters is CustomCentiBreedParams props && props.BodySegmentType == SegmentType.AquaCenti);

    public static bool HasSwimmerLegs(bool flag, CentipedeGraphics self) => flag || (self.centipede.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(SlowLegs));

    public static float GetShellScaleYFactor(float val, CentipedeGraphics self) => self.centipede.Template.breedParameters is CustomCentiBreedParams props ? Above0(props.ShellScaleYFactor) : val;

    public static float GetLegScaleXFactor(float val, CentipedeGraphics self) => self.centipede.Template.breedParameters is CustomCentiBreedParams props ? Above0(props.LegScaleXFactor) : val;

    public static float GetWhiskerShapeFactor(float val, CentipedeGraphics self) => self.centipede.Template.breedParameters is CustomCentiBreedParams props ? Above0(props.WhiskerShapeFactor) : val;

    public static void InitiateHueAndSaturation(CentipedeGraphics self)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props && props.MainColorType == PaletteColorType.Custom)
        {
            var state = Random.state;
            Random.InitState(self.centipede.abstractCreature.ID.RandomSeed);
            self.hue = Lerp(props.HueMin, props.HueMax, Random.value);
            self.saturation = Lerp(Above0(props.SaturationMin), Above0(props.SaturationMax), Random.value);
            Random.state = state;
        }
    }

    public static void UpdateHueAndSaturation(CentipedeGraphics self, RoomCamera rCam)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props && props.MainColorType != PaletteColorType.Custom)
        {
            var col = Custom.RGB2HSL(GetColorFromType(props.MainColorType, rCam.currentPalette, s_white));
            self.hue = col.x;
            self.saturation = col.y;
        }
    }

    public static void InitiateGraphics(CentipedeGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props)
        {
            var chunks = self.centipede.bodyChunks;
            var sprites = sLeaser.sprites;
            FAtlasElement legA = TryGetSprite(props.LegASprite, "CentipedeLegA"), legB = TryGetSprite(props.LegBSprite, "CentipedeLegB"),
                seg = TryGetSprite(props.SegmentSprite, "CentipedeSegment");
            int i, k;
            for (i = 0; i < chunks.Length; i++)
            {
                sprites[self.SegmentSprite(i)].element = seg;
                for (k = 0; k < 2; k++)
                {
                    sprites[self.LegSprite(i, k, 0)].element = legA;
                    sprites[self.LegSprite(i, k, 1)].element = legB;
                }
            }
            bool spikes = props.Flags.Get(ShellSpikes), wings = props.Flags.Get(Wings);
            string spr1 = SpriteExists(props.ShellSpikeSprite2) ? props.ShellSpikeSprite2! : "CentipedeSegment",
                spr0 = SpriteExists(props.ShellSpikeSprite1) ? props.ShellSpikeSprite1! : "Cicada8body";
            int pairs = self.wingPairs, bonus = wings ? (pairs / 2) : 0;
            if (spikes)
            {
                for (i = 0; i < (wings ? (pairs / 2) : pairs); i++)
                {
                    sprites[self.WingSprite(1, i)] = new(spr1);
                    sprites[self.WingSprite(0, i + bonus)] = new(spr0) { anchorY = .55f };
                }
            }
            var wingSh = ShaderExists(props.WingShader) ? props.WingShader! : "CicadaWing";
            bonus = spikes ? (pairs / 2) : 0;
            var flag = props.WingVariations > 1;
            //var state = Random.state;
            //Random.InitState(self.centipede.abstractCreature.ID.RandomSeed); vanilla doesn't do it here
            if (wings)
            {
                for (i = 0; i < 2; i++)
                {
                    for (k = 0; k < (spikes ? (pairs / 2) : pairs); k++)
                    {
                        string? s;
                        if (flag)
                            s = props.WingSprite + Random.Range(1, props.WingVariations + 1).ToString();
                        else
                            s = SpriteExists(props.WingSprite) ? props.WingSprite : props.WingSprite + "1";
                        sprites[self.WingSprite(i, i == 0 ? k : (k + bonus))] = new CustomFSprite(SpriteExists(s) ? s! : "CentipedeWing")
                        {
                            shader = Custom.rainWorld.Shaders[wingSh]
                        };
                    }
                }
            }
            //Random.state = state;
        }
    }

    public static string GetAdditionalShellSpriteShader(string s, CentipedeGraphics self) => self.centipede.Template.breedParameters is CustomCentiBreedParams props && ShaderExists(props.AdditionalShellTextureShader) ? props.AdditionalShellTextureShader! : s;

    public static void UpdateGraphics(CentipedeGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props)
        {
            bool spikes = props.Flags.Get(ShellSpikes), wings = props.Flags.Get(Wings);
            var sprites = sLeaser.sprites;
            FSprite spr;
            int twoOrOne = (!props.Flags.Get(AdditionalShellTexture)) ? 1 : 2, pairs = self.wingPairs, i, j;
            var chunks = self.centipede.bodyChunks;
            var back = TryGetSprite(props.BackShellSprite, "CentipedeBackShell");
            var belly = TryGetSprite(props.BellyShellSprite, "CentipedeBellyShell");
            for (i = 0; i < chunks.Length; i++)
            {
                var ch = chunks[i];
                var fVis = !props.Flags.Get(SmallFood) || self.centipede.BitesLeft > i;
                var flagS = fVis && (!props.Flags.Get(ShellParticles) || self.centipede.CentiState.shells[i]);
                for (j = 0; j < twoOrOne; j++)
                {
                    sprites[self.ShellSprite(i, j)].isVisible = flagS;
                    if (j == 0)
                    {
                        sprites[self.SegmentSprite(i)].isVisible = fVis;
                        if (i > 0)
                            sprites[self.SecondarySegmentSprite(i - 1)].isVisible = fVis;
                    }
                }
                var vector2 = Vector2.Lerp(chunks[0].lastPos, chunks[0].pos, timeStacker);
                vector2 += DirVec(Vector2.Lerp(chunks[1].lastPos, chunks[1].pos, timeStacker), vector2) * 10f;
                var vector3 = Vector2.Lerp(ch.lastPos, ch.pos, timeStacker);
                var vector4 = (i < chunks.Length - 1) ? Vector2.Lerp(chunks[i + 1].lastPos, chunks[i + 1].pos, timeStacker) : (vector3 + DirVec(vector2, vector3) * 10f);
                Vector2 normalized = self.RotatAtChunk(i, timeStacker).normalized, normalized2 = (vector2 - vector4).normalized;
                var shellHasVal = props.Flags2.Get(ShellColorType_HasValue);
                for (j = 0; j < twoOrOne; j++)
                {
                    spr = sprites[self.ShellSprite(i, j)];
                    if (normalized.y > 0f)
                    {
                        if (shellHasVal)
                            spr.color = self.ShellColor;
                        spr.element = back;
                    }
                    else
                        spr.element = belly;
                }
                var num = i / (float)(chunks.Length - 1);
                var num6 = Clamp((float)Math.Sin(num * Mathf.PI), 0f, 1f);
                num6 *= Lerp(1f, .5f, self.centipede.size);
                var tpSeg = (float)Math.Pow(Clamp((float)Math.Sin(num * Mathf.PI), 0f, 1f), 2d);
                if (props.BodySegmentType == SegmentType.Centiwing)
                    num6 = Lerp(.6f, .3f, tpSeg);
                else if (props.BodySegmentType == SegmentType.AquaCenti)
                    num6 = Lerp(.8f, .2f, tpSeg);
                if (spikes)
                {
                    var bonus = wings ? (pairs / 2) : 0;
                    var vector6 = DegToVec(VecToDeg(normalized) + ((normalized.x > 0f) ? (-90f) : 90f));
                    spr = sprites[self.WingSprite(1, i)];
                    var fac = Above0(props.SpikeScaleFactor);
                    if (vector6.y > 0f && flagS)
                    {
                        spr.isVisible = true;
                        spr.scaleX = ch.rad * Lerp(1f, Lerp(1.5f, .9f, Math.Abs(vector6.x)), num6) * .7f * vector6.y * (1f / 14f) * fac;
                        spr.scaleY = ch.rad * 1.2f * (1f / 11f) * fac;
                        var num8 = InverseLerp(-.5f, .5f, Vector2.Dot(normalized2, DegToVec(30f) * vector6.x));
                        num8 *= Math.Max(InverseLerp(.3f, .05f, Math.Abs(-.5f - vector6.x)), InverseLerp(.3f, .05f, Math.Abs(.5f - vector6.x)));
                        num8 *= (1f - self.darkness) * (1f - self.darkness);
                        spr.color = shellHasVal ? self.ShellColor : Lerp(Custom.HSL2RGB(self.hue, self.saturation, .5f + .25f * num8), self.blackColor, .3f + .7f * self.darkness * (1f - num8));
                        spr.x = (vector3 + PerpendicularVector(normalized2) * vector6.x * ch.rad * 1.2f).x - camPos.x;
                        spr.y = (vector3 + PerpendicularVector(normalized2) * vector6.x * ch.rad * 1.2f).y - camPos.y;
                        spr.rotation = VecToDeg((vector2 - vector4).normalized);
                    }
                    else
                        spr.isVisible = false;
                    spr = sprites[self.WingSprite(0, i + bonus)];
                    if (flagS)
                    {
                        spr.isVisible = true;
                        var num9 = (float)Math.Pow(Math.Abs(normalized.x), .5d) * Math.Sign(normalized.x);
                        spr.x = (vector3 + PerpendicularVector(normalized2) * num9 * ch.rad * 1.1f).x - camPos.x;
                        spr.y = (vector3 + PerpendicularVector(normalized2) * num9 * ch.rad * 1.1f).y - camPos.y;
                        spr.rotation = VecToDeg(Vector3.Slerp((num < .5f) ? normalized2 : (-normalized2), PerpendicularVector(normalized2) * Math.Sign(num9), .3f + .7f * (float)Math.Sin(num * Mathf.PI)));
                        var num10 = InverseLerp(-.5f, .5f, Vector2.Dot(normalized2, DegToVec(30f) * normalized.x));
                        num10 *= Math.Max(InverseLerp(.3f, .05f, Math.Abs(-.5f - normalized.x)), InverseLerp(.3f, .05f, Math.Abs(.5f - normalized.x)));
                        num10 *= (float)Math.Pow(1f - self.darkness, 2d);
                        spr.color = shellHasVal ? self.ShellColor : Lerp(Custom.HSL2RGB(self.hue, self.saturation, .5f + .25f * num10), self.blackColor, self.darkness);
                        spr.scaleY = Math.Abs(num9) * Lerp(-.25f, -.6f, (float)Math.Sin(num * Mathf.PI)) * fac;
                        spr.scaleX = Lerp(.15f, .25f, (float)Math.Sin(num * Mathf.PI)) * fac;
                    }
                    else
                        spr.isVisible = false;
                }
            }
            if (wings)
            {
                var col = GetColorFromType(props.WingColorType, rCam.currentPalette, props.WingColor);
                var hslc = Custom.RGB2HSL(col);
                var bonus = spikes ? (pairs / 2) : 0;
                for (i = 0; i < 2; i++)
                {
                    for (j = 0; j < (spikes ? (pairs / 2) : pairs); j++)
                    {
                        var vector15 = (j != 0) ? DirVec(self.ChunkDrawPos(j - 1, timeStacker), self.ChunkDrawPos(j, timeStacker)) : DirVec(self.ChunkDrawPos(0, timeStacker), self.ChunkDrawPos(1, timeStacker));
                        var vector16 = PerpendicularVector(vector15);
                        var vector17 = self.RotatAtChunk(j, timeStacker);
                        var vector18 = self.WingPos(i, j, vector15, vector16, vector17, timeStacker);
                        var vector19 = self.ChunkDrawPos(j, timeStacker) + chunks[j].rad * (i == 0 ? -1f : 1f) * vector16 * vector17.y;
                        var lhs = DegToVec(AimFromOneVectorToAnother(vector18, vector19) + VecToDeg(vector17));
                        var a = InverseLerp(.85f, 1f, Vector2.Dot(lhs, DegToVec(45f))) * Math.Abs(Vector2.Dot(DegToVec(45f + VecToDeg(vector17)), vector15));
                        var lhs2 = DegToVec(AimFromOneVectorToAnother(vector19, vector18) + VecToDeg(vector17));
                        var b = InverseLerp(.85f, 1f, Vector2.Dot(lhs2, DegToVec(45f))) * Math.Abs(Vector2.Dot(DegToVec(45f + VecToDeg(vector17)), -vector15));
                        a = (float)Math.Pow(Math.Max(a, b), .5d);
                        var num17 = 2f;
                        if (props.WingSizeType == WingType.AquaCenti)
                            num17 = 5f;
                        var sprc = (sprites[self.WingSprite(i, i == 0 ? j : (j + bonus))] as CustomFSprite)!;
                        sprc.MoveVertice(1, vector18 + vector15 * num17 - camPos);
                        sprc.MoveVertice(0, vector18 - vector15 * num17 - camPos);
                        sprc.MoveVertice(2, vector19 + vector15 * num17 - camPos);
                        sprc.MoveVertice(3, vector19 - vector15 * num17 - camPos);
                        sprc.verticeColors[0] = HSL2RGB(hslc.x - .4f * a * a, hslc.y, hslc.z + .5f * a, col.a + .5f * a);
                        sprc.verticeColors[1] = sprc.verticeColors[0];
                        sprc.verticeColors[2] = Lerp(self.blackColor, s_white, .5f * a);
                        sprc.verticeColors[3] = sprc.verticeColors[2];
                    }
                }
            }
        }
    }

    public static CentipedeShell GetCustomShell(CentipedeShell old, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            return new CustomCentiShell(old.pos, old.vel, old.hue, old.saturation, old.scaleX, old.scaleY, props.BackShellSprite!, (self.graphicsModule as CentipedeGraphics)!.blackColor);
        return old;
    }

    public static bool DoesNotHaveAStaticSize(bool flag, RelationshipTracker.DynamicRelationship dRelation)
    {
        return flag || (dRelation.trackerRep.representedCreature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.BodySizeGenerationType is not SizeGenerationType.StaticMin and not SizeGenerationType.StaticMax);
    }

    public static bool IsNotSmallFood(bool flag, RelationshipTracker.DynamicRelationship dRelation)
    {
        return flag || (dRelation.trackerRep.representedCreature.creatureTemplate.breedParameters is CustomCentiBreedParams props && !props.Flags.Get(SmallFood));
    }
}