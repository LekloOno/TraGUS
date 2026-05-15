using Godot;

namespace TraGUS.Nodes;

public partial class SettingResetButton : BaseButton
{
	private string _section;
	private string _key;
	private UserSetting _setting;

	private void SetSetting()
	{
		if (UserSettingsServer.Instance == null)
		{
			CallDeferred(nameof(SetSetting));
			return;
		}

		UserSettingsServer.GetSetting(_section, _key, out UserSetting setting);
		Setting = setting;
	}

	[Export]
	public string Section
	{
		get => _section;
		set
		{
			if (_section == value)
				return;

			_section = value;
			SetSetting();
		}
	}

	[Export]
	public string Key
	{
		get => _key;
		set
		{
			if (_key == value)
				return;

			_key = value;
			SetSetting();
		}
	}

	public UserSetting Setting
	{
		get => _setting;
		set
		{
			if (_setting == value)
				return;

			if (_setting != null)
				_setting.Changed -= Update;
			
			_setting = value;

			if (_setting != null)
				_setting.Changed += Update;
		}
	}

	private void OnPressed() =>
		_setting?.Reset();

	// Lazy string comparison ..
	private void Update(GodotObject _, Variant value) =>
		Visible = value.ToString() != _setting?.GdDefault().ToString();

	public override void _Ready()
	{
		Pressed += OnPressed;

		SetSetting();
	}
}
