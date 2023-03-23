using UNOLib.Cards;

namespace UNOLib.StackStyles;

/// <summary>
/// Interface for stack styles for the different forced draw cards
/// </summary>
internal interface IStackStyle
{
    /// <summary>
    /// Force draws the current player with number of accumulated cards in state
    /// </summary>
    /// <param name="state"></param>
    /// <returns>true always</returns>
    public bool ForcedDraw(ref GameState state);

    /// <summary>
    /// Force draws the current player and checks if stacking is possible in the implemented style.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="card">card to be checked</param>
    /// <returns>true if the player should be skipped, false otherwise</returns>
    public bool ForcedDraw(ref GameState state, ICard card);
}