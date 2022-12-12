using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNOLib
{
    internal class WildCard : Card
    {
        public new CardColors Color { get; set; }
        public WildCardSymbols CardSymbol { get; init; }

        public override bool CanBePlayed(ICard card)
        {
            throw new NotImplementedException();
        }
    }
}
