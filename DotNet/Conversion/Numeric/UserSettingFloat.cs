using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion.Numeric;

public abstract partial class UserSettingFloat<T> : UserSetting<T, float> where T : UserSettingFloat<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out float typedValue)
    {
        if (UserSettingNumericCommon.IsNotNumeric(variant))
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (float) variant;
        return true;
    }

    protected override sealed bool TryConvertValue(float typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}