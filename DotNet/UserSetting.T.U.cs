using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace TraGUS.DotNet;

public abstract partial class UserSetting<T, U> : UserSetting<T> where T : UserSetting<T, U>
{
    public static U Tval     { get; private set; }
    public static event Action<GodotObject, U>? ValueChanged;

    public sealed override void _Ready()
    {
        Instance.Changed += ForwardChanged;    
    }

    private void ForwardChanged(GodotObject sender, Variant value)
    {
        if (TryConvertVariant(value, out U? typed))
            ValueChanged?.Invoke(sender, typed);
    }

    /// <summary>
    /// Tries to convert a variant to the explicit c# type of this setting.
    /// </summary>
    /// <param name="variant">The variant to convert.</param>
    /// <param name="typedValue">The converted typed value.</param>
    /// <returns>Whether the variant could be accepted as a `U` typed value.</returns>
    protected abstract bool TryConvertVariant(Variant variant, [NotNullWhen(true)] out U? typedValue);

    /// <summary>
    /// Tries to convert an explicitly typed value to a godot Variant. 
    /// </summary>
    /// <param name="typedValue">The typed value to convert.</param>
    /// <param name="variant">The converted variant.</param>
    /// <returns>Whether the typed value could be accepted as a Variant.</returns>
    protected abstract bool TryConvertValue(U typedValue, [NotNullWhen(true)] out Variant variant);

    /// <summary>
    /// Tries to process the given value. If the provided value is rejected, `effectiveTypedValue` should contain the fallback value.
    /// You can use that fallback to, for example, clamp an input value. Yet, as long as the effective value is not exactly `typedValue`, the function should return false.
    /// </summary>
    /// <param name="typedValue">The typed value to process.</param>
    /// <param name="effectiveTypedValue">The effective value, if the input was rejected.</param>
    /// <returns>Whether the `typedValue` input was accepted.</returns>
    protected abstract bool ProcessTypedValue(U typedValue, out U effectiveTypedValue);

    protected override sealed bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (!TryConvertVariant(value, out U? typedValue))
        {
            effectiveValue = Value;
            return false;
        }

        if (!ProcessTypedValue(typedValue, out U effectiveTypedValue))
        {
            TryConvertValue(effectiveTypedValue, out effectiveValue);
            Tval = effectiveTypedValue;
            return false;
        }

        effectiveValue = value;
        Tval = typedValue;
        return true;
    }

    public static bool TryUpdateTypedValue(GodotObject sender, U value, out U effectiveValue)
    {
        effectiveValue = Tval;

        if (!Instance.TryConvertValue(value, out Variant variant))
            return false;

        if (!Instance.TryUpdateValue(sender, variant, out _))
            return false;

        effectiveValue = Tval;
        return true;
    }
}