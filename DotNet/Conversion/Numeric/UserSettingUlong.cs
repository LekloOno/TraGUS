using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion.Numeric;

public abstract partial class UserSettingUlong<T> : UserSetting<T, ulong> where T : UserSettingUlong<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out ulong typedValue)
    {
        typedValue = Tval;
        if (UserSettingNumericCommon.IsNotNumeric(variant))
            return false;

        long longVal = (long) variant;
        if (longVal < 0)
            return false;

        typedValue = (ulong) longVal;
        return true;
    }

    protected override sealed bool TryConvertValue(ulong typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}