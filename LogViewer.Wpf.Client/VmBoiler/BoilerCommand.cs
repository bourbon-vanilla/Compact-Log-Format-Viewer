using System;
using System.Windows.Input;
using LogViewer.Wpf.Annotations;


namespace LogViewer.Wpf.Client.VmBoiler
{
    internal class BoilerCommand : ICommand
    {
        [NotNull] private readonly Action _executeAction;
        [NotNull] private readonly Func<bool> _canExecute;


        public BoilerCommand([NotNull] Action executeAction, Func<bool> canExecute = null)
        {
            _executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
            _canExecute = canExecute ?? (() => true);
        }


        #region ICommand

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _executeAction();
        }

        public event EventHandler CanExecuteChanged;

        #endregion


        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
