using System;
using WpfChess.ChessModel;

namespace WpfChess.InputLib.Events
{
    public class InputDataEventArgs : EventArgs
    {
        #region Properties
        public Cell FromCell { get; private set; }
        public Cell ToCell { get; private set; }
        public string FigureName { get; private set; }
        #endregion

        #region Constructors
        public InputDataEventArgs(Cell fromCell, Cell toCell, string figureName = null)
        {
            FromCell = fromCell;
            ToCell = toCell;
            FigureName = figureName;
        }
        #endregion
    }
}
