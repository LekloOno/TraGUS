extends OptionButton

var resolutions = {
	"3840x2160": Vector2i(3840,2160),
	"2560x1440": Vector2i(2560,1440),
	"1920x1080": Vector2i(1920,1080),
	"1600x900": Vector2i(1600,900),
	"1440x900": Vector2i(1440,900),
	"1366x768": Vector2i(1366,768),
	"1280x720": Vector2i(1280,720),
	"1024x600": Vector2i(1024,600),
	"800x600": Vector2i(800,600)
}

var curr_res: Vector2i

# We assume that the ExampleResolutionUserSetting has been registered as an autoload under the name `UserSettingRes`.
# The first step is to initialize your ui element to the desired UserSetting
func _ready() -> void:
	for resolution in resolutions:
		add_item(resolution)

	# The `Changed` signal allows your UI to stay synchronized with the state of the setting,
	# in case you have, for example, multiple ways of manipulating the same setting (slider, input, reset button ..)
	UserSettingRes.Changed.connect(update_value)
	# Besides, you might also want to initialize your UI element to the initial value of the setting.		
	curr_res = UserSettingRes.Value;
	

# Here is our Changed callback. 
func update_value(sender, value):
	# First, we want to ignore any change that has been caused by ourselves.
	# We should already have taken care of the ui update in such case.
	if sender == self :
		return;
		
	# Otherwise, we can synchronize our value to the setting's state.
	curr_res = value

	# Then, we resolve the human readable format of such value.
	update_ui()

func update_ui():
	var window_size_str = str(
		curr_res.x,
		"x",
		curr_res.y
	)
	
	# And retrieve the corresponding element within our option button.
	var resolution_id = resolutions.keys().find(window_size_str)
	selected = resolution_id
		
# Finally, we simply want to send a request to the actual setting whenever we want to change its value.
func set_window_size() :
	# If it fails, we might want to resynchronize the UI to the actual value of the setting right away.
	if !UserSettingRes.GdTryUpdateValue(self, curr_res) :
		curr_res = UserSettingRes.Value
		update_ui()


func _on_item_selected(index: int) -> void:
	var key = get_item_text(index)
	var resolution = resolutions[key]
	curr_res = resolution
	set_window_size()
	
func add_resolution(resolution: Vector2i):
	var res_str = str(resolution.x, "x", resolution.y)
	if resolutions.has(res_str):
		return
	
	resolutions[res_str] = resolution
	add_item(res_str)
