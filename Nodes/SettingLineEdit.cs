using System;
using Godot;

namespace TraGUS.Nodes;

public partial class SettingLineEdit : LineEdit, ISettingUiWrapper
{
	private SettingBinder _binder;
	/// <summary>
	/// Follows the range's section, key, min and max value if true,
	/// if it can find a sibling node SettingRange.
	/// </summary>
	[Export] private bool _bindToRange = true;
	[Export] public string Section {get; private set;}
	[Export] public string Key {get; private set;}
	[Export] private double _minValue;
	[Export] private double _maxValue;

	[Export] private bool _allowFloat = true;
	[Export] private bool _allowNegatives = true;
	[Export] private int _decimals = 2;

	[Export] private bool _clampValue = false;


	public override void _Ready()
	{
		if (_bindToRange && GetRange(out SettingRange range))
		{
			_maxValue = range.MaxValue;
			_minValue = range.MinValue;
			Section = range.Section;
			Key = range.Key;
		}

		_binder = new(this);
		TextSubmitted += OnTextChanged;
	}

	private bool GetRange(out SettingRange range)
	{
		foreach (Node node in GetParent().GetChildren())
		{
			if (node is SettingRange rangeChild)
			{
				range = rangeChild;
				return true;
			}
		}

		range = null;
		return false;
	}

	public void Update(GodotObject sender, Variant value)
	{
		if (sender == this)
			return;

		Text = value.ToString();
	}

	private void OnTextChanged(string newText)
	{
		Variant newValue = FormatedValue(newText);
		Text = newValue.ToString();

		if (_binder == null)
			return;

		if (!_binder.Setting.TryUpdateValue(this, newValue, out Variant effectiveValue))
			Text = effectiveValue.ToString();
	}

	private Variant FormatedValue(string newText)
	{
		if (!newText.IsValidFloat())
			return 0;

		float floatVal = newText.ToFloat();

		if (!_allowNegatives)
			floatVal = Mathf.Max(0f, floatVal);
		
		if (_clampValue)
			floatVal = (float) Mathf.Clamp((double)floatVal, _minValue, _maxValue);

		if (_allowFloat)
			return (float) Math.Round((decimal)floatVal, _decimals);
		
		return Mathf.RoundToInt(floatVal);
	}
}
