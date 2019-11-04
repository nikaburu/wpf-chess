using System.IO;
using System.Reflection;
using WpfChess.ChessModel;
using WpfChess.InputLib.Input;
using WpfChess.WpfPresentation.ViewModel;

namespace WpfChess.WpfPresentation.Model
{
    public sealed class GamePreloader
    {
        #region Consts
        private const string ConfigFileName = "GameConfigData.xml";
        #endregion

        #region Properties
        public GameConfigData ConfigData
        {
            get
            {
                using (Stream stream = File.Open(ConfigFilePath(), FileMode.Open, FileAccess.Read))
                {
                    return GameConfigData.GetFromFile(stream);
                }
            }
        }
        #endregion

        #region Public members
        public void StartNewGame(GameConfigData gameConfigData)
        {
            Field field = new Field();
            FieldViewModel fieldViewModel = new FieldViewModel(field, GetInputController(gameConfigData, field));

            MainWindow window = new MainWindow();

            MainWindowViewModel viewModel = new MainWindowViewModel(fieldViewModel);
            window.DataContext = viewModel;
            window.Show();

            if (gameConfigData.GameMode == GameMode.NetworkMode)
            {
                SaveConfig(gameConfigData);
            }
        }

        #endregion

        #region Private members
        private static InputController GetInputController(GameConfigData gameConfigData, Field field)
        {
            InputController inputController;
            switch (gameConfigData.GameMode)
            {
                case GameMode.OnePcMode:
                    inputController = new OnePcInputController();
                    break;
                case GameMode.AiMode:
                    inputController = new AiInputController(field);
                    break;
                case GameMode.NetworkMode:
                    // inputController = new NetworkInputController(gameConfigData.ServiceUrl,
                    //                                              gameConfigData.EndpointAddress, gameConfigData.AdUrl,
                    //                                              !gameConfigData.IsFirstGo);
                    // break;
                default:
                    inputController = new OnePcInputController();
                    break;
            }

            return inputController;
        }

        private void SaveConfig(GameConfigData gameConfigData)
        {
            using (Stream stream = File.Open(ConfigFilePath(), FileMode.Truncate, FileAccess.Write))
            {
                GameConfigData.SetToFile(stream, gameConfigData);
            }
        }

        private string ConfigFilePath()
        {
            // if (ApplicationDeployment.IsNetworkDeployed)
            // {
            //     string path = ApplicationDeployment.CurrentDeployment.DataDirectory + @"\" + ConfigFileName;
            //     if (File.Exists(path))
            //     {
            //         return path;
            //     }
            // }
            // else
            {
                string pathToExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Data\" + ConfigFileName;
                if (File.Exists(pathToExe))
                {
                    return pathToExe;
                }
            }

            return ConfigFileName;
        }
        #endregion
    }
}
