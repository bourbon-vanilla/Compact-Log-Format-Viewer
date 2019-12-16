using System.ComponentModel;
using System.Runtime.CompilerServices;
using LogViewer.Wpf.Annotations;


namespace LogViewer.Wpf.Client.VmBoiler
{
    internal class BoilerVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
