using System.Collections.Generic;
using Godot;

namespace TraGUS;

[GlobalClass]
public partial class UserSettingsServer : Node
{
	const string SettingsFilePath = "user://settings.ini";
	const string DefaultSettingsFilePath = "res://config/user/default_settings.ini";
	public ConfigFile Config {get; private set;}
	public ConfigFile DefaultConfig {get; private set;}
	public static UserSettingsServer Instance {get; private set;}

    /// <summary>
    /// The purpose of this hashmap is so that we can easily load from .ini config file. 
    /// See `LoadSetting` for an example.
    /// </summary>
    private Dictionary<string, Dictionary<string, UserSetting>> _settings = [];


    public override void _EnterTree()
	{
		Instance = this;

        if (Engine.IsEditorHint())
            return;

		Config = new();
		DefaultConfig = new();

		DefaultConfig.Load(DefaultSettingsFilePath);

		if (FileAccess.FileExists(SettingsFilePath))
			Config.Load(SettingsFilePath);
		else
			Config.Load(DefaultSettingsFilePath);

        Load();
	}

    public static void Apply()
	{
		Instance.Config.Save(SettingsFilePath);
	}

	public static void Abort()
	{
		if (!FileAccess.FileExists(SettingsFilePath))
			return;

		Instance.Config.Load(SettingsFilePath);
        Load();
	}

    public static void Load()
	{
		foreach (string sectionKey in Instance.Config.GetSections())
			foreach (string settingKey in Instance.Config.GetSectionKeys(sectionKey))
			{
				var value = Instance.Config.GetValue(sectionKey, settingKey);
                if (!LoadSetting(sectionKey, settingKey, value, out Variant prevValue))
                {
					GD.PushWarning(
						"Could not load config value for:" +
						"\n - section: " + sectionKey +
						"\n - setting: " + settingKey
					);

                    if (!prevValue.Equals(default))
                        GD.PushWarning("\nValue remained: " + prevValue);
                }
			}
	}

    public static bool UnregisterSetting(UserSetting setting)
    {
        GD.Print("get section");
        if (!Instance._settings.TryGetValue(setting.Section, out Dictionary<string, UserSetting> sectionSettings))
            return false;
            
        GD.Print("unregister");

        return sectionSettings.Remove(setting.Key, out _);
    }

    public static bool RegisterSetting(UserSetting setting)
    {
        Dictionary<string, UserSetting> sectionSettings;

        if (!Instance._settings.TryGetValue(setting.Section, out sectionSettings))
        {
            sectionSettings = [];
            Instance._settings.Add(setting.Section, sectionSettings);
        }

        GD.Print("register");
        return sectionSettings.TryAdd(setting.Key, setting);
    }

    public static bool RefreshSetting(UserSetting setting)
    {
        UnregisterSetting(setting);
        return RegisterSetting(setting);
    }

    /// <summary>
    /// Tries to load the provided config.ini entry in the settings server.
    /// </summary>
    /// <param name="sectionKey">The section key of the .ini entry.</param>
    /// <param name="settingKey">The setting key of the .ini entry.</param>
    /// <param name="value">The value to set the setting to.</param>
    /// <param name="prevValue">If the provided value was not accepted by the setting, the value the setting remains at.</param>
    /// <returns>
    /// The function returns true if the operation was successful. 
    ///
    /// It returns false if either -
    ///     - The sectionKey corresponds to no actual user settings section.
    ///     - No setting is registered under the corresponding user settings section.
    ///     - The settingKey corresponds to no actual user settings within the provided section.
    ///     - The new setting value was rejected by the setting.
    /// </returns>
    public static bool LoadSetting(string section, string settingKey, Variant value, out Variant prevValue)
    {
        prevValue = default;

        if (!Instance._settings.TryGetValue(section, out Dictionary<string, UserSetting> sectionSettings))
            return false;

        if (!sectionSettings.TryGetValue(settingKey, out UserSetting setting))
            return false;

        if (setting == null)
            return false;

        return setting.TryUpdateValue(Instance, value, out prevValue);
    }
}