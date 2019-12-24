using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WpfChess.WpfPresentation.Model;
using WpfChess.WpfPresentation.View;

namespace WpfChess.WpfPresentation.ViewModel
{
    public sealed class GamePreloaderViewModel : ViewModelBase
    {
        #region Fields
        private readonly GamePreloaderView _view;
        private readonly GamePreloader _gamePreloader;
        private string _serviceUrl;
        private string _endpointAddress;
        #endregion

        #region Properties
        public string ServiceUrl
        {
            get
            {
                return _serviceUrl.Substring(_serviceUrl.IndexOf("//") +2);
            }
            set
            {
                _serviceUrl = "net.tcp://" + value;
            }
        }
        public string EndpointAddress
        {
            get { return _endpointAddress.Substring(_endpointAddress.IndexOf("//") + 2); }
            set
            {
                _endpointAddress = "net.tcp://" + value;
            }
        }

        public string AdUrl { get; set; }
        public bool IsFirst { get; set; }

        #endregion

        #region Constructors
        public GamePreloaderViewModel(GamePreloaderView view, GamePreloader gamePreloader)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));;
            _gamePreloader = gamePreloader ?? throw new ArgumentNullException(nameof(gamePreloader));

            GameConfigData configData = _gamePreloader.ConfigData;
            _serviceUrl = configData.ServiceUrl;
            _endpointAddress = configData.EndpointAddress;
            AdUrl = configData.AdUrl;
            IsFirst = configData.IsFirstGo;
        }
        #endregion Constructors

        #region Commands
        private RelayCommand<string> _gameButtonClickCommand;

        public RelayCommand<string> GameButtonClickCommand
        {
            get { return _gameButtonClickCommand ?? (_gameButtonClickCommand = new RelayCommand<string>(GameButtonClickCommandExecute)); }
        }

        private void GameButtonClickCommandExecute(string gameModeString)
        {
            GameMode gameMode;
            if (!Enum.TryParse<GameMode>(gameModeString, out gameMode))
            {
                gameMode = GameMode.OnePcMode;
            }
            
            _gamePreloader.StartNewGame(new GameConfigData(_serviceUrl, AdUrl, _endpointAddress, IsFirst, gameMode));
            _view.Close();
        }

        #endregion
    }
}
