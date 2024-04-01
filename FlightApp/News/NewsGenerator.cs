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

            reportersEnumerator = reporters.GetEnumerator();
            reportersEnumerator.MoveNext();
            reportablesEnumerator = reportables.GetEnumerator();
            reportablesEnumerator.MoveNext();

            reportablesEnumerator.Reset();
        }

        public string? GenerateNextNews()
        {
            string? info = null;
            if (reportersEnumerator.Current != null)
            {
                if (reportablesEnumerator.Current != null)
                {
                    info = reportersEnumerator.Current.Report(reportablesEnumerator.Current);
                    reportablesEnumerator.MoveNext();
                }
                else if (reportablesEnumerator.MoveNext())
                {
                    info = reportersEnumerator.Current.Report(reportablesEnumerator.Current!);
                    reportablesEnumerator.MoveNext();
                }
                else if (reportersEnumerator.MoveNext())
                {
                    reportablesEnumerator.Reset();
                    if (reportablesEnumerator.MoveNext())
                    {
                        info = reportersEnumerator.Current.Report(reportablesEnumerator.Current!);
                        reportablesEnumerator.MoveNext();
                    }
                }
            }

            return info;
        }
    }
}
