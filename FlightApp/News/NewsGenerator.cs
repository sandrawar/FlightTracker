using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightApp.News
{
    internal class NewsGenerator
    {
        private readonly IEnumerable<INewsReporter> reporters;
        private readonly IEnumerable<IReportable> reportables;

        private readonly IEnumerator<INewsReporter> reportersEnumerator;
        private readonly IEnumerator<IReportable> reportablesEnumerator;

        public NewsGenerator(
            IEnumerable<INewsReporter> reporters,
            IEnumerable<IReportable> reportables) 
        {
            this.reporters = reporters;
            this.reportables = reportables;

            this.reportersEnumerator = reporters.GetEnumerator();
            this.reportersEnumerator.MoveNext();
            this.reportablesEnumerator = reportables.GetEnumerator();
            this.reportablesEnumerator.MoveNext();

            this.reportablesEnumerator.Reset();
        }

        public string? GenerateNextNews()
        {
            string? info = null;
            if (this.reportersEnumerator.Current != null)
            {
                if (this.reportablesEnumerator.Current != null)
                {
                    info = this.reportersEnumerator.Current.Report(this.reportablesEnumerator.Current);
                    this.reportablesEnumerator.MoveNext();
                }
                else if (this.reportablesEnumerator.MoveNext())
                {
                    info = this.reportersEnumerator.Current.Report(this.reportablesEnumerator.Current!);
                    this.reportablesEnumerator.MoveNext();
                }
                else if (this.reportersEnumerator.MoveNext())
                {
                    this.reportablesEnumerator.Reset();
                    if (this.reportablesEnumerator.MoveNext())
                    {
                        info = this.reportersEnumerator.Current.Report(this.reportablesEnumerator.Current!);
                        this.reportablesEnumerator.MoveNext();
                    }
                }
            }

            return info;
        }
    }
}
