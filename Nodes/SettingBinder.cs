using System.Threading.Tasks;
using Godot;

namespace TraGUS.Nodes;

public partial class SettingBinder
{
    public SettingBinder(ISettingUiWrapper wrapper)
    {
        _section = wrapper.Section;
        _key = wrapper.Key;

        if (!UserSettingsServer.GetSetting(_section, _key, out UserSetting setting))
        {
            Setting = null;
            return;
        }

        
        Setting = setting;
        Setting.Changed += wrapper.Update;
    }
    private string _section;
	private string _key;

    // Quite dirty way of doing so ..
    public UserSetting Setting {get; private set;}
}