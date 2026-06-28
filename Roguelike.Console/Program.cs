using Roguelike.Console.Rendering;
using Roguelike.Console.Rendering.Characters;
using Roguelike.Console.Rendering.Combats;
using Roguelike.Console.Rendering.Items;
using Roguelike.Core.Configuration;
using Roguelike.Core.Game.GameLoop;

// Launch game engine
var settings = ConfigurationReader.LoadGameSettings();
var engine = new GameEngine(
    settings,
    new ConsoleRenderer(),
    new SystemClock(),
    new ConsoleCombatRenderer(),
    new ConsoleDialogueRenderer(settings),
    new ConsoleTreasurePicker(settings.Controls),
    new InventoryUI());

engine.Run();

// TODO: implement menu system
// TODO: implement quest system

// TODO: new npc : ḃ (give gold is specific enemy is dead, is at the west-south)
