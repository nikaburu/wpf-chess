using System;
using WpfChess.ChessModel;
using WpfChess.ChessModel.Figures;
using WpfChess.InputLib.Events;

namespace WpfChess.InputLib.Input
{
    public abstract class InputController
    {
        #region Properties

        private bool _isWaiting;
        public bool IsWaiting
        {
            get { return _isWaiting; }
            protected set
            {
                _isWaiting = value;
                InvokeIsWaitingChangedEvent();
            }
        }

        #endregion

        #region Constructors

        protected InputController(bool isStartWaiting)
        {
            IsWaiting = isStartWaiting;
        }
        #endregion

        #region Events
        public event EventHandler IsWaitingChangedEvent;
        private void InvokeIsWaitingChangedEvent()
        {
            EventHandler handler = IsWaitingChangedEvent;
            if (handler != null) handler(this, null);
        }

        public event EventHandler<InputDataEventArgs> CameInputDataEvent;
        private void InvokeCameInputDataEvent(InputDataEventArgs e)
        {
            EventHandler<InputDataEventArgs> handler = CameInputDataEvent;
            if (handler != null) handler(this, e);
        }
        #endregion

        #region Public members

        public abstract void SendInput(Cell fromCell, Cell toCell, Figure figure = null);

        public void MakeInput(Cell fromCell, Cell toCell, string figureName)
        {
            if (IsWaiting)
            {
                IsWaiting = false;
                InvokeCameInputDataEvent(new InputDataEventArgs(fromCell, toCell, figureName));
            }
        }

        public virtual void CleanUpResources() {}

        #endregion
    }
}
