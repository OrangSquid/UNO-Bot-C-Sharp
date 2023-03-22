namespace UNOLib.Cards;

internal class CardComparer : Comparer<string>
{
    public override int Compare(string? x, string? y)
    {
        if (x.StartsWith("Wild"))
        {
            if (y.StartsWith("Wild"))
            {
                return x.CompareTo(y);
            }

            return -1;
        }
        if (y.StartsWith("Wild"))
        {
            return 1;
        }
        return x.CompareTo(y);

    }
}
