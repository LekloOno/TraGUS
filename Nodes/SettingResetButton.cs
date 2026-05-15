using Godot;

namespace TraGUS.Nodes;

public partial class SettingResetButton : BaseButton, ISettingUiWrapper
{
	private SettingBinder _binder;
	[Export] public string Section {get; private set;}
	[Export] public string Key {get; private set;}

	private void OnPressed() =>
		_binder.Setting.Reset();

	// Lazy string comparison ..
	public void Update(GodotObject _, Variant value) =>
		Visible = value.ToString() != _binder?.Setting?.GdDefault().ToString();

	public override void _Ready()
	{
		_binder = new(this);
		Pressed += OnPressed;
	}
}
