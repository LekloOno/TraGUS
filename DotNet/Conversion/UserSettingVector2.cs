using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion;

public abstract partial class UserSettingVector2<T> : UserSetting<T, Vector2> where T : UserSettingVector2<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out Vector2 typedValue)
    {
        if (variant.VariantType is not Variant.Type.Vector2)
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (Vector2) variant;
        return true;
    }

    protected override sealed bool TryConvertValue(Vector2 typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}