using System.Text.Json.Serialization;

namespace Roguelike.Console.Configuration;

public class GameSettings
{
    public string Language { get; set; } = "EN"; // Default language is English

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Difficulty Difficulty { get; set; } = Difficulty.Normal; // Default difficulty is Normal

    public ControlsSettings Controls { get; set; } = new();
}
