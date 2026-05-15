using Godot;

namespace TraGUS;

public abstract partial class UserSetting : Node
{

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
    /// You can leave it as is, or specify a fallback behavior if needed.
    /// </summary>
    /// <returns></returns>
    public virtual Variant DefaultFallBack() => default;

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

    /// <summary>
    /// Called right after the singleton has been initalized, and right before it is registered,
    /// or its fields are initialized. Allows for some further custom initialization. 
    /// </summary>
    protected virtual void PreInitialize() {}
}