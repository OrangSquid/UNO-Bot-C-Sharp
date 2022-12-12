using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNOLib
{
    internal class ColorCard : Card
    {

        public ColorCardSymbols Symbol { get; init; }

        public override bool CanBePlayed(ICard card)
        {
            throw new NotImplementedException();
        }
    }
}
