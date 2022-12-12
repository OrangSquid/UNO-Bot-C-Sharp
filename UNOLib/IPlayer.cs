using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNOLib
{
    internal interface IPlayer
    {
        public int Id { get; }
        public void AddCard(ICard card);
    }
}
