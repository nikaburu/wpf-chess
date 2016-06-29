using System.Collections.Generic;
using System.Linq;

namespace WpfChess.ChessModel.Figures
{
    public class PawnFigure : Figure
    {
        public PawnFigure(bool isWhite, Cell cell) : base(isWhite, cell)
        {
        }

        public override List<List<Cell>> GetPossibleTurns()
        {
            List<Cell> path = new List<Cell> { CheckedCell(this.Cell.HorPosition + Orientation, this.Cell.VertPosition) };

            if (!IsAlreadyGo)
            {
                path.Add(CheckedCell(this.Cell.HorPosition + Orientation * 2, this.Cell.VertPosition));
            }

            return new List<List<Cell>>() { path.Where(rec => rec != null).ToList() };
        }
        
        private static Cell CheckedCell(int hor, int vert)
        {
            if (vert > 7 || vert < 0 || hor > 7 || hor < 0)
            {
                return null;
            }

            return new Cell(hor, vert);
        }
    }
}