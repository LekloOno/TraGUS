using Godot;

namespace TraGUS;

public abstract partial class UserSetting : Node
{
	// =================================================
	// .___        __                             .__   
	// |   | _____/  |_  ___________  ____ _____  |  |  
	// |   |/    \   __\/ __ \_  __ \/    \\__  \ |  |  
	// |   |   |  \  | \  ___/|  | \/   |  \/ __ \|  |__
	// |___|___|  /__|  \___  >__|  |___|  (____  /____/
	//          \/          \/           \/     \/      
	// =================================================
	public override sealed void _EnterTree()
	{
		Instance = this;

		PreInitialize();

		UserSettingsServer.TryRegisterSetting(this);

		Variant initValue;
		if (!UserSettingsServer.Init(this, out initValue)
			|| !TryDeserialize(initValue, out initValue))
			initValue = DefaultFallBack();

		if (!ProcessValue(initValue, out Variant effectiveValue))
			GD.PushWarning("The initial value (" + initValue +
				")for [section: " + Section + ", key: " + Key + "] was rejected.\n" +
				"- Falling back to: " + effectiveValue);

		Value = effectiveValue;
	}

	public override void _ExitTree()
	{
		UserSettingsServer.TryUnregisterSetting(this);
	}
}
