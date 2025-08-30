using Roguelike.Console.Rendering;
using Roguelike.Core.Configuration;
using Roguelike.Core.Game.GameLoop;

// Launch game engine
var settings = ConfigurationReader.LoadGameSettings();
var engine = new GameEngine(
    settings,
    new ConsoleRenderer(),
    new ConsoleInput(),
    new SystemClock(),
    new DefaultRng(),
    new ConsoleCombatRenderer());

engine.Run();

// TODO: more dialogue options
// TODO: implement menu system
// TODO: implement quest system

// TODO: new npc : ô (can identify next boss, appear at 350 steps)
// TODO: new npc : ù (gambling on items, appear at 450 steps)
// TODO: new npc : ÿ (upgrade items, appear at 550 steps + 1st boss dead)

// TODO: new npc : ḃ (give gold is specific enemy is dead, is at the west-south)
// TODO: new structure : dungeon on the east-north

// TODO: fix bug : enemy can move on the same tile as another enemy

// TODO: fill readme.md