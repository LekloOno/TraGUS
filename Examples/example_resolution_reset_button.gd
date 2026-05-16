# This button resets the current state of the resolution settings to the `tragus_default_settings.ini` value.
extends Button

func _on_button_pressed() -> void:
	UserSettingRes.Reset()
