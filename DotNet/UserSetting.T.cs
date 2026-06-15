namespace TraGUS.DotNet;

public abstract partial class UserSetting<T> : UserSetting where T : UserSetting<T>
{
    public static T Instance { get; private set; } = null!;
    protected override sealed void SetTypedInstance() => Instance = (T)this;
}