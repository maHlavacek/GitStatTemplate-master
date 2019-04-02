using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Utils
{
    public class MyLogger
    {
        public static SqlCommandsObserver SqlCommandLogObserver { get; private set; }

        public static void InitializeLogger()
        {
            SqlCommandLogObserver = new SqlCommandsObserver();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()                 // > Level (INFO, DEBUG, WARN, ERROR, FATAL)
                .WriteTo.Console()                          // => Console
                .WriteTo.File("Serilog.log")           // => File
                .WriteTo.Observers(events => events         // => SqlCommandLogObserver
                    .Subscribe(SqlCommandLogObserver))
                .CreateLogger();
        }
    }

}
