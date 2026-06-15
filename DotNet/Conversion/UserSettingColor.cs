using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion;

public abstract partial class UserSettingColor<T> : UserSetting<T, Color> where T : UserSettingColor<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out Color typedValue)
    {
        if (variant.VariantType is not Variant.Type.Color)
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (Color) variant;
        return true;
    }

    protected override sealed bool TryConvertValue(Color typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}