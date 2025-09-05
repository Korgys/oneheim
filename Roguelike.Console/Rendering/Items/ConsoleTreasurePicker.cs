namespace Roguelike.Console.Rendering.Items;

using Roguelike.Console.Rendering.Characters;
using Roguelike.Core.Configuration;
using Roguelike.Core.Game.Abstractions;
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

        Console.WriteLine();

        var keys = new[] { _controls.Choice1, _controls.Choice2, _controls.Choice3 };
        for (int i = 0; i < options.Count && i < keys.Length; i++)
        {
            var v = options[i];
            RarityRenderer.WriteColoredByRarity($"{keys[i]}. {v.Description}\n", v.Rarity);
        }

        Console.WriteLine();
        PlayerRenderer.RendererPlayerFullInfo(ctx.Player);

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
