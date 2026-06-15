using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion.Numeric;

public abstract partial class UserSettingInt<T> : UserSetting<T, int> where T : UserSettingInt<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out int typedValue)
    {
        if (UserSettingNumericCommon.IsNotNumeric(variant))
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (int) variant;
        return true;
    }

    protected override sealed bool TryConvertValue(int typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}