using System.Collections.Generic;
using LogViewer.JsonLogReader.Models;
using Serilog.Core;
using Serilog.Events;

namespace LogViewer.JsonLogReader.Parser
{
    public interface ILogParser
    {
        bool LogIsOpen { get; set; }

        string LogFilePath { get; set; }

        List<LogEvent> ReadLogs(string filePath, Logger logger = null);

        void ReadLogsTemp(string filePath);

        LogLevelCounts TotalCounts();

        int TotalErrors();

        void ExportTextFile(string messageTemplate, string newFileName);

        PagedResult<LogMessage> Search(int pageNumber = 1, int pageSize = 100, string filterExpression = null, SortOrder sort = SortOrder.Descending);

        List<LogTemplate> GetMessageTemplates();
    }
}
