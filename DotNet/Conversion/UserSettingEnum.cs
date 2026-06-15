using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion;

public abstract partial class UserSettingEnum<T, E> : UserSetting<T, E>
    where T : UserSettingEnum<T, E>
    where E : Enum
{
    private static readonly Type EnumType = typeof(E);

    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out E typedValue)
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
            narrowed = Convert.ChangeType(intVal, Enum.GetUnderlyingType(EnumType));
        }
        catch (OverflowException)
        {
            typedValue = Tval;
            return false;
        }

        if (!Enum.IsDefined(EnumType, narrowed))
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (E)Enum.ToObject(EnumType, narrowed);
        return true;
    }

    protected override sealed bool TryConvertValue(E typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = Convert.ToInt64(typedValue);
        return true;
    }
}