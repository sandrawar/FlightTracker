namespace FlightApp.Chain
{
    internal class Chain<TChainLink>
    {
        private IList<Func<TChainLink, TChainLink>> linkCreateFunc;
        private readonly Func<TChainLink> terminationCreate;

        public Chain(Func<TChainLink> chainTermination)
        {
            linkCreateFunc = new List<Func<TChainLink, TChainLink>>();
            terminationCreate = chainTermination;
        }

        protected void AddLink(Func<TChainLink, TChainLink> createLink)
            => linkCreateFunc.Insert(0, createLink);

        public void Reset() => ResetChain();

        public TChainLink Build()
        {
            var chainStart = terminationCreate();
            foreach (var linkCreate in linkCreateFunc)
            {
                chainStart = linkCreate(chainStart);
            }
            ResetChain();
            return chainStart;
        }

        private void ResetChain()
        {
            linkCreateFunc.Clear();
        }
    }
}
