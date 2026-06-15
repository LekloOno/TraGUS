using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion;

public abstract partial class UserSettingBool<T> : UserSetting<T, bool> where T : UserSettingBool<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out bool typedValue)
    {
        if (variant.VariantType is not Variant.Type.Bool)
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (bool) variant;
        return true;
    }

    protected override sealed bool TryConvertValue(bool typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}