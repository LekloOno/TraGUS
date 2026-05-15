using Godot;

namespace TraGUS.Nodes;

public interface ISettingUiWrapper
{
    public string Section {get;}
    public string Key {get;}
    public void Update(GodotObject sender, Variant value);
}