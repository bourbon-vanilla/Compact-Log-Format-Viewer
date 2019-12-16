using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LogViewer.JsonLogReader.Models;
using LogViewer.JsonLogReader.Parser;
using LogViewer.Wpf.Client.VmBoiler;


namespace LogViewer.Wpf.Client
{
    internal class MainWindowVm : BoilerVm
    {
        private const string TEMP_LOG_FILE_PATH =
            @"Temp\UmbracoTraceLog.UNITTEST.20181112.json";

        private readonly ILogParser _logParser;
        private PagedResult<LogMessage> _pagedLogMessages;

        // Property backing fields
        private ObservableCollection<LogMessage> _logMessages;
        private string _currentPageText;
        private string _filterExpression;
        private LogMessage _selectedLogMessage;


        public MainWindowVm(ILogParser logParser)
        {
            _logParser = logParser;
            _logParser.ReadLogsTemp(TEMP_LOG_FILE_PATH);
            SwitchToPage(1);

            RemoveFilterExpressionCommand = new BoilerCommand(RemoveFilterExpressionCommandAction);
            ExecuteFilterExpressionCommand = new BoilerCommand(ExecuteFilterExpressionCommandAction);

            FirstPageCommand = new BoilerCommand(SwitchToFirstPageCommandAction);
            PreviousPageCommand = new BoilerCommand(SwitchToPreviousPageCommandAction);
            NextPageCommand = new BoilerCommand(SwitchToNextPageCommandAction);
            LastPageCommand = new BoilerCommand(SwitchToLastPageCommandAction);
        }


        public string FilterExpression
        {
            get => _filterExpression;
            set
            {
                _filterExpression = value;
                OnPropertyChanged();
            }
        }

        public LogMessage SelectedLogMessage 
        { 
            get => _selectedLogMessage; 
            set 
            {
                _selectedLogMessage = value;
                UpdateLogEventDetails();
            } 
        }

        public ICommand RemoveFilterExpressionCommand { get; }

        public ICommand ExecuteFilterExpressionCommand { get; }

        public ObservableCollection<LogMessage> LogMessages 
        { 
            get => _logMessages;
            private set
            {
                _logMessages = value; 
                OnPropertyChanged();
            }
        }

        public string CurrentPageText
        {
            get => _currentPageText;
            private set
            {
                _currentPageText = value;
                OnPropertyChanged();
            }
        }

        public ICommand FirstPageCommand { get; }

        public ICommand PreviousPageCommand { get; }

        public ICommand NextPageCommand { get; }

        public ICommand LastPageCommand { get; }


        #region PRIVATE METHODS

        private void SwitchToFirstPageCommandAction()
        {
            if (_pagedLogMessages.PageNumber == 1)
                return;

            SwitchToPage(1);
        }

        private void SwitchToPreviousPageCommandAction()
        {
            if (_pagedLogMessages.PageNumber <= 1)
                return;

            var pageNumber = _pagedLogMessages.PageNumber - 1;
            SwitchToPage(pageNumber);
        }

        private void SwitchToNextPageCommandAction()
        {
            if (_pagedLogMessages.PageNumber >= _pagedLogMessages.TotalPages)
                return;

            var pageNumber = _pagedLogMessages.PageNumber + 1;
            SwitchToPage(pageNumber);
        }

        private void SwitchToLastPageCommandAction()
        {
            if (_pagedLogMessages.PageNumber == _pagedLogMessages.TotalPages)
                return;

            var pageNumber = _pagedLogMessages.TotalPages;
            SwitchToPage(pageNumber);
        }

        private void SwitchToPage(long pageNumber)
        {
            _pagedLogMessages = _logParser.Search((int) pageNumber, filterExpression: FilterExpression);
            LogMessages = new ObservableCollection<LogMessage>(_pagedLogMessages.Items);
            CurrentPageText = $"{_pagedLogMessages.PageNumber} / {_pagedLogMessages.TotalPages}";
        }

        private void RemoveFilterExpressionCommandAction()
        {
            FilterExpression = string.Empty;
            SwitchToPage(1);
        }

        private void ExecuteFilterExpressionCommandAction()
        {
            SwitchToPage(1);
        }

        private void UpdateLogEventDetails()
        {
            // TODO Log Event Details View
        }

        #endregion
    }

}
