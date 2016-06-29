using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Threading;
using WpfChess.InputLib.Exceptions;
using WpfChess.WpfPresentation.Exceptions;
using WpfChess.WpfPresentation.Model;
using WpfChess.WpfPresentation.View;
using WpfChess.WpfPresentation.ViewModel;

namespace WpfChess.WpfPresentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException +=
                      new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);

            Dispatcher.UnhandledException +=
                new DispatcherUnhandledExceptionEventHandler(DispatcherUnhandledExceptionExecute);

            GamePreloaderView view = new GamePreloaderView();
            GamePreloaderViewModel viewModel = new GamePreloaderViewModel(view, new GamePreloader());

            //TryResource();

            view.DataContext = viewModel;
            view.Show();
        }

        private static void DispatcherUnhandledExceptionExecute(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is InputException || e.Exception is ChessGameException)
            {
                string message = e.Exception.Message;
                MessageBox.Show(message, "Chess game error",
                                MessageBoxButton.OK, MessageBoxImage.Error);

                e.Dispatcher.InvokeShutdown();
                e.Handled = true;
            }
        }

        private void TryResource()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            using (IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream("", FileMode.Open, storage))
            {

            }

        }


        void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            MessageBox.Show(e.ExceptionObject.ToString(), "Chess game unexpected exception!",
                            MessageBoxButton.OK, MessageBoxImage.Error);

            foreach (Window window in this.Windows)
            {
                window.Close();
            }
        }
    }
}
