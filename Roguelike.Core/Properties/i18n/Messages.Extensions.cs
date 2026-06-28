namespace Roguelike.Core.Properties.i18n;

public partial class Messages
{
    public static string Get(string name) => ResourceManager.GetString(name, resourceCulture) ?? name;

    public static string Format(string name, params object[] args) => string.Format(Get(name), args);
}
