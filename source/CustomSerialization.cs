global using static CustomCentisMod.CustomSerialization;
using UnityEngine;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;

namespace CustomCentisMod;

public static class CustomSerialization
{
    /*
    constants:
    string
    number
    Y
    N
    symbols:
    ~
    #
    :
    =
    ;
    methods:
    HSLA
    RGBA
    */
    public static readonly char Yes = 'Y', No = 'N', Separator = '~', Comment = '#', Method = ':', Definition = '=', ArraySeparator = ';';

    public static StringBuilder Feed(this StringBuilder sb, string name, CustomRelation[]? rels)
    {
        if (rels?.Length > 0)
        {
            sb.Append(name).Bind();
            var last = rels.Length - 1;
            for (var i = 0; i < rels.Length; i++)
            {
                ref readonly var rel = ref rels[i];
                sb.Code(rel.ActivePosition).Link().Append(rel.Target?.value).Link().Append(rel.Type?.value).Link().Append(rel.Intensity.ToStringInvariant());
                if (i != last)
                    sb.Part();
            }
            sb.AppendLine();
        }
        return sb;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] UndoLink(this string s) => s.Split(s_sep);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] UndoWork(this string s) => s.Split(s_meth);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] UndoBind(this string s) => s.Split(s_def);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] UndoPart(this string s) => s.Split(s_arSep);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Code(this StringBuilder sb, bool value) => sb.Append(value ? Yes : No);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Link(this StringBuilder sb) => sb.Append(Separator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Part(this StringBuilder sb) => sb.Append(ArraySeparator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Bind(this StringBuilder sb) => sb.Append(Definition);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Work(this StringBuilder sb) => sb.Append(Method);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Hint(this StringBuilder sb) => sb.Append(Comment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToStringInvariant(this float f) => f.ToString(CultureInfo.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToStringInvariant(this int i) => i.ToString(CultureInfo.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToStringInvariant(this bool b) => b.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ParseFloatInvariant(this string s, out float f) => float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ParseIntInvariant(this string s, out int i) => int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float ParseFloatInvariant(this string s)
    {
        float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var f);
        return f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ParseIntInvariant(this string s)
    {
        int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var i);
        return i;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ParseBoolInvariant(this string s)
    {
        bool.TryParse(s, out var b);
        return b;
    }

    internal static Color HexToColor(this string hex)
    {
        var a = hex.Length == 8 ? (Convert.ToInt32(hex.Substring(6, 2), 16) / 255f) : 1f;
        return new(Convert.ToInt32(hex.Substring(0, 2), 16) / 255f, Convert.ToInt32(hex.Substring(2, 2), 16) / 255f, Convert.ToInt32(hex.Substring(4, 2), 16) / 255f, a);
    }

    internal static string ColorToHex(this Color color) => Mathf.RoundToInt(color.r * 255f).ToString("X2", CultureInfo.InvariantCulture) + Mathf.RoundToInt(color.g * 255f).ToString("X2", CultureInfo.InvariantCulture) + Mathf.RoundToInt(color.b * 255f).ToString("X2", CultureInfo.InvariantCulture);

    public static StringBuilder Feed(this StringBuilder sb, string name, in CustomRelation rel) => sb.Append(name).Bind().Code(rel.ActivePosition).Link().Append(rel.Target?.value).Link().Append(rel.Type?.value).Link().AppendLine(rel.Intensity.ToStringInvariant());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed(this StringBuilder sb, string name, bool flag) => sb.Append(name).Bind().Code(flag).AppendLine();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed(this StringBuilder sb, string name, bool flag, bool hasValue) => hasValue ? sb.Feed(name, flag) : sb;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed(this StringBuilder sb, string name, float value) => sb.Append(name).Bind().AppendLine(value.ToStringInvariant());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed(this StringBuilder sb, string name, int value) => sb.Append(name).Bind().AppendLine(value.ToStringInvariant());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed(this StringBuilder sb, string name, ExtEnumBase? type) => type is not null ? sb.Append(name).Bind().AppendLine(type.value) : sb;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Tell(this StringBuilder sb, string name, string s) => sb.Hint().Append(name).Bind().AppendLine(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed<T>(this StringBuilder sb, string name, T type) where T : struct, Enum => sb.Append(name).Bind().AppendLine(type.ToString().ToUpperInvariant());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed<T>(this StringBuilder sb, string name, T type, bool hasValue) where T : struct, Enum => hasValue ? sb.Feed(name, type) : sb;

    public static StringBuilder Feed(this StringBuilder sb, string name, PaletteColorType type, Color defaultColor) =>
        type != PaletteColorType.Custom ? sb.Append(name).Bind().AppendLine(type.ToString().ToUpperInvariant())
        : sb.Append(name).Bind().Append("RGBA").Work().Append(defaultColor.r.ToStringInvariant()).Link().Append(defaultColor.g.ToStringInvariant()).Link().Append(defaultColor.b.ToStringInvariant()).Link().AppendLine(defaultColor.a.ToStringInvariant());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed(this StringBuilder sb, string name, PaletteColorType type, Color defaultColor, bool hasValue) => hasValue ? sb.Feed(name, type, defaultColor) : sb;

    public static StringBuilder Feed(this StringBuilder sb, string name, Color clr) => sb.Append(name).Bind().Append("RGBA").Work().Append(clr.r.ToStringInvariant()).Link().Append(clr.g.ToStringInvariant()).Link().Append(clr.b.ToStringInvariant()).Link().AppendLine(clr.a.ToStringInvariant());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Feed(this StringBuilder sb, string name, string? s) => string.IsNullOrEmpty(s) ? sb : sb.Append(name).Bind().AppendLine(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ParseBool(string s)
    {
        s = s.RemoveSpaces().ToUpperInvariant();
        return s.Length > 0 && s[0] == Yes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ParseEnum<T>(string s) where T : struct, Enum
    {
        Enum.TryParse<T>(s.RemoveSpaces(), true, out var t);
        return t;
    }

    public static CustomRelation ParseCustomRelation(string s)
    {
        var res = default(CustomRelation);
        var sAr = s.RemoveSpaces().UndoLink();
        if (sAr.Length > 3)
        {
            res.ActivePosition = ParseBool(sAr[0]);
            res.Target = new(sAr[1]);
            res.Type = new(sAr[2]);
            res.Intensity = ParseFloat(sAr[3]);
        }
        return res;
    }

    public static CustomRelation[]? ParseCustomRelationArray(string s)
    {
        var sAr = s.RemoveSpaces().UndoPart();
        if (sAr.Length > 0)
        {
            var list = new List<CustomRelation>();
            for (var i = 0; i < sAr.Length; i++)
                list.Add(ParseCustomRelation(sAr[i]));
            return [.. list];
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseInt(string s)
    {
        s.RemoveSpaces().ParseIntInvariant(out var res);
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ParseFloat(string s)
    {
        s.RemoveSpaces().ParseFloatInvariant(out var res);
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void SetColorField(ref Color c, int i, float val)
    {
        switch (i)
        {
            case 0:
                c.r = val;
                break;
            case 1:
                c.g = val;
                break;
            case 2:
                c.b = val;
                break;
            default:
                c.a = val;
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void SetVector4Field(ref Vector4 c, int i, float val)
    {
        switch (i)
        {
            case 0:
                c.x = val;
                break;
            case 1:
                c.y = val;
                break;
            case 2:
                c.z = val;
                break;
            default:
                c.w = val;
                break;
        }
    }

    static Color ParseRGBAColor(string[] sAr)
    {
        var res = default(Color);
        for (var i = 0; i < sAr.Length; i++)
        {
            sAr[i].ParseFloatInvariant(out var t);
            SetColorField(ref res, i, t);
        }
        return res;
    }

    static Color ParseHSLAColor(string[] sAr)
    {
        var res = default(Vector4);
        for (var i = 0; i < sAr.Length; i++)
        {
            sAr[i].ParseFloatInvariant(out var t);
            SetVector4Field(ref res, i, t);
        }
        return HSL2RGB(res.x, res.y, res.z, res.w);
    }

    public static Color ParseColor(string s)
    {
        var res = default(Color);
        var sf = s.RemoveSpaces().ToUpperInvariant().UndoWork();
        if (sf.Length > 1)
        {
            if (sf[0] == "RGBA")
                res = ParseRGBAColor(sf[1].UndoLink());
            else if (sf[0] == "HSLA")
                res = ParseHSLAColor(sf[1].UndoLink());
        }
        return res;
    }

    public static (PaletteColorType Type, Color Clr) ParsePaletteColor(string s)
    {
        var res = default((PaletteColorType tp, Color clr));
        s = s.RemoveSpaces().ToUpperInvariant();
        if (s.Length > 0 && !Enum.TryParse(s, true, out res.tp))
        {
            var sf = s.UndoWork();
            if (sf[0] == "RGBA")
                res.clr = ParseRGBAColor(sf[1].UndoLink());
            else if (sf[0] == "HSLA")
                res.clr = ParseHSLAColor(sf[1].UndoLink());
        }
        return res;
    }
}