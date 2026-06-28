using System.Xml.Linq;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Tests.Properties;

[TestClass]
public class I18nMessagesTests
{
    [TestMethod]
    public void Messages_EnglishAndFrenchResources_HaveSameKeys()
    {
        var english = ReadKeys("Messages.resx");
        var french = ReadKeys("Messages.fr.resx");

        CollectionAssert.AreEquivalent(english, french);
    }

    [TestMethod]
    public void Messages_NewGameplayStrings_ArePresent()
    {
        string[] keys =
        {
            "EberIntroFirst",
            "IchemSmallTalk1",
            "OmanaProphecy",
            "GambleForGold",
            "UpgradeFirstItemForGold",
            "BaseCampDefendedReward",
            "NightTreasureBonus"
        };

        foreach (var key in keys)
        {
            Assert.AreNotEqual(key, Messages.Get(key));
            Assert.IsFalse(string.IsNullOrWhiteSpace(Messages.Get(key)));
        }
    }

    private static string[] ReadKeys(string fileName)
    {
        var path = Path.GetFullPath(
            Path.Combine("..", "..", "..", "..", "Roguelike.Core", "Properties", "i18n", fileName),
            AppContext.BaseDirectory);

        return XDocument.Load(path)
            .Descendants("data")
            .Select(e => e.Attribute("name")?.Value)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .OrderBy(name => name)
            .ToArray();
    }
}
