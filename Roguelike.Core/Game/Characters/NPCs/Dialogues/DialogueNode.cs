namespace Roguelike.Core.Game.Characters.NPCs.Dialogues;

public class DialogueNode
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public Func<string>? Text { get; init; }             // Allow dynamic text
    public List<DialogueOption> Options { get; } = new(); // 0 = end node
}
