using System.Collections.Generic;
using Godot;

namespace TraGUS;

public partial class UserSettingsServer : Node
{
    // ===================================================
    //                _____ __________.___ 
    //               /  _  \\______   \   |
    //              /  /_\  \|     ___/   |
    //             /    |    \    |   |   |
    //             \____|__  /____|   |___|
    //                     \/              
    // ===================================================
    // The methods exposed and used to manipulate configs.

    /// <summary>
    /// Writes the current modifications into the user's .ini config file.
    /// </summary>
    public void Save()
	{
		Config.Save(SettingsFilePath);
	}

    /// <summary>
    /// Aborts the current config modifications, and restores the active settings to the user's .ini config file. 
    /// </summary>
	public void Abort()
	{
		if (!FileAccess.FileExists(SettingsFilePath))
			return;

		Config.Load(SettingsFilePath);
        Load();
	}

    /// <summary>
    /// Effectively loads the current state of the configuration into the run-time <see cref="UserSetting"/> settings.
    /// </summary>
    public void Load()
	{
		foreach (string sectionKey in Config.GetSections())
			foreach (string settingKey in Config.GetSectionKeys(sectionKey))
			{
				var value = Config.GetValue(sectionKey, settingKey);

                LoadResult result = LoadSetting(sectionKey, settingKey, value, out Variant effectiveValue);
                if (result == LoadResult.Success)
                    continue;

                string warning = 
                    "Could not load config value for:" +
                    "\n - section: " + sectionKey +
                    "\n - setting: " + settingKey + "\n";

                switch (result)
                {
                    case LoadResult.SectionNotFound:
                        warning += "---section `" + sectionKey + "` is not registered";
                        break;
                    case LoadResult.KeyNotFound:
                        warning += "---key `" + settingKey + "` is not registered";
                        break;
                    case LoadResult.ValueRejected:
                        warning += "---value `" + value + "` was rejected." +
                        "\n  -Falling back to " + effectiveValue;
                        break;
                }

                GD.PushWarning(warning);
			}
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
    ///     - The settingKey corresponds to no actual user settings within the provided section.
    ///     - The new setting value was rejected by the setting.
    /// </returns>
    public bool TryLoadSetting(string section, string settingKey, Variant value, out Variant effectiveValue)
    {
        LoadResult result = LoadSetting(section, settingKey, value, out effectiveValue);
        return result == LoadResult.Success;
    }

    /// <summary>
    /// Tries to reset all the settings from the specified section to the values specified in tragus_default_settings.ini.
    /// <br/>
    /// This DOES NOT apply the changes to the user's config file.
    /// For that, you must still call <see cref="Save"/> afterwards.
    /// </summary>
    /// <param name="section">The section to reset.</param>
    public void ResetSection(string section)
    {
        if (!_settings.TryGetValue(section, out Dictionary<string, UserSetting> settings))
            return;

        foreach (UserSetting setting in settings.Values)
            Reset(setting);
    }

    /// <summary>
    /// Tries to reset all the settings to the value specified in tragus_default_settings.ini.
    /// <br/>
    /// This DOES NOT apply the changes to the user's config file.
    /// For that, you must still call <see cref="Save"/> afterwards.
    /// </summary>
    public void ResetAll()
    {
        foreach (string section in _settings.Keys)
            ResetSection(section);
    }
}