using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightApp.DataProcessor
{
    internal interface IFlightAppDataProcessor
    {
        void Start(IFlightAppCompleteData flightAppCompleteData);
    }
}
