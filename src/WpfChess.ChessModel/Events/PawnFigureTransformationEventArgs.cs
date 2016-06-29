using System;

namespace WpfChess.ChessModel.Events
{
    public class PawnFigureTransformationEventArgs : EventArgs
    {
        public PawnFigureTransformationEventArgs(Cell onCell)
        {
            Cell = onCell;
        }

        public Cell Cell { get; private set; }
    }
}
