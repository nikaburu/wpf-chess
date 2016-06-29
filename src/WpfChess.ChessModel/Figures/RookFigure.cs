using System.Collections.Generic;

namespace WpfChess.ChessModel.Figures
{
    public class RookFigure : Figure
    {
        public RookFigure(bool isWhite, Cell cell) : base(isWhite, cell)
        {
        }

        public override List<List<Cell>> GetPossibleTurns()
        {
            var possibleTurns = new List<List<Cell>>(4);

            #region how Rook
            List<Cell> path = new List<Cell>();
            for (int i = this.Cell.VertPosition + 1; i < 8; i++) // ->
            {
                path.Add(new Cell(this.Cell.HorPosition, i));
            }
            possibleTurns.Add(path);
            
            path = new List<Cell>();
            for (int i = this.Cell.VertPosition - 1; i >= 0; i--)
            {
                path.Add(new Cell(this.Cell.HorPosition, i));
            }
            possibleTurns.Add(path);

            path = new List<Cell>();
            for (int i = this.Cell.HorPosition + 1; i < 8; i++)
            {
                path.Add(new Cell(i, this.Cell.VertPosition));
            }
            possibleTurns.Add(path);

            path = new List<Cell>();
            for (int i = this.Cell.HorPosition - 1; i >= 0; i--)
            {
                path.Add(new Cell(i, this.Cell.VertPosition));
            }
            possibleTurns.Add(path);
            #endregion

            return possibleTurns;
        }
    }
}