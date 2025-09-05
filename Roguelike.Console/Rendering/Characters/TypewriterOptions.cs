namespace Roguelike.Console.Rendering.Characters;

public sealed class TypewriterOptions
{
    // Print per-word (not per-char).
    public int BaseWordDelayMs { get; set; } = 40;         // base delay between words
    public int CommaExtraDelayMs { get; set; } = 120;      // extra delay after , ; :
    public int PeriodExtraDelayMs { get; set; } = 220;     // extra delay after . ! ?
    public bool EnableColorMarkup { get; set; } = true;    // [gold]text[/], [red]danger[/]
}
