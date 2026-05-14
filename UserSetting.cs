using Godot;

namespace TraGUS;

[GlobalClass]
public abstract partial class UserSetting : Node
{
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
    /// <summary>
    /// A value to be used if the value can't be initialized from .ini config files.
    /// </summary>
    /// <returns></returns>
    public abstract Variant Default();

    /// <summary>
    /// The 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected abstract bool ProcessValue(Variant value);

    public override void _EnterTree()
	{
		Instance = this;
        UserSettingsServer.RegisterSetting(this);
        Value = Default();
	}

    public override void _ExitTree()
	{
        UserSettingsServer.UnregisterSetting(this);
	}

    public bool GdTryUpdateValue(Node sender, Variant value) =>
        TryUpdateValue(sender, value, out _);

    public bool TryUpdateValue(Node sender, Variant value, out Variant prevValue)
    {
        prevValue = Value;
        if (!ProcessValue(value))
            return false;
            
        Value = value;
        UserSettingsServer.Instance.Config.SetValue(Section.ToSnakeCase(), Key, Value);
        EmitSignal(SignalName.Changed, sender, value);
        return true;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
            UserSettingsServer.UnregisterSetting(this);
    }
}