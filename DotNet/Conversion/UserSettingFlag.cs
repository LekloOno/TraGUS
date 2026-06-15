using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion;

public abstract partial class UserSettingFlag<T, F> : UserSetting<T, F>
    where T : UserSettingFlag<T, F>
    where F : Enum
{
    private static readonly Type FlagType = typeof(F);

    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out F typedValue)
    {
        if (variant.VariantType is not Variant.Type.Int)
        {
            typedValue = Tval;
            return false;
        }

        long intVal = (long)variant;
        
        object narrowed;
        try
        {
            narrowed = Convert.ChangeType(intVal, Enum.GetUnderlyingType(FlagType));
        }
        catch (OverflowException)
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (F)Enum.ToObject(FlagType, narrowed);
        return true;
    }

    protected override sealed bool TryConvertValue(F typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = Convert.ToInt64(typedValue);
        return true;
    }
}