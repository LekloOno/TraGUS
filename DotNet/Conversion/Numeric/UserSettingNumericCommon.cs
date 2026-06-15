using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion.Numeric;

public static class UserSettingNumericCommon
{
    public static bool IsNumeric(Variant variant) =>
        variant.VariantType is Variant.Type.Float ||
        variant.VariantType is Variant.Type.Int;

    public static bool IsNotNumeric(Variant variant) =>
        variant.VariantType is not Variant.Type.Int &&
        variant.VariantType is not Variant.Type.Float;
}