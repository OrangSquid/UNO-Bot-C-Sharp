namespace UNOLib.Cards;

internal class CardComparer : Comparer<string>
{
    // Compare by putting Wild Cards first then the rest of the cards alphabetically
    public override int Compare(string? x, string? y)
    {
        if (x != null && x.StartsWith("Wild"))
        {
            if (y != null && y.StartsWith("Wild"))
            {
                return string.Compare(x, y, StringComparison.Ordinal);
            }

            return -1;
        }
        if (y != null && y.StartsWith("Wild"))
        {
            return 1;
        }
        return string.Compare(x, y, StringComparison.Ordinal);
    }
}