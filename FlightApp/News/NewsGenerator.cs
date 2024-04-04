namespace FlightApp.News
{
    internal class NewsGenerator
    {
        private readonly IEnumerator<(INewsReporter news, IReportable reportable)> enumerator;

        public NewsGenerator(
            IEnumerable<INewsReporter> newsReporters,
            IEnumerable<IReportable> reportables)
        {
            var newsReportables = newsReporters.SelectMany(
                n => reportables.Select(r => (n, r)).ToArray());
            enumerator = newsReportables.GetEnumerator();
        }

        public string? GenerateNextNews() =>
            enumerator.MoveNext()
                ? enumerator.Current.news.Report(enumerator.Current.reportable)
                : null;
    }
}
