using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WpfChess.ChessModel;
using WpfChess.InputLib.Input;
using WpfChess.WpfPresentation.Model;
using WpfChess.WpfPresentation.View;

namespace WpfChess.WpfPresentation.ViewModel
{
    public sealed class GamePreloaderViewModel : ViewModelBase
    {
        #region Fields
        private readonly GamePreloaderView _view;
        #endregion

        #region Properties
        public bool IsFirst { get; set; } = true;

        #endregion

        #region Constructors
        public GamePreloaderViewModel(GamePreloaderView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
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
            
            StartNewGame(new GameConfig(IsFirst, gameMode));
            _view.Close();
        }

        #endregion

        #region Private members
        
        private static void StartNewGame(GameConfig config)
        {
            Field field = new Field();
            FieldViewModel fieldViewModel = new FieldViewModel(field, GetInputController(config, field));

            MainWindow window = new MainWindow();

            MainWindowViewModel viewModel = new MainWindowViewModel(fieldViewModel);
            window.DataContext = viewModel;
            window.Show();
        }

        private static InputController GetInputController(GameConfig config, Field field)
        {
            InputController inputController;
            switch (config.GameMode)
            {
                case GameMode.OnePcMode:
                    inputController = new OnePcInputController();
                    break;
                case GameMode.AiMode:
                    inputController = new AiInputController(field, !config.IsFirstGo);
                    break;
                default:
                    inputController = new OnePcInputController();
                    break;
            }

            return inputController;
        }
        #endregion
    }
}
