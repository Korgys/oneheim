using System.Reflection;

namespace Roguelike.Core.Tests;

public static class TestHelper
{
    public static void SetPrivateField<T>(object target, string fieldName, T value)
    {
        var f = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(f, $"Champ privé introuvable: {fieldName}");
        f.SetValue(target, value!);
    }
}
