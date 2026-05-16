using System.Collections.Generic;
using Godot;

namespace TraGUS;

/// <summary>
/// The User Settings Manager.
/// Handles configuration files and stores and manage settings states at runtime.
/// <br/>
/// It should be an autoload, and be loaded before any <see cref="UserSetting"/>.
/// </summary>
public partial class UserSettingsServer : Node
{
	const string SettingsFilePath = "user://settings.ini";
	const string DefaultSettingsFilePath = "res://tragus_default_settings.ini";
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
}