namespace Roguelike.Console.Rendering.Characters;

using System.Text.RegularExpressions;
using System;

public static class DialogueUI
{
    public static void RenderTypewriter(string text, TypewriterOptions? opts = null)
    {
        opts ??= new TypewriterOptions();

        if (!opts.EnableColorMarkup)
        {
            WriteWords(text, opts, null);
            Console.WriteLine();
            return;
        }

        // Simple color markup: [gold]text[/], [red]text[/], [green], [yellow], [cyan], [magenta], [white], [gray]
        // Nested blocks are not supported (kept simple for console UI).
        var pattern = new Regex(@"\[(?<color>\w+)\](?<content>.*?)\[/\]", RegexOptions.Singleline);
        int lastIndex = 0;
        foreach (Match m in pattern.Matches(text))
        {
            // Write plain part before colored block
            if (m.Index > lastIndex)
            {
                string plain = text.Substring(lastIndex, m.Index - lastIndex);
                WriteWords(plain, opts, null);
            }

            // Write colored block
            var colorName = m.Groups["color"].Value.Trim().ToLowerInvariant();
            var content = m.Groups["content"].Value;

            var color = MapColor(colorName);
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteWords(content, opts, color);
            Console.ForegroundColor = prev;

            lastIndex = m.Index + m.Length;
        }

        // Remainder
        if (lastIndex < text.Length)
        {
            string rest = text.Substring(lastIndex);
            WriteWords(rest, opts, null);
        }

        Console.WriteLine();
    }

    private static void WriteWords(string text, TypewriterOptions opts, ConsoleColor? currentColor)
    {
        var words = TokenizeWords(text).ToList(); // preserves punctuation tokens

        for (int i = 0; i < words.Count; i++)
        {
            var word = words[i];

            // Print token
            Console.Write(word);

            // Compute delay
            int delay = opts.BaseWordDelayMs;
            char last = word.Length > 0 ? word[^1] : '\0';
            if (last == '.' || last == '!' || last == '?') delay += opts.PeriodExtraDelayMs;
            else if (last == ',' || last == ';' || last == ':') delay += opts.CommaExtraDelayMs;

            // Do not slow down after newlines
            if (word.Contains('\n')) delay = 0;

            Thread.Sleep(delay);

            // Space after "word-like" tokens (not after explicit punctuation tokens we already printed with trailing space if embedded)
            if (i < words.Count - 1 && !IsPunctuationToken(words[i + 1]) && !EndsWithWhitespace(word))
                Console.Write(' ');
        }
    }

    private static bool EndsWithWhitespace(string s) => s.Length > 0 && char.IsWhiteSpace(s[^1]);

    private static bool IsPunctuationToken(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        // Treat standalone punctuation as punctuation tokens
        if (s.Length == 1 && ".,;:!?".IndexOf(s[0]) >= 0) return true;
        return false;
    }

    // Split into visible tokens without losing punctuation/newlines.
    private static IEnumerable<string> TokenizeWords(string text)
    {
        // Split on spaces but keep punctuation as part of tokens; this is a simple approach for console.
        // Also preserve \n to avoid adding extra spaces/delays.
        var parts = text.Replace("\r\n", "\n").Split(' ', StringSplitOptions.None);
        for (int i = 0; i < parts.Length; i++)
        {
            var token = parts[i];
            if (token.Length == 0)
            {
                yield return ""; // extra spaces collapse to quick prints
                continue;
            }
            yield return token;
        }
    }

    private static ConsoleColor MapColor(string name) =>
        name switch
        {
            "gold" or "yellow" => ConsoleColor.Yellow,
            "red" => ConsoleColor.Red,
            "green" => ConsoleColor.Green,
            "cyan" => ConsoleColor.Cyan,
            "magenta" => ConsoleColor.Magenta,
            "white" => ConsoleColor.White,
            "gray" or "grey" => ConsoleColor.Gray,
            _ => ConsoleColor.White
        };
}
