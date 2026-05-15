using Godot;

namespace TraGUS;

/// <summary>
/// The base abstraction for a user setting.
/// Implementors must be set up as autoloads to work properly.
/// </summary>
public abstract partial class UserSetting : Node
{
    /// <summary>
    /// The autoload Instance of this setting.
    /// </summary>
    public static UserSetting Instance {get; private set;}

    /// <summary>
    /// The current value of this setting.
    /// </summary>
    public Variant Value {get; private set;}
    
    /// <summary>
    /// Emited when the value of this setting has changed.
    /// </summary>
    /// <param name="sender">
    /// The sender object that triggered the change.
    /// Can notably used by ui element to ignore the change and not update
    /// their display if it comes from themselves. 
    /// </param>
    /// <param name="value">The new value of this setting.</param>
    [Signal]
    public delegate void ChangedEventHandler(GodotObject sender, Variant value);
}