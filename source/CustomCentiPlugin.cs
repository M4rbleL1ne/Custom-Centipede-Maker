global using static CustomCentisMod.CustomCentiPlugin;
using BepInEx;
using Fisobs.Core;
using MonoMod.RuntimeDetour;
using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using System.Diagnostics.CodeAnalysis;
using MonoMod.Cil;
using Mono.Cecil.Cil;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace CustomCentisMod;

[BepInPlugin(K_ID, nameof(CustomCentisMod), K_VERSION), BepInDependency("io.github.dual.fisobs")]
public sealed class CustomCentiPlugin : BaseUnityPlugin
{
    internal const BindingFlags K_ALL_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
    internal const string K_ID = "lb-fgf-m4r-ik.custom-centis", K_VERSION = "10.0.0";
    [AllowNull] internal static Dictionary<string, CustomCentiCritob> s_dict;
    internal static HashSet<CreatureTemplate.Type>? s_majorCreatures = [];

    CustomCentiPlugin() { }

    internal void OnEnable()
    {
        s_logger = Logger;
        var locate = Assembly.GetExecutingAssembly().Location.Replace("CustomCentis.dll", "Example.tp");
        if (!File.Exists(locate))
        {
            var strm = new StreamWriter(locate, false);
            strm.Write(GenerateExample());
            strm.Close();
        }
        /*On.MultiplayerUnlocks.ctor += (orig, self, progression, allLevels) =>
        {
            try
            {
                orig(self, progression, allLevels);
            }
            catch (Exception e)
            {
                s_logger.LogError(e);
                s_logger.LogWarning(ExtEnum<CreatureTemplate.Type>.values.Count);
                s_logger.LogWarning(self.creaturesUnlockedForLevelSpawn.Length);
                s_logger.LogWarning(MultiplayerUnlocks.CreatureUnlockList.Count);
                s_logger.LogWarning(CreatureTemplate.Type.values.Count);
                /*for (int l = 0; l < self.unlockedBatches.Count; l++)
                {
                    for (int m = 0; m < self.unlockedBatches[l].creatures.Count; m++)
                    {
                        if (self.unlockedBatches[l].creatures[m].Index != -1)
                        {
                            if (self.unlockedBatches[l].creatures[m].Index >= self.creaturesUnlockedForLevelSpawn.Length)
                                s_logger.LogWarning(self.unlockedBatches[l].creatures[m]);
                        }
                    }
                }*
                foreach (var creatureUnlock in MultiplayerUnlocks.CreatureUnlockList)
                {
                    var i = (int)MultiplayerUnlocks.SymbolDataForSandboxUnlock(creatureUnlock).critType;
                    if (self.SandboxItemUnlocked(creatureUnlock) && (i >= self.creaturesUnlockedForLevelSpawn.Length || i < 0))
                        s_logger.LogWarning(MultiplayerUnlocks.SymbolDataForSandboxUnlock(creatureUnlock).critType);
                }
            }
        };*/
        //On.Menu.MainMenu.ctor += s_On_MainMenu_ctor;
        On.StaticWorld.InitStaticWorld += s_On_StaticWorld_InitStaticWorld;
        On.ShelterDoor.IsThisBigCreatureForShelter += s_On_ShelterDoor_IsThisBigCreatureForShelter;
        On.ArenaCreatureSpawner.IsMajorCreature += s_On_ArenaCreatureSpawner_IsMajorCreature;
        On.Player.EatMeatOmnivoreGreenList += s_On_Player_EatMeatOmnivoreGreenList;
        IL.ScavengerAI.IUseARelationshipTracker_UpdateDynamicRelationship += s_IL_ScavengerAI_IUseARelationshipTracker_UpdateDynamicRelationship;
        IL.BigSpiderAI.IUseARelationshipTracker_UpdateDynamicRelationship += s_IL_BigSpiderAI_IUseARelationshipTracker_UpdateDynamicRelationship;
        IL.Player.SlugcatGrab += s_IL_Player_SlugcatGrab;
        On.Player.IsCreatureLegalToHoldWithoutStun += s_On_Player_IsCreatureLegalToHoldWithoutStun;
        IL.Spear.HitSomething += s_IL_Spear_HitSomething;
        On.MoreSlugcats.SlugNPCAI.GetFoodType += s_On_SlugNPCAI_GetFoodType;
        IL.Player.CanMaulCreature += s_IL_Player_CanMaulCreature;
        var tp = typeof(Centipede);
        new Hook(tp.GetMethod("get_AquacentiSwim", K_ALL_FLAGS), s_On_Centipede_get_AquacentiSwim);
        new Hook(tp.GetMethod("get_AutomaticPickUp", K_ALL_FLAGS), s_On_Centipede_get_AutomaticPickUp);
        new Hook(tp.GetMethod("get_Edible", K_ALL_FLAGS), s_On_Centipede_get_Edible);
        new Hook(tp.GetMethod("get_FoodPoints", K_ALL_FLAGS), s_On_Centipede_get_FoodPoints);
        IL.Centipede.ctor += s_IL_Centipede_ctor;
        On.Centipede.AccessibleTile_IntVector2 += s_On_Centipede_AccessibleTile_IntVector2;
        IL.Centipede.Act += s_IL_Centipede_Act;
        On.Centipede.ClimbableTile_IntVector2 += s_On_Centipede_ClimbableTile_IntVector2;
        IL.Centipede.Collide += s_IL_Centipede_Collide;
        IL.Centipede.Crawl += s_IL_Centipede_Crawl;
        IL.Centipede.Fly += s_IL_Centipede_Fly;
        On.Centipede.GenerateSize += s_On_Centipede_GenerateSize;
        IL.Centipede.Shock += s_IL_Centipede_Shock;
        On.Centipede.ShortCutColor += s_On_Centipede_ShortCutColor;
        IL.Centipede.Stun += s_IL_Centipede_Stun;
        IL.Centipede.Update += s_IL_Centipede_Update;
        IL.Centipede.Violence += s_IL_Centipede_Violence;
        On.Centipede.SpearStick += s_On_Centipede_SpearStick;
        On.CentipedeAI.DoIWantToShockCreature += s_On_CentipedeAI_DoIWantToShockCreature;
        IL.CentipedeAI.ctor += s_IL_CentipedeAI_ctor;
        IL.CentipedeAI.AnnoyingCollision += s_IL_CentipedeAI_AnnoyingCollision;
        IL.CentipedeAI.IdleScore += s_IL_CentipedeAI_IdleScore;
        On.CentipedeAI.IUseARelationshipTracker_CreateTrackedCreatureState += s_On_CentipedeAI_IUseARelationshipTracker_CreateTrackedCreatureState;
        IL.CentipedeAI.IUseARelationshipTracker_UpdateDynamicRelationship += s_IL_CentipedeAI_IUseARelationshipTracker_UpdateDynamicRelationship;
        On.CentipedeAI.TravelPreference += s_On_CentipedeAI_TravelPreference;
        IL.CentipedeAI.Update += s_IL_CentipedeAI_Update;
        IL.CentipedeAI.VisualScore += s_IL_CentipedeAI_VisualScore;
        IL.CentipedePather.HeuristicForCell += s_IL_CentipedePather_HeuristicForCell;
        tp = typeof(CentipedeGraphics);
        new Hook(tp.GetMethod("get_TotalSprites", K_ALL_FLAGS), s_On_CentipedeGraphics_get_TotalSprites);
        new Hook(tp.GetMethod("get_ShellColor", K_ALL_FLAGS), s_On_CentipedeGraphics_get_ShellColor);
        new Hook(tp.GetMethod("get_SecondaryShellColor", K_ALL_FLAGS), s_On_CentipedeGraphics_get_SecondaryShellColor);
        IL.CentipedeGraphics.ctor += s_IL_CentipedeGraphics_ctor;
        IL.CentipedeGraphics.ApplyPalette += s_IL_CentipedeGraphics_ApplyPalette;
        IL.CentipedeGraphics.Update += s_IL_CentipedeGraphics_Update;
        IL.CentipedeGraphics.WingPos += s_IL_CentipedeGraphics_WingPos;
        IL.CentipedeGraphics.WingSprite += s_IL_CentipedeGraphics_WingSprite;
        On.CentipedeGraphics.WhiskerLength += s_On_CentipedeGraphics_WhiskerLength;
        IL.CentipedeGraphics.InitiateSprites += s_IL_CentipedeGraphics_InitiateSprites;
        IL.CentipedeGraphics.DrawSprites += s_IL_CentipedeGraphics_DrawSprites;
        IL.SporeCloud.Update += s_IL_SporeCloud_Update;
        On.RainWorld.OnModsInit += s_On_RainWorld_OnModsInit;
        On.RainWorld.PostModsInit += s_On_RainWorld_PostModsInit;
        On.RainWorld.OnModsDisabled += s_On_RainWorld_OnModsDisabled;
        On.Menu.Remix.MixedUI.UIconfig._UndoCallChanges += s_On_UIconfig__UndoCallChanges;
        new Hook(typeof(OpTextBox).GetMethod("set_maxLength", K_ALL_FLAGS), s_On_OpTextBox_set_maxLength);
        On.Menu.Remix.MixedUI.OpTextBox.KeyboardAccept += s_On_OpTextBox_KeyboardAccept;
        On.Player.Collide += s_On_Player_Collide;
        On.CreatureTemplate.CreatureRelationship_CreatureTemplate += s_On_CreatureTemplate_CreatureRelationship_CreatureTemplate;
        IL.Menu.Remix.ConfigMenuTab.ButtonManager.Update += s_IL_ButtonManager_Update;
        On.MultiplayerUnlocks.ctor += s_On_MultiplayerUnlocks_ctor;
        IL.AbstractCreature.setCustomFlags += s_IL_AbstractCreature_setCustomFlags;
        On.ModManager.ModFolderHasDLLContent += s_On_ModManager_ModFolderHasDLLContent;
    }

    internal static bool On_ModManager_ModFolderHasDLLContent(On.ModManager.orig_ModFolderHasDLLContent orig, string folder) => orig(folder) || Directory.Exists(Path.Combine(folder, "CustomCentis"));

    internal static void IL_AbstractCreature_setCustomFlags(ILContext il)
    {
        var c = new ILCursor(il);
        for (var i = 1; i <= 2; i++)
        {
            if (c.TryGotoNext(MoveType.After,
                s_MatchLdfld_World_region))
                c.EmitCentiCall(nameof(CustomCentiCalls.SetCustomFlagsRegionNullCheck));
            else
                s_logger.LogError($"Couldn't ILHook AbstractCreature.setCustomFlags (part {i})!");
        }
    }

    public static void On_MultiplayerUnlocks_ctor(On.MultiplayerUnlocks.orig_ctor orig, MultiplayerUnlocks self, PlayerProgression progression, List<string> allLevels)
    {
        try
        {
            orig(self, progression, allLevels);
        }
        catch (Exception ex)
        {
            s_logger.LogError("Error while creating multiplayer unlocks! If it's an IndexOutOfRangeException, it's likely that an unlock wasn't properly registered! It is recommended to restart the game to fix any missing unlocks.");
            s_logger.LogError(ex);
        }
    }

    /*public static void On_MainMenu_ctor(On.Menu.MainMenu.orig_ctor orig, Menu.MainMenu self, ProcessManager manager, bool showRegionSpecificBkg)
    {
        foreach (var pair in s_dict)
        {
            if (pair.Value?.Props is CustomCentiBreedParams props)
            {
                if (props.CreatureType is CreatureTemplate.Type tp && tp.Index < 0)
                {
                    CreatureTemplate.Type.values.AddEntry(tp.value);
                    tp.index = CreatureTemplate.Type.values.Count - 1;
                    ++CreatureTemplate.Type.valuesVersion;
                }
                if (props.UnlockID is MultiplayerUnlocks.SandboxUnlockID id && id.Index < 0)
                {
                    MultiplayerUnlocks.SandboxUnlockID.values.AddEntry(id.value);
                    id.index = MultiplayerUnlocks.SandboxUnlockID.values.Count - 1;
                    ++MultiplayerUnlocks.SandboxUnlockID.valuesVersion;
                }
                if (props.SmallUnlockID is MultiplayerUnlocks.SandboxUnlockID sId && sId.Index < 0)
                {
                    MultiplayerUnlocks.SandboxUnlockID.values.AddEntry(sId.value);
                    sId.index = MultiplayerUnlocks.SandboxUnlockID.values.Count - 1;
                    ++MultiplayerUnlocks.SandboxUnlockID.valuesVersion;
                }
                if (props.BigUnlockID is MultiplayerUnlocks.SandboxUnlockID bId && bId.Index < 0)
                {
                    MultiplayerUnlocks.SandboxUnlockID.values.AddEntry(bId.value);
                    bId.index = MultiplayerUnlocks.SandboxUnlockID.values.Count - 1;
                    ++MultiplayerUnlocks.SandboxUnlockID.valuesVersion;
                }
            }
        }
        orig(self, manager, showRegionSpecificBkg);
    }*/

    public static void IL_ButtonManager_Update(ILContext il)
    {
        var c = new ILCursor(il);
        if (c.TryGotoNext(
            s_MatchLdarg_0,
            s_MatchLdfld_ButtonManager_saveButton,
            s_MatchLdfld_UIfocusable_greyedOut))
        {
            c.Emit(OpCodes.Ldarg_0)
             .EmitCentiCall(nameof(CustomCentiCalls.MakeSaveButtonGrey));
        }
        else
            s_logger.LogError("Couldn't ILHook ButtonManager.Update!");
    }

    public static void On_StaticWorld_InitStaticWorld(On.StaticWorld.orig_InitStaticWorld orig)
    {
        orig();
        if (s_dict is Dictionary<string, CustomCentiCritob> d)
        {
            foreach (var x in d)
            {
                if (x.Value?.Props is CustomCentiBreedParams props && !props._alreadySetRels)
                    CustomCentiTemplates.SetParentRelationships(props);
            }
        }
    }

    public static CreatureTemplate.Relationship On_CreatureTemplate_CreatureRelationship_CreatureTemplate(On.CreatureTemplate.orig_CreatureRelationship_CreatureTemplate orig, CreatureTemplate self, CreatureTemplate crit)
    {
        var res = orig(self, crit);
        if (self.breedParameters is CustomCentiBreedParams props && props._realRelationships.TryGetValue(new() { _a = self.type, _b = crit.type }, out var rel))
        {
            res.type = rel._type;
            res.intensity = rel._intensity;
        }
        else if (crit.breedParameters is CustomCentiBreedParams props2 && props2._realRelationships.TryGetValue(new() { _a = self.type, _b = crit.type }, out var rel2))
        {
            res.type = rel2._type;
            res.intensity = rel2._intensity;
        }
        return res;
    }

    public static void On_RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        for (var i = 0; i < newlyDisabledMods.Length; i++)
        {
            if (newlyDisabledMods[i].id == K_ID)
            {
                if (s_dict is not null)
                {
                    foreach (var pair in s_dict)
                    {
                        var val = pair.Value;
                        val.ItemProps = null!;
                        if (val.Props is CustomCentiBreedParams props)
                        {
                            props._critob = null;
                            props._template = null;
                            if (props.UnlockID is MultiplayerUnlocks.SandboxUnlockID unlockID)
                            {
                                if (MultiplayerUnlocks.CreatureUnlockList is List<MultiplayerUnlocks.SandboxUnlockID> list && list.Contains(unlockID))
                                    list.Remove(unlockID);
                                unlockID.Unregister();
                                props.UnlockID = null;
                            }
                            if (props.CreatureType is CreatureTemplate.Type tp)
                            {
                                tp.Unregister();
                                props.CreatureType = null!;
                            }
                            val.Props = null!;
                        }
                    }
                    s_dict = null;
                }
                s_majorCreatures = null;
                break;
            }
        }
    }

    public static void On_RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        if (MachineConnector._registeredOIs.TryGetValue(K_ID, out var value) && value is not CustomCentiInterface)
            MachineConnector.SetRegisteredOI(K_ID, new CustomCentiInterface());
    }

    public static void On_RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
    {
        orig(self);
        s_dict ??= [];
        var mods = ModManager.ActiveMods;
        try
        {
            for (var i = 0; i < mods.Count; i++)
            {
                var path = Path.Combine(mods[i].path, "CustomCentis");
                if (!Directory.Exists(path))
                    continue;
                var files = Directory.GetFiles(path);
                for (var j = 0; j < files.Length; j++)
                {
                    var file = files[j];
                    var fnm = Path.GetFileName(file);
                    if (fnm.EndsWith(".tp") && !CustomCentiTemplates.AlreadyExists(fnm))
                    {
                        fnm = fnm.Replace(".tp", string.Empty);
                        var lines = File.ReadAllLines(file);
                        var c = new CustomCentiBreedParams(new(fnm, true)) { Path = file, _tempPath = file };
                        for (var k = 0; k < lines.Length; k++)
                        {
                            var line = lines[k];
                            if (line.Length > 0 && line[0] != Comment)
                            {
                                var sAr = line.UndoBind();
                                if (sAr.Length > 1)
                                {
                                    string s0 = sAr[0], s1 = sAr[1];
                                    if (s_fieldHashDict.TryGetValue(s0, out var s0Hash))
                                    {
                                        if (k == 0 && s0Hash == K_Inherit && CustomCentiTemplates.s_templateHash.TryGetValue(s1, out var s1Hash))
                                        {
                                            if (s1Hash == CustomCentiTemplates.K_AquaCenti && !ModManager.DLCShared)
                                            {
                                                c.ParentType = CreatureTemplate.Type.Centipede;
                                                c._nonMSCAqua = true;
                                            }
                                            else
                                            {
                                                var parent = c.ParentType = new(s1);
                                                if (parent.Index == -1)
                                                    c.ParentType = null;
                                            }
                                            CustomCentiTemplates.SetParentFields(c, s1Hash);
                                        }
                                        else
                                            c[s0Hash] = s1;
                                    }
                                }
                            }
                        }
                        var critob = new CustomCentiCritob(c);
                        s_dict[fnm] = critob;
                        Content.Register(critob);
                    }
                    else if (fnm.EndsWith(".png"))
                    {
                        fnm = fnm.Replace(".png", string.Empty);
                        if (!Futile.atlasManager.DoesContainAtlas(fnm))
                        {
                            var pth = Path.Combine("CustomCentis", fnm);
                            if (File.Exists(Path.Combine(path, fnm + ".txt")))
                                Futile.atlasManager.ActuallyLoadAtlasOrImage(fnm, pth + Futile.resourceSuffix, pth + Futile.resourceSuffix);
                            else
                                Futile.atlasManager.ActuallyLoadAtlasOrImage(fnm, pth + Futile.resourceSuffix, string.Empty);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            s_logger.LogError(e);
        }
    }

    internal void OnDisable()
    {
        CustomCentiCalls.Dispose();
        InternalMatch.Dispose();
        InternalEnable.Dispose();
        CustomCentiInterface.Dispose();
        InternalMisc.Dispose();
        CustomCentiHooks.Dispose();
    }
}