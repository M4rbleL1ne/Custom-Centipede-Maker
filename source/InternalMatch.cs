global using static CustomCentisMod.InternalMatch;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace CustomCentisMod;

static class InternalMatch
{
    internal static Func<Instruction, bool>
        s_MatchLdarg_0 = MatchLdarg_0,
        s_MatchCallOrCallvirt_Centipede_get_Small = MatchCallOrCallvirt_Centipede_get_Small,
        s_MatchCallOrCallvirt_Centipede_get_Centiwing = MatchCallOrCallvirt_Centipede_get_Centiwing,
        s_MatchCallOrCallvirt_Centipede_get_Red = MatchCallOrCallvirt_Centipede_get_Red,
        s_MatchCallOrCallvirt_Centipede_get_AquaCenti = MatchCallOrCallvirt_Centipede_get_AquaCenti,
        s_MatchLdfld_CentipedeAI_centipede = MatchLdfld_CentipedeAI_centipede,
        s_MatchLdfld_CentipedeGraphics_centipede = MatchLdfld_CentipedeGraphics_centipede,
        s_MatchNewobj_Color = MatchNewobj_Color,
        s_MatchCall_Mathf_Lerp = MatchCall_Mathf_Lerp,
        s_MatchIsinst_Centipede = MatchIsinst_Centipede,
        s_MatchCallOrCallvirt_Centipede_get_Edible = MatchCallOrCallvirt_Centipede_get_Edible,
        s_MatchLdlocAndRetrieveLocIndex = MatchLdlocAndRetrieveLocIndex,
        s_MatchCall_Mathf_Pow = MatchCall_Mathf_Pow,
        s_MatchStlocAndRetrieveLocIndex = MatchStlocAndRetrieveLocIndex,
        s_MatchNewarr_BodyChunk = MatchNewarr_BodyChunk,
        s_MatchNewobj_PhysicalObject_BodyChunkConnection = MatchNewobj_PhysicalObject_BodyChunkConnection,
        s_MatchNewobj_CentipedeShell = MatchNewobj_CentipedeShell,
        s_MatchLdsfld_CreatureTemplate_Type_Centipede = MatchLdsfld_CreatureTemplate_Type_Centipede,
        s_MatchLdfld_RoomPalette_blackColor = MatchLdfld_RoomPalette_blackColor,
        s_MatchLdstr_AquapedeBody = MatchLdstr_AquapedeBody,
        s_MatchCall_Any = MatchCall_Any,
        s_MatchCallOrCallvirt_AbstractRoom_AddEntity = MatchCallOrCallvirt_AbstractRoom_AddEntity,
        s_MatchLdcR4_1_5 = MatchLdcR4_1_5,
        s_MatchLdcR4_0_9 = MatchLdcR4_0_9,
        s_MatchLdcR4_0_985 = MatchLdcR4_0_985,
        s_MatchLdcR4_1 = MatchLdcR4_1,
        s_MatchLdcR4_5 = MatchLdcR4_5,
        s_MatchCallOrCallvirt_PhysicalObject_get_TotalMass = MatchCallOrCallvirt_PhysicalObject_get_TotalMass,
        s_MatchIsinst_InsectoidCreature = MatchIsinst_InsectoidCreature,
        s_MatchCallOrCallvirt_Any = MatchCallOrCallvirt_Any,
        s_MatchCallOrCallvirt_AbstractCreature_get_realizedCreature = MatchCallOrCallvirt_AbstractCreature_get_realizedCreature,
        s_MatchLdfld_AbstractRoom_creatures = MatchLdfld_AbstractRoom_creatures,
        s_MatchCallOrCallvirt_Room_get_abstractRoom = MatchCallOrCallvirt_Room_get_abstractRoom,
        s_MatchLdfld_UpdatableAndDeletable_room = MatchLdfld_UpdatableAndDeletable_room,
        s_MatchBrfalseAndRetrieveLabel = MatchBrfalseAndRetrieveLabel,
        s_MatchLdfld_CentipedeGraphics_bodyRotations = MatchLdfld_CentipedeGraphics_bodyRotations,
        s_MatchLdcI4_0 = MatchLdcI4_0,
        s_MatchCallOrCallvirt_Array_GetLength = MatchCallOrCallvirt_Array_GetLength,
        s_MatchLdfld_ButtonManager_saveButton = MatchLdfld_ButtonManager_saveButton,
        s_MatchLdfld_UIfocusable_greyedOut = MatchLdfld_UIfocusable_greyedOut,
        s_MatchLdfld_World_region = MatchLdfld_World_region;

    internal static void Dispose()
    {
        s_MatchLdarg_0 = null!;
        s_MatchCallOrCallvirt_Centipede_get_Small = null!;
        s_MatchCallOrCallvirt_Centipede_get_Centiwing = null!;
        s_MatchCallOrCallvirt_Centipede_get_Red = null!;
        s_MatchCallOrCallvirt_Centipede_get_AquaCenti = null!;
        s_MatchLdfld_CentipedeAI_centipede = null!;
        s_MatchLdfld_CentipedeGraphics_centipede = null!;
        s_MatchNewobj_Color = null!;
        s_MatchCall_Mathf_Lerp = null!;
        s_MatchIsinst_Centipede = null!;
        s_MatchCallOrCallvirt_Centipede_get_Edible = null!;
        s_MatchLdlocAndRetrieveLocIndex = null!;
        s_MatchCall_Mathf_Pow = null!;
        s_MatchStlocAndRetrieveLocIndex = null!;
        s_MatchNewarr_BodyChunk = null!;
        s_MatchNewobj_PhysicalObject_BodyChunkConnection = null!;
        s_MatchNewobj_CentipedeShell = null!;
        s_MatchLdsfld_CreatureTemplate_Type_Centipede = null!;
        s_MatchLdfld_RoomPalette_blackColor = null!;
        s_MatchLdstr_AquapedeBody = null!;
        s_MatchCall_Any = null!;
        s_MatchCallOrCallvirt_AbstractRoom_AddEntity = null!;
        s_MatchLdcR4_1_5 = null!;
        s_MatchLdcR4_0_9 = null!;
        s_MatchLdcR4_0_985 = null!;
        s_MatchLdcR4_1 = null!;
        s_MatchLdcR4_5 = null!;
        s_MatchCallOrCallvirt_PhysicalObject_get_TotalMass = null!;
        s_MatchIsinst_InsectoidCreature = null!;
        s_MatchCallOrCallvirt_Any = null!;
        s_MatchCallOrCallvirt_AbstractCreature_get_realizedCreature = null!;
        s_MatchLdfld_AbstractRoom_creatures = null!;
        s_MatchCallOrCallvirt_Room_get_abstractRoom = null!;
        s_MatchLdfld_UpdatableAndDeletable_room = null!;
        s_MatchBrfalseAndRetrieveLabel = null!;
        s_MatchLdfld_CentipedeGraphics_bodyRotations = null!;
        s_MatchLdcI4_0 = null!;
        s_MatchCallOrCallvirt_Array_GetLength = null!;
        s_MatchLdfld_ButtonManager_saveButton = null!;
        s_MatchLdfld_UIfocusable_greyedOut = null!;
        s_MatchLdfld_World_region = null!;
    }

    internal static bool MatchLdarg_0(Instruction x) => x.MatchLdarg(0);

    internal static bool MatchCallOrCallvirt_Centipede_get_Small(Instruction x) => x.MatchCallOrCallvirt<Centipede>("get_Small");

    internal static bool MatchCallOrCallvirt_Centipede_get_Centiwing(Instruction x) => x.MatchCallOrCallvirt<Centipede>("get_Centiwing");

    internal static bool MatchCallOrCallvirt_Centipede_get_Red(Instruction x) => x.MatchCallOrCallvirt<Centipede>("get_Red");

    internal static bool MatchCallOrCallvirt_Centipede_get_AquaCenti(Instruction x) => x.MatchCallOrCallvirt<Centipede>("get_AquaCenti");

    internal static bool MatchLdfld_CentipedeAI_centipede(Instruction x) => x.MatchLdfld<CentipedeAI>(s_centipede);

    internal static bool MatchLdfld_CentipedeGraphics_centipede(Instruction x) => x.MatchLdfld<CentipedeGraphics>(s_centipede);

    internal static bool MatchNewobj_Color(Instruction x) => x.MatchNewobj<Color>();

    internal static bool MatchCall_Mathf_Lerp(Instruction x) => x.MatchCall<Mathf>("Lerp");

    internal static bool MatchLdcR4_1_5(Instruction x) => x.MatchLdcR4(1.5f);

    internal static bool MatchLdcR4_0_9(Instruction x) => x.MatchLdcR4(.9f);

    internal static bool MatchLdcR4_0_985(Instruction x) => x.MatchLdcR4(.985f);

    internal static bool MatchLdcR4_1(Instruction x) => x.MatchLdcR4(1f);

    internal static bool MatchLdcR4_5(Instruction x) => x.MatchLdcR4(5f);

    internal static bool MatchIsinst_Centipede(Instruction x) => x.MatchIsinst<Centipede>();

    internal static bool MatchCallOrCallvirt_Centipede_get_Edible(Instruction x) => x.MatchCallOrCallvirt<Centipede>("get_Edible");

    internal static bool MatchLdlocAndRetrieveLocIndex(Instruction x) => x.MatchLdloc(out s_loc);

    internal static bool MatchCall_Mathf_Pow(Instruction x) => x.MatchCall<Mathf>("Pow");

    internal static bool MatchStlocAndRetrieveLocIndex(Instruction x) => x.MatchStloc(out s_loc);

    internal static bool MatchNewarr_BodyChunk(Instruction x) => x.MatchNewarr<BodyChunk>();

    internal static bool MatchNewobj_PhysicalObject_BodyChunkConnection(Instruction x) => x.MatchNewobj<PhysicalObject.BodyChunkConnection>();

    internal static bool MatchNewobj_CentipedeShell(Instruction x) => x.MatchNewobj<CentipedeShell>();

    internal static bool MatchLdsfld_CreatureTemplate_Type_Centipede(Instruction x) => x.MatchLdsfld<CreatureTemplate.Type>("Centipede");

    internal static bool MatchLdfld_RoomPalette_blackColor(Instruction x) => x.MatchLdfld<RoomPalette>("blackColor");

    internal static bool MatchLdstr_AquapedeBody(Instruction x) => x.MatchLdstr("AquapedeBody");

    internal static bool MatchCall_Any(Instruction x) => x.MatchCall(out _);

    internal static bool MatchCallOrCallvirt_Any(Instruction x) => x.MatchCallOrCallvirt(out _);

    internal static bool MatchCallOrCallvirt_AbstractRoom_AddEntity(Instruction x) => x.MatchCallOrCallvirt<AbstractRoom>("AddEntity");

    internal static bool MatchCallOrCallvirt_AbstractCreature_get_realizedCreature(Instruction x) => x.MatchCallOrCallvirt<AbstractCreature>("get_realizedCreature");

    internal static bool MatchCallOrCallvirt_PhysicalObject_get_TotalMass(Instruction x) => x.MatchCallOrCallvirt<PhysicalObject>("get_TotalMass");

    internal static bool MatchIsinst_InsectoidCreature(Instruction x) => x.MatchIsinst<InsectoidCreature>();

    internal static bool MatchLdfld_AbstractRoom_creatures(Instruction x) => x.MatchLdfld<AbstractRoom>("creatures");

    internal static bool MatchCallOrCallvirt_Room_get_abstractRoom(Instruction x) => x.MatchCallOrCallvirt<Room>("get_abstractRoom");

    internal static bool MatchLdfld_UpdatableAndDeletable_room(Instruction x) => x.MatchLdfld<UpdatableAndDeletable>("room");

    internal static bool MatchBrfalseAndRetrieveLabel(Instruction x) => x.MatchBrfalse(out s_label);

    internal static bool MatchLdfld_CentipedeGraphics_bodyRotations(Instruction x) => x.MatchLdfld<CentipedeGraphics>("bodyRotations");

    internal static bool MatchLdcI4_0(Instruction x) => x.MatchLdcI4(0);

    internal static bool MatchCallOrCallvirt_Array_GetLength(Instruction x) => x.MatchCallOrCallvirt(typeof(Array).GetMethod("GetLength"));

    internal static bool MatchLdfld_ButtonManager_saveButton(Instruction x) => x.MatchLdfld<Menu.Remix.ConfigMenuTab.ButtonManager>("saveButton");

    internal static bool MatchLdfld_UIfocusable_greyedOut(Instruction x) => x.MatchLdfld<Menu.Remix.MixedUI.UIfocusable>("greyedOut");

    internal static bool MatchLdfld_World_region(Instruction x) => x.MatchLdfld<World>("region");
}