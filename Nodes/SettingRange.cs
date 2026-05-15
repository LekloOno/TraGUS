using Godot;

namespace TraGUS.Nodes;

public partial class SettingRange : Range, ISettingUiWrapper
{
    private SettingBinder _binder;
	[Export] public string Section {get; private set;}
	[Export] public string Key {get; private set;}

    public override void _Ready()
	{
        _binder = new(this);
		ValueChanged += OnValueChanged;
    }

    private void OnValueChanged(double value)
    {
        if (_binder == null)
            return;

        if (!_binder.Setting.TryUpdateValue(this, value, out Variant effectiveValue))
            Value = (double)effectiveValue;
    }

    public void Update(GodotObject sender, Variant value)
    {
        if (sender == this)
            return;

        Value = (double)value;
    }
}