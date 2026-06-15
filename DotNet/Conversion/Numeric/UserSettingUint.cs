using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion.Numeric;

public abstract partial class UserSettingUint<T> : UserSetting<T, uint> where T : UserSettingUint<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out uint typedValue)
    {
        typedValue = Tval;
        if (UserSettingNumericCommon.IsNotNumeric(variant))
            return false;

        int intVal = (int) variant;

        if (intVal < 0)
            return false;

        typedValue = (uint) intVal;
        return true;
    }

    protected override sealed bool TryConvertValue(uint typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}