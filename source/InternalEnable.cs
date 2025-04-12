global using static CustomCentisMod.InternalEnable;
using MonoMod.Cil;
using UnityEngine;
using System;
using Menu.Remix.MixedUI;

namespace CustomCentisMod;

static class InternalEnable
{
    internal static ILContext.Manipulator
        s_IL_Player_SlugcatGrab = IL_Player_SlugcatGrab,
        s_IL_Spear_HitSomething = IL_Spear_HitSomething,
        s_IL_Player_CanMaulCreature = IL_Player_CanMaulCreature,
        s_IL_Centipede_ctor = IL_Centipede_ctor,
        s_IL_Centipede_Act = IL_Centipede_Act,
        s_IL_Centipede_Collide = IL_Centipede_Collide,
        s_IL_Centipede_Crawl = IL_Centipede_Crawl,
        s_IL_Centipede_Fly = IL_Centipede_Fly,
        s_IL_Centipede_Shock = IL_Centipede_Shock,
        s_IL_Centipede_Stun = IL_Centipede_Stun,
        s_IL_Centipede_Update = IL_Centipede_Update,
        s_IL_Centipede_Violence = IL_Centipede_Violence,
        s_IL_CentipedeAI_ctor = IL_CentipedeAI_ctor,
        s_IL_CentipedeAI_AnnoyingCollision = IL_CentipedeAI_AnnoyingCollision,
        s_IL_CentipedeAI_IdleScore = IL_CentipedeAI_IdleScore,
        s_IL_CentipedeAI_IUseARelationshipTracker_UpdateDynamicRelationship = IL_CentipedeAI_IUseARelationshipTracker_UpdateDynamicRelationship,
        s_IL_CentipedeAI_Update = IL_CentipedeAI_Update,
        s_IL_CentipedeAI_VisualScore = IL_CentipedeAI_VisualScore,
        s_IL_CentipedePather_HeuristicForCell = IL_CentipedePather_HeuristicForCell,
        s_IL_CentipedeGraphics_ctor = IL_CentipedeGraphics_ctor,
        s_IL_CentipedeGraphics_ApplyPalette = IL_CentipedeGraphics_ApplyPalette,
        s_IL_CentipedeGraphics_Update = IL_CentipedeGraphics_Update,
        s_IL_CentipedeGraphics_WingPos = IL_CentipedeGraphics_WingPos,
        s_IL_CentipedeGraphics_WingSprite = IL_CentipedeGraphics_WingSprite,
        s_IL_CentipedeGraphics_InitiateSprites = IL_CentipedeGraphics_InitiateSprites,
        s_IL_CentipedeGraphics_DrawSprites = IL_CentipedeGraphics_DrawSprites,
        s_IL_ScavengerAI_IUseARelationshipTracker_UpdateDynamicRelationship = IL_ScavengerAI_IUseARelationshipTracker_UpdateDynamicRelationship,
        s_IL_BigSpiderAI_IUseARelationshipTracker_UpdateDynamicRelationship = IL_BigSpiderAI_IUseARelationshipTracker_UpdateDynamicRelationship,
        s_IL_SporeCloud_Update = IL_SporeCloud_Update,
        s_IL_ButtonManager_Update = IL_ButtonManager_Update,
        s_IL_AbstractCreature_setCustomFlags = IL_AbstractCreature_setCustomFlags;
    internal static On.Player.hook_IsCreatureLegalToHoldWithoutStun s_On_Player_IsCreatureLegalToHoldWithoutStun = On_Player_IsCreatureLegalToHoldWithoutStun;
    internal static On.MoreSlugcats.SlugNPCAI.hook_GetFoodType s_On_SlugNPCAI_GetFoodType = On_SlugNPCAI_GetFoodType;
    internal static Func<Func<Centipede, bool>, Centipede, bool>
        s_On_Centipede_get_AquacentiSwim = On_Centipede_get_AquacentiSwim,
        s_On_Centipede_get_AutomaticPickUp = On_Centipede_get_AutomaticPickUp,
        s_On_Centipede_get_Edible = On_Centipede_get_Edible;
	internal static Func<Func<Centipede, int>, Centipede, int> s_On_Centipede_get_FoodPoints = On_Centipede_get_FoodPoints;
    internal static On.Centipede.hook_AccessibleTile_IntVector2 s_On_Centipede_AccessibleTile_IntVector2 = On_Centipede_AccessibleTile_IntVector2;
    internal static On.Centipede.hook_ClimbableTile_IntVector2 s_On_Centipede_ClimbableTile_IntVector2 = On_Centipede_ClimbableTile_IntVector2;
    internal static On.Centipede.hook_GenerateSize s_On_Centipede_GenerateSize = On_Centipede_GenerateSize;
    internal static On.Centipede.hook_ShortCutColor s_On_Centipede_ShortCutColor = On_Centipede_ShortCutColor;
    internal static On.Centipede.hook_SpearStick s_On_Centipede_SpearStick = On_Centipede_SpearStick;
    internal static On.CentipedeAI.hook_IUseARelationshipTracker_CreateTrackedCreatureState s_On_CentipedeAI_IUseARelationshipTracker_CreateTrackedCreatureState = On_CentipedeAI_IUseARelationshipTracker_CreateTrackedCreatureState;
    internal static On.CentipedeAI.hook_TravelPreference s_On_CentipedeAI_TravelPreference = On_CentipedeAI_TravelPreference;
    internal static Func<Func<CentipedeGraphics, int>, CentipedeGraphics, int> s_On_CentipedeGraphics_get_TotalSprites = On_CentipedeGraphics_get_TotalSprites;
	internal static Func<Func<CentipedeGraphics, Color>, CentipedeGraphics, Color>
        s_On_CentipedeGraphics_get_ShellColor = On_CentipedeGraphics_get_ShellColor,
        s_On_CentipedeGraphics_get_SecondaryShellColor = On_CentipedeGraphics_get_SecondaryShellColor;
    internal static On.CentipedeGraphics.hook_WhiskerLength s_On_CentipedeGraphics_WhiskerLength = On_CentipedeGraphics_WhiskerLength;
    internal static On.Player.hook_EatMeatOmnivoreGreenList s_On_Player_EatMeatOmnivoreGreenList = On_Player_EatMeatOmnivoreGreenList;
    internal static On.ArenaCreatureSpawner.hook_IsMajorCreature s_On_ArenaCreatureSpawner_IsMajorCreature = On_ArenaCreatureSpawner_IsMajorCreature;
    internal static On.ShelterDoor.hook_IsThisBigCreatureForShelter s_On_ShelterDoor_IsThisBigCreatureForShelter = On_ShelterDoor_IsThisBigCreatureForShelter;
    internal static On.CentipedeAI.hook_DoIWantToShockCreature s_On_CentipedeAI_DoIWantToShockCreature = On_CentipedeAI_DoIWantToShockCreature;
    internal static On.RainWorld.hook_PostModsInit s_On_RainWorld_PostModsInit = On_RainWorld_PostModsInit;
    internal static On.RainWorld.hook_OnModsInit s_On_RainWorld_OnModsInit = On_RainWorld_OnModsInit;
    internal static On.RainWorld.hook_OnModsDisabled s_On_RainWorld_OnModsDisabled = On_RainWorld_OnModsDisabled;
    internal static On.Menu.Remix.MixedUI.OpTextBox.hook_KeyboardAccept s_On_OpTextBox_KeyboardAccept = On_OpTextBox_KeyboardAccept;
    internal static On.Menu.Remix.MixedUI.UIconfig.hook__UndoCallChanges s_On_UIconfig__UndoCallChanges = On_UIconfig__UndoCallChanges;
    internal static Action<Action<OpTextBox, int>, OpTextBox, int> s_On_OpTextBox_set_maxLength = On_OpTextBox_set_maxLength;
    internal static On.Player.hook_Collide s_On_Player_Collide = On_Player_Collide;
    internal static On.CreatureTemplate.hook_CreatureRelationship_CreatureTemplate s_On_CreatureTemplate_CreatureRelationship_CreatureTemplate = On_CreatureTemplate_CreatureRelationship_CreatureTemplate;
    internal static On.StaticWorld.hook_InitStaticWorld s_On_StaticWorld_InitStaticWorld = On_StaticWorld_InitStaticWorld;
    internal static On.MultiplayerUnlocks.hook_ctor s_On_MultiplayerUnlocks_ctor = On_MultiplayerUnlocks_ctor;
    internal static On.ModManager.hook_ModFolderHasDLLContent s_On_ModManager_ModFolderHasDLLContent = On_ModManager_ModFolderHasDLLContent;
    //internal static On.Menu.MainMenu.hook_ctor s_On_MainMenu_ctor = On_MainMenu_ctor;

    internal static void Dispose()
    {
        s_IL_Player_SlugcatGrab = null!;
        s_IL_Spear_HitSomething = null!;
        s_IL_Player_CanMaulCreature = null!;
        s_IL_Centipede_ctor = null!;
        s_IL_Centipede_Act = null!;
        s_IL_Centipede_Collide = null!;
        s_IL_Centipede_Crawl = null!;
        s_IL_Centipede_Fly = null!;
        s_IL_Centipede_Shock = null!;
        s_IL_Centipede_Stun = null!;
        s_IL_Centipede_Update = null!;
        s_IL_Centipede_Violence = null!;
        s_IL_CentipedeAI_ctor = null!;
        s_IL_CentipedeAI_AnnoyingCollision = null!;
        s_IL_CentipedeAI_IdleScore = null!;
        s_IL_CentipedeAI_IUseARelationshipTracker_UpdateDynamicRelationship = null!;
        s_IL_CentipedeAI_Update = null!;
        s_IL_CentipedeAI_VisualScore = null!;
        s_IL_CentipedePather_HeuristicForCell = null!;
        s_IL_CentipedeGraphics_ctor = null!;
        s_IL_CentipedeGraphics_ApplyPalette = null!;
        s_IL_CentipedeGraphics_Update = null!;
        s_IL_CentipedeGraphics_WingPos = null!;
        s_IL_CentipedeGraphics_WingSprite = null!;
        s_IL_CentipedeGraphics_InitiateSprites = null!;
        s_IL_CentipedeGraphics_DrawSprites = null!;
        s_IL_ScavengerAI_IUseARelationshipTracker_UpdateDynamicRelationship = null!;
        s_IL_BigSpiderAI_IUseARelationshipTracker_UpdateDynamicRelationship = null!;
        s_On_Player_IsCreatureLegalToHoldWithoutStun = null!;
        s_On_SlugNPCAI_GetFoodType = null!;
        s_On_Centipede_get_AquacentiSwim = null!;
        s_On_Centipede_get_AutomaticPickUp = null!;
        s_On_Centipede_get_Edible = null!;
        s_On_Centipede_get_FoodPoints = null!;
        s_On_Centipede_AccessibleTile_IntVector2 = null!;
        s_On_Centipede_ClimbableTile_IntVector2 = null!;
        s_On_Centipede_GenerateSize = null!;
        s_On_Centipede_ShortCutColor = null!;
        s_On_Centipede_SpearStick = null!;
        s_On_CentipedeAI_IUseARelationshipTracker_CreateTrackedCreatureState = null!;
        s_On_CentipedeAI_TravelPreference = null!;
        s_On_CentipedeGraphics_get_TotalSprites = null!;
        s_On_CentipedeGraphics_get_ShellColor = null!;
        s_On_CentipedeGraphics_get_SecondaryShellColor = null!;
        s_On_CentipedeGraphics_WhiskerLength = null!;
        s_On_Player_EatMeatOmnivoreGreenList = null!;
        s_On_ArenaCreatureSpawner_IsMajorCreature = null!;
        s_On_ShelterDoor_IsThisBigCreatureForShelter = null!;
        s_On_CentipedeAI_DoIWantToShockCreature = null!;
        s_On_RainWorld_PostModsInit = null!;
        s_On_RainWorld_OnModsDisabled = null!;
        s_On_RainWorld_OnModsInit = null!;
        s_On_OpTextBox_KeyboardAccept = null!;
        s_On_UIconfig__UndoCallChanges = null!;
        s_On_OpTextBox_set_maxLength = null!;
        s_On_Player_Collide = null!;
        s_On_CreatureTemplate_CreatureRelationship_CreatureTemplate = null!;
        s_On_StaticWorld_InitStaticWorld = null!;
        s_IL_SporeCloud_Update = null!;
        s_IL_ButtonManager_Update = null!;
        s_On_MultiplayerUnlocks_ctor = null!;
        s_IL_AbstractCreature_setCustomFlags = null!;
        s_On_ModManager_ModFolderHasDLLContent = null!;
        //s_On_MainMenu_ctor = null!;
    }
}