namespace Filmder.DTOs;

public class EmojiPuzzleDto
{
    public required string Movie { get; set; }
    public required string Emoji { get; set; }
    public required List<string> Options { get; set; }
}
