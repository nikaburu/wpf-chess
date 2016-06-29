using System.Collections.Generic;
using System.Linq;

namespace WpfChess.ChessModel.Figures
{
    public class KnightFigure : Figure
    {
        public KnightFigure(bool isWhite, Cell cell) : base(isWhite, cell)
        {
        }

        public override List<List<Cell>> GetPossibleTurns()
        {
            int vert = this.Cell.VertPosition;
            int hor = this.Cell.HorPosition;
            var possibleTurns = new List<List<Cell>>()
                                    {
                                CheckedCell(hor - 1, vert + 2),
                                CheckedCell(hor - 1, vert - 2),
                                CheckedCell(hor + 1, vert + 2),
                                CheckedCell(hor + 1, vert - 2),
                                CheckedCell(hor - 2, vert + 1),
                                CheckedCell(hor - 2, vert - 1),
                                CheckedCell(hor + 2, vert + 1),
                                CheckedCell(hor + 2, vert - 1)
                            };

            return possibleTurns.Where(rec => rec != null).ToList();
        }

        private static List<Cell> CheckedCell(int hor, int vert)
        {
            if (vert > 7 || vert < 0 || hor > 7 || hor < 0)
            {
                return null;
            }

            return new List<Cell>() { new Cell(hor, vert) };
        }
    }
}