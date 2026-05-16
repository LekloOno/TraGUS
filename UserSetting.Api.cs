using Godot;

namespace TraGUS;

public abstract partial class UserSetting : Node
{
    // ===================================================
    //                _____ __________.___ 
    //               /  _  \\______   \   |
    //              /  /_\  \|     ___/   |
    //             /    |    \    |   |   |
    //             \____|__  /____|   |___|
    //                     \/              
    // ===================================================
    // The methods exposed and used to manipulate settings

    /// <summary>
    /// A gdscript-friendly adaptater of <see cref="TryUpdateValue"/>.
    /// 
    /// To mimic <see cref="TryUpdateValue"/>, you can simply retrieve the setting value manually afterwards, through <see cref="Value"/> property.
    /// </summary>
    /// <param name="sender">The sender object that requesting the change.</param>
    /// <param name="value">The value to update the setting to.</param>
    /// <returns>
    /// Whether the requested update was performed successfully.
    /// It could typically return false if the concrete type of the provided variant was rejected.
    /// </returns>
    public bool GdTryUpdateValue(GodotObject sender, Variant value) =>
        TryUpdateValue(sender, value, out _);

    /// <summary>
    /// Tries to update the value of this setting.
    /// </summary>
    /// <param name="sender">The sender object that requesting the change.</param>
    /// <param name="value">The value to update the setting to.</param>
    /// <param name="effectiveValue">The value the setting was truly set to, if the requested one was rejected.</param>
    /// <returns>
    /// Whether the requested update was performed successfully.
    /// It could typically return false if the concrete type of the provided variant was rejected.
    /// </returns>
    public bool TryUpdateValue(GodotObject sender, Variant value, out Variant effectiveValue)
    {
        bool processed = ProcessValue(value, out effectiveValue);
        Value = effectiveValue;

        // Comparing Variant values is a bit of hastle, I don't think doing all the
        // necessary type checks is worth the optimization of not
        // triggering edit and signal fire when the inherent value has not changed.
        UserSettingsServer.Instance.Config.SetValue(Section.ToSnakeCase(), Key, Value);
        EmitSignal(SignalName.Changed, sender, value);
        
        return processed;
    }

    /// <summary>
    /// Tries to reset the setting to the value specified in tragus_default_settings.ini.
    /// </summary>
    /// <returns>
    /// Whether the setting could be reset.
    /// It can fail:
    /// - If the tragus_default_settings.ini did not contain an entry for this setting, and the setting can't handle <see cref="default"/>.
    /// - If the tragus_default_settings.ini entry values is rejected by the setting.
    /// </returns>
    public bool Reset() =>
        UserSettingsServer.Reset(this);

    /// <summary>
    /// Tries to retrieve the default value of this setting from `tragus_default_settings.ini`.
    /// </summary>
    /// <param name="value">The retrieved value.</param>
    /// <returns>Whether a corresponding entry in `tragus_default_settings.ini` was found.</returns>
    public bool Default(out Variant value) =>
        UserSettingsServer.Default(this, out value);

    /// <summary>
    /// A gdscript compatible adaptater of <see cref="Default"/>.
    /// It falls back to the setting DefaultFallBack() if no corresponding entry is found in `tragus_default_settings.ini`.
    /// </summary>
    /// <returns>
    /// The corresponding entry in `tragus_default_settings.ini`, or the default fall back if none was found.
    /// </returns>
    public Variant GdDefault()
    {
        Variant value = DefaultFallBack();
        Default(out value);
        return value;
    }
}