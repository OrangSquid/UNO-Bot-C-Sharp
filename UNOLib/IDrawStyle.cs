namespace UNOLib
{
    /// <summary>
    /// Interface for the various draw styles. Handles the card stack and drawing methods
    /// </summary>
    internal interface IDrawStyle
    {
        /// <summary>
        /// Adds a card to the played card stack.
        /// </summary>
        /// <param name="card"></param>
        void Push(ICard card);

        /// <summary>
        /// Draws a single card from the card stack. If there aren't any left, it shuffles the played card stack.
        /// Only use for forced draws like start of the game and 2+ situations.
        /// </summary>
        /// <returns>Drawn card</returns>
        ICard Draw();

        /// <summary>
        /// Draws the card in the implemented style. Changes the GameState accordingly.
        /// </summary>
        /// <param name="state">the state of the game to take into account</param>
        /// <returns>true if the player can play any of the drawn cards, false otherwise</returns>
        bool GameDraw(ref GameState state);
    }
}
