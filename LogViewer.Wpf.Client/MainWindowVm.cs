using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using LogViewer.JsonLogReader.Models;
using LogViewer.JsonLogReader.Parser;
using LogViewer.Wpf.Client.Helper;
using LogViewer.Wpf.Client.VmBoiler;
using Microsoft.Win32;


namespace LogViewer.Wpf.Client
{
    internal class MainWindowVm : BoilerVm
    {
        private readonly ILogParser _logParser;
        private PagedResult<LogMessage> _pagedLogMessages;
        private string _openFileDialogDirectory;

        // Property backing fields
        private ObservableCollection<LogMessage> _logMessages;
        private string _currentPageText;
        private string _filterExpression;
        private LogMessage _selectedLogMessage;
        private bool _logEventSelected;
        private ObservableCollection<LogEventProperty> _selectedLogEventProperties;
        

        public MainWindowVm(ILogParser logParser)
        {
            _logParser = logParser;

            OpenFileDialogCommand = new BoilerCommand(OpenFileDialogCommandAction);

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
                
                UpdateLogEventDetails(_selectedLogMessage);
            } 
        }

        public ObservableCollection<LogEventProperty> SelectedLogEventProperties
        {
            get => _selectedLogEventProperties;
            set
            {
                _selectedLogEventProperties = value;
                OnPropertyChanged();
            }
        }

        public bool LogEventSelected
        {
            get => _logEventSelected;
            set
            {
                _logEventSelected = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenFileDialogCommand { get; }

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

        [Obsolete("This is a hack for prototype purposes to call file open from code behind")]
        public void OpenLogFile(string logFilePath)
        {
            if (string.IsNullOrEmpty(logFilePath))
                throw new ArgumentNullException(nameof(logFilePath));

            if (!File.Exists(logFilePath))
                throw new ArgumentOutOfRangeException(nameof(logFilePath));

            _logParser.ReadLogsTemp(logFilePath);
            SwitchToPage(1);
            UpdateLogEventDetails(null);
        }

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

        private void OpenFileDialogCommandAction()
        {
            if (!GetFilePathFromUiDialog(out var openFilePath)) 
                return;

            OpenLogFile(openFilePath);
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

        private void UpdateLogEventDetails(LogMessage logMessage)
        {
            if (logMessage == null)
            {
                LogEventSelected = false;

                SelectedLogEventProperties = new ObservableCollection<LogEventProperty>();
            }
            else 
            { 
                LogEventSelected = true;

                var logEventProperties = logMessage.Properties
                        .Select(x => new LogEventProperty { Name = x.Key, Value = x.Value.ToString() })
                        .ToList();

                if (!string.IsNullOrEmpty(logMessage.Exception))
                {
                    logEventProperties.Add(new LogEventProperty{Name = "Exception", Value = logMessage.Exception});
                }

                SelectedLogEventProperties = new ObservableCollection<LogEventProperty>(
                    logEventProperties);
            }
        }

        private bool GetFilePathFromUiDialog(out string localFileName)
        {
            _openFileDialogDirectory = _openFileDialogDirectory ?? Environment.CurrentDirectory;
            var  openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = _openFileDialogDirectory,
                    Filter = SystemDialog.GetOpenFileDialogFilterString(new []{"json", "clef"})
                };

            // Show open file dialog box
            var fileSelected = openFileDialog.ShowDialog();
            if (fileSelected == true)
            {
                localFileName = openFileDialog.FileName;
                _openFileDialogDirectory = Path.GetDirectoryName(localFileName);
                return true;
            }

            localFileName = null;
            return false;
        }

        #endregion

    }

    internal class LogEventProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
