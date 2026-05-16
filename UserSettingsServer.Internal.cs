using System.Collections.Generic;
using Godot;

namespace TraGUS;

public partial class UserSettingsServer : Node
{
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
	}

	/// <summary>
	/// Tries to retrieve the saved default value of this setting from `tragus_default_settings.ini`
	/// </summary>
	/// <param name="setting">The setting to initialize.</param>
	/// <param name="value">The retrieved default value for this setting.</param>
	/// <returns>
	/// Whether the default config file contained a corresponding (not necessarily valid) entry.
	/// </returns>
	public static bool Default(UserSetting setting, out Variant value)
	{
		if (!Instance.DefaultConfig.HasSectionKey(setting.Section, setting.Key))
		{
			value = default;
			return false;
		}

		value = Instance.DefaultConfig.GetValue(setting.Section, setting.Key);
		return true;
	}

	/// <summary>
	/// Tries to retrieve the initial value of the setting, using the user's `.ini` config file if possible
	/// and the `tragus_default_settings.ini` otherwise.
	/// </summary>
	/// <param name="setting">The setting to initialize.</param>
	/// <param name="value">The retrieved initial value for this setting.</param>
	/// <returns>
	/// Whether either the user's or default config file contained a corresponding (not necessarily valid) entry. 
	/// </returns>
	public static bool Init(UserSetting setting, out Variant value)
	{
		if (!Instance.Config.HasSectionKey(setting.Section, setting.Key))
			return Default(setting, out value);

		value = Instance.Config.GetValue(setting.Section, setting.Key);
		return true;
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
	/// <param name="key">The setting key of the .ini entry.</param>
	/// <param name="value">The value to set the setting to.</param>
	/// <param name="effectiveValue">If the provided value was not accepted by the setting, the value the setting remains at.</param>
	/// <returns>
	/// The function returns a LoadResult, corresponding to either a success, or one of the possible failures -
	///     - The sectionKey corresponds to no actual user settings section.
	///     - The settingKey corresponds to no actual user settings within the provided section.
	///     - The new setting value was rejected by the setting.
	/// </returns>
	private static LoadResult LoadSetting(string section, string key, Variant value, out Variant effectiveValue)
	{
		effectiveValue = default;

		if (!Instance._settings.TryGetValue(section, out Dictionary<string, UserSetting> sectionSettings))
			return LoadResult.SectionNotFound;

		if (!sectionSettings.TryGetValue(key, out UserSetting setting))
			return LoadResult.KeyNotFound;

		if (!setting.TryDeserialize(value, out Variant deserialized))
			return LoadResult.ValueRejected;

		if (setting.TryUpdateValue(Instance, deserialized, out effectiveValue))
			return LoadResult.Success;

		return LoadResult.ValueRejected;
	}

	public static bool GetSetting(string section, string key, out UserSetting setting)
	{
		if (!Instance._settings.TryGetValue(section, out Dictionary<string, UserSetting> sectionSettings))
		{
			setting = default;
			return false;
		}

		return sectionSettings.TryGetValue(key, out setting);
	}

	private enum LoadResult
	{
		Success,
		SectionNotFound,
		KeyNotFound,
		ValueRejected,
	}


	/// <summary>
	/// Tries to reset the setting to the value specified in tragus_default_settings.ini, if present.
	/// <br/>
	/// This DOES NOT apply the changes to the user's config file.
	/// For that, you must still call <see cref="Save"/> afterwards.
	/// </summary>
	/// <param name="setting">The setting to reset.</param>
	/// <returns>
	/// Whether the setting could be reset.
	/// It can fail:
	/// - If the tragus_default_settings.ini did not contain an entry for this setting, and the setting can't handle <see cref="default"/>.
	/// - If the tragus_default_settings.ini entry values is rejected by the setting.
	/// </returns>
	public static bool Reset(UserSetting setting)
	{
		var value = Instance.DefaultConfig.GetValue(setting.Section, setting.Key);
		return setting.TryUpdateValue(Instance, value, out _);
	}
}
