using System.Collections.Generic;

namespace WpfChess.ChessModel.Figures
{
    public class BishopFigure : Figure
    {
        public BishopFigure(bool isWhite, Cell cell) : base(isWhite, cell)
        {
        }

        public override List<List<Cell>> GetPossibleTurns()
        {
            var possibleTurns = new List<List<Cell>>(4);

            #region how Bishop
            List<Cell> path = new List<Cell>();
            int vert = this.Cell.VertPosition + 1;
            int hor = this.Cell.HorPosition + 1;
            while (vert < 8 && hor < 8)
            {
                path.Add(new Cell(hor, vert));
                vert++;
                hor++;
            }
            possibleTurns.Add(path);

            path = new List<Cell>();
            vert = this.Cell.VertPosition + 1;
            hor = this.Cell.HorPosition - 1;
            while (vert < 8 && hor >= 0)
            {
                path.Add(new Cell(hor, vert));
                vert++;
                hor--;
            }
            possibleTurns.Add(path);

            path = new List<Cell>();
            vert = this.Cell.VertPosition - 1;
            hor = this.Cell.HorPosition - 1;
            while (vert >= 0 && hor >= 0)
            {
                path.Add(new Cell(hor, vert));
                vert--;
                hor--;
            }
            possibleTurns.Add(path);

            path = new List<Cell>();
            vert = this.Cell.VertPosition - 1;
            hor = this.Cell.HorPosition + 1;
            while (vert >= 0 && hor < 8)
            {
                path.Add(new Cell(hor, vert));
                vert--;
                hor++;
            }
            possibleTurns.Add(path);

            #endregion

            return possibleTurns;
        }
    }
}