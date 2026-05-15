using Godot;
using TraGUS;

/// <summary>
/// This is an example of how to implement UserSetting.
/// This setting controls the game's resolution.
/// 
/// To work properly, you must add it as an autoload, and make sure it loads after the <see cref="UserSettingsServer"/> (that is, it is lower in the autoloads list).
/// </summary>
public partial class ExampleResolutionUserSetting : UserSetting
{
    public override string Section => "video";
    public override string Key => "resolution";

    public override Variant DefaultFallBack() =>
        new Vector2I(1920, 1080);

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Vector2I)
        {
            effectiveValue = Value;
            return false;
        }

        Vector2I res = (Vector2I)value;
        effectiveValue = value;
        
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
            GetWindow().Size = res;
        GetWindow().ContentScaleSize = res;

        return true;
    }
}