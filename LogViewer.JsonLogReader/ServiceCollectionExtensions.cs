using LogViewer.JsonLogReader.Parser;
using Microsoft.Extensions.DependencyInjection;

namespace LogViewer.JsonLogReader
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonLogReaderService(this IServiceCollection services)
        {
            services.AddSingleton<ILogParser, LogParser>();
            return services;
        }
    }
}
