# (Tra) Godot User Settings

[![Documentation](https://img.shields.io/badge/docs-TraGUS-blue)](https://lekloono.github.io/TraGUS/)  

Tragus is a neat piercing, but now, its is also an easy to use plugin to handle user-settings save, load, runtime edit, and easy UI-bindings.

## Features overview

The plugin is based on :
- **UserSettingsServer**: It handles all the config file boiler plate, and lets you focus on setting up new user settings naturally.
- **UserSetting**: A base abstraction for you to extend into new user settings !

The plugin allows, auto-magically :
- User settings persistence using .ini files.
- Default settings setup.
- Runtime and responsive edition of settings.
- An easy and straightforward way to add new user settings.
- An even easier way to bind UI to edit such settings, and keep multiple sources synchronized.

# Base Setup

First, you must enable the plugin, of course, under `Project > Project Settings > Global > Plugins`.

Then, you must make [UserSettingsServer](UserSettingsServer.cs) an [autoload](https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html).

For this, go under `Project > Project Settings > Global`, then in `Autoload`, select the directory icon and browse in your game file to select [addons/TraGUS/UserSettingsServer.cs](UserSettingsServer.cs).

![auto_load_server](_docs/res/auto_load_server.png)

### ⚠️ Now, very important !

Any [UserSetting](UserSetting.cs) you will implement (see [Adding new user settings](#adding-new-user-settings)) must appear lower in the autoloads list than the [UserSettingsServer](UserSettingsServer.cs), that is, the Server must be loaded before the settings.


# By example

Below, I wrote a very quick guide on what you need to do to implement new user settings, and how to setup UI to edit them.

However, it's sometimes easier to learn by example ! So I added a quick one in [Examples](Examples/).

- [ExampleResolutionUserSetting](Examples/ExampleResolutionUserSetting.cs) defines a user setting for the game's rendering resolution.
- [example_resolution_picker](Examples/example_resolution_picker.gd) shows how to make a UI element that allows the player to edit this setting.
- [example_resolution_reset_button](Examples/example_resolution_reset_button.gd) shows how to make a little button that resets the setting to the value specified in `default_settings.ini`.
- [example_apply_button](Examples/example_apply_button.gd) show how to make a button that saves the currently edited settings to the user's `.ini` config file.
- [example_abort_button](Examples/example_abort_button.gd) show how to make a button that aborts the currently edited settings, and resets them to the user's `.ini` config file.


You can read more about the detailed features in the [full documentation](https://lekloono.github.io/TraGUS/) of the plugin.


# Adding new user settings

To add new user settings, you must implement a class extending `UserSetting`, and set it as an autoload.

## Implement [UserSetting](UserSetting.cs)
You have only 4, straightforward, things to implement.

### Section & Key
```cs
public override string Section => "section";
public override string Key => "setting_key";
```
Defines the section this setting will be registered under in the user's `.ini` config file, and the setting name.

This setting for example, will be seen as follows in the `.ini` file.

```ini
[section]

setting_key=#...
```

### Default
```cs
public override Variant Default() => //...;
```
Makes sure the setting is always initialized to a valid value, even if the none of the user's or default `.ini` config file contains a valid entry for it.

If you really trust your `default_settings.ini`, you can simply let it return `default`.

### ProcessValue
```cs
protected override bool ProcessValue(Variant value, out Variant effectiveValue)
{
    // ...
}
```
Holds the actual behavior of the setting, like, changing the size of the window, the engine's frame rate, the color of the UI etc.

## Register the autoload

Once you have implemented these 4, function, you can register the node as an auto-load, just like you did for the settings server.

### ⚠️ Now, very important ! (reminder)

Any [UserSetting](UserSetting.cs) you will implement (see [Adding new user settings](#adding-new-user-settings)) must appear lower in the autoloads list than the [UserSettingsServer](UserSettingsServer.cs), that is, the Server must be loaded before the settings.

# Binding this to the UI

Now, to bind this to the UI, nothing is simpler !

## Stay synchronized !

First, you must define a callback to keep the UI synchronized if other sources can modify the same setting.

```gd
func update_value(sender, value) -> void:
    if sender == self:
        return
    
    my_ui_display_value = value
    update_ui() # for example, text = "value is " + my_ui_display_value
```

The sender is the person responsible for the setting's value change. If it's ourself, we don't need to update the UI since we actively did it !

## Enable the synchronisation

Now, you need to connect this callback to the setting's `Changed` signal, typically in your node's `_ready` function, and maybe also start your ui synchronized !

Let's say you have implemented a `UserSetting`, and registered it as autoload under the name `SuperNeatSetting`.

```gd
func _ready() -> void:
    # Some initialization ..
    SuperNeatSetting.Changed.connect(update_value)
    my_ui_display_value = SuperNeatSetting.Value
    update_ui()
```

## Send edit requests

Finally, for your UI to be able to modify the setting, you simply need to send a request whenever you want to, for example, on a button click, or any other callback, while stamping the request with your name (self)

```gd
func _on_button_pressed() -> void:
    SuperNeatSetting.GdTryUpdateValue(self, my_ui_display_value)
```

Additionnaly, you might want to handle the case where the operation was not successfull.

```gd
func _on_button_pressed() -> void:
    if !SuperNeatSetting.GdTryUpdateValue(self, my_ui_display_value):
        my_ui_display_value = SuperNeatSetting.Value # We resynchronize to the actual value of the setting.
```

And that's it ! You can have as many ways to edit the same setting, and they'll stay synchronized !

Additionnaly, you can also reset a setting to a default value you have set in `default_settings.ini` using `SuperNeatSetting.Reset()`.