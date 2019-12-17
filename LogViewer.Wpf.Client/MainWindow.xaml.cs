using System;
using System.Linq;
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

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            var filesPresent = e.Data.GetDataPresent(DataFormats.FileDrop);
            if (!filesPresent)
                return;

            var files = (string[]) e.Data.GetData(DataFormats.FileDrop);

            if (files == null || files.Length > 1)
                throw new ArgumentOutOfRangeException(nameof(e));

            var fileToOpen = files.First();

            _vm.OpenLogFile(fileToOpen); // TODO: This is a hack for prototype purposes - add dependency property to handle this
        }
    }
}
