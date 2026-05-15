# This button aborts the current state of the settings and resets them to the user's `.ini` config file.
extends Button

func _on_button_pressed() -> void:
	UserSettingsServer.Abort()
