using System.Windows;
using LogViewer.JsonLogReader.Parser;


namespace LogViewer.Wpf.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVm _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainWindowVm(new LogParser()); // TODO: After prototype done make LogParser internal and do it via dependency injection
            DataContext = _vm;
        }
    }
}
