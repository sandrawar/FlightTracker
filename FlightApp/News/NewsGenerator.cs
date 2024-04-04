namespace FlightApp.News
{
    internal class NewsGenerator
    {
        private readonly IEnumerator<string> enumerator;

        public NewsGenerator(
            IEnumerable<INewsReporter> newsReporters,
            IEnumerable<IReportable> reportables)
        {
            enumerator = newsReporters
                .SelectMany(news => reportables.Select(
                    reportable => (news, reportable)))
                .Select(v => v.news.Report(v.reportable))
                .GetEnumerator();
        }

        public string? GenerateNextNews() =>
            enumerator.MoveNext()
                ? enumerator.Current
                : null;
    }
}
