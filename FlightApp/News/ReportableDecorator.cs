using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightApp.News
{
    internal abstract class ReportableDecorator : IReportable
    {
        public abstract string Report(INewsReporter newsReporter);
    }

    internal class AirportReportableDecorator: ReportableDecorator
    {
        private readonly IAirport airport;

        public AirportReportableDecorator(IAirport airport)
            => this.airport = airport;

        public override string Report(INewsReporter newsReporter)
            => newsReporter.Report(airport);
    }

    internal class PassengerPlaneReportableDecorator : ReportableDecorator
    {
        private readonly IPassengerPlane passengerPlane;

        public PassengerPlaneReportableDecorator(IPassengerPlane passengerPlane)
            => this.passengerPlane = passengerPlane;

        public override string Report(INewsReporter newsReporter)
            => newsReporter.Report(passengerPlane);
    }

    internal class CargoPlaneReportableDecorator : ReportableDecorator
    {
        private readonly ICargoPlane cargoPlane;

        public CargoPlaneReportableDecorator(ICargoPlane cargoPlane)
            => this.cargoPlane = cargoPlane;

        public override string Report(INewsReporter newsReporter)
            => newsReporter.Report(cargoPlane);
    }
}
