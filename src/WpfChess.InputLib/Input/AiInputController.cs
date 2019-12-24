using System;
using System.Linq;
using System.Threading;
using WpfChess.ChessModel;
using WpfChess.ChessModel.Figures;

namespace WpfChess.InputLib.Input
{
    public sealed class AiInputController : InputController
    {
        #region Consts
        private readonly string[] _possiblePawnTransformations = new [] { "RookFigure", "KnightFigure", "BishopFigure", "QueenFigure" };
        #endregion Consts

        #region Fields
        private readonly Field _field;
        private readonly Random _random;
        private readonly bool _isWhite;

        private bool _pawnNearBoardAfterTurn;
        #endregion

        public AiInputController(Field field): base(false)
        {
            _field = field;
            _isWhite = false;
            _random = new Random((int)DateTime.Now.Ticks);
        }

        public override void SendInput(Cell fromCell, Cell toCell, Figure figure = null)
        {
            IsWaiting = true;
            
            Action act = MakeTurn;
            act.BeginInvoke(null, null);
        }

        #region Private members
        private void MakeTurn()
        {
            Thread.Sleep(1500);
            var turn = FindNewTurn((Field)_field.Clone());
            base.MakeInput(turn.Item1, turn.Item2, turn.Item3);
        }

        private Tuple<Cell, Cell, string> FindNewTurn(Field field)
        {
            var cells = field.Cells.Where(rec => rec.Figure != null && rec.Figure.IsWhite == _isWhite);
            var possibleTurns = (from cell in cells
                                 let turns = field.PossibleTurns(cell)
                                 where turns.Count > 0
                                 select new {FromCell = cell, Turns = turns}).ToList();
            
            if (possibleTurns.Any(rec => rec.Turns.Any(turn => turn.Figure != null)))
            {
                possibleTurns = possibleTurns.Where(rec => rec.Turns.Any(turn => turn.Figure != null)).ToList();
            }

            var from = possibleTurns[_random.Next(possibleTurns.Count)];
            Cell fromCell = from.FromCell;
            Cell toCell = from.Turns[_random.Next(from.Turns.Count)];
            string figureName = null;

            field.PawnFigureTransformationEvent += PawnFigureNearBoard;
            field.MakeTurn(fromCell, toCell);
            if (_pawnNearBoardAfterTurn)
            {
                if (_random.Next(100) < 10)
                {
                    figureName = _possiblePawnTransformations[_random.Next(_possiblePawnTransformations.Length - 1)];
                }
                else
                {
                    figureName = _possiblePawnTransformations[_possiblePawnTransformations.Length - 1];
                }
                

                _pawnNearBoardAfterTurn = false;
            }
            field.PawnFigureTransformationEvent -= PawnFigureNearBoard;



            return new Tuple<Cell, Cell, string>(fromCell, toCell, figureName);
        }

        private void PawnFigureNearBoard(object source, EventArgs args)
        {
            _pawnNearBoardAfterTurn = true;
        }
        #endregion
    }
}