﻿using System.Collections.Concurrent;

namespace FlightApp.Logger
{
    internal interface ILogger
    {
        void LogData(string data);
    }
    internal sealed class Logger : ILogger, IDisposable
    {
        private readonly Guid runId = Guid.NewGuid();
        private readonly ConcurrentQueue<string> logData = new();
        private readonly CancellationTokenSource cancellationTokenSource;
        private bool disposedValue;

        public Logger()
        {
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () => await LogData(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        public void LogData(string data)
        {
            logData.Enqueue(data);
        }

        private async Task LogData(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!logData.IsEmpty)
                {
                    var fileName = $"appLog_{DateTime.Today: yyyymmdd}.log";
                    using (var writer = new StreamWriter(fileName, true))
                    {
                        while (logData.TryDequeue(out var data))
                        {
                            writer.WriteLine($"{runId}: {data}");
                        }
                    }
                }

                await Task.Delay(100, cancellationToken);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
