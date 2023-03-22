using System.Runtime;
using UNOLib.Exceptions;

namespace UNOLib;

internal interface IGameSystem
{
    /// <summary>
    /// Plays the card and checks if the players has finished their deck. 
    /// CardAction is called afterwards to do the action specified for that card.
    /// </summary>
    /// <param name="cardId">Id of the card to be played</param>
    /// <exception cref="GameIsFinishedException">Game is already over. Cannot do more actions.</exception>
    /// <exception cref="CardCannotBePlayedException">Card does not meet the requirements to be played or does not exist at all.</exception>
    public void CardPlay(int playerId, string cardId);

    /// <summary>
    /// Gets the player with the specified Id
    /// </summary>
    /// <param name="playerId">Id of the Player</param>
    /// <returns></returns>
    public IPlayer GetPlayer(int playerId);

    /// <summary>
    /// Should be caled when the current player wants or needs to draw cards
    /// </summary>
    /// <exception cref="GameIsFinishedException">Game is already over. Cannot do more actions.</exception>
    public void DrawCard(int playerId);

    /// <summary>
    /// Changes the color when a Wild Card is played to the specified by the current Player.
    /// If a Four Plus Wild Card is played it also makes the next player draw 4 cards and skips it.
    /// </summary>
    /// <param name="color">color that was chosen</param>
    /// <exception cref="GameIsFinishedException">Game is already over. Cannot do more actions.</exception>
    /// <exception cref="CannotChangeColorException">Cannot change the color because there was no Wild Card played.</exception>
    /// <exception cref="ArgumentException">color is invalid.</exception>
    public void ChangeOnTableColor(int playerId, string color);
}
