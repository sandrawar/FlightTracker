namespace FlightApp.DataProcessor
{
    internal sealed class UnionDataProcessor : IFlightAppDataProcessor
    {
        private readonly IEnumerable<IFlightAppDataProcessor> dataProcessors;

        public UnionDataProcessor(IEnumerable<IFlightAppDataProcessor> dataProcessors)
        {
            this.dataProcessors = dataProcessors;
        }

        public void Start()
        {
            foreach (var dataProcessor in dataProcessors)
            {
                dataProcessor.Start();
            }
        }
    }
}
