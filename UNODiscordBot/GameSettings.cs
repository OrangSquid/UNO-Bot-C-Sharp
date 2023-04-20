namespace UNODiscordBot;

internal struct GameSettings
{
    public required bool DrawUntilPlayableCard { get; set; }
    public required bool StackPlusTwo { get; set; }
    public required bool MustPlay { get; set; }
    public required bool JumpIn { get; set; }
    public required int UnoPenalty { get; set; }
}