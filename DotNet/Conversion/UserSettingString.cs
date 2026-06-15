using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet.Conversion;

public abstract partial class UserSettingString<T> : UserSetting<T, string> where T : UserSettingString<T>
{
    protected override sealed bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out string typedValue)
    {
        if (variant.VariantType is not Variant.Type.String)
        {
            typedValue = Tval;
            return false;
        }

        typedValue = (string) variant;
        return true;
    }

    protected override sealed bool TryConvertValue(string typedValue, [NotNullWhen(true)] out Variant variant)
    {
        variant = typedValue;
        return true;
    }
}