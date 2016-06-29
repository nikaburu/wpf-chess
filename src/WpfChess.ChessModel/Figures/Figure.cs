using System.Collections.Generic;

namespace WpfChess.ChessModel.Figures
{
    public abstract class Figure
    {
        #region Fields
        protected Cell Cell;
        #endregion

        #region Constructors
        protected Figure(bool isWhite, Cell cell)
        {
            IsWhite = isWhite;
            Cell = cell;
            IsAlreadyGo = false;
        }
        #endregion

        #region Properties
        public bool IsWhite { get; private set; }
        public int Orientation
        {
            get { return IsWhite ? 1 : -1; }
        }
        public bool IsAlreadyGo { get; private set; }
        #endregion

        #region Public members
        public abstract List<List<Cell>> GetPossibleTurns();
        public void MakeTurn(Cell toCell)
        {
            toCell.Figure = this;
            this.Cell.Figure = null;
            this.Cell = toCell;

            IsAlreadyGo = true;
            return;
        }
        #endregion
    }
}
