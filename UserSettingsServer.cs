using System.Collections.Generic;
using Godot;

namespace TraGUS;

/// <summary>
/// The User Settings Manager.
/// Handles configuration files and stores and manage settings states at runtime.
/// <br/>
/// It should be an autoload, and be loaded before any <see cref="UserSetting"/>.
/// </summary>
[GlobalClass]
public partial class UserSettingsServer : Node
{
	const string SettingsFilePath = "user://settings.ini";
	const string DefaultSettingsFilePath = "res://addons/TraGUS/default_settings.ini";
    /// <summary>
    /// The autoload Instance of this setting.
    /// </summary>
	public static UserSettingsServer Instance {get; private set;}

    /// <summary>
    /// The config file edited on-the-go.
    /// </summary>
	public ConfigFile Config {get; private set;}
    /// <summary>
    /// The preloaded default config file.
    /// </summary>
	public ConfigFile DefaultConfig {get; private set;}

    /// <summary>
    /// The purpose of this hashmap is so that we can easily load from .ini config file. <br/>
    /// <br/>
    /// The inner hashmap references user settings by their .ini key. <br/>
    /// The outer hashmap references the inner hashmap, by their .ini section. <br/>
    /// <br/>
    /// See <see cref="LoadSetting"/> for an example.
    /// </summary>
    private Dictionary<string, Dictionary<string, UserSetting>> _settings = [];


    public override void _EnterTree()
	{
		Instance = this;

		Config = new();
		DefaultConfig = new();


        if (FileAccess.FileExists(DefaultSettingsFilePath))
		    DefaultConfig.Load(DefaultSettingsFilePath);
        else
            FileAccess.Open(DefaultSettingsFilePath, FileAccess.ModeFlags.Write);


		if (FileAccess.FileExists(SettingsFilePath))
			Config.Load(SettingsFilePath);
		else
			Config.Load(DefaultSettingsFilePath);

        Load();
	}

    /// <summary>
    /// Writes the current modifications into the user's .ini config file.
    /// </summary>
    public static void Save()
	{
		Instance.Config.Save(SettingsFilePath);
	}

    /// <summary>
    /// Aborts the current config modifications, and restores the active settings to the user's .ini config file. 
    /// </summary>
	public static void Abort()
	{
		if (!FileAccess.FileExists(SettingsFilePath))
			return;

		Instance.Config.Load(SettingsFilePath);
        Load();
	}

    /// <summary>
    /// Effectively loads the current state of the configuration into the run-time <see cref="UserSetting"/> settings.
    /// </summary>
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

    /// <summary>
    /// Tries to register the provided setting, using its <see cref="UserSetting.Section"/> and <see cref="UserSetting.Key"/>.
    /// </summary>
    /// <param name="setting">The <see cref="UserSetting"/> to register.</param>
    /// <returns>
    /// Whether the setting was successfully registered, that is, if a setting has already been registered under this section and key.
    /// </returns>
    public static bool TryRegisterSetting(UserSetting setting)
    {
        Dictionary<string, UserSetting> sectionSettings;

        if (!Instance._settings.TryGetValue(setting.Section, out sectionSettings))
        {
            sectionSettings = [];
            Instance._settings.Add(setting.Section, sectionSettings);
        }

        return sectionSettings.TryAdd(setting.Key, setting);
    }

    /// <summary>
    /// Tries to unregister the provided setting, using its <see cref="UserSetting.Section"/> and <see cref="UserSetting.Key"/>.
    /// </summary>
    /// <param name="setting">The <see cref="UserSetting"/> to unregister.</param>
    /// <returns>
    /// Whether the setting was successfully unregistered. <br/>
    /// It can fail if : <br/>
    ///  - No setting is registered under this setting's section. <br/>
    ///  - This setting is not registerd under its section. <br/>
    /// </returns>
    public static bool TryUnregisterSetting(UserSetting setting)
    {
        if (!Instance._settings.TryGetValue(setting.Section, out Dictionary<string, UserSetting> sectionSettings))
            return false;

        return sectionSettings.Remove(setting.Key, out _);
    }

    /// <summary>
    /// Tries to load the provided config.ini entry in the settings server.
    /// </summary>
    /// <param name="sectionKey">The section key of the .ini entry.</param>
    /// <param name="settingKey">The setting key of the .ini entry.</param>
    /// <param name="value">The value to set the setting to.</param>
    /// <param name="effectiveValue">If the provided value was not accepted by the setting, the value the setting remains at.</param>
    /// <returns>
    /// The function returns true if the operation was successful. 
    ///
    /// It returns false if either -
    ///     - The sectionKey corresponds to no actual user settings section.
    ///     - No setting is registered under the corresponding user settings section.
    ///     - The settingKey corresponds to no actual user settings within the provided section.
    ///     - The new setting value was rejected by the setting.
    /// </returns>
    public static bool LoadSetting(string section, string settingKey, Variant value, out Variant effectiveValue)
    {
        effectiveValue = default;

        if (!Instance._settings.TryGetValue(section, out Dictionary<string, UserSetting> sectionSettings))
            return false;

        if (!sectionSettings.TryGetValue(settingKey, out UserSetting setting))
            return false;

        return setting.TryUpdateValue(Instance, value, out effectiveValue);
    }

    /// <summary>
    /// Tries to reset the setting to the value specified in default_settings.ini, if present.
    /// <br/>
    /// This DOES NOT apply the changes to the user's config file.
    /// For that, you must still call <see cref="Save"/> afterwards.
    /// </summary>
    /// <param name="setting">The setting to reset.</param>
    /// <returns>
    /// Whether the setting could be reset.
    /// It can fail:
    /// - If the default_settings.ini did not contain an entry for this setting, and the setting can't handle <see cref="default"/>.
    /// - If the default_settings.ini entry values is rejected by the setting.
    /// </returns>
    public static bool Reset(UserSetting setting)
    {
        var value = Instance.DefaultConfig.GetValue(setting.Section, setting.Key);
        return setting.TryUpdateValue(Instance, value, out _);
    }

    /// <summary>
    /// Tries to reset all the settings from the specified section to the values specified in default_settings.ini.
    /// <br/>
    /// This DOES NOT apply the changes to the user's config file.
    /// For that, you must still call <see cref="Save"/> afterwards.
    /// </summary>
    /// <param name="section">The section to reset.</param>
    public static void ResetSection(string section)
    {
        if (!Instance._settings.TryGetValue(section, out Dictionary<string, UserSetting> settings))
            return;

        foreach (UserSetting setting in settings.Values)
            Reset(setting);
    }

    /// <summary>
    /// Tries to reset all the settings to the value specified in default_settings.ini.
    /// <br/>
    /// This DOES NOT apply the changes to the user's config file.
    /// For that, you must still call <see cref="Save"/> afterwards.
    /// </summary>
    public static void ResetAll()
    {
        foreach (string section in Instance._settings.Keys)
            ResetSection(section);
    }
}