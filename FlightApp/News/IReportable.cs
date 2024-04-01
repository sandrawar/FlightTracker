using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightApp.News
{
    internal interface IReportable
    {
        string ClassType { get; }
        string? Name { get; }
        string? Country { get; }
        string? Serial{ get; }
        string? Model { get; }
    }
}
