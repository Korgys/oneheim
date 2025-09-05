namespace Roguelike.Console.Rendering;

using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
using Roguelike.Core.Game.Collectables.Items;
using System;

public sealed class ConsoleTreasurePicker : ITreasurePicker
{
    private readonly ControlsSettings _controls;

    public ConsoleTreasurePicker(ControlsSettings controls) => _controls = controls;

    public int Pick(TreasurePickerContext ctx, IReadOnlyList<TreasureOptionView> options)
    {
        Console.Clear();
        if (!string.IsNullOrWhiteSpace(ctx.Title))
            Console.WriteLine(ctx.Title);

        Console.WriteLine(
            $"Steps:{ctx.Player.Steps}  LVL:{ctx.Player.Level}  XP:{ctx.Player.XP}  Gold:{ctx.Player.Gold}");
        Console.WriteLine(
            $"HP:{ctx.Player.LifePoint}/{ctx.Player.MaxLifePoint}  STR:{ctx.Player.Strength}  ARM:{ctx.Player.Armor}  SPD:{ctx.Player.Speed}  VIS:{ctx.Player.Vision}");
        Console.WriteLine();

        var keys = new[] { _controls.Choice1, _controls.Choice2, _controls.Choice3 };
        for (int i = 0; i < options.Count && i < keys.Length; i++)
        {
            var v = options[i];
            RarityRenderer.WriteColored($"{keys[i]}. {v.Description}\n", v.Rarity);
        }

        int chosen = -1;
        while (chosen == -1)
        {
            var key = Console.ReadKey(true).Key.ToString().ToUpperInvariant();
            for (int i = 0; i < options.Count && i < keys.Length; i++)
                if (key == keys[i].ToUpperInvariant()) { chosen = i; break; }
        }
        return chosen;
    }
}
