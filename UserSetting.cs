using Godot;

namespace TraGUS;

/// <summary>
/// The base abstraction for a user setting.
/// Implementors must be set up as autoloads to work properly.
/// </summary>
[GlobalClass]
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

    // =================================================
    //  .___    .___             __  .__  __          
    //  |   | __| _/____   _____/  |_|__|/  |_ ___.__.
    //  |   |/ __ |/ __ \ /    \   __\  \   __<   |  |
    //  |   / /_/ \  ___/|   |  \  | |  ||  |  \___  |
    //  |___\____ |\___  >___|  /__| |__||__|  / ____|
    //           \/    \/     \/               \/     
    // =================================================

    /// <summary>
    /// The .ini section associated to this setting.
    /// By convention, use snake_case.
    /// </summary>
    public abstract string Section {get;}
    /// <summary>
    /// The .ini key associated to this setting.
    /// By convention, use snake_case.
    /// </summary>
    public abstract string Key {get;}


    // =================================================
    //     .___               .__                  
    //     |   | _____ ______ |  |   ____   _____  
    //     |   |/     \\____ \|  | _/ __ \ /     \ 
    //     |   |  Y Y  \  |_> >  |_\  ___/|  Y Y  \
    //     |___|__|_|  /   __/|____/\___  >__|_|  /
    //               \/|__|             \/      \/ 
    // =================================================

    /// <summary>
    /// A value to be used if the value can't be initialized from .ini config files.
    /// </summary>
    /// <returns></returns>
    public abstract Variant Default();

    /// <summary>
    /// Defines the underlying behavior tied to this setting, provided a new value to be set. <br/>
    /// It could be for example, resizing the game's window, changing the engine's max frame rate,
    /// or other configuration more specific to your game. 
    /// </summary>
    /// <param name="value">The value to update the setting to.</param>
    /// <param name="effectiveValue">The value the setting was truly set to, if the requested one was rejected.</param>
    /// <returns>
    /// Whether the value could be successfully processed or not.
    /// Typically, if the concrete type of the Variant is not the expected one.
    /// </returns>
    protected abstract bool ProcessValue(Variant value, out Variant effectiveValue);


    // =================================================
    // .___        __                             .__   
    // |   | _____/  |_  ___________  ____ _____  |  |  
    // |   |/    \   __\/ __ \_  __ \/    \\__  \ |  |  
    // |   |   |  \  | \  ___/|  | \/   |  \/ __ \|  |__
    // |___|___|  /__|  \___  >__|  |___|  (____  /____/
    //          \/          \/           \/     \/      
    // =================================================

    public override void _EnterTree()
	{
		Instance = this;
        Value = Default();
        UserSettingsServer.RegisterSetting(this);
	}

    public override void _ExitTree()
	{
        UserSettingsServer.UnregisterSetting(this);
	}

    /// <summary>
    /// A gdscript-friendly adaptater of [`TryUpdateValue`].
    /// 
    /// To mimic [`TryUpdateValue`], you can simply retrieve the setting value manually afterwards, through [`Value`] property.
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
        Variant prevValue = Value;

        bool processed = ProcessValue(value, out effectiveValue);
        Value = effectiveValue;
        
        if (!prevValue.Equals(effectiveValue))
        {
            UserSettingsServer.Instance.Config.SetValue(Section.ToSnakeCase(), Key, Value);
            EmitSignal(SignalName.Changed, sender, value);
        }

        return processed;
    }
}