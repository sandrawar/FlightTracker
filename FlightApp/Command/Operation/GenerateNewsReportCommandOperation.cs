using FlightApp.DataProcessor;
using FlightApp.News;

namespace FlightApp.Command.Operation
{
    internal class GenerateNewsReportCommandOperation : CommandChainRead
    {
        public GenerateNewsReportCommandOperation(IFlightAppDataRead data, IFlighAppCommand nextCommandInChain) : base(data, nextCommandInChain)
        {
        }

        protected override CommandResult? ExecuteCommand(string command)
        {
            if (command != "report")
            {
                return null;
            }

            var generator = new NewsGenerator(
                [
                    new Televison("Telewizja Abelowa"),
                    new Televison("Kanał TV-tensor"),
                    new Radio("Radio Kwantyfikator"),
                    new Radio("Radio Shmem"),
                    new Newspaper("Gazeta Kategoryczna"),
                    new Newspaper("Dziennik Politechniczny"),
                ],
                [
                .. Data.GetAirports().Select(a => new AirportReportableDecorator(a)),
                .. Data.GetCargoPlanes().Select(cp => new CargoPlaneReportableDecorator(cp)),
                .. Data.GetPassengerPlanes().Select(pp => new PassengerPlaneReportableDecorator(pp)),
            ]);


            return new CommandResult(GenerateNews(generator).ToArray());
        }

        private IEnumerable<string> GenerateNews(NewsGenerator generator)
        {
            string? info;
            do
            {
                info = generator.GenerateNextNews();
                if (info != null)
                {
                    yield return info;
                }
            }
            while (info != null);
        }
    }
}
