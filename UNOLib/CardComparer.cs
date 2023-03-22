
namespace UNOLib;

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
            else
            {
                return -1;
            }
        }
        else
        {
            if (y.StartsWith("Wild"))
            {
                return 1;
            }
            else
            {
                return x.CompareTo(y);
            }
        }
            
    }
}
