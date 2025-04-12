using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CustomCentisMod;

[StructLayout(LayoutKind.Sequential)]
public struct CustomRelation : IEquatable<CustomRelation> // 24 - in
{
    public CreatureTemplate.Type Target;
    public CreatureTemplate.Relationship.Type Type;
    public float Intensity;
    public bool ActivePosition;
    // + 3 padding

    public readonly bool Equals(CustomRelation r) => r.Target == Target && r.Type == Type && r.Intensity == Intensity && r.ActivePosition == ActivePosition;

    public override readonly bool Equals(object obj) => obj is CustomRelation r && r.Target == Target && r.Type == Type && r.Intensity == Intensity && r.ActivePosition == ActivePosition;

    public override readonly int GetHashCode() => CombineHashCodes(CombineHashCodes(CombineHashCodes(Target?.Index ?? 0, Type?.Index ?? 0), Intensity.GetHashCode()), ActivePosition.GetHashCode());

    public override readonly string ToString() => new StringBuilder().Code(ActivePosition).Link().Append(Target?.value).Link().Append(Type?.value).Link().Append(Intensity.ToStringInvariant()).ToString();
}

[StructLayout(LayoutKind.Sequential)] // 16
internal struct RelationKey : IEquatable<RelationKey>
{
    internal CreatureTemplate.Type _a, _b;

    public readonly bool Equals(RelationKey r) => r._a == _a && r._b == _b;

    public override readonly bool Equals(object obj) => obj is RelationKey r && r._a == _a && r._b == _b;

    public override readonly int GetHashCode() => CombineHashCodes(_a?.Index ?? 0, _b?.Index ?? 0);

    public override readonly string ToString() => string.Empty;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RelationValue : IEquatable<RelationValue> // 16
{
    internal CreatureTemplate.Relationship.Type _type;
    internal float _intensity;
    // + 4 padding

    public readonly bool Equals(RelationValue r) => r._type == _type && r._intensity == _intensity;

    public override readonly bool Equals(object obj) => obj is RelationValue r && r._type == _type && r._intensity == _intensity;

    public override readonly int GetHashCode() => CombineHashCodes(_type?.Index ?? 0, _intensity.GetHashCode());

    public override readonly string ToString() => string.Empty;
}