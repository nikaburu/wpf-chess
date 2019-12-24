using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace WpfChess.WpfPresentation.ViewModel
{
    public class AlertViewModel : ViewModelBase
    {
        #region Fields
        private bool _isOpen;
        private string _messageText;
        private bool _isCritical;
        private bool _isClosed;
        #endregion

        #region Properties
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (!_isClosed)
                {
                    _isOpen = value;
                    RaisePropertyChanged(nameof(IsOpen));
                }
            }
        }
        public string MessageText
        {
            get { return _messageText; }
            private set
            {
                _messageText = value;
                RaisePropertyChanged(nameof(MessageText));
            }
        }

        #endregion

        #region Commands
        private RelayCommand _hideAlertCommand;

        public RelayCommand HideAlertCommand
        {
            get { return _hideAlertCommand ?? (_hideAlertCommand = new RelayCommand(HideAlertCommandExecute)); }
        }

        private void HideAlertCommandExecute()
        {
            IsOpen = false;
            _isClosed = !_isCritical;
        }

        #endregion

        public void ShowAlertMessage(string messageText, bool isCritical)
        {
            _isClosed = false;
            _isCritical = isCritical;
            MessageText = messageText;
            IsOpen = true;
        }
    }
}