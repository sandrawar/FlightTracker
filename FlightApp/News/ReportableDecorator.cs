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
        private readonly Airport airport;

        public AirportReportableDecorator(Airport airport)
            => this.airport = airport;

        public override string Report(INewsReporter newsReporter)
            => newsReporter.Report(airport);
    }

    internal class PassangerPlaneReportableDecorator : ReportableDecorator
    {
        private readonly PassangerPlane passangerPlane;

        public PassangerPlaneReportableDecorator(PassangerPlane passangerPlane)
            => this.passangerPlane = passangerPlane;

        public override string Report(INewsReporter newsReporter)
            => newsReporter.Report(passangerPlane);
    }

    internal class CargoPlaneReportableDecorator : ReportableDecorator
    {
        private readonly CargoPlane cargoPlane;

        public CargoPlaneReportableDecorator(CargoPlane cargoPlane)
            => this.cargoPlane = cargoPlane;

        public override string Report(INewsReporter newsReporter)
            => newsReporter.Report(cargoPlane);
    }
}
