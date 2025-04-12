using RWCustom;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using Configurables = System.Collections.Generic.Dictionary<string, ConfigurableBase>;
using InteractiveUIElements = System.Collections.Generic.Dictionary<string, Menu.Remix.MixedUI.UIfocusable>;
using System.Text;
using Mono.Cecil.Cil;

namespace CustomCentisMod;

public sealed class CustomCentiInterface : OptionInterface
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ElemWrap : IEquatable<ElemWrap> // 8
    {
        internal UIelement _elem;

        public readonly bool Equals(ElemWrap e) => e._elem == _elem;

        public override readonly bool Equals(object obj) => obj is ElemWrap e && e._elem == _elem;

        public override readonly int GetHashCode() => _elem?.GetHashCode() ?? 0;

        public override readonly string ToString() => string.Empty;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageWrap : IEquatable<ImageWrap> // 8
    {
        internal ReloadArrow _img;

        public readonly bool Equals(ImageWrap i) => i._img == _img;

        public override readonly bool Equals(object obj) => obj is ImageWrap i && i._img == _img;

        public override readonly int GetHashCode() => _img?.GetHashCode() ?? 0;

        public override readonly string ToString() => string.Empty;
    }

    internal CustomCentiBreedParams? _current;
    const int K_MainMenu = 0, K_Readme = 1, K_General = 2, K_Movement = 3, K_Resistance = 4, K_Body = 5, K_Mind = 6, K_Electricity = 7, K_Shells = 8, K_Appendages = 9, K_Arena = 10, K_Relations = 11;
    readonly InteractiveUIElements _interUI = [];
    static Configurables s_configs = [];
    readonly List<ImageWrap> _rlds = [];
    [AllowNull] OpImage _centiIcon;
    [AllowNull] ElemWrap[] _rels;
    static Vector2 s_titleSz = new() { x = 600f, y = 20f }, s_titlePos = new() { x = 0f, y = 575f };
    static ListItem s_null = new(string.Empty, s_Def, 1);
    static ConfigAcceptableBase s_f01 = new ConfigAcceptableRange<float>(0f, 1f), s_fU = new ConfigAcceptableRange<float>(0f, 99999f),
        s_fS = new ConfigAcceptableRange<float>(-99999f, 99999f);
    internal static OnSignalHandler s_CustomCentiInterface_MoveToFront = CustomCentiInterface_MoveToFront;
    [AllowNull] readonly OnSignalHandler _CustomCentiInterface_SaveFile, _CustomCentiInterface_SaveRelations, _CustomCentiInterface_ResetFields,
        _CustomCentiInterface_SetParentFields, _CustomCentiInterface_MakeSaveFileGreen, _CustomCentiInterface_MakeSaveRelationshipsGreen,
        _CustomCentiInterface_MakeSaveFileRed2;
    [AllowNull] readonly OnValueChangeHandler _CustomCentiInterface_UpdateStringField, _CustomCentiInterface_UpdateIntField, _CustomCentiInterface_UpdateBoolField,
        _CustomCentiInterface_UpdateFloatField, _CustomCentiInterface_UpdateAlphaField, _CustomCentiInterface_UpdateColorField,
        _CustomCentiInterface_UpdateColorTypeField, _CustomCentiInterface_UpdateCurrent, _CustomCentiInterface_UpdateShaderField,
        _CustomCentiInterface_UpdateSpriteField, _CustomCentiInterface_UpdateRelationIntensityField, _CustomCentiInterface_UpdateRelationTypeField,
        _CustomCentiInterface_MakeArrowsRed, _CustomCentiInterface_MakeSaveFileRed, _CustomCentiInterface_MakeSaveRelationshipsRed;
    static OnValueChangeHandler s_CustomCentiInterface_MakeTextGrey = CustomCentiInterface_MakeTextGrey;
    static List<ListItem> s_palette =
    [
        new(nameof(PaletteColorType.Custom), 1),
        new(nameof(PaletteColorType.BlackColor), 2),
        new(nameof(PaletteColorType.WaterColor1), 3),
        new(nameof(PaletteColorType.WaterColor2), 4),
        new(nameof(PaletteColorType.WaterSurfaceColor1), 5),
        new(nameof(PaletteColorType.WaterSurfaceColor2), 6),
        new(nameof(PaletteColorType.WaterShineColor), 7),
        new(nameof(PaletteColorType.FogColor), 8),
        new(nameof(PaletteColorType.ShortCutSymbol), 9),
        new(nameof(PaletteColorType.SkyColor), 10),
        new(nameof(PaletteColorType.ShortcutColor1), 11),
        new(nameof(PaletteColorType.ShortcutColor2), 12),
        new(nameof(PaletteColorType.ShortcutColor3), 13)
    ],
    s_nullPalette =
    [
        s_null,
        new(nameof(PaletteColorType.Custom), 2),
        new(nameof(PaletteColorType.BlackColor), 3),
        new(nameof(PaletteColorType.WaterColor1), 4),
        new(nameof(PaletteColorType.WaterColor2), 5),
        new(nameof(PaletteColorType.WaterSurfaceColor1), 6),
        new(nameof(PaletteColorType.WaterSurfaceColor2), 7),
        new(nameof(PaletteColorType.WaterShineColor), 8),
        new(nameof(PaletteColorType.FogColor), 9),
        new(nameof(PaletteColorType.ShortCutSymbol), 10),
        new(nameof(PaletteColorType.SkyColor), 11),
        new(nameof(PaletteColorType.ShortcutColor1), 12),
        new(nameof(PaletteColorType.ShortcutColor2), 13),
        new(nameof(PaletteColorType.ShortcutColor3), 14)
    ],
    s_nullYesNo =
    [
        s_null,
        new("Yes", 2),
        new("No", 3)
    ];

    internal CustomCentiInterface()
    {
        _CustomCentiInterface_SaveFile = CustomCentiInterface_SaveFile;
        _CustomCentiInterface_SaveRelations = CustomCentiInterface_SaveRelations;
        _CustomCentiInterface_MakeArrowsRed = CustomCentiInterface_MakeArrowsRed;
        _CustomCentiInterface_UpdateStringField = CustomCentiInterface_UpdateStringField;
        _CustomCentiInterface_UpdateIntField = CustomCentiInterface_UpdateIntField;
        _CustomCentiInterface_UpdateBoolField = CustomCentiInterface_UpdateBoolField;
        _CustomCentiInterface_UpdateFloatField = CustomCentiInterface_UpdateFloatField;
        _CustomCentiInterface_UpdateAlphaField = CustomCentiInterface_UpdateAlphaField;
        _CustomCentiInterface_UpdateColorField = CustomCentiInterface_UpdateColorField;
        _CustomCentiInterface_UpdateColorTypeField = CustomCentiInterface_UpdateColorTypeField;
        _CustomCentiInterface_UpdateCurrent = CustomCentiInterface_UpdateCurrent;
        _CustomCentiInterface_UpdateShaderField = CustomCentiInterface_UpdateShaderField;
        _CustomCentiInterface_UpdateSpriteField = CustomCentiInterface_UpdateSpriteField;
        _CustomCentiInterface_UpdateRelationIntensityField = CustomCentiInterface_UpdateRelationIntensityField;
        _CustomCentiInterface_UpdateRelationTypeField = CustomCentiInterface_UpdateRelationTypeField;
        _CustomCentiInterface_ResetFields = CustomCentiInterface_ResetFields;
        _CustomCentiInterface_SetParentFields = CustomCentiInterface_SetParentFields;
        _CustomCentiInterface_MakeSaveFileRed = CustomCentiInterface_MakeSaveFileRed;
        _CustomCentiInterface_MakeSaveFileRed2 = CustomCentiInterface_MakeSaveFileRed2;
        _CustomCentiInterface_MakeSaveRelationshipsRed = CustomCentiInterface_MakeSaveRelationshipsRed;
        _CustomCentiInterface_MakeSaveFileGreen = CustomCentiInterface_MakeSaveFileGreen;
        _CustomCentiInterface_MakeSaveRelationshipsGreen = CustomCentiInterface_MakeSaveRelationshipsGreen;
    }

    public void DeactivateUI()
    {
        _centiIcon.color = s_grey2;
        _centiIcon.sprite.element = Futile.atlasManager.GetElementWithName(s_Kill_Centipede2);
        (_interUI[s_Save_File] as CustomOpSimpleButton)!.colorEdge = s_coolGreen;
        (_interUI[s_Save_Relationships] as CustomOpSimpleButton)!.colorEdge = s_coolGreen;
        foreach (var kvpair in _interUI)
        {
            if (kvpair.Key != s_Current)
            {
                var val = kvpair.Value;
                if (val is UIconfig cfg)
                    cfg.ResetAndDeactivate(this);
                else
                    val.greyedOut = true;
            }
        }
        var rlds = _rlds;
        int i;
        for (i = 0; i < rlds.Count; i++)
        {
            var r = rlds[i]._img;
            r.description = string.Empty;
        }
        var rels = _rels;
        for (i = 0; i < rels.Length; i++)
        {
            if (rels[i]._elem is UIconfig cfg)
                cfg.ResetAndDeactivate(this);
        }
        UpdateHueSaturationPreview(_current, true);
    }

    public void ReactivateUI()
    {
        if (_current is not CustomCentiBreedParams props)
        {
            (_interUI[s_Save_File] as CustomOpSimpleButton)!.colorEdge = s_coolGreen;
            (_interUI[s_Save_Relationships] as CustomOpSimpleButton)!.colorEdge = s_coolGreen;
        }
        else
        {
            (_interUI[s_Save_File] as CustomOpSimpleButton)!.colorEdge = props._propsChanged ? s_coolRed : s_coolGreen;
            (_interUI[s_Save_Relationships] as CustomOpSimpleButton)!.colorEdge = props._relationsChanged ? s_coolRed : s_coolGreen;
        }
        foreach (var kvpair in _interUI)
            kvpair.Value.greyedOut = false;
        var rlds = _rlds;
        var flag = _current is not CustomCentiBreedParams c || !c._requiresReloading;
        var descr = flag ? s_No_changes_detected : s_Changes_detected;
        var clr = flag ? s_coolGreen : s_coolRed;
        int i;
        for (i = 0; i < rlds.Count; i++)
        {
            var r = rlds[i]._img;
            r.color = clr;
            r.description = descr;
        }
        var rels = _rels;
        for (i = 0; i < rels.Length; i++)
        {
            if (rels[i]._elem is UIfocusable f)
                f.greyedOut = false;
        }
    }

    internal static void UpdateHueSaturationPreview(CustomCentiBreedParams? current, bool disable)
    {
        disable = disable || current is null;
        var prev = (Futile.atlasManager.GetAtlasWithName("CustomCentiMenu_HueSaturationPreview").texture as Texture2D)!;
        float hueMin = disable ? 0f : current!.HueMin,
            hueMax = disable ? 0f : current!.HueMax,
            satMin = disable ? 0f : Above0(current!.SaturationMin),
            satMax = disable ? 0f : Above0(current!.SaturationMax),
            l = disable ? .1f : .5f;
        for (var i = 0; i < 100; i++)
        {
            var h = Lerp(hueMin, hueMax, i * .01f);
            for (var j = 0; j < 101; j++)
                prev.SetPixel(i, j, Custom.HSL2RGB(h, Lerp(satMin, satMax, j * .01f), l));
        }
        prev.Apply();
    }

    public override unsafe void Initialize()
    {
        base.Initialize();
        var interUI = _interUI;
        interUI.Clear();
        _rlds.Clear();
        Tabs = [new(this, "Main Menu"),
            new(this, "Help"),
            new(this, "General"),
            new(this, "Movement"),
            new(this, "Resistance"),
            new(this, "Body"),
            new(this, "Mind"),
            new(this, "Electricity"),
            new(this, "Shells"),
            new(this, "Appendages"),
            new(this, "Arena"),
            new(this, "Relationships")];
        var tabs = Tabs;

        var tab = tabs[K_MainMenu];
        MakeTitle(tab, "Custom Centipede Maker");
        tab._AddItem(new CustomOpLabel(new() { x = 100f, y = 550f }, new() { x = 500f, y = 20f }, "V." + K_VERSION, FLabelAlignment.Center));
        tab._AddItem(new CustomOpLabel(new() { x = 20, y = 550f }, new() { x = 500f, y = 20f }, "by LB/M4rbleL1ne", FLabelAlignment.Center));
        tab._AddItem(new OpImage(new() { x = 320f, y = 240f }, "CustomCentiMenu_MainImage") { anchor = new() { x = .5f, y = .5f } });
        var list = new List<ListItem> { s_null };
        foreach (var kvpair in s_dict)
        {
            var val = kvpair.Value;
            if (val.Type is CreatureTemplate.Type tp)
                list.Add(new(tp.value, tp.Index + 2));
        }
        var button = new CustomOpSimpleButton(new() { x = 480f, y = 574f }, new() { x = 120f, y = 30f }, s_Save_File) { colorEdge = _current is not CustomCentiBreedParams props || !props._propsChanged ? s_coolGreen : s_coolRed, description = "Saves all changes to the centipede .tp file." };
        button.OnClick += _CustomCentiInterface_SaveFile;
        button.OnClick += _CustomCentiInterface_MakeSaveFileGreen;
        tab._AddItem(interUI[s_Save_File] = button);
        var button2 = new OpHoldButton(new() { x = 480f }, 60f, s_Reset) { colorEdge = s_coolRed, description = "Sets all fields to their default value, except \"Path\"." };
        button2.OnPressDone += _CustomCentiInterface_ResetFields;
        tab._AddItem(interUI[s_Reset] = button2);
        var button3 = new OpHoldButton(new() { x = 340f }, 60f, s_Apply_Parent) { colorEdge = s_coolRed, description = "Sets all fields to the default value of the current parent centipede, except \"Path\" and \"Inherit\". Requires \"Inherit\" not to be set to \"------\"." };
        button3.OnPressDone += _CustomCentiInterface_SetParentFields;
        tab._AddItem(interUI[s_Apply_Parent] = button3);
        var box = new CustomOpComboBox(GetOrCreateConfig(s_Current, string.Empty), new() { x = s_Current.GetTextSize() + 8f, y = 522f }, 200f, list) { description = "Choose the centipede you want to edit." };
        tab._AddItem(new CustomOpLabel(0f, 526f, "Current:") { bumpBehav = box.bumpBehav, description = "Choose the centipede you want to edit." });
        tab._AddItem(_centiIcon = new OpImage(new() { x = 300f, y = 440f }, s_Kill_Centipede2) { scale = new() { x = 3f, y = 3f }, anchor = new() { x = .5f, y = .5f } });
        box.OnValueChanged += _CustomCentiInterface_UpdateCurrent;
        tab._AddItem(interUI[s_Current] = box);
        MakeTextBox(interUI, tab, s_Path, 0f, 52f, 570f, true, "The current path to the centipede .tp file. Clicking on \"Save File\" with a new path will delete the previous file and make a new one using the new path.");
        list.Clear();
        list.Add(s_null);
        list.Add(new("Centipede", 2));
        list.Add(new("SmallCentipede", 3));
        list.Add(new("Centiwing", 4));
        list.Add(new("RedCentipede", 5));
        list.Add(new("AquaCenti", 6));
        MakeComboBox(interUI, tab, s_Inherit, list, 300f, 22f, string.Empty, 200f, "Choose a parent for your centipede. Your centipede cannot attack creatures who have the same parent.");
        //MakeReloadArrow(interUI, _rlds, tab, s_Inherit);
        var flag = _current is CustomCentiBreedParams c && c._requiresReloading;
        var arrow = new ReloadArrow(new() { x = 250f, y = 20f }, "CustomCentiMenu_BigReloadArrow") { color = flag ? s_coolRed : s_coolGreen, description = flag ? s_Changes_detected : s_No_changes_detected, _bumpBehav = button2.bumpBehav, _bumpBehav2 = button3.bumpBehav };
        tab._AddItem(arrow);
        _rlds.Add(new() { _img = arrow });

        tab = tabs[K_Readme];
        MakeTitle(tab, "Help");
        var readBox = new OpScrollBox(default, new() { x = 600f, y = 564f }, 1288f);
        tab._AddItem(readBox);
        var readText = new OpLabelLong(new() { x = 10f, y = 712f }, new() { x = 600f, y = 564f }, new StringBuilder("WHAT IS IT?")
            .AppendLine()
            .AppendLine()
            .AppendLine("A tool that lets you make your own centipede variants WITHOUT CODE.")
            .AppendLine()
            .AppendLine("TO CREATE A NEW CENTIPEDE:")
            .AppendLine()
            .AppendLine("1 - Make your own mod folder in Rain World/RainWorld_Data/StreamingAssets/mods.")
            .AppendLine()
            .AppendLine("2 - Create a CustomCentis folder inside it.")
            .AppendLine()
            .AppendLine("3 - Create a new text file whose name is the name of your centipede,")
            .AppendLine("    then rename the extension to .tp.")
            .AppendLine("    Don't forget to enable file extensions in your file explorer!")
            .AppendLine()
            .AppendLine("The path should be:")
            .AppendLine(" Rain World/RainWorld_Data/StreamingAssets/mods/mymod/CustomCentis/MyCenti.tp")
            .AppendLine(" (replace mymod with the name of your mod folder and MyCenti with the name of your centipede)")
            .AppendLine()
            .AppendLine("(Optional) - If you need additional images/atlases to be loaded, put them in the same folder.")
            .AppendLine()
            .AppendLine("(Optional) - You can make multiple .tp files if you need multiple custom centipedes.")
            .AppendLine()
            .AppendLine("4 - Launch the game.")
            .AppendLine()
            .AppendLine("5 - In the Remix menu, ENABLE YOUR MOD and APPLY mods.")
            .AppendLine()
            .AppendLine("6 - RESTART the game and you should now be able to edit your centipede(s).")
            .AppendLine("    Don't forget to SAVE using the Save File button in the Main Menu!")
            .AppendLine("    The BACK button DOES NOT SAVE your centipede!")
            .AppendLine()
            .AppendLine("IF YOUR CENTIPEDE STILL DOESN'T APPEAR:")
            .AppendLine()
            .AppendLine("- Double check your path.")
            .AppendLine("- Try to write mymod (the name of your mod folder) directly in")
            .AppendLine("  Rain World/RainWorld_Data/StreamingAssets/enabledMods.txt")
            .AppendLine("  IF IT IS NOT ALREADY THERE, then save and launch the game.")
            .AppendLine()
            .AppendLine("USER INFO:")
            .AppendLine()
            .AppendLine("- ALWAYS RESTART THE GAME AFTER ENABLING/DISABLING CUSTOM CENTIPEDE MODS")
            .AppendLine("if you want them to appear/disappear properly.")
            .AppendLine("- This mod SHOULD BE COMPATIBLE with other mods as long as they don't do cursed things.")
            .AppendLine("- If a sprite or shader field is invalid, THE DEFAULT CENTIPEDE ONE WILL BE USED.")
            .AppendLine("- If a field is empty (relationship, sprite, shader, grabability...),")
            .AppendLine("  THE DEFAULT CENTIPEDE VALUE WILL BE USED.")
            .AppendLine("  For relationships, the default value depends on the Inherit field.")
            .AppendLine("- Fields marked with a CIRCULAR ARROW require your to restart the game for changes to apply.")
            .AppendLine("- Some fields depend on some other fields.")
            .AppendLine("- The SHELLS of your centipede use the main color (hue and saturation) by default.")
            .AppendLine("  The fields Shell Color and Secondary Shell Color are used to OVERRIDE the color of the shells")
            .AppendLine("  if they are not empty.")
            .AppendLine("- If two custom centipedes using the SAME ID (the name of the .tp file) are enabled,")
            .AppendLine("  ONLY THE FIRST LOADED ONE WILL EXIST. Try to MAKE YOUR ID UNIQUE to avoid incompatibilities.")
            .AppendLine("- Don't forget to change the ALPHA SLIDER if you don't want a color to be TRANSPARENT.")
            .AppendLine("- TRANSPARENCY DOESN'T WORK FOR MIDGROUND SPRITES because of how Rain World works.")
            .AppendLine("- Using a pure black color or setting alpha to 0% makes a sprite INVISIBLE.")
            .AppendLine()
            .AppendLine("MODDER INFO:")
            .AppendLine()
            .AppendLine("- The mod DOESN'T REQUIRE More Slugcats to be enabled, but be careful not to use")
            .AppendLine("  More Slugcats sprites like the aquapede wings if you don't want your mod to depend on it.")
            .AppendLine("- To set spawns for your centipede in a region using the world file, use the NAME of the .tp file")
            .AppendLine("  (in the example path: MyCenti). It is CASE INSENSITIVE.")
            .AppendLine("- You can edit the .tp file manually, though it's not recommended.")
            .AppendLine("  Look at the Example.tp file found in the same folder as the CustomCentis.dll")
            .AppendLine("  (probably hidden in the Steam workshop folder).")
            .AppendLine("- To access the ExtEnum of your centipede while making a mod that depends on CustomCentis.dll,")
            .AppendLine("  use the NAME of the .tp file as the ONLY argument when calling the constructor of")
            .AppendLine("  CreatureTemplate.Type:")
            .AppendLine()
            .AppendLine("  var type = new CreatureTemplate.Type(\"MyCenti\");")
            .AppendLine("  // Try to save this instance in a static field not to create too much garbage!")
            .AppendLine("  // You can then set your static field to null in OnDisable for example.")
            .AppendLine("  // Don't register/unregister the ExtEnum!")
            .AppendLine()
            .AppendLine("- Custom centipedes are registered in PostModsInit. Don't try to use your ExtEnum before!")
            .AppendLine()
            .AppendLine("SPECIAL THANKS:")
            .AppendLine()
            .Append("Thanks to Balagaga and eli for testing.").ToString());
        if (readText._AddToScrollBox(readBox))
        {
            tab.AddItems(readText);
            readBox.items.Add(readText);
        }

        tab = tabs[K_General];
        MakeTitle(tab, "General");
        MakeCheckBoxes(interUI, tab, ["Small Food", "Automatic Pick Up", "Edible By Omnivores"], 0f, 0f, ["Whether or not the centipede should be eaten like a small centipede by the player.", "Whether or not the centipede should be automatically grabbed by the player when it is hungry. Requires \"Small Food\" to be set to true.", "Whether or not the centipede should be edible by omnivorous slugcats. Requires \"Small Food\" to be set to false."]);
        int* mins = stackalloc int[] { 0, 1, 0, 0 }, maxs = stackalloc int[] { 99999, 99999, 99999, 99999 };
        MakeIntBoxes(interUI, tab, ["Food Points", "Bites"], mins, maxs, 0f, 90f, ["The number of food points the centipede gives to the player after eating it. Requires \"Small Food\" to be set to true.", "The number of bites it takes for the player to eat the centipede. Requires \"Small Food\" to be set to true."]);
        list.Clear();
        list.Add(s_null);
        list.Add(new(nameof(Player.ObjectGrabability.CantGrab), 2));
        list.Add(new(nameof(Player.ObjectGrabability.OneHand), 3));
        list.Add(new(nameof(Player.ObjectGrabability.BigOneHand), 4));
        list.Add(new(nameof(Player.ObjectGrabability.TwoHands), 5));
        list.Add(new(nameof(Player.ObjectGrabability.Drag), 6));
        MakeComboBox(interUI, tab, "Grabability", list, 0f, 150f, string.Empty, 105f, "Whether or not the player can grab the centipede when it is alive and the way it is grabbed.");
        MakeComboBox(interUI, tab, "Throwable", s_nullYesNo, 0f, 180f, string.Empty, 65f, "Whether or not the player can throw the centipede. Requires \"Grabability\" not to be set to \"CantGrab\" nor \"------\".");
        MakeFloatBoxes(interUI, tab, ["Meat Min", "Meat Max", "Scaryness", "Dangerous To Player"], 0f, 210f, ["The minimum amount of food the centipede gives to the player after eating it. Requires \"Small Food\" to be set to false.", "The maximum amount of food the centipede gives to the player after eating it. Requires \"Small Food\" to be set to false.", "How much the centipede scares other creatures.", "How dangerous the centipede is to the player."]);
        MakeTextBox(interUI, tab, "Dev Name", 0f, 330f, 145f, true, "The name of the centipede on the DevTools map page.");
        MakeColorPicker(interUI, tab, "Dev Color", 0f, 360f, "The color of the name of the centipede on the DevTools map page.");

        tab = tabs[K_Movement];
        MakeTitle(tab, "Movement");
        MakeCheckBoxes(interUI, tab, ["Swimming", "Water Only"], 0f, 0f, ["Whether or not the centipede can swim like an AquaCenti.", "Whether or not the centipede can't move outside water."]);
        MakeFloatBoxes(interUI, tab, ["Lung Capacity", "Min Buoyancy", "Max Buoyancy"], 0f, 60f, ["How long the centipede can breath underwater.", "The minimum power of the upward push as the centipede swims.", "The maximum power of the upward push as the centipede swims."]);
        MakeCheckBoxes(interUI, tab, ["Forbid Standard Shortcut Entry", "Too Big For Shelter", "Blizzard Wanderer"], 0f, 150f, ["Whether or not the centipede should be unable to enter room entrance pipes.", "Whether or not the centipede should be moved out of the shelter if the player sleeps with it.", "Whether or not the centipede can stay during the blizzard."]);
        MakeComboBoxes(interUI, tab, ["Night Only", "Pre Cycle", "Ignores Cycle"], s_nullYesNo, 0f, 240f, string.Empty, 65f, ["Forces the centipede to appear only at night.", "Forces the centipede to appear only during pre-cycle events.", "Whether or not the centipede should ignore cycle time."]);
        list.Clear();
        list.Add(new(nameof(HeadMoveType.Normal), 1));
        list.Add(new(nameof(HeadMoveType.Small), 2));
        MakeComboBox(interUI, tab, "Head Velocity Type", list, 0f, 330f, nameof(HeadMoveType.Normal), 75f, "The way the centipede moves its head.");
        MakeFloatBoxes(interUI, tab, ["Head Velocity Factor", "Head Global Velocity Factor", "Velocity Factor", "Global Velocity Factor"], 0f, 360f, ["How fast the centipede moves its head.", "How fast the centipede moves its head whether it is conscious or not.", "How fast the centipede moves.", "How fast the centipede moves whether it is conscious or not."]);
        MakeCheckBox(interUI, tab, "Flying", 0f, 480f, "Whether or not the centipede can fly like a Centiwing.");
        MakeFloatBox(interUI, tab, "Global Flying Velocity Factor", 0f, 510f, "How fast the centipede flies. Requires \"Flying\" to be set to true.");

        tab = tabs[K_Resistance];
        MakeTitle(tab, "Resistance");
        MakeCheckBox(interUI, tab, "Easy Kill", 0f, 0f, "Whether or not the centipede should be killed instantly when injured, most of the time.");
        list.Clear();
        list.Add(new(nameof(ViolenceResistanceType.Normal), 1));
        list.Add(new(nameof(ViolenceResistanceType.Red), 2));
        list.Add(new(nameof(ViolenceResistanceType.AquaCenti), 3));
        list.Add(new(nameof(ViolenceResistanceType.Centiwing), 4));
        MakeComboBox(interUI, tab, "Resistance Type", list, 0f, 30f, nameof(ViolenceResistanceType.Normal), 90f, "The way the centipede reacts to injuries.");
        MakeFloatBoxes(interUI, tab, ["Base Damage Resistance", "Explosion Resistance", "Damage Reduction Factor"], 0f, 60f, ["How resistant the centipede is to damage.", "How resistant the centipede is to explosion damage.", "By how much damage should be reduced."]);
        MakeCheckBoxes(interUI, tab, ["Weak To Stun", "No Violence Stun"], 0f, 150f, ["Whether or not the centipede is stunned easily.", "Whether or not the centipede should be stunned when injured."]);
        MakeFloatBoxes(interUI, tab, ["Base Stun Resistance", "Explosion Stun Resistance"], 0f, 210f, ["How resistant the centipede is to damage stun.", "How resistant the centipede is to explosion damage stun."]);
        MakeCheckBox(interUI, tab, "WormGrass Immune", 0f, 270f, "Whether or not the centipede should ignore and be ignored by WormGrass.");
        MakeComboBoxes(interUI, tab, ["Lava Immune", "Tentacle Immune", "Hypothermia Immune"], s_nullYesNo, 0f, 300f, string.Empty, 65f, ["Whether or not the centipede should ignore lava/acid water damage.", "Whether or not the centipede should be uncatchable by creature tentacles (Daddy Long Legs, Daddy Corruption, WormGrass).", "Whether or not the centipede is immune to cold."]);
        MakeCheckBox(interUI, tab, "Spore Cloud Immune", 0f, 390f, "Whether or not the centipede should ignore poison damage from spore clouds (like Puff Ball clouds).");

        tab = tabs[K_Body];
        MakeTitle(tab, "Body");
        MakeFloatBoxes(interUI, tab, ["Min Size", "Max Size", "Body Size Estimate"], 0f, 0f, ["The minimum size factor of the centipede. 1 is 100%, higher numbers no longer change the body size but affect other settings. Requires \"Body Size Generation Type\" not to be set to \"StaticMax\".", "The maximum size factor of the centipede. 1 is 100%, higher numbers no longer change the body size but affect other settings. Requires \"Body Size Generation Type\" not to be set to \"StaticMin\".", "How big the centipede is seen by other creatures."]);
        list.Clear();
        list.Add(new(nameof(SizeGenerationType.RandomRangePow), 1));
        list.Add(new(nameof(SizeGenerationType.RandomRange), 2));
        list.Add(new(nameof(SizeGenerationType.StaticMin), 3));
        list.Add(new(nameof(SizeGenerationType.StaticMax), 4));
        list.Add(new(nameof(SizeGenerationType.FromWorldString), 5));
        MakeComboBox(interUI, tab, "Body Size Generation Type", list, 0f, 90f, nameof(SizeGenerationType.RandomRangePow), 145f, "The way the centipede generates its size.");
        MakeFloatBox(interUI, tab, "Connection Elasticity Reduction", 0f, 120f, "By how much the elasticity of the body chunk connections should be reduced.");
        list.Clear();
        list.Add(new(nameof(SegmentType.Normal), 1));
        list.Add(new(nameof(SegmentType.Centiwing), 2));
        list.Add(new(nameof(SegmentType.AquaCenti), 3));
        MakeComboBox(interUI, tab, "Body Segment Type", list, 0f, 150f, nameof(SegmentType.Normal), 90f, "How segment sizes are distributed throughout the body.");
        MakeSpriteBox(interUI, tab, "Segment Sprite", 0f, 180f, 200f, "The sprite of the centipede segment.");
        mins[0] = mins[1] = 2;
        MakeIntBoxes(interUI, tab, ["Min Chunk Amount", "Max Chunk Amount"], mins, maxs, 0f, 210f, ["The minimum number of segments of the centipede.", "The maximum number of segments of the centipede."]);
        list.Clear();
        list.Add(new(nameof(ChunkRadType.Normal), 1));
        list.Add(new(nameof(ChunkRadType.Small), 2));
        list.Add(new(nameof(ChunkRadType.Centiwing), 3));
        MakeComboBox(interUI, tab, "Body Chunk Rad Type", list, 0f, 270f, nameof(ChunkRadType.Normal), 90f, "The size type of the centipede segments.");
        MakeFloatBoxes(interUI, tab, ["Body Chunk Rad Bonus", "Body Chunk Mass Bonus"], 0f, 300f, ["A bonus added to the size of the centipede segments.", "A bonus added to the mass of the centipede segments."], false);
        MakeIntBox(interUI, tab, "ShortCut Segments", 1, 999, 0f, 360f, "How long the shortcut representation of the centipede is.");
        MakeColorPicker(interUI, tab, "ShortCut Color", 0f, 390f, "The color the shortcut representation of the centipede should use.");
        MakeCheckBox(interUI, tab, "Small Tube", 328f, 0f, "Whether or not the body tube of the centipede should be like that of a small centipede.");
        MakeComboBox(interUI, tab, "Main Color Type", s_palette, 328f, 30f, nameof(PaletteColorType.Custom), 150f, "The general color palette type of the centipede. Affects most parts like shells and legs if \"Shell Color\" and \"Secondary Shell Color\" are empty.");
        MakeFloatBoxes(interUI, tab, ["Hue Min", "Hue Max"], 328f, 60f, ["The minimum main color hue. Requires \"Main Color Type\" to be set to \"Custom\".", "The maximum main color hue. Requires \"Main Color Type\" to be set to \"Custom\"."], false);
        MakeFloatBoxes(interUI, tab, ["Saturation Min", "Saturation Max"], 328f, 120f, ["The minimum main color saturation. Requires \"Main Color Type\" to be set to \"Custom\".", "The maximum main color saturation. Requires \"Main Color Type\" to be set to \"Custom\"."]);
        tab._AddItem(new OpImage(new() { x = 498f, y = 400.5f }, "CustomCentiMenu_HueSaturationPreview") { description = "A preview for \"Hue Min\" (left), \"Hue Max\" (right), \"Saturation Min\" (bottom) and \"Saturation Max\" (top)." });
        MakeColorTypeBox(interUI, tab, "Body Black Color Type", s_palette, 328f, 180f, nameof(PaletteColorType.Custom), 150f, "The color palette type used for the black parts of the centipede.");
        MakeColorPicker(interUI, tab, "Body Black Color", 328f, 210f, "The custom color used for the black parts of the centipede. Requires \"Body Black Color Type\" to be set to \"Custom\".");
        MakeReloadArrow(interUI, _rlds, tab, "Body Size Generation Type");

        tab = tabs[K_Mind];
        MakeTitle(tab, "Mind");
        MakeCheckBoxes(interUI, tab, ["Detects Annoying Collisions", "Noise Tracker", "Ignores Community"], 0f, 0f, ["Whether or not the centipede should react to collisions with other creatures.", "Whether or not the centipede can track noise.", "Whether or not the centipede should ignore the influence of other centipedes."]);
        MakeFloatBox(interUI, tab, "Community Influence", 0f, 90f, "How much other centipedes influence the behavior of a centipede. Requires \"Ignores Community\" to be set to false.");
        MakeCheckBox(interUI, tab, "Tries To Stay In Room", 0f, 120f, "Whether or not the centipede should avoid moving to another room.");
        mins[0] = mins[1] = 0;
        MakeIntBoxes(interUI, tab, ["Abstracted Laziness", "Pather Steps Per Frame"], mins, maxs, 0f, 150f, ["How lazy the centipede is to move to another room.", "How often the centipede updates its path."]);
        list.Clear();
        list.Add(new(nameof(IdleScoreTypes.Normal), 1));
        list.Add(new(nameof(IdleScoreTypes.Centiwing), 2));
        list.Add(new(nameof(IdleScoreTypes.AquaCenti), 3));
        list.Add(new(nameof(IdleScoreTypes.AquaCentiAndCentiwing), 3));
        MakeComboBox(interUI, tab, "Allowed Idle Types", list, 0f, 210f, nameof(IdleScoreTypes.Normal), 170f, "How and where the centipede can stay still.");
        list.Clear();
        list.Add(new(nameof(ExcitementTrackerType.Normal), 1));
        list.Add(new(nameof(ExcitementTrackerType.Centiwing), 2));
        list.Add(new(nameof(ExcitementTrackerType.Red), 3));
        MakeComboBox(interUI, tab, "Excitement Type", list, 0f, 240f, nameof(ExcitementTrackerType.Normal), 90f, "How excited the centipede is.");
        list.Clear();
        list.Add(new(nameof(RelationshipChangeType.Normal), 1));
        list.Add(new(nameof(RelationshipChangeType.Centiwing), 2));
        list.Add(new(nameof(RelationshipChangeType.Red), 3));
        MakeComboBox(interUI, tab, "Dynamic Relationship Type", list, 0f, 270f, nameof(RelationshipChangeType.Normal), 90f, "The way the relationships of the centipede to other creatures change.");
        list.Clear();
        list.Add(new(nameof(PreyTrackerType.Normal), 1));
        list.Add(new(nameof(PreyTrackerType.Red), 2));
        MakeComboBox(interUI, tab, "Prey Track Type", list, 0f, 300f, nameof(PreyTrackerType.Normal), 75f, "The way the centipede tracks its prey.");
        MakeFloatBoxes(interUI, tab, ["Prey Tracker Weight", "Sure To Get Prey Distance"], 0f, 330f, ["How much the centipede loves to hunt.", "The minimum distance for which the centipede is sure to continue tracking its prey."]);
        list.Clear();
        list.Add(new(nameof(VisualScoreChangeTypes.Normal), 1));
        list.Add(new(nameof(VisualScoreChangeTypes.Centiwing), 2));
        list.Add(new(nameof(VisualScoreChangeTypes.Red), 3));
        list.Add(new(nameof(VisualScoreChangeTypes.RedAndCentiwing), 4));
        MakeComboBox(interUI, tab, "Visual Score Types", list, 0f, 390f, nameof(VisualScoreChangeTypes.Normal), 140f, "The way the centipede sees its environment.");
        MakeFloatBoxes(interUI, tab, ["Visual Radius", "Movement Based Vision", "Water Vision", "Through Surface Vision"], 0f, 420f, ["The size of the centipede visual range.", "How well the centipede sees moving things.", "How well the centipede can see in water.", "How well the centipede sees things through water surface."]);

        tab = tabs[K_Electricity];
        MakeTitle(tab, "Electricity");
        MakeCheckBoxes(interUI, tab, ["Wants To Shock", "Shocks When Grabbed"], 0f, 0f, ["Whether or not the centipede wants to shock other creatures.", "Whether or not the centipede shocks creatures that grab it like Small Cenetipedes."]);
        MakeFloatBox(interUI, tab, "Grabbed Shock Charge Reduction", 0f, 60f, "By how much the charge speed of a centipede should be reduced when grabbed. Requires \"Shocks When Grabbed\" to be set to true.", false);
        MakeFloatBox(interUI, tab, "Shock Resistance Reduction Factor", 0f, 90f, "By how much the resistance of creatures to a shock should be reduced.");
        list.Clear();
        list.Add(new(nameof(ShockDamageType.Normal), 1));
        list.Add(new(nameof(ShockDamageType.Small), 2));
        list.Add(new(nameof(ShockDamageType.AquaCenti), 3));
        MakeComboBox(interUI, tab, "Shock Type", list, 0f, 120f, nameof(ShockDamageType.Normal), 90f, "The way the centipede shocks grabbed creatures.");
        MakeColorTypeBox(interUI, tab, "Shock Color Type", s_palette, 0f, 150f, nameof(PaletteColorType.Custom), 150f, "The color palette type of the sparks or underwater sparks that appear when a centipede shocks a creature.");
        MakeColorPicker(interUI, tab, "Shock Color", 0f, 180f, "The custom color of the sparks or underwater sparks that appear when a centipede shocks a creature. Requires \"Shock Color Type\" to be set to \"Custom\".");
        list.Clear();
        list.Add(new(nameof(FlashColorNoiseType.None), 1));
        list.Add(new(nameof(FlashColorNoiseType.Red), 2));
        list.Add(new(nameof(FlashColorNoiseType.Green), 3));
        list.Add(new(nameof(FlashColorNoiseType.Blue), 4));
        MakeComboBox(interUI, tab, "Light Flash Noise Type", list, 0f, 336f, nameof(FlashColorNoiseType.None), 70f, "The dominant color of the of the light that appears when a centipede shocks a creature.");
        MakeColorTypeBox(interUI, tab, "Light Flash Color Type", s_palette, 0f, 366f, nameof(PaletteColorType.Custom), 150f, "The color palette type of the light that appears when a centipede shocks a creature.");
        MakeColorPicker(interUI, tab, "Light Flash Color", 0f, 396f, "The custom color of the light that appears when a centipede shocks a creature. Requires \"Light Flash Color Type\" to be set to \"Custom\".");
        MakeCheckBox(interUI, tab, "Glowing Head", 300f, 0f, "Whether or not the head of the centipede should glow in the dark or underwater.");
        MakeColorTypeBox(interUI, tab, "Head Glow Min Color Type", s_palette, 300f, 30f, nameof(PaletteColorType.Custom), 150f, "The minimum color palette type of the centipede head glow. Requires \"Glowing Head\" to be set to true.");
        MakeColorPicker(interUI, tab, "Head Glow Min Color", 300f, 60f, "The minimum custom color of the centipede head glow. Requires \"Head Glow Min Color Type\" to be set to \"Custom\".");
        MakeColorTypeBox(interUI, tab, "Head Glow Max Color Type", s_palette, 300f, 216f, nameof(PaletteColorType.Custom), 150f, "The maximum color palette type of the centipede head glow. Requires \"Glowing Head\" to be set to true.");
        MakeColorPicker(interUI, tab, "Head Glow Max Color", 300f, 246f, "The maximum custom color of the centipede head glow. Requires \"Head Glow Max Color Type\" to be set to \"Custom\".");

        tab = tabs[K_Shells];
        MakeTitle(tab, "Shells");
        MakeSpriteBoxes(interUI, tab, ["Belly Shell Sprite", "Back Shell Sprite"], 0f, 0f, 170f, ["The sprite of the centipede belly shell.", "The sprite of the centipede back shell."]);
        MakeFloatBox(interUI, tab, "Shell Scale Y Factor", 0f, 60f, "How wide a shell is.");
        MakeColorTypeBox(interUI, tab, "Shell Color Type", s_nullPalette, 0f, 90f, string.Empty, 150f, "The color palette type replacement for back shells. Replaces the hue, saturation and lightness of back shells. Removes dynamic back shells lighting.");
        MakeColorPicker(interUI, tab, "Shell Color", 0f, 120f, "The custom color replacement for back shells. Replaces the hue, saturation and lightness of back shells. Removes dynamic back shells lighting. Requires \"Shell Color Type\" to be set to \"Custom\".");
        MakeColorTypeBox(interUI, tab, "Secondary Shell Color Type", s_nullPalette, 0f, 276f, string.Empty, 150f, "The color palette type replacement for belly shells and legs. Replaces the hue, saturation and lightness of belly shells and legs. Removes dynamic belly shells lighting.");
        MakeColorPicker(interUI, tab, "Secondary Shell Color", 0f, 306f, "The color palette type replacement for belly shells and legs. Replaces the hue, saturation and lightness of belly shells and legs. Removes dynamic belly shells lighting. Requires \"Secondary Shell Color Type\" to be set to \"Custom\".");
        MakeFloatBox(interUI, tab, "Secondary Shell Color Bonus", 0f, 462f, "A bonus added to the secondary shell color to make it lighter.");
        MakeCheckBox(interUI, tab, "Shell Particles", 0f, 492f, "Whether or not shells should fall when the centipede is injured.");
        MakeFloatBox(interUI, tab, "Dead Shell Chance", 0f, 522f, "The chance for the centipede to spawn with missing shells. Requires \"Shell Particles\" to be set to true.");
        MakeCheckBox(interUI, tab, "Additional Shell Texture", 280f, 0f, "Whether or not the centipede shells should use an additional texture like AquaCenti.");
        MakeShaderBox(interUI, tab, "Additional Shell Texture Shader", 280f, 30f, 145f, "The shader of the additional texture used by the centipede shells. Requires \"Additional Shell Texture\" to be set to true.");
        MakeCheckBox(interUI, tab, "Shields", 280f, 60f, "Whether or not the centipede shells should protect the centipede like a Red Centipede.");
        MakeCheckBox(interUI, tab, "Shell Spikes", 280f, 90f, "Whether or not the centipede back shells should have spikes on them.");
        MakeSpriteBoxes(interUI, tab, ["Shell Spike Sprite 1", "Shell Spike Sprite 2"], 280f, 120f, 200f, ["The sprite of the shell spike base. Requires \"Shell Spikes\" to be set to true.", "The sprite of the shell spike tip. Requires \"Shell Spikes\" to be set to true."]);
        MakeFloatBox(interUI, tab, "Spike Scale Factor", 280f, 180f, "The size of the shell spikes. Requires \"Shell Spikes\" to be set to true.");

        tab = tabs[K_Appendages];
        MakeTitle(tab, "Appendages");
        MakeCheckBox(interUI, tab, "Slow Legs", 0f, 0f, "Whether or not the centipede should have less visible legs.");
        MakeFloatBoxes(interUI, tab, ["Leg Length Factor", "Leg Scale X Factor"], 0f, 30f, ["The length of the legs.", "How thick a leg is."]);
        MakeSpriteBoxes(interUI, tab, ["Leg A Sprite", "Leg B Sprite"], 0f, 90f, 200f, ["The sprite used by the first part of a leg.", "The sprite used by the second part of a leg."]);
        MakeCheckBox(interUI, tab, "Wings", 0f, 150f, "Whether or not the centipede should have wings like Centiwings.");
        MakeIntBox(interUI, tab, "Wing Variations", 1, 999, 0f, 180f, "The number of variations of the wing sprite. Requires \"Wings\" to be set to true.");
        list.Clear();
        list.Add(new(nameof(WingType.StaticLengthFactor), 1));
        list.Add(new(nameof(WingType.Centiwing), 2));
        list.Add(new(nameof(WingType.AquaCenti), 3));
        MakeComboBox(interUI, tab, "Wing Size Type", list, 0f, 210f, nameof(WingType.StaticLengthFactor), 145f, "How wing sizes are distributed throughout the body. Requires \"Wings\" to be set to true.");
        MakeFloatBox(interUI, tab, "Wing Length Factor", 0f, 240f, "The size of the wings. Requires \"Wings\" to be set to true.");
        MakeColorTypeBox(interUI, tab, "Wing Color Type", s_palette, 0f, 270f, nameof(PaletteColorType.Custom), 150f, "The color palette type of the wings. Requires \"Wings\" to be set to true.");
        MakeColorPicker(interUI, tab, "Wing Color", 0f, 300f, "The custom color of the wings. Requires \"Wing Color Type\" to be set to \"Custom\".");
        MakeSpriteBox(interUI, tab, "Wing Sprite", 0f, 456f, 200f, "The sprite used by the wings. Don't forget to add 1, 2, 3, ... after the name of the image file if there are several variations! Requires \"Wings\" to be set to true.");
        MakeShaderBox(interUI, tab, "Wing Shader", 0f, 486f, 200f, "The shader used by the wings. Requires \"Wings\" to be set to true.");
        MakeCheckBox(interUI, tab, "Body Size Dependant Whisker", 300f, 0f, "Whether or not whiskers should depend on body size.");
        MakeFloatBoxes(interUI, tab, ["Whisker Length Factor", "Small Whisker Length", "Big Whisker Length", "Whisker Shape Factor"], 300f, 30f, ["The length of the two pairs of whiskers.", "The length of the small pair of whiskers.", "The length of the big pair of whiskers.", "How thick a whisker is."]);

        tab = tabs[K_Arena];
        MakeTitle(tab, "Arena");
        MakeCheckBoxes(interUI, tab, ["Kill Score Hidden", "Counts As A Kill"], 0f, 0f, ["Whether or not the centipede kill score should be hidden and non-configurable or not. Requires \"Unlock ID\" not to be empty.", "Whether or not the player should get a score reward after killing the centipede."]);
        MakeIntBoxes(interUI, tab, ["Standard Kill Score", "Big Kill Score", "Small Kill Score", "Expedition Score"], mins, maxs, 0f, 60f, ["The kill score for the medium-sized centipede.", "The kill score for the large centipede. Requires \"Body Size Generation Type\" not to be set to \"StaticMin\" nor \"StaticMax\".", "The kill score for the small centipede. Requires \"Body Size Generation Type\" not to be set to \"StaticMin\" nor \"StaticMax\".", "The centipede expedition kill score, not size-dependent."]);
        MakeCheckBox(interUI, tab, "Major Creature", 0f, 180f, "Whether or not the centipede should be recognized as a large creature by arena mode.");
        MakeFloatBoxes(interUI, tab, ["Linear Sandbox Cost", "Exponential Sandbox Cost", "Room Performance Cost"], 0f, 210f, ["The linear part of the sandbox performance cost for the centipede.", "The exponential part of the sandbox room performance cost for the centipede.", "The room performance cost for the centipede."]);
        MakeTextBox(interUI, tab, "Unlock ID", 0f, 300f, 180f, false, "The unlock ID for the medium-sized centipede.");
        var lUnk = new List<ListItem> { s_null };
        var entries = CreatureTemplate.Type.values.entries;
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            lUnk.Add(new(entry, i + 2));
        }
        MakeComboBox(interUI, tab, "Parent Unlock", lUnk, 0f, 330f, string.Empty, 180f, "The unlock ID of the parent unlock for the medium-sized centipede. Child unlocks are unlocked when the parent unlock token is collected. Requires \"Unlock ID\" not to be empty.");
        MakeSpriteBox(interUI, tab, "Standard Icon Sprite", 0f, 360f, 160f, "The icon sprite for the medium-sized centipede.");
        MakeColorPicker(interUI, tab, "Standard Icon Color", 0f, 390f, "The icon color for the medium-sized centipede.");
        MakeCheckBox(interUI, tab, "Uses Unlock Data", 300f, 0f, "Whether or not the centipede should have multiple arena unlocks for its different sizes. Requires \"Body Size Generation Type\" not to be set to \"StaticMin\" nor \"StaticMax\" and \"Unlock ID\" not to be empty.");
        MakeTextBox(interUI, tab, "Big Unlock ID", 300f, 30f, 180f, false, "The unlock ID for the large centipede. Requires \"Uses Unlock Data\" to be set to true.");
        MakeComboBox(interUI, tab, "Big Parent Unlock", lUnk, 300f, 60f, string.Empty, 170f, "The unlock ID of the parent unlock for the large centipede. Child unlocks are unlocked when the parent unlock token is collected. Requires \"Big Unlock ID\" not to be empty.");
        MakeSpriteBox(interUI, tab, "Big Icon Sprite", 300f, 90f, 180f, "The icon sprite for the large centipede. Requires \"Body Size Generation Type\" not to be set to \"StaticMin\" nor \"StaticMax\".");
        MakeColorPicker(interUI, tab, "Big Icon Color", 300f, 120f, "The icon color for the large centipede. Requires \"Body Size Generation Type\" not to be set to \"StaticMin\" nor \"StaticMax\".");
        MakeTextBox(interUI, tab, "Small Unlock ID", 300f, 276f, 180f, false, "The unlock ID for the small centipede. Requires \"Uses Unlock Data\" to be set to true.");
        MakeComboBox(interUI, tab, "Small Parent Unlock", lUnk, 300f, 306f, string.Empty, 170f, "The unlock ID of the parent unlock for the small centipede. Child unlocks are unlocked when the parent unlock token is collected. Requires \"Small Unlock ID\" not to be empty.");
        MakeSpriteBox(interUI, tab, "Small Icon Sprite", 300f, 336f, 180f, "The icon sprite for the small centipede. Requires \"Body Size Generation Type\" not to be set to \"StaticMin\" nor \"StaticMax\".");
        MakeColorPicker(interUI, tab, "Small Icon Color", 300f, 366f, "The icon color for the small centipede. Requires \"Body Size Generation Type\" not to be set to \"StaticMin\" nor \"StaticMax\".");
        MakeReloadArrows(interUI, _rlds, tab, ["Uses Unlock Data", "Unlock ID", "Parent Unlock", "Small Parent Unlock", "Big Parent Unlock", "Small Unlock ID", "Big Unlock ID", "Kill Score Hidden", "Standard Kill Score", "Big Kill Score", "Small Kill Score", "Expedition Score"]);

        tab = tabs[K_Relations];
        MakeTitle(tab, "Relationships");
        button = new CustomOpSimpleButton(new() { x = 480f, y = 574f }, new() { x = 120f, y = 30f }, s_Save_Relationships) { colorEdge = _current is not CustomCentiBreedParams props2 || !props2._relationsChanged ? s_coolGreen : s_coolRed, description = "Saves the temporary relationship changes. You still need to click on \"Save File\" to write into the .tp file." };
        button.OnClick += _CustomCentiInterface_SaveRelations;
        button.OnClick += _CustomCentiInterface_MakeSaveRelationshipsGreen;
        button.OnClick += _CustomCentiInterface_MakeSaveFileRed2;
        tab._AddItem(interUI[s_Save_Relationships] = button);
        MakeRelationships(ref _rels, tab, _current);

        if (_current is null)
            DeactivateUI();
        else
        {
            (interUI[s_Current] as CustomOpComboBox)!.ForceChangeValue(_current.CreatureType.value, this);
            UpdateFields();
        }
    }

    public void UpdateFields()
    {
        var current = _current!;
        foreach (var kvpair in _interUI)
        {
            var key = kvpair.Key;
            if (key != s_Current && kvpair.Value is UIconfig val)
            {
                var cfg = val.cfgEntry;
                if (cfg is Configurable<bool> b)
                {
                    current.SetConfig(b);
                    val.ForceChangeValue(b.Value.ToStringInvariant(), this);
                }
                else if (cfg is Configurable<int> i)
                {
                    current.SetConfig(i);
                    val.ForceChangeValue(i.Value.ToStringInvariant(), this);
                }
                else if (cfg is Configurable<Color> c)
                {
                    current.SetConfig(c);
                    val.ForceChangeValue(c.Value.ColorToHex(), this);
                }
                else if (cfg is Configurable<string> s)
                {
                    if (key != "Main Color Type" && key.Contains("Color Type"))
                        current.SetColorTypeConfig(s);
                    else
                        current.SetConfig(s);
                    val.ForceChangeValue(s.Value, this);
                }
                else if (cfg is Configurable<float> f)
                {
                    if (key.Contains(s_Alpha))
                        current.SetAlphaConfig(f);
                    else
                        current.SetConfig(f);
                    val.ForceChangeValue(f.Value.ToStringInvariant(), this);
                }
            }
        }
        UpdateDependencies();
    }

    public void UpdateDependencies()
    {
        var cur = _current!;
        var icon = _centiIcon;
        icon.scale = new() { x = 3f, y = 3f };
        icon.color = cur.StandardIconColor;
        icon.sprite.element = TryGetSprite(cur.StandardIconSprite, s_Kill_Centipede2);
        var interUI = _interUI;
        if (string.IsNullOrEmpty(GetValue<string>(s_Inherit)))
            interUI[s_Apply_Parent].greyedOut = true;
        else
            interUI[s_Apply_Parent].greyedOut = false;
        if (GetValue<bool>("Flying"))
            interUI["Global Flying Velocity Factor"].greyedOut = false;
        else
        {
            interUI["Global Flying Velocity Factor"].ResetAndDeactivate(this);
            cur.Set(SetValue("GlobalFlyingVelocityFactor", 0f));
        }
        if (GetValue<bool>("SmallFood"))
        {
            interUI["Edible By Omnivores"].ResetAndDeactivate(this);
            cur.Set(SetValue("EdibleByOmnivores", false));
            interUI["Meat Min"].ResetAndDeactivate(this);
            cur.Set(SetValue("MeatMin", 0f));
            interUI["Meat Max"].ResetAndDeactivate(this);
            cur.Set(SetValue("MeatMax", 0f));
            interUI["Automatic Pick Up"].greyedOut = interUI["Bites"].greyedOut = interUI["Food Points"].greyedOut = false;
        }
        else
        {
            interUI["Edible By Omnivores"].greyedOut = interUI["Meat Min"].greyedOut = interUI["Meat Max"].greyedOut = false;
            interUI["Automatic Pick Up"].ResetAndDeactivate(this);
            cur.Set(SetValue("AutomaticPickUp", false));
            interUI["Bites"].ResetAndDeactivate(this);
            cur.Set(SetValue("Bites", 1));
            interUI["Food Points"].ResetAndDeactivate(this);
            cur.Set(SetValue("FoodPoints", 0));
        }
        var val = GetValue<string>("Grabability");
        if (string.IsNullOrEmpty(val) || val == nameof(Player.ObjectGrabability.CantGrab))
        {
            interUI["Throwable"].ResetAndDeactivate(this);
            cur.Set(SetValue("Throwable", string.Empty));
        }
        else
            interUI["Throwable"].greyedOut = false;
        val = GetValue<string>("BodySizeGenerationType");
        bool max = val == nameof(SizeGenerationType.StaticMax), min = val == nameof(SizeGenerationType.StaticMin);
        if (max)
        {
            interUI["Min Size"].ResetAndDeactivate(this);
            cur.Set(SetValue("MinSize", 0f));
        }
        else
            interUI["Min Size"].greyedOut = false;
        if (min)
        {
            interUI["Max Size"].ResetAndDeactivate(this);
            cur.Set(SetValue("MaxSize", 0f));
        }
        else
            interUI["Max Size"].greyedOut = false;
        if (max || min)
        {
            interUI["Big Kill Score"].ResetAndDeactivate(this);
            cur.Set(SetValue("BigKillScore", 0));
            interUI["Small Kill Score"].ResetAndDeactivate(this);
            cur.Set(SetValue("SmallKillScore", 0));
            interUI["Big Icon Sprite"].ResetAndDeactivate(this);
            cur.Set(SetValue("BigIconSprite", string.Empty));
            interUI["Small Icon Sprite"].ResetAndDeactivate(this);
            cur.Set(SetValue("SmallIconSprite", string.Empty));
            interUI["Big Icon Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("BigIconColor", s_black));
            interUI["Big Icon Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("BigIconColorAlpha", 0f));
            interUI["Small Icon Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("SmallIconColor", s_black));
            interUI["Small Icon Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("SmallIconColorAlpha", 0f));
        }
        else
            interUI["Small Kill Score"].greyedOut = interUI["Big Kill Score"].greyedOut = interUI["Big Icon Sprite"].greyedOut = interUI["Big Icon Color"].greyedOut = interUI["Big Icon Color Alpha"].greyedOut = interUI["Small Icon Sprite"].greyedOut = interUI["Small Icon Color"].greyedOut = interUI["Small Icon Color Alpha"].greyedOut = false;
        var uud = interUI["Uses Unlock Data"];
        var nullUnkID = string.IsNullOrEmpty(GetValue<string>("UnlockID"));
        if (max || min || nullUnkID)
        {
            uud.ResetAndDeactivate(this);
            cur.Set(SetValue("UsesUnlockData", false));
        }
        else
            uud.greyedOut = false;
        if (nullUnkID)
        {
            cur.Set(SetValue("UsesUnlockData", false));
            interUI["Parent Unlock"].ResetAndDeactivate(this);
            cur.Set(SetValue("ParentUnlock", string.Empty));
            interUI["Kill Score Hidden"].ResetAndDeactivate(this);
            cur.Set(SetValue("KillScoreHidden", false));
        }
        else
            interUI["Parent Unlock"].greyedOut = interUI["Kill Score Hidden"].greyedOut = false;
        if (uud.greyedOut || !GetValue<bool>("UsesUnlockData"))
        {
            interUI["Big Unlock ID"].ResetAndDeactivate(this);
            cur.Set(SetValue("BigUnlockID", string.Empty));
            interUI["Small Unlock ID"].ResetAndDeactivate(this);
            cur.Set(SetValue("SmallUnlockID", string.Empty));
        }
        else
            interUI["Big Unlock ID"].greyedOut = interUI["Small Unlock ID"].greyedOut = false;
        if (interUI["Big Unlock ID"].greyedOut || string.IsNullOrEmpty(GetValue<string>("BigUnlockID")))
        {
            interUI["Big Parent Unlock"].ResetAndDeactivate(this);
            cur.Set(SetValue("BigParentUnlock", string.Empty));
        }
        else
            interUI["Big Parent Unlock"].greyedOut = false;
        if (interUI["Small Unlock ID"].greyedOut || string.IsNullOrEmpty(GetValue<string>("SmallUnlockID")))
        {
            interUI["Small Parent Unlock"].ResetAndDeactivate(this);
            cur.Set(SetValue("SmallParentUnlock", string.Empty));
        }
        else
            interUI["Small Parent Unlock"].greyedOut = false;
        if (GetValue<string>("MainColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Hue Min"].ResetAndDeactivate(this);
            cur.Set(SetValue("HueMin", 0f));
            interUI["Hue Max"].ResetAndDeactivate(this);
            cur.Set(SetValue("HueMax", 0f));
            interUI["Saturation Min"].ResetAndDeactivate(this);
            cur.Set(SetValue("SaturationMin", 0f));
            interUI["Saturation Max"].ResetAndDeactivate(this);
            cur.Set(SetValue("SaturationMax", 0f));
        }
        else
            interUI["Hue Min"].greyedOut = interUI["Hue Max"].greyedOut = interUI["Saturation Min"].greyedOut = interUI["Saturation Max"].greyedOut = false;
        if (GetValue<string>("BodyBlackColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Body Black Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("BodyBlackColor", s_black));
            interUI["Body Black Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("BodyBlackColorAlpha", 0f));
        }
        else
            interUI["Body Black Color"].greyedOut = interUI["Body Black Color Alpha"].greyedOut = false;
        if (GetValue<bool>("IgnoresCommunity"))
        {
            interUI["Community Influence"].ResetAndDeactivate(this);
            cur.Set(SetValue("CommunityInfluence", 0f));
        }
        else
            interUI["Community Influence"].greyedOut = false;
        if (!GetValue<bool>("ShocksWhenGrabbed"))
        {
            interUI["Grabbed Shock Charge Reduction"].ResetAndDeactivate(this);
            cur.Set(SetValue("GrabbedShockChargeReduction", 0f));
        }
        else
            interUI["Grabbed Shock Charge Reduction"].greyedOut = false;
        if (GetValue<string>("ShockColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Shock Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("ShockColor", s_black));
            interUI["Shock Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("ShockColorAlpha", 0f));
        }
        else
            interUI["Shock Color"].greyedOut = interUI["Shock Color Alpha"].greyedOut = false;
        if (GetValue<string>("LightFlashColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Light Flash Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("LightFlashColor", s_black));
            interUI["Light Flash Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("LightFlashColorAlpha", 0f));
        }
        else
            interUI["Light Flash Color"].greyedOut = interUI["Light Flash Color Alpha"].greyedOut = false;
        if (GetValue<bool>("GlowingHead"))
            interUI["Head Glow Min Color Type"].greyedOut = interUI["Head Glow Max Color Type"].greyedOut = false;
        else
        {
            interUI["Head Glow Min Color Type"].ResetAndDeactivate(this);
            cur.SetColorType(SetValue("HeadGlowMinColorType", nameof(PaletteColorType.Custom)));
            interUI["Head Glow Max Color Type"].ResetAndDeactivate(this);
            cur.SetColorType(SetValue("HeadGlowMaxColorType", nameof(PaletteColorType.Custom)));
        }
        if (interUI["Head Glow Min Color Type"].greyedOut || GetValue<string>("HeadGlowMinColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Head Glow Min Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("HeadGlowMinColor", s_black));
            interUI["Head Glow Min Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("HeadGlowMinColorAlpha", 0f));
        }
        else
            interUI["Head Glow Min Color"].greyedOut = interUI["Head Glow Min Color Alpha"].greyedOut = false;
        if (interUI["Head Glow Max Color Type"].greyedOut || GetValue<string>("HeadGlowMaxColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Head Glow Max Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("HeadGlowMaxColor", s_black));
            interUI["Head Glow Max Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("HeadGlowMaxColorAlpha", 0f));
        }
        else
            interUI["Head Glow Max Color"].greyedOut = interUI["Head Glow Max Color Alpha"].greyedOut = false;
        if (GetValue<string>("ShellColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Shell Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("ShellColor", s_black));
            interUI["Shell Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("ShellColorAlpha", 0f));
        }
        else
            interUI["Shell Color"].greyedOut = interUI["Shell Color Alpha"].greyedOut = false;
        if (GetValue<string>("SecondaryShellColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Secondary Shell Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("SecondaryShellColor", s_black));
            interUI["Secondary Shell Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("SecondaryShellColorAlpha", 0f));
        }
        else
            interUI["Secondary Shell Color"].greyedOut = interUI["Secondary Shell Color Alpha"].greyedOut = false;
        if (GetValue<bool>("ShellParticles"))
            interUI["Dead Shell Chance"].greyedOut = false;
        else
        {
            interUI["Dead Shell Chance"].ResetAndDeactivate(this);
            cur.Set(SetValue("DeadShellChance", 0f));
        }
        if (GetValue<bool>("AdditionalShellTexture"))
            interUI["Additional Shell Texture Shader"].greyedOut = false;
        else
        {
            interUI["Additional Shell Texture Shader"].ResetAndDeactivate(this);
            cur.Set(SetValue("AdditionalShellTextureShader", string.Empty));
        }
        if (GetValue<bool>("ShellSpikes"))
            interUI["Shell Spike Sprite 1"].greyedOut = interUI["Shell Spike Sprite 2"].greyedOut = interUI["Spike Scale Factor"].greyedOut = false;
        else
        {
            interUI["Shell Spike Sprite 1"].ResetAndDeactivate(this);
            cur.Set(SetValue("ShellSpikeSprite1", string.Empty));
            interUI["Shell Spike Sprite 2"].ResetAndDeactivate(this);
            cur.Set(SetValue("ShellSpikeSprite2", string.Empty));
            interUI["Spike Scale Factor"].ResetAndDeactivate(this);
            cur.Set(SetValue("SpikeScaleFactor", 0f));
        }
        if (GetValue<bool>("Wings"))
            interUI["Wing Variations"].greyedOut = interUI["Wing Size Type"].greyedOut = interUI["Wing Length Factor"].greyedOut = interUI["Wing Sprite"].greyedOut = interUI["Wing Color Type"].greyedOut = interUI["Wing Shader"].greyedOut = false;
        else
        {
            interUI["Wing Variations"].ResetAndDeactivate(this);
            cur.Set(SetValue("WingVariations", 1));
            interUI["Wing Size Type"].ResetAndDeactivate(this);
            cur.Set(SetValue("WingSizeType", nameof(WingType.StaticLengthFactor)));
            interUI["Wing Length Factor"].ResetAndDeactivate(this);
            cur.Set(SetValue("WingLengthFactor", 0f));
            interUI["Wing Color Type"].ResetAndDeactivate(this);
            cur.SetColorType(SetValue("WingColorType", nameof(PaletteColorType.Custom)));
            interUI["Wing Sprite"].ResetAndDeactivate(this);
            cur.Set(SetValue("WingSprite", string.Empty));
            interUI["Wing Shader"].ResetAndDeactivate(this);
            cur.Set(SetValue("WingShader", string.Empty));
        }
        if (interUI["Wing Color Type"].greyedOut || GetValue<string>("WingColorType") != nameof(PaletteColorType.Custom))
        {
            interUI["Wing Color"].ResetAndDeactivate(this);
            cur.Set(SetValue("WingColor", s_black));
            interUI["Wing Color Alpha"].ResetAndDeactivate(this);
            cur.SetAlpha(SetValue("WingColorAlpha", 0f));
        }
        else
            interUI["Wing Color"].greyedOut = interUI["Wing Color Alpha"].greyedOut = false;
        UpdateHueSaturationPreview(cur, cur.MainColorType != PaletteColorType.Custom);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T GetValue<T>(string name) => (s_configs[name] as Configurable<T>)!.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Configurable<T> SetValue<T>(string name, T value)
    {
        var c = (s_configs[name] as Configurable<T>)!;
        c.Value = value;
        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void MakeTitle(OpTab tab, string title) => tab._AddItem(new CustomOpLabel(s_titlePos, s_titleSz, title, FLabelAlignment.Center, true));

    void MakeReloadArrows(InteractiveUIElements interUI, List<ImageWrap> rlds, OpTab tab, string[] names)
    {
        var flag = _current is CustomCentiBreedParams c && c._requiresReloading;
        var color = flag ? s_coolRed : s_coolGreen;
        var descr = flag ? s_Changes_detected : s_No_changes_detected;
        for (var i = 0; i < names.Length; i++)
        {
            var nm = names[i];
            var ui = interUI[nm];
            var arrow = new ReloadArrow(ui.pos + new Vector2() { x = ui.size.x, y = 2f }, s_ReloadArrowImage) { color = color, description = descr, _bumpBehav = ui.bumpBehav };
            tab._AddItem(arrow);
            rlds.Add(new() { _img = arrow });
            (ui as UIconfig)!.OnValueChanged += _CustomCentiInterface_MakeArrowsRed;
        }
    }

    void MakeReloadArrow(InteractiveUIElements interUI, List<ImageWrap> rlds, OpTab tab, string name)
    {
        var flag = _current is CustomCentiBreedParams c && c._requiresReloading;
        var ui = interUI[name];
        var arrow = new ReloadArrow(ui.pos + new Vector2() { x = ui.size.x, y = 2f }, s_ReloadArrowImage) { color = flag ? s_coolRed : s_coolGreen, description = flag ? s_Changes_detected : s_No_changes_detected, _bumpBehav = ui.bumpBehav };
        tab._AddItem(arrow);
        rlds.Add(new() { _img = arrow });
        (ui as UIconfig)!.OnValueChanged += _CustomCentiInterface_MakeArrowsRed;
    }

    void MakeCheckBoxes(InteractiveUIElements interUI, OpTab tab, string[] names, float x, float y, string[] descrs)
    {
        for (var i = 0; i < names.Length; i++)
        {
            var d = descrs[i];
            var nm = names[i];
            var box = new CustomOpCheckBox(GetOrCreateConfig(nm.RemoveSpaces(), false), x + nm.GetTextSize() + 8f, 544f - y) { description = d };
            box.OnValueChanged += _CustomCentiInterface_UpdateBoolField;
            box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
            tab._AddItem(new CustomOpLabel(x, 548f - y, nm + ":") { bumpBehav = box.bumpBehav, description = d });
            tab._AddItem(interUI[nm] = box);
            y += 30f;
        }
    }

    void MakeFloatBoxes(InteractiveUIElements interUI, OpTab tab, string[] names, float x, float y, string[] descrs, bool unsigned = true)
    {
        for (var i = 0; i < names.Length; i++)
        {
            var d = descrs[i];
            var nm = names[i];
            var box = new OpTextBox(GetOrCreateConfig(nm.RemoveSpaces(), 0f, unsigned ? s_fU : s_fS), new() { x = x + nm.GetTextSize() + 14f, y = 544f - y }, 60f) { allowSpace = true, description = d };
            box.OnValueChanged += _CustomCentiInterface_UpdateFloatField;
            box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
            tab._AddItem(new CustomOpLabel(x, 548f - y, nm + ":") { bumpBehav = box.bumpBehav, description = d });
            tab._AddItem(interUI[nm] = box);
            y += 30f;
        }
    }

    void MakeFloatBox(InteractiveUIElements interUI, OpTab tab, string name, float x, float y, string descr, bool unsigned = true)
    {
        var box = new OpTextBox(GetOrCreateConfig(name.RemoveSpaces(), 0f, unsigned ? s_fU : s_fS), new() { x = x + name.GetTextSize() + 14f, y = 544f - y }, 60f) { allowSpace = true, description = descr };
        box.OnValueChanged += _CustomCentiInterface_UpdateFloatField;
        box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = box.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = box);
    }

    void MakeCheckBox(InteractiveUIElements interUI, OpTab tab, string name, float x, float y, string descr)
    {
        var box = new CustomOpCheckBox(GetOrCreateConfig(name.RemoveSpaces(), false), x + name.GetTextSize() + 8f, 544f - y) { description = descr };
        box.OnValueChanged += _CustomCentiInterface_UpdateBoolField;
        box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = box.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = box);
    }

    void MakeComboBoxes(InteractiveUIElements interUI, OpTab tab, string[] names, List<ListItem> items, float x, float y, string def, float width, string[] descrs)
    {
        for (var i = 0; i < names.Length; i++)
        {
            var nm = names[i];
            var d = descrs[i];
            var box = new CustomOpComboBox(GetOrCreateConfig(nm.RemoveSpaces(), def), new() { x = x + nm.GetTextSize() + 8f, y = 544f - y }, width, items) { description = d };
            box.OnValueChanged += _CustomCentiInterface_UpdateStringField;
            box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
            tab._AddItem(new CustomOpLabel(x, 548f - y, nm + ":") { bumpBehav = box.bumpBehav, description = d });
            tab._AddItem(interUI[nm] = box);
            y += 30f;
        }
    }

    void MakeComboBox(InteractiveUIElements interUI, OpTab tab, string name, List<ListItem> items, float x, float y, string def, float width, string descr)
    {
        var box = new CustomOpComboBox(GetOrCreateConfig(name.RemoveSpaces(), def), new() { x = x + name.GetTextSize() + 8f, y = 544f - y }, width, items) { description = descr };
        box.OnValueChanged += _CustomCentiInterface_UpdateStringField;
        box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = box.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = box);
    }

    void MakeColorTypeBox(InteractiveUIElements interUI, OpTab tab, string name, List<ListItem> items, float x, float y, string def, float width, string descr)
    {
        var box = new CustomOpComboBox(GetOrCreateConfig(name.RemoveSpaces(), def), new() { x = x + name.GetTextSize() + 8f, y = 544f - y }, width, items) { description = descr };
        box.OnValueChanged += _CustomCentiInterface_UpdateColorTypeField;
        box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = box.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = box);
    }

    void MakeColorPickers(InteractiveUIElements interUI, OpTab tab, string[] names, float x, float y, string[] descrs)
    {
        for (var i = 0; i < names.Length; i++)
        {
            var d = descrs[i];
            var nm = names[i];
            var ts = nm.GetTextSize();
            var nmt = nm.RemoveSpaces();
            var slider = new CustomOpFloatSlider(GetOrCreateConfig(nmt + s_Alpha, 0f, s_f01), new() { x = x + ts / 2f - 16f, y = 434f - y }, 80, 1, true) { description = d };
            slider.OnValueChanged += _CustomCentiInterface_UpdateAlphaField;
            slider.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
            tab._AddItem(new CustomOpLabel(new() { x = x, y = 528f - y }, new() { x = ts, y = 20f }, s_Alpha, FLabelAlignment.Center) { bumpBehav = slider.bumpBehav, description = d });
            tab._AddItem(interUI[nm + " Alpha"] = slider);
            var clrPick = new CustomOpColorPicker(GetOrCreateConfig(nmt, s_black), new() { x = x + ts + 8f, y = 418f - y }) { description = d };
            clrPick.OnValueChanged += _CustomCentiInterface_UpdateColorField;
            clrPick.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
            tab._AddItem(new CustomOpLabel(x, 548f - y, nm + ":") { bumpBehav = slider.bumpBehav, _bumpBehav2 = clrPick.bumpBehav, description = d });
            tab._AddItem(interUI[nm] = clrPick);
            y += 156f;
        }
    }

    void MakeColorPicker(InteractiveUIElements interUI, OpTab tab, string name, float x, float y, string descr)
    {
        var ts = name.GetTextSize();
        var nmt = name.RemoveSpaces();
        var slider = new CustomOpFloatSlider(GetOrCreateConfig(nmt + s_Alpha, 0f, s_f01), new() { x = x + ts / 2f - 16f, y = 434f - y }, 80, 1, true) { description = descr };
        slider.OnValueChanged += _CustomCentiInterface_UpdateAlphaField;
        slider.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(new() { x = x, y = 528f - y }, new() { x = ts, y = 20f }, s_Alpha, FLabelAlignment.Center) { bumpBehav = slider.bumpBehav, description = descr });
        tab._AddItem(interUI[name + " Alpha"] = slider);
        var clrPick = new CustomOpColorPicker(GetOrCreateConfig(nmt, s_black), new() { x = x + ts + 8f, y = 418f - y }) { description = descr };
        clrPick.OnValueChanged += _CustomCentiInterface_UpdateColorField;
        clrPick.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = slider.bumpBehav, _bumpBehav2 = clrPick.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = clrPick);
    }

    void MakeTextBoxes(InteractiveUIElements interUI, OpTab tab, string[] names, float x, float y, float width, bool allowSpace, string[] descrs)
    {
        for (var i = 0; i < names.Length; i++)
        {
            var d = descrs[i];
            var nm = names[i];
            var box = new LongStringOpTextBox(GetOrCreateConfig(nm.RemoveSpaces(), string.Empty), new() { x = x + nm.GetTextSize() + 8f, y = 544f - y }, width, allowSpace) { description = d };
            box.OnValueChanged += _CustomCentiInterface_UpdateStringField;
            box.OnValueUpdate += s_CustomCentiInterface_MakeTextGrey;
            box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
            tab._AddItem(new CustomOpLabel(x, 548f - y, nm + ":") { bumpBehav = box.bumpBehav, description = d });
            tab._AddItem(interUI[nm] = box);
            y += 30f;
        }
    }

    void MakeTextBox(InteractiveUIElements interUI, OpTab tab, string name, float x, float y, float width, bool allowSpace, string descr)
    {
        var box = new LongStringOpTextBox(GetOrCreateConfig(name.RemoveSpaces(), string.Empty), new() { x = x + name.GetTextSize() + 8f, y = 544f - y }, width, allowSpace) { description = descr };
        box.OnValueChanged += _CustomCentiInterface_UpdateStringField;
        box.OnValueUpdate += s_CustomCentiInterface_MakeTextGrey;
        box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = box.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = box);
    }

    void MakeSpriteBoxes(InteractiveUIElements interUI, OpTab tab, string[] names, float x, float y, float width, string[] descrs)
    {
        for (var i = 0; i < names.Length; i++)
        {
            var d = descrs[i];
            var nm = names[i];
            var box = new LongStringOpTextBox(GetOrCreateConfig(nm.RemoveSpaces(), string.Empty), new() { x = x + nm.GetTextSize() + 8f, y = 544f - y }, width, true) { description = d };
            box.OnValueChanged += _CustomCentiInterface_UpdateSpriteField;
            box.OnValueUpdate += s_CustomCentiInterface_MakeTextGrey;
            box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
            tab._AddItem(new CustomOpLabel(x, 548f - y, nm + ":") { bumpBehav = box.bumpBehav, description = d });
            tab._AddItem(interUI[nm] = box);
            y += 30f;
        }
    }

    void MakeSpriteBox(InteractiveUIElements interUI, OpTab tab, string name, float x, float y, float width, string descr)
    {
        var box = new LongStringOpTextBox(GetOrCreateConfig(name.RemoveSpaces(), string.Empty), new() { x = x + name.GetTextSize() + 8f, y = 544f - y }, width, true) { description = descr };
        box.OnValueChanged += _CustomCentiInterface_UpdateSpriteField;
        box.OnValueUpdate += s_CustomCentiInterface_MakeTextGrey;
        box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = box.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = box);
    }

    void MakeShaderBox(InteractiveUIElements interUI, OpTab tab, string name, float x, float y, float width, string descr)
    {
        var box = new LongStringOpTextBox(GetOrCreateConfig(name.RemoveSpaces(), string.Empty), new() { x = x + name.GetTextSize() + 8f, y = 544f - y }, width, true) { description = descr };
        box.OnValueChanged += _CustomCentiInterface_UpdateShaderField;
        box.OnValueUpdate += s_CustomCentiInterface_MakeTextGrey;
        box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = box.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = box);
    }

    static Configurable<T> GetOrCreateConfig<T>(string name, T def, ConfigAcceptableBase? accept = null)
    {
        if (s_configs.TryGetValue(name, out var config))
            return (config as Configurable<T>)!;
        var res = new Configurable<T>(null, name, def, accept is null ? null : new(string.Empty, accept, string.Empty));
        s_configs[name] = res;
        return res;
    }

    void MakeRelationships(ref ElemWrap[] rels, OpTab tab, CustomCentiBreedParams? current)
    {
        var box = new OpScrollBox(default, new() { x = 600f, y = 564f }, 0f);
        var y = 8f;
        string cnm = current?._critob.CreatureName ?? s_Def, a = cnm + " > ", b = " > " + cnm + ":";
        var nms = CreatureTemplate.Type.values.entries;
        var l = nms.Count;
        var list = new List<ListItem>
        {
            s_null,
            new(CreatureTemplate.Relationship.Type.Afraid.value, 2),
            new(CreatureTemplate.Relationship.Type.Antagonizes.value, 3),
            new(CreatureTemplate.Relationship.Type.DoesntTrack.value, 4),
            new(CreatureTemplate.Relationship.Type.Eats.value, 5),
            new(CreatureTemplate.Relationship.Type.Ignores.value, 6)
        };
        var list2 = new List<ListItem> { s_null };
        var tpEn = CreatureTemplate.Relationship.Type.values.entries;
        for (var i = 0; i < tpEn.Count; i++)
            list2.Add(new(tpEn[i], i + 2));
        rels = new ElemWrap[l * 6];
        for (var i = 0; i < l; i++)
        {
            string nm = nms[l - i - 1], anm = a + nm + ":";
            var anmPos = 10f + anm.GetTextSize() + 8f;
            ref readonly CustomRelation rel = ref NullRelation;
            if (current is not null)
                rel = ref current.GetRelation(nm, true);
            var val = rel.Type?.value ?? string.Empty;
            var cbox = new CustomOpComboBox(GetOrCreateConfig(nm + "ActiveType", val), new() { x = anmPos, y = y }, 140f, list) { description = "The type of the relationship." };
            cbox.OnValueChanged += _CustomCentiInterface_UpdateRelationTypeField;
            cbox.OnValueChanged += _CustomCentiInterface_MakeSaveRelationshipsRed;
            cbox.ForceChangeValue(val, this);
            rels[i + l]._elem = cbox;
            var slider = new CustomOpFloatSlider(GetOrCreateConfig(nm + "ActiveIntensity", rel.Intensity, s_f01), new() { x = anmPos + 155f, y = y - 4f }, 80) { description = "How strong the relationship is." };
            slider.OnValueChanged += _CustomCentiInterface_UpdateRelationIntensityField;
            slider.OnValueChanged += _CustomCentiInterface_MakeSaveRelationshipsRed;
            slider.ForceChangeValue(rel.Intensity.ToStringInvariant(), this);
            rels[i + l * 2]._elem = slider;
            rels[i]._elem = new CustomOpLabel(10f, y + 4f, anm) { bumpBehav = cbox.bumpBehav, _bumpBehav2 = slider.bumpBehav };
            y += 30f;
            anm = nm + b;
            anmPos = 10f + anm.GetTextSize() + 8f;
            rel = ref NullRelation;
            if (current is not null)
                rel = ref current.GetRelation(nm, false);
            val = rel.Type?.value ?? string.Empty;
            cbox = new CustomOpComboBox(GetOrCreateConfig(nm + "PassiveType", val), new() { x = anmPos, y = y }, 140f, list2) { description = "The type of the relationship." };
            cbox.OnValueChanged += _CustomCentiInterface_UpdateRelationTypeField;
            cbox.OnValueChanged += _CustomCentiInterface_MakeSaveRelationshipsRed;
            cbox.ForceChangeValue(val, this);
            rels[i + l * 4]._elem = cbox;
            slider = new CustomOpFloatSlider(GetOrCreateConfig(nm + "PassiveIntensity", rel.Intensity, s_f01), new() { x = anmPos + 155f, y = y - 4f }, 80) { description = "How strong the relationship is." };
            slider.OnValueChanged += _CustomCentiInterface_UpdateRelationIntensityField;
            slider.OnValueChanged += _CustomCentiInterface_MakeSaveRelationshipsRed;
            slider.ForceChangeValue(rel.Intensity.ToStringInvariant(), this);
            rels[i + l * 5]._elem = slider;
            var label = new CustomOpLabel(10f, y + 4f, anm) { bumpBehav = cbox.bumpBehav, _bumpBehav2 = slider.bumpBehav };
            rels[i + l * 3]._elem = label;
            if (nm == cnm)
            {
                cbox.ResetAndDeactivate(this);
                slider.ResetAndDeactivate(this);
                //label.color = s_grey2;
            }
            y += 30f;
        }
        box.SetContentSize(y + 16f);
        tab._AddItem(box);
        box.AddItems(rels);
    }

    unsafe void MakeIntBoxes(InteractiveUIElements interUI, OpTab tab, string[] names, int* mins, int* maxs, float x, float y, string[] descrs)
    {
        for (var i = 0; i < names.Length; i++)
        {
            var d = descrs[i];
            var nm = names[i];
            var m = mins[i];
            var box = new OpTextBox(GetOrCreateConfig(nm.RemoveSpaces(), m, new ConfigAcceptableRange<int>(m, maxs[i])), new() { x = x + nm.GetTextSize() + 14f, y = 544f - y }, 60f) { allowSpace = true, description = d };
            box.OnValueChanged += _CustomCentiInterface_UpdateIntField;
            box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
            tab._AddItem(new CustomOpLabel(x, 548f - y, nm + ":") { bumpBehav = box.bumpBehav, description = d });
            tab._AddItem(interUI[nm] = box);
            y += 30f;
        }
    }

    void MakeIntBox(InteractiveUIElements interUI, OpTab tab, string name, int min, int max, float x, float y, string descr)
    {
        var box = new OpTextBox(GetOrCreateConfig(name.RemoveSpaces(), min, new ConfigAcceptableRange<int>(min, max)), new() { x = x + name.GetTextSize() + 14f, y = 544f - y }, 60f) { allowSpace = true, description = descr };
        box.OnValueChanged += _CustomCentiInterface_UpdateIntField;
        box.OnValueChanged += _CustomCentiInterface_MakeSaveFileRed;
        tab._AddItem(new CustomOpLabel(x, 548f - y, name + ":") { bumpBehav = box.bumpBehav, description = descr });
        tab._AddItem(interUI[name] = box);
    }

    internal static void Dispose()
    {
        s_palette = null!;
        s_nullYesNo = null!;
        s_nullPalette = null!;
        s_CustomCentiInterface_MoveToFront = null!;
        s_configs = null!;
        s_f01 = null!;
        s_fS = null!;
        s_fU = null!;
        s_CustomCentiInterface_MakeTextGrey = null!;
    }

    void CustomCentiInterface_UpdateStringField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<string>)!;
        cfg.Value = value;
        if (_current is CustomCentiBreedParams current)
        {
            current.Set(cfg);
            UpdateDependencies();
        }
    }

    void CustomCentiInterface_UpdateRelationTypeField(UIconfig config, string value, string oldValue) => (config.cfgEntry as Configurable<string>)!.Value = value;

    void CustomCentiInterface_UpdateRelationIntensityField(UIconfig config, string value, string oldValue) => (config.cfgEntry as Configurable<float>)!.Value = value.ParseFloatInvariant();

    void CustomCentiInterface_UpdateShaderField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<string>)!;
        cfg.Value = value;
        if (!ShaderExists(value))
            (config as OpTextBox)!.colorText = s_coolRed;
        if (_current is CustomCentiBreedParams current)
        {
            current.Set(cfg);
            UpdateDependencies();
        }
    }

    static void CustomCentiInterface_MakeTextGrey(UIconfig config, string value, string oldValue) => (config as OpTextBox)!.colorText = MenuColorEffect.rgbMediumGrey;

    void CustomCentiInterface_UpdateSpriteField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<string>)!;
        cfg.Value = value;
        if (!SpriteExists(value) && !SpriteExists(value + "1"))
            (config as OpTextBox)!.colorText = s_coolRed;
        if (_current is CustomCentiBreedParams current)
        {
            current.Set(cfg);
            UpdateDependencies();
        }
    }

    void CustomCentiInterface_UpdateIntField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<int>)!;
        var accept = (cfg.info.acceptable as ConfigAcceptableRange<int>)!;
        var i = Clamp(value.ParseIntInvariant(), accept.MinValue, accept.MaxValue);
        cfg.Value = i;
        config.ForceValue(i.ToStringInvariant());
        if (_current is CustomCentiBreedParams current)
        {
            current.Set(cfg);
            UpdateDependencies();
        }
    }

    void CustomCentiInterface_UpdateFloatField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<float>)!;
        var accept = (cfg.info.acceptable as ConfigAcceptableRange<float>)!;
        var f = Clamp(value.ParseFloatInvariant(), accept.MinValue, accept.MaxValue);
        cfg.Value = f;
        config.ForceValue(f.ToStringInvariant());
        if (_current is CustomCentiBreedParams current)
        {
            current.Set(cfg);
            UpdateDependencies();
        }
    }

    void CustomCentiInterface_UpdateAlphaField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<float>)!;
        cfg.Value = value.ParseFloatInvariant();
        if (_current is CustomCentiBreedParams current)
        {
            current.SetAlpha(cfg);
            UpdateDependencies();
        }
    }

    void CustomCentiInterface_UpdateColorTypeField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<string>)!;
        cfg.Value = value;
        if (_current is CustomCentiBreedParams current)
        {
            current.SetColorType(cfg);
            UpdateDependencies();
        }
    }

    void CustomCentiInterface_UpdateColorField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<Color>)!;
        cfg.Value = value.HexToColor();
        if (_current is CustomCentiBreedParams current)
        {
            current.Set(cfg);
            UpdateDependencies();
        }
    }

    void CustomCentiInterface_UpdateBoolField(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<bool>)!;
        cfg.Value = value.ParseBoolInvariant();
        if (_current is CustomCentiBreedParams current)
        {
            current.Set(cfg);
            UpdateDependencies();
        }
    }

    void CustomCentiInterface_UpdateCurrent(UIconfig config, string value, string oldValue)
    {
        var cfg = (config.cfgEntry as Configurable<string>)!;
        cfg.Value = value;
        _current = string.IsNullOrEmpty(value) ? null : s_dict[value].Props;
        if (_current is CustomCentiBreedParams c)
        {
            ReactivateUI();
            var rels = _rels;
            string a = value + " > ", b = " > " + value + ":", oldNm = string.IsNullOrEmpty(oldValue) ? s_Def : oldValue;
            var nms = CreatureTemplate.Type.values.entries;
            var l = nms.Count;
            for (var i = 0; i < l; i++)
            {
                string nm = nms[l - i - 1], anm = a + nm + ":";
                ref readonly var rel = ref c.GetRelation(nm, true);
                var anmPos = 10f + anm.GetTextSize() + 8f;
                (rels[i]._elem as CustomOpLabel)!.text = anm;
                var cbox = (rels[i + l]._elem as CustomOpComboBox)!;
                cbox.PosX = anmPos;
                var val = rel.Type?.value ?? string.Empty;
                (cbox.cfgEntry as Configurable<string>)!.Value = val;
                cbox.ForceChangeValue(val, this);
                var slider = (rels[i + l * 2]._elem as CustomOpFloatSlider)!;
                slider.PosX = anmPos + 155f;
                (slider.cfgEntry as Configurable<float>)!.Value = rel.Intensity;
                slider.ForceChangeValue(rel.Intensity.ToStringInvariant(), this);
                anm = nm + b;
                rel = ref c.GetRelation(nm, false);
                anmPos = 10f + anm.GetTextSize() + 8f;
                var label = (rels[i + l * 3]._elem as CustomOpLabel)!;
                label.text = anm;
                cbox = (rels[i + l * 4]._elem as CustomOpComboBox)!;
                cbox.PosX = anmPos;
                val = rel.Type?.value ?? string.Empty;
                (cbox.cfgEntry as Configurable<string>)!.Value = val;
                cbox.ForceChangeValue(val, this);
                slider = (rels[i + l * 5]._elem as CustomOpFloatSlider)!;
                slider.PosX = anmPos + 155f;
                (slider.cfgEntry as Configurable<float>)!.Value = rel.Intensity;
                slider.ForceChangeValue(rel.Intensity.ToStringInvariant(), this);
                if (nm == value)
                {
                    cbox.ResetAndDeactivate(this);
                    slider.ResetAndDeactivate(this);
                    //label.color = s_grey2;
                }
                /*else if (nm == oldNm)
                    label.color = MenuColorEffect.rgbMediumGrey;*/
            }
            UpdateFields();
        }
        else
        {
            DeactivateUI();
            var rels = _rels;
            var oldNm = string.IsNullOrEmpty(oldValue) ? s_Def : oldValue;
            var nms = CreatureTemplate.Type.values.entries;
            var l = nms.Count;
            for (var i = 0; i < l; i++)
            {
                string nm = nms[l - i - 1], anm = "------ > " + nm + ":";
                var anmPos = 10f + anm.GetTextSize() + 8f;
                (rels[i]._elem as CustomOpLabel)!.text = anm;
                (rels[i + l]._elem as CustomOpComboBox)!.PosX = anmPos;
                (rels[i + l * 2]._elem as CustomOpFloatSlider)!.PosX = anmPos + 155f;
                anm = nm + " > ------:";
                anmPos = 10f + anm.GetTextSize() + 8f;
                var label = (rels[i + l * 3]._elem as CustomOpLabel)!;
                label.text = anm;
                var cbox = (rels[i + l * 4]._elem as CustomOpComboBox)!;
                cbox.PosX = anmPos;
                var slider = (rels[i + l * 5]._elem as CustomOpFloatSlider)!;
                slider.PosX = anmPos + 155f;
                /*if (nm == oldNm)
                    label.color = MenuColorEffect.rgbMediumGrey;*/
            }
        }
    }

    void UpdateRelations()
    {
        if (_current is CustomCentiBreedParams c)
        {
            var value = c.CreatureType.value;
            var rels = _rels;
            string a = value + " > ", b = " > " + value + ":";
            var nms = CreatureTemplate.Type.values.entries;
            var l = nms.Count;
            for (var i = 0; i < l; i++)
            {
                string nm = nms[l - i - 1], anm = a + nm + ":";
                ref readonly var rel = ref c.GetRelation(nm, true);
                var anmPos = 10f + anm.GetTextSize() + 8f;
                (rels[i]._elem as CustomOpLabel)!.text = anm;
                var cbox = (rels[i + l]._elem as CustomOpComboBox)!;
                cbox.PosX = anmPos;
                var val = rel.Type?.value ?? string.Empty;
                (cbox.cfgEntry as Configurable<string>)!.Value = val;
                cbox.ForceChangeValue(val, this);
                var slider = (rels[i + l * 2]._elem as CustomOpFloatSlider)!;
                slider.PosX = anmPos + 155f;
                (slider.cfgEntry as Configurable<float>)!.Value = rel.Intensity;
                slider.ForceChangeValue(rel.Intensity.ToStringInvariant(), this);
                anm = nm + b;
                rel = ref c.GetRelation(nm, false);
                anmPos = 10f + anm.GetTextSize() + 8f;
                var label = (rels[i + l * 3]._elem as CustomOpLabel)!;
                label.text = anm;
                cbox = (rels[i + l * 4]._elem as CustomOpComboBox)!;
                cbox.PosX = anmPos;
                val = rel.Type?.value ?? string.Empty;
                (cbox.cfgEntry as Configurable<string>)!.Value = val;
                cbox.ForceChangeValue(val, this);
                slider = (rels[i + l * 5]._elem as CustomOpFloatSlider)!;
                slider.PosX = anmPos + 155f;
                (slider.cfgEntry as Configurable<float>)!.Value = rel.Intensity;
                slider.ForceChangeValue(rel.Intensity.ToStringInvariant(), this);
                if (nm == value)
                {
                    cbox.ResetAndDeactivate(this);
                    slider.ResetAndDeactivate(this);
                    //label.color = s_grey2;
                }
            }
        }
    }

    void CustomCentiInterface_SetParentFields(UIfocusable trigger)
    {
        if (_current is CustomCentiBreedParams c && c.ParentType is CreatureTemplate.Type tp)
        {
            foreach (var kvpair in _interUI)
            {
                var key = kvpair.Key;
                if (key != s_Current && key != s_Path && key != s_Inherit && kvpair.Value is UIconfig cfg)
                    cfg.value = cfg.defaultValue;
            }
            var rels = _rels;
            for (var i = 0; i < rels.Length; i++)
            {
                if (rels[i]._elem is UIconfig cfg)
                    cfg.value = cfg.defaultValue;
            }
            c.Relationships = null;
            c._realRelationships.Clear();
            CustomCentiTemplates.SetParentFields(c, ComputeStringHash(tp.value));
            CustomCentiTemplates.SetParentRelationships(c);
            UpdateRelations();
            UpdateFields();
            CustomCentiTemplates.UpdateTemplateFields(c);
            (_interUI[s_Save_Relationships] as CustomOpSimpleButton)!.colorEdge = s_coolGreen;
            c._relationsChanged = false;
        }
    }

    static void CustomCentiInterface_MoveToFront(UIfocusable trigger)
    {
        if (!trigger.greyedOut)
            trigger.MoveToFront();
    }

    void CustomCentiInterface_MakeSaveRelationshipsRed(UIconfig config, string value, string oldValue)
    {
        if (_current is CustomCentiBreedParams c && !c._relationsChanged)
        {
            (_interUI[s_Save_Relationships] as CustomOpSimpleButton)!.colorEdge = s_coolRed;
            c._relationsChanged = true;
        }
    }

    void CustomCentiInterface_MakeSaveFileRed(UIconfig config, string value, string oldValue)
    {
        if (_current is CustomCentiBreedParams c && !c._propsChanged)
        {
            (_interUI[s_Save_File] as CustomOpSimpleButton)!.colorEdge = s_coolRed;
            c._propsChanged = true;
        }
    }

    void CustomCentiInterface_MakeSaveFileRed2(UIfocusable trigger)
    {
        if (_current is CustomCentiBreedParams c && !c._propsChanged)
        {
            (_interUI[s_Save_File] as CustomOpSimpleButton)!.colorEdge = s_coolRed;
            c._propsChanged = true;
        }
    }

    void CustomCentiInterface_MakeSaveRelationshipsGreen(UIfocusable trigger)
    {
        if (_current is CustomCentiBreedParams c && c._relationsChanged)
        {
            (trigger as CustomOpSimpleButton)!.colorEdge = s_coolGreen;
            c._relationsChanged = false;
        }
    }

    void CustomCentiInterface_MakeSaveFileGreen(UIfocusable trigger)
    {
        if (_current is CustomCentiBreedParams c && c._propsChanged)
        {
            (trigger as CustomOpSimpleButton)!.colorEdge = s_coolGreen;
            c._propsChanged = false;
        }
    }

    void CustomCentiInterface_MakeArrowsRed(UIconfig config, string value, string oldValue)
    {
        if (_current is CustomCentiBreedParams c && !c._requiresReloading)
        {
            var rlds = _rlds;
            for (var i = 0; i < rlds.Count; i++)
            {
                var r = rlds[i]._img;
                r.color = s_coolRed;
                r.description = s_Changes_detected;
            }
            c._requiresReloading = true;
        }
    }

    void CustomCentiInterface_SaveFile(UIfocusable trigger)
    {
        if (_current is CustomCentiBreedParams c)
        {
            var pth = c.Path;
            c.Path = c._tempPath;
            try
            {
                c.SaveFile();
            }
            catch
            {
                c.Path = pth;
                (_interUI[s_Path] as OpTextBox)!.colorText = s_coolRed;
                return;
            }
            if (c.Path != pth && File.Exists(pth))
                File.Delete(pth);
        }
    }

    void CustomCentiInterface_ResetFields(UIfocusable trigger)
    {
        _centiIcon.color = default;
        _centiIcon.sprite.element = Futile.atlasManager.GetElementWithName(s_Kill_Centipede2);
        foreach (var kvpair in _interUI)
        {
            var key = kvpair.Key;
            if (key != s_Current && key != s_Path && kvpair.Value is UIconfig config)
                config.value = config.defaultValue;
        }
        var rels = _rels;
        for (var i = 0; i < rels.Length; i++)
        {
            if (rels[i]._elem is UIconfig config)
                config.value = config.defaultValue;
        }
        if (_current is CustomCentiBreedParams c)
        {
            c.Relationships = null;
            c._realRelationships.Clear();
        }
    }

    void CustomCentiInterface_SaveRelations(UIfocusable trigger)
    {
        if (_current is CustomCentiBreedParams c)
        {
            var cnm = c.CreatureType.value;
            var rels = _rels;
            var l = rels.Length / 6;
            var rela = new List<CustomRelation>(l * 2 - 1);
            for (var i = 0; i < l; i++)
            {
                CreatureTemplate.Relationship.Type relType;
                float intensity;
                string[] nms;
                var val = ((rels[i + l]._elem as CustomOpComboBox)!.cfgEntry as Configurable<string>)!.Value;
                if (!string.IsNullOrEmpty(val))
                {
                    relType = new(val);
                    intensity = ((rels[i + l * 2]._elem as CustomOpFloatSlider)!.cfgEntry as Configurable<float>)!.Value;
                    nms = Regex.Split((rels[i]._elem as CustomOpLabel)!.text.Replace(":", string.Empty), " > ");
                    rela.Add(new()
                    {
                        ActivePosition = true,
                        Target = new(nms[1]),
                        Type = relType,
                        Intensity = intensity,
                    });
                }
                val = ((rels[i + l * 4]._elem as CustomOpComboBox)!.cfgEntry as Configurable<string>)!.Value;
                if (!string.IsNullOrEmpty(val))
                {
                    nms = Regex.Split((rels[i + l * 3]._elem as CustomOpLabel)!.text.Replace(":", string.Empty), " > ");
                    var nm = nms[0];
                    if (nm != cnm)
                    {
                        relType = new(val);
                        intensity = ((rels[i + l * 5]._elem as CustomOpFloatSlider)!.cfgEntry as Configurable<float>)!.Value;
                        rela.Add(new()
                        {
                            Target = new(nm),
                            Type = relType,
                            Intensity = intensity,
                        });
                    }
                }
            }
            c.Relationships = rela.ToArray();
            c._realRelationships.Clear();
            c._critob.SetCustomRelationships();
        }
    }
}

sealed class CustomOpFloatSlider : OpFloatSlider
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CustomOpFloatSlider(Configurable<float> config, Vector2 pos, int length, byte decimalNum = 1, bool vertical = false) : base(config, pos, length, decimalNum, vertical) => OnFocusGet += CustomCentiInterface.s_CustomCentiInterface_MoveToFront;

    public override void GrafUpdate(float timeStacker)
    {
        if (greyedOut)
            _rect.fillAlpha = bumpBehav.FillAlpha;
        base.GrafUpdate(timeStacker);
        if (greyedOut)
            _rect.GrafUpdate(timeStacker);
    }

    public override void Change()
    {
        base.Change();
        _label.text = this.GetValueFloat().ToString("N2", NumberFormatInfo.InvariantInfo);
    }
}

sealed class CustomOpLabel : OpLabel
{
    internal BumpBehaviour? _bumpBehav2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CustomOpLabel(Vector2 pos, Vector2 size, string text = "TEXT", FLabelAlignment alignment = FLabelAlignment.Center, bool bigText = false, FTextParams? textParams = null) : base(pos, size, text, alignment, bigText, textParams) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CustomOpLabel(float posX, float posY, string text = "TEXT", bool bigText = false) : base(posX, posY, text, bigText) { }

    public override void Change()
    {
        if (_bumpBehav2?.owner == this)
            _bumpBehav2.Update();
        base.Change();
    }

    public override void GrafUpdate(float timeStacker)
    {
        base.GrafUpdate(timeStacker);
        if (!_IsLong)
        {
            if (bumpBehav is not BumpBehaviour bhv1)
            {
                if (_bumpBehav2 is not BumpBehaviour bhv2)
                    label.color = color;
                else
                    label.color = bhv2.GetColor(color);
            }
            else
            {
                if (_bumpBehav2 is not BumpBehaviour bhv2)
                    label.color = bhv1.GetColor(color);
                else
                    label.color = bhv1.greyedOut ? bhv2.GetColor(color) : (bhv2.greyedOut ? bhv1.GetColor(color) : bhv1.GetColor(bhv2.GetColor(color)));
            }
        }
    }
}

sealed class ReloadArrow : OpImage
{
    internal BumpBehaviour? _bumpBehav, _bumpBehav2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReloadArrow(Vector2 pos, string fAtlasElement) : base(pos, fAtlasElement) { }

    public override void Change()
    {
        if (_bumpBehav?.owner == this)
            _bumpBehav.Update();
        if (_bumpBehav2?.owner == this)
            _bumpBehav2.Update();
        base.Change();
    }

    public override void GrafUpdate(float timeStacker)
    {
        base.GrafUpdate(timeStacker);
        if (sprite is FSprite s)
        {
            if (_bumpBehav is not BumpBehaviour bhv1)
            {
                if (_bumpBehav2 is not BumpBehaviour bhv2)
                    s.color = color;
                else
                    s.color = bhv2.GetColor(bhv2.greyedOut ? s_coolGreen : color);
            }
            else
            {
                if (_bumpBehav2 is not BumpBehaviour bhv2)
                    s.color = bhv1.GetColor(bhv1.greyedOut ? s_coolGreen : color);
                else
                    s.color = bhv1.greyedOut ? bhv2.GetColor(bhv2.greyedOut ? s_coolGreen : color) : (bhv2.greyedOut ? bhv1.GetColor(bhv1.greyedOut ? s_coolGreen : color) : bhv1.GetColor(bhv2.GetColor(color)));
            }
        }
    }
}

sealed class CustomOpComboBox : OpComboBox
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CustomOpComboBox(Configurable<string> config, Vector2 pos, float width, List<ListItem> list) : base(config, pos, width, list) => OnListOpen += CustomCentiInterface.s_CustomCentiInterface_MoveToFront;

    public override void _MouseModeUpdate()
    {
        if (greyedOut)
            return;
        base._MouseModeUpdate();
    }

    public override void _NonMouseModeUpdate()
    {
        if (greyedOut)
            return;
        base._NonMouseModeUpdate();
    }
}

sealed class CustomOpColorPicker : OpColorPicker
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CustomOpColorPicker(Configurable<Color> config, Vector2 pos) : base(config, pos) { }

    public override void GreyOut()
    {
        if (_greyTrigger)
            _SwitchMode(PickerMode.HSL);
        base.GreyOut();
    }
}

sealed class CustomOpSimpleButton : OpSimpleButton
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CustomOpSimpleButton(Vector2 pos, Vector2 size, string displayText) : base(pos, size, displayText) { }

    public override void GrafUpdate(float timeStacker)
    {
        if (greyedOut)
            _rect.fillAlpha = bumpBehav.FillAlpha;
        base.GrafUpdate(timeStacker);
    }
}

sealed class CustomOpCheckBox : OpCheckBox
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CustomOpCheckBox(Configurable<bool> config, float posX, float posY) : base(config, posX, posY) { }

    public override void GrafUpdate(float timeStacker)
    {
        if (greyedOut)
            rect.fillAlpha = bumpBehav.FillAlpha;
        base.GrafUpdate(timeStacker);
    }
}

sealed class LongStringOpTextBox : OpTextBox
{
    internal readonly int _staticLength;
    internal string _temp = string.Empty, _lastTemp = string.Empty;

    internal LongStringOpTextBox(ConfigurableBase config, Vector2 pos, float sizeX, bool allowSpace) : base(config, pos, sizeX)
    {
        _staticLength = Mathf.FloorToInt((size.x - 20f) / LabelTest.CharMean(false));
        this.allowSpace = allowSpace;
        _maxLength = 30000;
        label.text = _value;
        _cursor.SetPosition(LabelTest.GetWidth(_value) + LabelTest.CharMean(false), size.y * .5f);
    }

    public override bool held
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _held;
        set
        {
            if (_held == value)
                return;
            _held = value;
            (s_OnHeld.GetValue(this) as OnHeldHandler)?.Invoke(_held);
            if (value)
            {
                if (ContextWrapped)
                    wrapper.menu.selectedObject = wrapper;
                else
                    ConfigContainer.instance._FocusNewElement(this);
            }
            else if (!Focused)
                return;
            if (ContextWrapped)
                wrapper.tabWrapper.holdElement = value;
            else
                ConfigContainer.holdElement = value;
            if (!value && Focused && (lastValue != _value || _lastTemp != _temp))
                (s_OnValueChanged.GetValue(this) as OnValueChangeHandler)?.Invoke(this, _temp + _value, _lastTemp + lastValue);
            _lastTemp = _temp;
            lastValue = _value;
        }
    }

    public override string value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _temp + _value;
        set
        {
            if (_value == value && string.IsNullOrEmpty(_temp))
                return;
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            if (!allowSpace)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    if (char.IsWhiteSpace(value[i]))
                        return;
                }
            }
            if (value.Length > 0 && !s_A.IsMatch(value))
                return;
            var temp = string.Empty;
            if (value.Length > _staticLength)
            {
                var t = value.Length - _staticLength;
                temp = value.Remove(t, _staticLength);
                value = value.Remove(0, t);
            }
            if (_KeyboardOn && Input.anyKey && !Input.GetKey(KeyCode.Backspace))
                PlaySound(SoundID.MENU_Checkbox_Uncheck);
            if (_value != value || _temp != temp)
            {
                FocusMoveDisallow();
                var oldValue = this.value;
                _temp = temp;
                _value = value;
                var newVal = temp + value;
                if (!ContextWrapped && ConfigContainer.instance is not null)
                    ConfigContainer.instance.NotifyConfigChange(this, oldValue, newVal);
                (s_OnValueUpdate.GetValue(this) as OnValueChangeHandler)?.Invoke(this, newVal, oldValue);
                Change();
                if (!held)
                {
                    (s_OnValueChanged.GetValue(this) as OnValueChangeHandler)?.Invoke(this, newVal, _lastTemp + lastValue);
                    _lastTemp = temp;
                    lastValue = value;
                }
            }
        }
    }

    public override void Change()
    {
        base.Change();
        label.text = _value;
        _curTextWidth = LabelTest.GetWidth(_value);
    }
}

static class CustomInterfaceExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ResetAndDeactivate(this UIfocusable config, CustomCentiInterface interf)
    {
        var cfg = (config as UIconfig)!;
        cfg.ForceChangeValue(cfg.defaultValue, interf);
        cfg.greyedOut = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ForceChangeValue(this UIconfig config, string value, CustomCentiInterface interf)
    {
        var temp = interf._current;
        interf._current = null;
        config.value = value;
        interf._current = temp;
    }

    internal static void AddItems(this OpScrollBox self, CustomCentiInterface.ElemWrap[] items)
    {
        for (var i = 0; i < items.Length; i++)
        {
            var uIelement = items[i]._elem;
            if (uIelement._AddToScrollBox(self))
            {
                self.tab.AddItems(uIelement);
                self.items.Add(uIelement);
                if (self._lastFocusedElement is null && uIelement is UIfocusable f)
                    self._lastFocusedElement = f;
            }
        }
    }
}