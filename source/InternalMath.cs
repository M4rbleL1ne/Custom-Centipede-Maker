global using static CustomCentisMod.InternalMath;
using System.Runtime.CompilerServices;
using UnityEngine;
using RWCustom;
using System;

namespace CustomCentisMod;

static class InternalMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Lerp(int a, int b, float t) => (int)(a + (b - a) * Clamp01(t));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Clamp(int value, int min, int max) => value < min ? min : (value > max ? max : value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Clamp01(float value) => value < 0f ? 0f : (value > 1f ? 1f : value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float InverseLerp(float a, float b, float value)
    {
        if (a != b)
            return Clamp01((value - a) / (b - a));
        return 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Above0(float val)
    {
        if (val > 0f)
            return val;
        return 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Above0(int val)
    {
        if (val > 0)
            return val;
        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Above1(int val)
    {
        if (val > 1)
            return val;
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Above2(int val)
    {
        if (val > 2)
            return val;
        return 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float LerpMap(float val, float fromA, float toA, float fromB, float toB) => Lerp(fromB, toB, InverseLerp(fromA, toA, val));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Color Lerp(Color a, Color b, float t)
    {
        t = Clamp01(t);
        a.r += (b.r - a.r) * t;
        a.g += (b.g - a.g) * t;
        a.b += (b.b - a.b) * t;
        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Color Clamp01(Color a)
    {
        a.r = Clamp01(a.r);
        a.g = Clamp01(a.g);
        a.b = Clamp01(a.b);
        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Color HSL2RGB(float h, float s, float l, float a) => Custom.HSL2RGB(h, s, l) with { a = a };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float AimFromOneVectorToAnother(Vector2 p1, Vector2 p2) => VecToDeg(p2 - p1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float VecToDeg(Vector2 v) => (float)Math.Atan2(v.x, v.y) / (Mathf.PI * 2f) * 360f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 DegToVec(float ang)
    {
        var th = ang * (Mathf.PI / 180f);
        return new() { x = (float)Math.Sin(th), y = (float)Math.Cos(th) };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 DirVec(Vector2 p1, Vector2 p2)
    {
        if (p1 == p2)
            return Vector2.up;
        p1.x = p2.x - p1.x;
        p1.y = p2.y - p1.y;
        p1.Normalize();
        return p1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 PerpendicularVector(Vector2 v)
    {
        v.Normalize();
        var x = v.x;
        v.x = -v.y;
        v.y = x;
        return v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ComputeStringHash(string s)
    {
        /*
        var fs = typeof(CustomCentiBreedParams).GetFields();
        Console.Write("const int ");
        for (var i = 0; i < fs.Length; i++)
        {
            var nm = fs[i].Name;
            if (i != 0)
                Console.Write("        ");
            Console.Write("K_" + nm + " = " + ComputeStringHash(nm));
            if (i != fs.Length - 1)
                Console.WriteLine(",");
            else
                Console.WriteLine(";");
        }
        */
        var num = -2128831035;
        for (var i = 0; i < s.Length; i++)
            num = (s[i] ^ num) * 16777619;
        return num;
    }

    internal static float GetTextSize(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return 0f;
	    var fontWithName = Futile.atlasManager.GetFontWithName(Custom.GetFont());
	    var num = 0f;
	    var quadInfoForText = fontWithName.GetQuadInfoForText(s, s_defTParams);
	    for (var i = 0; i < quadInfoForText.Length; i++)
            num = Math.Max(num, quadInfoForText[i].bounds.width);
	    return num;
    }

    /*static int GetCharSize(char c) => c switch
    {
        'i' or 'j' or 'l' or ' ' or ':' => 2,
        'I' => 3,
        'f' or 'r' or 's' or 't' or 'J' or '1' => 4,
        'a' or 'c' or 'y' or 'z' or 'E' or 'F' or 'L' or '-' or '>' => 5,
        'n' or 'A' or 'B' or 'K' or 'R' or 'V' or 'X' => 7,
        'D' or 'G' or 'H' or 'N' or 'O' or 'Q' or 'U' => 8,
        'm' or 'w' => 9,
        'M' => 10,
        'W' => 11,
        _ => 6
    }; + 1 for space between letters*/

    /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string AddSpaces(this string s) => s_addSpaces.Replace(s, "$1 $2");*/

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string RemoveSpaces(this string s) => s.Replace(" ", string.Empty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CombineHashCodes(int hash1, int hash2) => unchecked((((hash1 << 5) | (hash1 >> 27)) + hash1) ^ hash2);
}