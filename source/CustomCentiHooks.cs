global using static CustomCentisMod.CustomCentiHooks;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;
using System;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;
using Menu.Remix.MixedUI;
using System.Runtime.CompilerServices;

namespace CustomCentisMod;

public static class CustomCentiHooks
{
    internal static int s_loc;
    internal static ILLabel? s_label;

    public static void On_Player_Collide(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        if (self.Consious && !self.isNPC && self.FoodInStomach < self.MaxFoodInStomach && otherObject is Centipede c && c.grabbedBy?.Count is null or 0 && c.abstractCreature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.Flags.Get(SmallFood | AutomaticPickUp) && self.grasps is Creature.Grasp[] g && g.Length >= 2)
        {
            if (g[0] is null)
                self.SlugcatGrab(c, 0);
            else if (g[1] is null)
                self.SlugcatGrab(c, 1);
        }
        orig(self, otherObject, myChunk, otherChunk);
    }

    public static void On_OpTextBox_set_maxLength(Action<OpTextBox, int> orig, OpTextBox self, int value)
    {
        if (self is not LongStringOpTextBox)
            orig(self, value);
    }

    public static void On_UIconfig__UndoCallChanges(On.Menu.Remix.MixedUI.UIconfig.orig__UndoCallChanges orig, UIconfig self)
    {
        if (self is LongStringOpTextBox t)
        {
            var bigVal = t._value;
            if (bigVal.Length > t._staticLength)
            {
                var l = bigVal.Length - t._staticLength;
                t._temp = bigVal.Remove(l, t._staticLength);
                t._value = bigVal.Remove(0, l);
            }
            else
                t._temp = string.Empty;
            (s_OnValueUpdate.GetValue(t) as OnValueChangeHandler)?.Invoke(t, t._temp + t._value, t._lastTemp + t.lastValue);
            (s_OnValueChanged.GetValue(t) as OnValueChangeHandler)?.Invoke(t, t._temp + t._value, t._lastTemp + t.lastValue);
            t._lastTemp = t._temp;
            t.lastValue = t._value;
        }
        else
            orig(self);
    }

    public static void On_OpTextBox_KeyboardAccept(On.Menu.Remix.MixedUI.OpTextBox.orig_KeyboardAccept orig, OpTextBox self, char input)
    {
        orig(self, input);
        if (self is LongStringOpTextBox t && input is '\b' && t._temp?.Length > 0)
        {
            var l = t._temp.Length - 1;
            self._value = t._temp[l].ToString() + self._value;
            t._temp = t._temp.Remove(l, 1);
        }
    }

    public static bool On_CentipedeAI_DoIWantToShockCreature(On.CentipedeAI.orig_DoIWantToShockCreature orig, CentipedeAI self, AbstractCreature critter)
    {
        return (self.creature.creatureTemplate.breedParameters is not CustomCentiBreedParams props || props.Flags.Get(WantsToShock)) && orig(self, critter);
    }

    public static bool On_ShelterDoor_IsThisBigCreatureForShelter(On.ShelterDoor.orig_IsThisBigCreatureForShelter orig, AbstractCreature creature)
    {
        return orig(creature) || (creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.Flags.Get(TooBigForShelter));
    }

    public static bool On_ArenaCreatureSpawner_IsMajorCreature(On.ArenaCreatureSpawner.orig_IsMajorCreature orig, CreatureTemplate.Type type)
    {
        return orig(type) || s_majorCreatures?.Contains(type) is true;
    }

    public static bool On_Player_EatMeatOmnivoreGreenList(On.Player.orig_EatMeatOmnivoreGreenList orig, Player self, Creature crit)
    {
        return orig(self, crit) || (crit.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(EdibleByOmnivores));
    }

    internal static void IL_ScavengerAI_IUseARelationshipTracker_UpdateDynamicRelationship(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdsfld_CreatureTemplate_Type_Centipede,
            s_MatchCall_Any))
        {
            c.Emit(OpCodes.Ldarg_1)
             .EmitCentiCall(nameof(CustomCentiCalls.IsNotSmallFood));
        }
    }

    internal static void IL_BigSpiderAI_IUseARelationshipTracker_UpdateDynamicRelationship(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdsfld_CreatureTemplate_Type_Centipede,
            s_MatchCall_Any))
        {
            c.Emit(OpCodes.Ldarg_1)
             .EmitCentiCall(nameof(CustomCentiCalls.DoesNotHaveAStaticSize));
        }
    }

    internal static void IL_Player_SlugcatGrab(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchIsinst_Centipede,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_1)
             .Emit(OpCodes.Isinst, typeof(Creature))
             .EmitCentiCall(nameof(CustomCentiCalls.IsSmallFood));
        }
    }

    public static bool On_Player_IsCreatureLegalToHoldWithoutStun(On.Player.orig_IsCreatureLegalToHoldWithoutStun orig, Player self, Creature grabCheck)
    {
        if (grabCheck is Centipede c && c.Template.breedParameters is CustomCentiBreedParams props)
            return props.Flags.Get(SmallFood);
        return orig(self, grabCheck);
    }

    internal static void IL_Spear_HitSomething(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdlocAndRetrieveLocIndex,
            s_MatchIsinst_Centipede,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldloc, il.Body.Variables[s_loc])
             .EmitCentiCall(nameof(CustomCentiCalls.IsSmallFood));
        }
    }

    public static MoreSlugcats.SlugNPCAI.Food On_SlugNPCAI_GetFoodType(On.MoreSlugcats.SlugNPCAI.orig_GetFoodType orig, MoreSlugcats.SlugNPCAI self, PhysicalObject food)
    {
        if (food is Centipede c && c.Template.breedParameters is CustomCentiBreedParams props)
        {
            if (props.Flags.Get(SmallFood))
                return MoreSlugcats.SlugNPCAI.Food.SmallCentipede;
            return MoreSlugcats.SlugNPCAI.Food.Centipede;
        }
        return orig(self, food);
    }

    internal static void IL_Player_CanMaulCreature(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchCallOrCallvirt_Centipede_get_Edible))
        {
            c.Emit(OpCodes.Ldarg_1)
             .EmitCentiCall(nameof(CustomCentiCalls.IsSmallFood));
        }
        else
            s_logger.LogError("Couldn't ILHook Player.CanMaulCreature!");
    }

    public static bool On_Centipede_get_AquacentiSwim(Func<Centipede, bool> orig, Centipede self)
    {
        return orig(self) || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Swimming) && self.Submersion > .5f && !self.flying);
    }

    public static bool On_Centipede_get_AutomaticPickUp(Func<Centipede, bool> orig, Centipede self)
    {
        return orig(self) || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(AutomaticPickUp));
    }

    public static bool On_Centipede_get_Edible(Func<Centipede, bool> orig, Centipede self)
    {
        return orig(self) || (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(SmallFood));
    }

    public static int On_Centipede_get_FoodPoints(Func<Centipede, int> orig, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            return Above0(props.FoodPoints);
        return orig(self);
    }

    internal static void IL_Centipede_ctor(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.InitiateMeat));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.ctor (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewarr_BodyChunk))
        {
            var prev = c.Prev;
            prev.OpCode = OpCodes.Ldarg_0;
            prev.Operand = null;
            c.EmitCentiCall(nameof(CustomCentiCalls.InitiateChunks))
             .Emit(OpCodes.Newarr, typeof(BodyChunk));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.ctor (part 2)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchCall_Mathf_Lerp,
            s_MatchCall_Mathf_Pow,
            s_MatchCall_Mathf_Lerp,
            s_MatchStlocAndRetrieveLocIndex))
        {
            var vars = il.Method.Body.Variables;
            c.Emit(OpCodes.Ldarg_0)
             .Emit(OpCodes.Ldloc, vars[s_loc - 1])
             .Emit(OpCodes.Ldloc, vars[s_loc])
             .EmitCentiCall(nameof(CustomCentiCalls.InitiateChunkRads))
             .Emit(OpCodes.Stloc, vars[s_loc]);
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.ctor (part 3)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Red)
        && c.TryGotoNext(
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.InitiateChunkMasses));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.ctor (part 4)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.DoesNotHaveShellParticles));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.ctor (part 5)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdcR4_0_985))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetDeadShellChance));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.ctor (part 6)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_PhysicalObject_BodyChunkConnection))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.InitiateChunkConnections));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.ctor (part 7)!");
        c.Index = il.Body.Instructions.Count - 1;
        c.Emit(OpCodes.Ldarg_0)
         .EmitCentiCall(nameof(CustomCentiCalls.InitiateLastParams));
    }

    public static bool On_Centipede_AccessibleTile_IntVector2(On.Centipede.orig_AccessibleTile_IntVector2 orig, Centipede self, IntVector2 testPos)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Flying) && !self.flying)
            return self.RatherClimbThanFly(testPos);
        return orig(self, testPos);
    }

    internal static void IL_Centipede_Act(ILContext il)
    {
        var c = new ILCursor(il);
        for (var i = 1; i < 3; i++)
        {
            if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
            {
                c.Emit(OpCodes.Ldarg_0)
                 .EmitCentiCall(nameof(CustomCentiCalls.IsFlyingAndNotMovingInWater));
            }
            else
                s_logger.LogError($"Couldn't ILHook Centipede.Act (part {i})!");
        }
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.IsFlyingOrSwimming));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Act (part 3)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.SetMaxBuoyancy));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Act (part 4)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.SetMinBuoyancy));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Act (part 5)!");
    }

    public static bool On_Centipede_ClimbableTile_IntVector2(On.Centipede.orig_ClimbableTile_IntVector2 orig, Centipede self, IntVector2 testPos)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Flying) && !self.flying)
            return self.RatherClimbThanFly(testPos);
        return orig(self, testPos);
    }

    internal static void IL_Centipede_Collide(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .Emit<Creature>(OpCodes.Callvirt, "get_abstractCreature")
             .EmitCentiCall(nameof(CustomCentiCalls.DoesNotDetectAnnoyingCollisions));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Collide (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.IsFlying));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Collide (part 2)!");
    }

    internal static void IL_Centipede_Crawl(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Red)
        && c.TryGotoNext(MoveType.After,
            s_MatchLdcR4_1))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetVelocityFactor));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Crawl (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesSmallHeadMoveType));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Crawl (part 2)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing)
        && c.TryGotoNext(MoveType.After,
            s_MatchLdcR4_1))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetHeadVelocityFactor));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Crawl (part 3)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.IsFlying));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Crawl (part 4)!");
        if (c.TryGotoNext(
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.EmitCentiCall(nameof(CustomCentiCalls.SetHeadGlobalVelocityFactor))
             .Emit(OpCodes.Ldarg_0);
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Crawl (part 5)!");
    }

    internal static void IL_Centipede_Fly(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0)
         .EmitCentiCall(nameof(CustomCentiCalls.SetFlyingIfUnderWater));
        for (var i = 1; i < 3; i++)
        {
            if (c.TryGotoNext(MoveType.After,
                s_MatchLdarg_0,
                s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
            {
                c.Emit(OpCodes.Ldarg_0)
                 .EmitCentiCall(nameof(CustomCentiCalls.IsSwimmingAndMovingInWater));
            }
            else
                s_logger.LogError($"Couldn't ILHook Centipede.Fly (part {i})!");
        }
    }

    public static float On_Centipede_GenerateSize(On.Centipede.orig_GenerateSize orig, AbstractCreature abstrCrit)
    {
        if (abstrCrit.creatureTemplate.breedParameters is CustomCentiBreedParams props)
        {
            float res = 0f, min = Above0(props.MinSize), max = Above0(props.MaxSize);
            var state = Random.state;
            Random.InitState(abstrCrit.ID.RandomSeed);
            switch (props.BodySizeGenerationType)
            {
                case SizeGenerationType.RandomRange:
                    res = Lerp(min, max, Random.value);
                    break;
                case SizeGenerationType.StaticMin:
                    res = min;
                    break;
                case SizeGenerationType.StaticMax:
                    res = max;
                    break;
                case SizeGenerationType.FromWorldString:
                    if (abstrCrit.spawnData is string st && st.Length > 2)
                    {
                        if (!float.TryParse(st.Substring(1, st.Length - 2), NumberStyles.Any, CultureInfo.InvariantCulture, out res))
                            goto default;
                        res = Lerp(min, max, res);
                    }
                    break;
                default:
                    res = Lerp(min, max, (float)Math.Pow(Random.value, 1.5d));
                    break;
            }
            Random.state = state;
            return res;
        }
        return orig(abstrCrit);
    }

    internal static void IL_Centipede_Shock(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_Color))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetShockColor));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Shock (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesAquaCentiShockType));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Shock (part 2)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_Color))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetShockColor));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Shock (part 3)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesSmallShockType));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Shock (part 4)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchCallOrCallvirt_PhysicalObject_get_TotalMass))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetShockResistanceReductionFactor));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Shock (part 5)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_Color))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetShockColor));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Shock (part 6)!");
    }

    public static Color On_Centipede_ShortCutColor(On.Centipede.orig_ShortCutColor orig, Centipede self)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props)
            return props.ShortCutColor;
        return orig(self);
    }

    internal static void IL_Centipede_Stun(ILContext il)
    {
        var c = new ILCursor(il);
        for (var i = 1; i < 3; i++)
        {
            if (c.TryGotoNext(MoveType.After,
                s_MatchLdarg_0,
                s_MatchCallOrCallvirt_Centipede_get_Centiwing))
            {
                c.Emit(OpCodes.Ldarg_0)
                 .EmitCentiCall(nameof(CustomCentiCalls.IsWeakToStun));
            }
            else
                s_logger.LogError($"Couldn't ILHook Centipede.Stun (part {i})!");
        }
    }

    internal static void IL_Centipede_Update(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasGlowingHead));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Update (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_Color))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetHeadGlowMaxColor));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Update (part 2)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.CanShockWhenGrabbed));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Update (part 3)!");
        c.Index = il.Body.Instructions.Count - 1;
        c.Emit(OpCodes.Ldarg_0)
         .EmitCentiCall(nameof(CustomCentiCalls.SetGlobalVelocityFactorsAndShockCharge));
    }

    internal static void IL_Centipede_Violence(ILContext il)
    {
        var c = new ILCursor(il);
        var prms = il.Method.Parameters;
        var prm = 0;
        while (prms[prm].Name != "damage")
            ++prm;
        c.Emit(OpCodes.Ldarg_S, prms[prm])
         .Emit(OpCodes.Ldarg_0)
         .EmitCentiCall(nameof(CustomCentiCalls.GetDamageReductionFactor))
         .Emit(OpCodes.Starg_S, prms[prm]);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.DoesNotHaveShellParticles));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Violence (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesCentiwingResistanceType));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Violence (part 2)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasShields));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Violence (part 3)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_CentipedeShell))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetCustomShell));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Violence (part 4)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasShields));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Violence (part 5)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasShields));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Violence (part 6)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesRedResistanceType));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Violence (part 7)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasShieldsAndParticles));
        }
        else
            s_logger.LogError("Couldn't ILHook Centipede.Violence (part 8)!");
        for (var i = 0; i < 2; i++)
        {
            if (c.TryGotoNext(MoveType.After,
                s_MatchLdarg_0,
                s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
            {
                c.Emit(OpCodes.Ldarg_0)
                 .EmitCentiCall(nameof(CustomCentiCalls.UsesAquaCentiResistanceType));
            }
            else
                s_logger.LogError($"Couldn't ILHook Centipede.Violence (part {9 + i})!");
        }
        c.Index = il.Body.Instructions.Count - 1;
        c.Emit(OpCodes.Ldarg_0)
         .EmitCentiCall(nameof(CustomCentiCalls.UpdateStun));
    }

    internal static void IL_CentipedeAI_ctor(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.SetStepsPerFrame));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.ctor (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.CanTrackNoise));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.ctor (part 2)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Red)
        && c.TryGotoNext(MoveType.After,
            s_MatchLdcR4_5))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetPreyTrackerPersistance));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.ctor (part 3)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing)
        && c.TryGotoNext(MoveType.After,
            s_MatchLdcR4_0_9))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetPreyTrackerWeight));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.ctor (part 4)!");
    }

    internal static void IL_CentipedeAI_AnnoyingCollision(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .Emit<ArtificialIntelligence>(OpCodes.Ldfld, "creature")
             .EmitCentiCall(nameof(CustomCentiCalls.DoesNotDetectAnnoyingCollisions));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.AnnoyingCollision!");
    }

    internal static void IL_CentipedeAI_IdleScore(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesAquaCentiIdleType));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.IdleScore (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesCentiwingIdleType));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.IdleScore (part 2)!");
    }

    public static RelationshipTracker.TrackedCreatureState On_CentipedeAI_IUseARelationshipTracker_CreateTrackedCreatureState(On.CentipedeAI.orig_IUseARelationshipTracker_CreateTrackedCreatureState orig, CentipedeAI self, RelationshipTracker.DynamicRelationship rel)
    {
        if (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && !props.Flags.Get(DetectsAnnoyingCollisions))
            return new();
        return orig(self, rel);
    }

    internal static void IL_CentipedeAI_IUseARelationshipTracker_UpdateDynamicRelationship(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesCentiwingDynamicRelationship));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.IUseARelationshipTracker.UpdateDynamicRelationship (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesRedDynamicRelationship));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.IUseARelationshipTracker.UpdateDynamicRelationship (part 2)!");
    }

    public static PathCost On_CentipedeAI_TravelPreference(On.CentipedeAI.orig_TravelPreference orig, CentipedeAI self, MovementConnection coord, PathCost cost)
    {
        if (self.creature.creatureTemplate.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Flying) && coord.destinationCoord.TileDefined && self.centipede is Centipede c && !c.AquacentiSwim)
        {
            if (!c.flying && !c.RatherClimbThanFly(coord.DestTile))
                return cost with { resistance = cost.resistance + 1000f };
            if (c.flying)
            {
                var tProx = c.room.aimap.getTerrainProximity(coord.destinationCoord);
                return cost with { resistance = cost.resistance + (tProx < 2 ? 0f : LerpMap(tProx, 1f, 6f, 500f, 0f)) };
            }
        }
        return orig(self, coord, cost);
    }

    internal static void IL_CentipedeAI_Update(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesRedPreyTracker));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.Update (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesCentiwingIdleType));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.Update (part 2)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesAquaCentiOrCentiwingIdleType));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.Update (part 3)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesCentiwingIdleType));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.Update (part 4)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesCentiwingExcitementType));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.Update (part 5)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesRedExcitementType));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.Update (part 6)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdsfld_CreatureTemplate_Type_Centipede))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetMyTemplateType));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.Update (part 7)!");
    }

    internal static void IL_CentipedeAI_VisualScore(ILContext il)
    {
        var c = new ILCursor(il);
        for (var i = 1; i < 3; i++)
        {
            if (c.TryGotoNext(MoveType.After,
                s_MatchLdarg_0,
                s_MatchLdfld_CentipedeAI_centipede,
                s_MatchCallOrCallvirt_Centipede_get_Red))
            {
                c.Emit(OpCodes.Ldarg_0)
                 .EmitCentiCall(nameof(CustomCentiCalls.UsesRedOrCentiwingVisualScore));
            }
            else
                s_logger.LogError($"Couldn't ILHook CentipedeAI.VisualScore (part {i})!");
        }
    }

    internal static void IL_CentipedePather_HeuristicForCell(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdfld_CentipedeAI_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.IsFlyingAndNotMovingInWaterForPather));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeAI.HeuristicForCell!");
    }

    public static int On_CentipedeGraphics_get_TotalSprites(Func<CentipedeGraphics, int> orig, CentipedeGraphics self)
    {
        var res = orig(self);
        if (self.centipede?.abstractCreature is AbstractCreature c && c.creatureTemplate.breedParameters is CustomCentiBreedParams props)
        {
            if (props.Flags.Get(AdditionalShellTexture))
                res += self.totSegs;
        }
        return res;
    }

    public static Color On_CentipedeGraphics_get_ShellColor(Func<CentipedeGraphics, Color> orig, CentipedeGraphics self)
    {
        if (self.centipede is Centipede c && c.Template.breedParameters is CustomCentiBreedParams props && props.Flags2.Get(ShellColorType_HasValue))
            return Lerp(c.room?.game?.cameras[0] is RoomCamera cam ? GetColorFromType(props.ShellColorType, cam.currentPalette, props.ShellColor) : Clamp01(props.ShellColor), self.blackColor, self.darkness);
        return orig(self);
    }

    public static Color On_CentipedeGraphics_get_SecondaryShellColor(Func<CentipedeGraphics, Color> orig, CentipedeGraphics self)
    {
        var res = orig(self);
        if (self.centipede is Centipede c && c.Template.breedParameters is CustomCentiBreedParams props)
        {
            if (props.Flags2.Get(SecondaryShellColorType_HasValue))
                res = Lerp(c.room?.game?.cameras[0] is RoomCamera cam ? GetColorFromType(props.SecondaryShellColorType, cam.currentPalette, props.SecondaryShellColor) : Clamp01(props.SecondaryShellColor), self.blackColor, self.darkness);
            var b = Above0(props.SecondaryShellColorBonus);
            if (b > 0f)
            {
                var state = Random.state;
                Random.InitState(c.abstractCreature.ID.RandomSeed);
                res = Lerp(res, new() { r = res.r + b, g = res.g + b, b = res.b + b, a = 1f }, Lerp(.1f, .2f, Random.value));
                Random.state = state;
            }
        }
        return res;
    }

    internal static void IL_CentipedeGraphics_ctor(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.InitiateWings));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.ctor (part 1)!");
        c.Index += 3;
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing)
        && c.TryGotoNext(MoveType.After,
            s_MatchLdcR4_1))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetLegLengthFactor));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.ctor (part 2)!");
        c.Index = il.Body.Instructions.Count - 1;
        c.Emit(OpCodes.Ldarg_0)
         .EmitCentiCall(nameof(CustomCentiCalls.InitiateHueAndSaturation));
    }

    internal static void IL_CentipedeGraphics_ApplyPalette(ILContext il)
    {
        var c = new ILCursor(il);
        c.Emit(OpCodes.Ldarg_0)
         .Emit(OpCodes.Ldarg_2)
         .EmitCentiCall(nameof(CustomCentiCalls.UpdateHueAndSaturation));
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_Color))
        {
            c.Emit(OpCodes.Ldarg_0)
             .Emit(OpCodes.Ldarg_2)
             .EmitCentiCall(nameof(CustomCentiCalls.GetHeadGlowMinColorWithRCam));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.ApplyPalette (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_Color))
        {
            c.Emit(OpCodes.Ldarg_0)
             .Emit(OpCodes.Ldarg_2)
             .EmitCentiCall(nameof(CustomCentiCalls.GetHeadGlowMaxColorWithRCam));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.ApplyPalette (part 2)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdfld_RoomPalette_blackColor))
        {
            c.Emit(OpCodes.Ldarg_0)
             .Emit(OpCodes.Ldarg_2)
             .EmitCentiCall(nameof(CustomCentiCalls.GetBodyBlackColor));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.ApplyPalette (part 3)!");
    }

    internal static void IL_CentipedeGraphics_Update(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdfld_CentipedeGraphics_bodyRotations,
            s_MatchLdcI4_0,
            s_MatchCallOrCallvirt_Array_GetLength)
        && c.TryGotoNext(MoveType.After,
            s_MatchLdfld_CentipedeGraphics_bodyRotations,
            s_MatchLdcI4_0,
            s_MatchCallOrCallvirt_Array_GetLength))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.BodyRotationsWhenFlyingFix));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.Update (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchNewobj_Color))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetFlashColor));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.Update (part 2)!");
    }

    internal static void IL_CentipedeGraphics_WingPos(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.IsSwimmingAndUnderWater));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.WingPos!");
    }

    internal static void IL_CentipedeGraphics_WingSprite(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasAdditionalShellSprite));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.WingSprite!");
    }

    public static float On_CentipedeGraphics_WhiskerLength(On.CentipedeGraphics.orig_WhiskerLength orig, CentipedeGraphics self, int part)
    {
        if (self.centipede.Template.breedParameters is CustomCentiBreedParams props)
        {
            var res = (part == 0 ? Above0(props.SmallWhiskerLength) : Above0(props.BigWhiskerLength)) * Above0(props.WhiskerLengthFactor);
            if (props.Flags.Get(BodySizeDependantWhisker))
                res *= Lerp(.5f, 1.5f, self.centipede.size);
            return res;
        }
        return orig(self, part);
    }

    internal static void IL_CentipedeGraphics_InitiateSprites(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdstr_AquapedeBody))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetAdditionalShellSpriteShader));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.InitiateSprites (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasAdditionalShellSprite));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.InitiateSprites (part 2)!");
        if (c.TryGotoNext(
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Red))
        {
            c.Emit(OpCodes.Ldarg_0)
             .Emit(OpCodes.Ldarg_1)
             .Emit(OpCodes.Ldarg_2)
             .EmitCentiCall(nameof(CustomCentiCalls.InitiateGraphics));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.InitiateSprites (part 3)!");
    }

    internal static void IL_CentipedeGraphics_DrawSprites(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Small))
        {
            c.Emit(OpCodes.Ldarg_0)
             .Emit<CentipedeGraphics>(OpCodes.Ldfld, "centipede")
             .EmitCentiCall(nameof(CustomCentiCalls.DoesNotHaveShellParticles));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.DrawSprites (part 1)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasAdditionalShellSprite));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.DrawSprites (part 2)!");
        for (var i = 0; i < 2; i++)
        {
            if (c.TryGotoNext(MoveType.After,
                s_MatchLdarg_0,
                s_MatchLdfld_CentipedeGraphics_centipede,
                s_MatchCallOrCallvirt_Centipede_get_Small))
            {
                c.Emit(OpCodes.Ldarg_0)
                 .EmitCentiCall(nameof(CustomCentiCalls.UsesSmallTube));
            }
            else
                s_logger.LogError($"Couldn't ILHook CentipedeGraphics.DrawSprites (part {3 + i})!");
        }
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Centiwing))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesCentiwingSegment));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.DrawSprites (part 5)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.UsesAquaCentiSegment));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.DrawSprites (part 6)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_Red)
        && c.TryGotoNext(MoveType.After,
            s_MatchLdcR4_1_5))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetShellScaleYFactor));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.DrawSprites (part 7)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasAdditionalShellSprite));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.DrawSprites (part 8)!");
        if (c.TryGotoNext(MoveType.After,
            s_MatchLdarg_0,
            s_MatchLdfld_CentipedeGraphics_centipede,
            s_MatchCallOrCallvirt_Centipede_get_AquaCenti))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.HasSwimmerLegs));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.DrawSprites (part 9)!");
        for (var i = 0; i < 2; i++)
        {
            if (c.TryGotoNext(MoveType.After,
                s_MatchLdarg_0,
                s_MatchLdfld_CentipedeGraphics_centipede,
                s_MatchCallOrCallvirt_Centipede_get_Red)
            && c.TryGotoNext(MoveType.After,
                s_MatchLdcR4_1))
            {
                c.Emit(OpCodes.Ldarg_0)
                 .EmitCentiCall(nameof(CustomCentiCalls.GetLegScaleXFactor));
            }
            else
                s_logger.LogError($"Couldn't ILHook CentipedeGraphics.DrawSprites (part {10 + i})!");
        }
        if (c.TryGotoNext(MoveType.After,
                s_MatchLdarg_0,
                s_MatchLdfld_CentipedeGraphics_centipede,
                s_MatchCallOrCallvirt_Centipede_get_Small)
           && c.TryGotoNext(MoveType.After,
                s_MatchLdcR4_1))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.GetWhiskerShapeFactor));
        }
        else
            s_logger.LogError("Couldn't ILHook CentipedeGraphics.DrawSprites (part 12)!");
        c.Index = il.Body.Instructions.Count - 1;
        c.Emit(OpCodes.Ldarg_0)
         .Emit(OpCodes.Ldarg_1)
         .Emit(OpCodes.Ldarg_2)
         .Emit(OpCodes.Ldarg_3)
         .Emit(OpCodes.Ldarg_S, il.Method.Parameters[4])
         .EmitCentiCall(nameof(CustomCentiCalls.UpdateGraphics));
    }

    public static bool On_Centipede_SpearStick(On.Centipede.orig_SpearStick orig, Centipede self, Weapon source, float dmg, BodyChunk chunk, PhysicalObject.Appendage.Pos appPos, Vector2 direction)
    {
        if (self.Template.breedParameters is CustomCentiBreedParams props && props.Flags.Get(Shields | ShellParticles) && chunk is not null && chunk.index >= 0 && chunk.index < self.CentiState.shells.Length && (chunk.index == self.shellJustFellOff || self.CentiState.shells[chunk.index]))
        {
            if (chunk.index == self.shellJustFellOff)
                self.shellJustFellOff = -1;
            return false;
        }
        return orig(self, source, dmg, chunk, appPos, direction);
    }

    internal static void IL_SporeCloud_Update(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(MoveType.After,
            s_MatchIsinst_InsectoidCreature))
            c.EmitCentiCall(nameof(CustomCentiCalls.SporeCloudInvulnerableCheck));
        else
            s_logger.LogError("Couldn't ILHook SporeCloud.Update!");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Dispose() => s_label = null;
}