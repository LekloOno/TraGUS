using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion.Numeric;

public abstract partial class UserSettingLong<T> : UserSetting<T, long> where T : UserSettingLong<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out long typedValue)
    {
        if (UserSettingNumericCommon.IsNotNumeric(variant))
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (long) variant;
        return true;
    }

    protected override sealed bool TryConvertValue(long typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}