using System.Runtime.CompilerServices;
using LogViewer.JsonLogReader.Parser;
using Microsoft.Extensions.DependencyInjection;

[assembly:InternalsVisibleTo("LogViewer.Server.Tests")]

namespace LogViewer.JsonLogReader
{
    public class JsonLogReaderServiceModule
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ILogParser, LogParser>();
        }
    }
}
