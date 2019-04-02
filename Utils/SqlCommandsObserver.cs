using Serilog.Events;
using System;
using System.Linq;

namespace Utils
{
    public class SqlCommandsObserver : IObserver<LogEvent>
    {
        public string LogText { get; private set; }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(LogEvent logEvent)
        {
            foreach (var property in logEvent.Properties.Where(_ => _.Key == "commandText"))
            {
                LogText += $"{property.Value}\n";
            }
        }

        public void ClearLogText()
        {
            LogText = string.Empty;
        }
    }

}
