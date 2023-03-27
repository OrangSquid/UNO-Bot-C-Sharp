using UNOLib.Exceptions;

namespace UNOLib;

internal static class GuardClauses
{
    internal static void GuardAgainstFinishedGame(GameState state)
    {
        if (state.GameFinished)
        {
            throw new GameIsFinishedException();
        }
    }
}