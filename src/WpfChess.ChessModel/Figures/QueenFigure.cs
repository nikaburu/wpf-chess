using System.Collections.Generic;

namespace WpfChess.ChessModel.Figures
{
    public class QueenFigure : Figure
    {
        public QueenFigure(bool isWhite, Cell cell)
            : base(isWhite, cell)
        {
        }

        public override List<List<Cell>> GetPossibleTurns()
        {
            var possibleTurns = new List<List<Cell>>(8);

            possibleTurns.AddRange((new RookFigure(IsWhite, Cell)).GetPossibleTurns());
            possibleTurns.AddRange((new BishopFigure(IsWhite, Cell)).GetPossibleTurns());
            
            return possibleTurns;
        }
    }
}