using UNOLib.Cards;

namespace UNOLib.Player;

public interface IPlayer : IEnumerable<ICard>
{
    /// <summary>
    /// Returns the player's ID. Can range from 0 to the maximum number of players
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Returns the number of cards the player has
    /// </summary>
    public int NumCards { get; }

    /// <summary>
    /// Returns true if a player has at least one plus two
    /// </summary>
    internal bool HasPlusTwoCards { get; }

    /// <summary>
    /// Returns true if a player has at least one wild plus four
    /// </summary>
    internal bool HasWildPlusFourCards { get; }

    /// <summary>
    /// Add a card to the player's deck
    /// </summary>
    /// <param name="card">the card to be added</param>
    internal void AddCard(ICard card);

    /// <summary>
    /// Remove from the player deck the card with the same cardId
    /// </summary>
    /// <param name="cardId">the cardId of the card to be removed</param>
    /// <exception cref="PlayerDoesNotHaveCardException"></exception>
    internal void RemoveCard(string cardId);
}