namespace UNOLib
{
    public interface IPlayer : IEnumerable<ICard>
    {
        public int Id { get; }
        internal void AddCard(ICard card);
        internal bool HasCard(string cardId);
        internal void RemoveCard(string cardId);
        internal ICard GetCard(string cardId);
    }
}
