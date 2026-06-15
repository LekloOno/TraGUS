using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion;

public abstract partial class UserSettingVector2I<T> : UserSetting<T, Vector2I> where T : UserSettingVector2I<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out Vector2I typedValue)
    {
        if (variant.VariantType is not Variant.Type.Vector2I)
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (Vector2I) variant;
        return true;
    }

    protected override sealed bool TryConvertValue(Vector2I typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}