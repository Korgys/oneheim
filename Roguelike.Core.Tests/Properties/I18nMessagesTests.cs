using System.Xml.Linq;
using Roguelike.Core.Properties.i18n;

namespace Roguelike.Core.Tests.Properties;

[TestClass]
public class I18nMessagesTests
{
    [TestMethod]
    public void Messages_EnglishAndFrenchResources_HaveSameKeys()
    {
        var english = ReadKeys(@"..\..\..\..\Roguelike.Core\Properties\i18n\Messages.resx");
        var french = ReadKeys(@"..\..\..\..\Roguelike.Core\Properties\i18n\Messages.fr.resx");

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

    private static string[] ReadKeys(string relativePath)
    {
        var path = Path.GetFullPath(relativePath, AppContext.BaseDirectory);
        return XDocument.Load(path)
            .Descendants("data")
            .Select(e => e.Attribute("name")?.Value)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .OrderBy(name => name)
            .ToArray();
    }
}
