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
        private const string EXCEPTION = "Exception";
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
        private string _openFilePath;
        private LogEventProperty _selectedLogEventProperty;


        public MainWindowVm(ILogParser logParser)
        {
            _logParser = logParser;

            OpenFileDialogCommand = new BoilerCommand(OpenFileDialogCommandAction);

            RemoveFilterExpressionCommand = new BoilerCommand(RemoveFilterExpressionCommandAction, IsOpenAndFilterSet);
            ExecuteFilterExpressionCommand = new BoilerCommand(ExecuteFilterExpressionCommandAction, IsOpenAndFilterSet);

            FirstPageCommand = new BoilerCommand(SwitchToFirstPageCommandAction, IsOpenAndNotFirstPage);
            PreviousPageCommand = new BoilerCommand(SwitchToPreviousPageCommandAction, IsOpenAndNotFirstPage);
            NextPageCommand = new BoilerCommand(SwitchToNextPageCommandAction, IsOpenAndNotLastPage);
            LastPageCommand = new BoilerCommand(SwitchToLastPageCommandAction, IsOpenAndNotLastPage);
        }

        public string OpenFilePath
        {
            get => _openFilePath;
            set
            {
                _openFilePath = value;
                OnPropertyChanged();
            }
        }

        public string FilterExpression
        {
            get => _filterExpression;
            set
            {
                _filterExpression = value;
                OnPropertyChanged();
                UpdateFilterCommands();
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

        public LogEventProperty SelectedLogEventProperty
        {
            get => _selectedLogEventProperty;
            set
            {
                _selectedLogEventProperty = value;
                OnPropertyChanged();
                SetFilterExpression(_selectedLogEventProperty);
            }
        }

        private void SetFilterExpression(LogEventProperty selectedLogEventProperty)
        {
            if (selectedLogEventProperty == null) 
                return;

            if (selectedLogEventProperty.Name == EXCEPTION)
                return;

            var processedValue = selectedLogEventProperty.Value.Replace("\"", "'");

            FilterExpression = $"{selectedLogEventProperty.Name} = {processedValue}";
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

            FilterExpression = string.Empty;

            _logParser.ReadLogsTemp(logFilePath);
            SwitchToPage(1);
            UpdateLogEventDetails(null);
            OpenFilePath = logFilePath;
        }


        #region PRIVATE METHODS

        private void SwitchToFirstPageCommandAction()
        {
            if (!IsOpenAndNotFirstPage())
                return;

            SwitchToPage(1);
        }

        private void SwitchToPreviousPageCommandAction()
        {
            if (!IsOpenAndNotFirstPage())
                return;

            var pageNumber = _pagedLogMessages.PageNumber - 1;
            SwitchToPage(pageNumber);
        }

        private void SwitchToNextPageCommandAction()
        {
            if (!IsOpenAndNotLastPage())
                return;

            var pageNumber = _pagedLogMessages.PageNumber + 1;
            SwitchToPage(pageNumber);
        }

        private void SwitchToLastPageCommandAction()
        {
            if (!IsOpenAndNotLastPage())
                return;

            var pageNumber = _pagedLogMessages.TotalPages;
            SwitchToPage(pageNumber);
        }

        private bool IsOpenAndNotFirstPage()
        {
            return IsOpen() && 
                   _pagedLogMessages.PageNumber > 1;
        }

        private bool IsOpenAndNotLastPage()
        {
            return IsOpen() &&
                   _pagedLogMessages.PageNumber < _pagedLogMessages.TotalPages;
        }

        private bool IsOpenAndFilterSet()
        {
            return IsOpen() &&
                   !string.IsNullOrEmpty(FilterExpression);
        }

        private bool IsOpen()
        {
            return _pagedLogMessages != null;
        }

        private void SwitchToPage(long pageNumber)
        {
            _pagedLogMessages = _logParser.Search((int) pageNumber, filterExpression: FilterExpression);
            LogMessages = new ObservableCollection<LogMessage>(_pagedLogMessages.Items);
            CurrentPageText = $"{_pagedLogMessages.PageNumber} / {_pagedLogMessages.TotalPages}";

            UpdateNavigationCommands();
            UpdateFilterCommands();
        }

        private void UpdateNavigationCommands()
        {
            ((BoilerCommand)FirstPageCommand).RaiseCanExecuteChanged();
            ((BoilerCommand)PreviousPageCommand).RaiseCanExecuteChanged();
            ((BoilerCommand)NextPageCommand).RaiseCanExecuteChanged();
            ((BoilerCommand)LastPageCommand).RaiseCanExecuteChanged();
        }

        private void UpdateFilterCommands()
        {
            ((BoilerCommand)RemoveFilterExpressionCommand).RaiseCanExecuteChanged();
            ((BoilerCommand)ExecuteFilterExpressionCommand).RaiseCanExecuteChanged();
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
                        .Select(x => new LogEventProperty ( x.Key, x.Value.ToString() ))
                        .ToList();

                if (!string.IsNullOrEmpty(logMessage.Exception))
                {
                    logEventProperties.Add(new LogEventProperty( EXCEPTION, logMessage.Exception));
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

        public LogEventProperty(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}
