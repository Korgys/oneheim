namespace Roguelike.Core.Game.Characters.NPCs.Dialogues;

public class DialogueOption
{
    public string Label { get; init; } = "";
    public Func<string>? LabelFactory { get; set; }
    public Func<string?>? Action { get; init; }           // Return a one-line feedback message or null
    public DialogueNode? Next { get; init; }              // If null => end dialogue after action
}
