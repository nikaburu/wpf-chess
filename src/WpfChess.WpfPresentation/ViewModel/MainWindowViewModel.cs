using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WpfChess.WpfPresentation.Exceptions;

namespace WpfChess.WpfPresentation.ViewModel
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        #region Delegates
        public delegate void ShowErrorMessageDelegate(string errorText);
        #endregion

        #region Properties
        public FieldViewModel FieldViewModel { get; private set; }
        public AlertViewModel AlertViewModel { get; private set; }
        #endregion

        #region Constructors
        public MainWindowViewModel(FieldViewModel fieldViewModel)
        {
            FieldViewModel = fieldViewModel;
            AlertViewModel = new AlertViewModel();
        }

        #endregion

        #region Commands
        private RelayCommand _restartGameCommand;

        public RelayCommand RestartGameCommand
        {
            get { return _restartGameCommand ?? (_restartGameCommand = new RelayCommand(RestartGameCommandeExecute)); }
        }
        #endregion

        #region Public members
        public void ShowErrorMessage(string errorText)
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new ShowErrorMessageDelegate(ShowErrorMessageExecute), errorText);
        }
        #endregion

        #region Private members

        private void RestartGameCommandeExecute()
        {
            //(new GamePreloader()).StartNewGame();
            //ShowErrorMessage(Guid.NewGuid().ToString());

            throw new ChessGameException("bla");
        }

        private void ShowErrorMessageExecute(string errorText)
        {
            AlertViewModel.ShowAlertMessage(errorText, true);
        }

        #endregion
    }
}
