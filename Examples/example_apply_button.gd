# This button saves the current state of the settings to the user's `.ini` config file.
extends Button

func _on_button_pressed() -> void:
	UserSettingsServer.Save()
