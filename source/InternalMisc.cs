global using static CustomCentisMod.InternalMisc;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace CustomCentisMod;

static class InternalMisc
{
    [AllowNull] internal static ManualLogSource s_logger;
    internal static FieldInfo s_OnValueUpdate = typeof(UIconfig).GetField("OnValueUpdate", K_ALL_FLAGS),
        s_OnValueChanged = typeof(UIconfig).GetField("OnValueChanged", K_ALL_FLAGS),
        s_OnHeld = typeof(UIfocusable).GetField("OnHeld", K_ALL_FLAGS);
        //s_KillScore = typeof(SandboxUnlock).GetField(nameof(SandboxUnlock.KillScore), K_ALL_FLAGS),
        //s_KillScoreValue = typeof(KillScore).GetField(nameof(KillScore.Value), K_ALL_FLAGS),
        //s_KillScoreIsConfigurable = typeof(KillScore).GetField(nameof(KillScore.IsConfigurable), K_ALL_FLAGS);
    internal static Regex s_addSpaces = new("([a-z])([A-Z])"), s_A = new("^[ -~/s]+$");
    internal static FTextParams s_defTParams = new();
    internal static char[] s_sep = [Separator], s_arSep = [ArraySeparator], s_def = [Definition], s_meth = [Method];
    internal static Color s_blue = new() { b = 1f, a = 1f }, s_black = new() { a = 1f }, s_grey2 = Lerp(new() { r = .5f, g = .5f, b = .5f, a = 1f }, s_black, .5f),
        s_coolGreen = new() { r = .44706f, g = .8902f, b = .21961f, a = 1f }, s_coolRed = new() { r = 46f / 51f, g = .05490196f, b = .05490196f, a = 1f }, s_white = new() { r = 1f, g = 1f, b = 1f, a = 1f };
    internal static string[] s_cb = ["<cB>"];
    internal static string s_Kill_Centipede2 = "Kill_Centipede2",
        s_Kill_Centipede3 = "Kill_Centipede3",
        s_Kill_Centipede1 = "Kill_Centipede1",
        s_Kill_Centiwing = "Kill_Centiwing",
        s_centipede = "centipede",
        s_Current = "Current",
        s_Path = "Path",
        s_Save_File = "Save File",
        s_Save_Relationships = "Save Relationships",
        s_Reset = "Reset",
        s_Inherit = "Inherit",
        s_Alpha = "Alpha",
        s_Def = "------",
        s_ReloadArrowImage = "CustomCentiMenu_ReloadArrow",
        s_No_changes_detected = "No changes detected.",
        s_Changes_detected = "Changes detected! Restart your game for them to apply properly!",
        s_Apply_Parent = "Apply Parent";

    internal static void Dispose()
    {
        s_logger = null;
        s_OnValueUpdate = null!;
        s_OnValueChanged = null!;
        s_OnHeld = null!;
        s_addSpaces = null!;
        s_A = null!;
        s_defTParams = null!;
        s_sep = null!;
        s_arSep = null!;
        s_def = null!;
        s_meth = null!;
        s_cb = null!;
        s_Kill_Centipede2 = null!;
        s_Kill_Centipede3 = null!;
        s_Kill_Centipede1 = null!;
        s_Kill_Centiwing = null!;
        s_centipede = null!;
        s_Current = null!;
        s_Path = null!;
        s_Save_File = null!;
        s_Save_Relationships = null!;
        s_Reset = null!;
        s_Inherit = null!;
        s_Alpha = null!;
        s_Def = null!;
        s_ReloadArrowImage = null!;
        s_No_changes_detected = null!;
        s_Changes_detected = null!;
        s_Apply_Parent = null!;
        /*s_KillScore = null!;
        s_KillScoreValue = null!;
        s_KillScoreIsConfigurable = null!;*/
    }
}
