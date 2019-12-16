using LogViewer.JsonLogReader.Parser;
using Microsoft.Extensions.DependencyInjection;

namespace LogViewer.JsonLogReader
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLogReaderService(this IServiceCollection services)
        {
            return services.AddSingleton<ILogParser, LogParser>();
        }
    }
}
