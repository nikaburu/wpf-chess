using System;
using System.Collections.Generic;
using System.Linq;
using WpfChess.ChessModel.Events;
using WpfChess.ChessModel.Figures;

namespace WpfChess.ChessModel
{
    public sealed class Field : ICloneable
    {
        #region Fields
        private readonly List<Cell> _cells;
        private bool? _isCheckOnField;
        #endregion

        #region Constructor
        /// <summary>
        /// Add cells for field and initialize standard figures placement for game
        /// </summary>
        public Field()
        {
            IsWhiteGo = true;
            IsCheckOnField = null;
            IsMateOnField = false;

            _cells = new List<Cell>(64);
            for (int i = 0; i < 64; i++)
            {
                int horPos = i / 8;
                int vertPos = i % 8;

                _cells.Add(new Cell(horPos, vertPos));
            }
            InitializeField();
        }
        #endregion

        #region Events
        public event EventHandler<PawnFigureTransformationEventArgs> PawnFigureTransformationEvent;
        private void InvokePawnFigureTransformationEvent(PawnFigureTransformationEventArgs e)
        {
            EventHandler<PawnFigureTransformationEventArgs> handler = PawnFigureTransformationEvent;
            if (handler != null) handler(this, e);
        }
        #endregion

        #region Properties
        public Cell this[int hor, int vert]
        {
            get
            {
                Cell cell = _cells[vert + hor * 8];
                return cell;
            }
            set { this[hor, vert] = value; }
        }
        public List<Cell> Cells
        {
            get
            {
                return _cells;
            }
        }

        public bool IsWhiteGo { get; private set; }
        public bool? IsCheckOnField
        {
            get { return _isCheckOnField; }
            private set
            {
                _isCheckOnField = value;

                if (value != null)
                {
                    IsMateOnField = IsMateSituation(value.Value);
                }
            }
        }
        public bool IsMateOnField { get; private set; }

        private List<Cell> WhiteFigureCells
        {
            get { return Cells.Where(rec => rec.Figure != null && rec.Figure.IsWhite).ToList(); }
        }
        private List<Cell> BlackFigureCells
        {
            get { return Cells.Where(rec => rec.Figure != null && !rec.Figure.IsWhite).ToList(); }
        }
        #endregion

        #region Public members
        /// <summary>
        /// Make turn for figure on one cell to another cell
        /// </summary>
        /// <param name="fromCell">cell that contain figure for turn</param>
        /// <param name="toCell">target cell for turn</param>
        /// <returns></returns>
        public bool MakeTurn(Cell fromCell, Cell toCell)
        {
            if (fromCell.Figure == null || (toCell.Figure != null && toCell.Figure is KingFigure))
            {
                return false;
            }

            fromCell.Figure.MakeTurn(toCell);

            if (toCell.Figure is PawnFigure && (toCell.HorPosition == 7 || toCell.HorPosition == 0))
            {
                InvokePawnFigureTransformationEvent(new PawnFigureTransformationEventArgs(toCell));
            }

            IsWhiteGo = !IsWhiteGo;

            IsCheckOnField = IsCheckSituationOnField(IsWhiteGo) ? IsWhiteGo : (bool?)null;
            return true;
        }

        /// <summary>
        /// Get possible turns for figure in cell
        /// </summary>
        /// <param name="fromCell">cell that contain figure</param>
        /// <returns></returns>
        public List<Cell> PossibleTurns(Cell fromCell)
        {
            if (fromCell.Figure != null)
            {
                List<Cell> cells = FilterAccordingFigureComposition(fromCell);

                //todo: Castling for King and 123 of PawnFigure and pawn if king near board

                foreach (var cell in cells.ToList())
                {
                    if (IsCheckForTurn(fromCell, cell))
                    {
                        cells.Remove(cell);
                    }
                }
                return cells;
            }

            return new List<Cell>();
        }

        #endregion

        #region Private members
        /// <summary>
        /// Initialize field with standard figures placement
        /// </summary>
        private void InitializeField()
        {
            //Pawns
            for (int i = 0; i < 8; i++)
            {
                this[1, i].Figure = new PawnFigure(true, this[1, i]);
                this[6, i].Figure = new PawnFigure(false, this[6, i]);
            }

            //Rook
            this[0, 0].Figure = new RookFigure(true, this[0, 0]);
            this[0, 7].Figure = new RookFigure(true, this[0, 7]);
            this[7, 0].Figure = new RookFigure(false, this[7, 0]);
            this[7, 7].Figure = new RookFigure(false, this[7, 7]);

            //Knights
            this[0, 1].Figure = new KnightFigure(true, this[0, 1]);
            this[0, 6].Figure = new KnightFigure(true, this[0, 6]);
            this[7, 1].Figure = new KnightFigure(false, this[7, 1]);
            this[7, 6].Figure = new KnightFigure(false, this[7, 6]);

            //Bishops
            this[0, 2].Figure = new BishopFigure(true, this[0, 2]);
            this[0, 5].Figure = new BishopFigure(true, this[0, 5]);
            this[7, 2].Figure = new BishopFigure(false, this[7, 2]);
            this[7, 5].Figure = new BishopFigure(false, this[7, 5]);

            //Queens
            this[0, 3].Figure = new QueenFigure(true, this[0, 3]);
            this[7, 3].Figure = new QueenFigure(false, this[7, 3]);

            //Kings
            this[0, 4].Figure = new KingFigure(true, this[0, 4]);
            this[7, 4].Figure = new KingFigure(false, this[7, 4]);
        }

        /// <summary>
        /// Filter possible figures turns according its position on this field
        /// </summary>
        /// <param name="cellForTurn">cell with figure for turn</param>
        /// <param name="fromCell">free cell</param>
        /// <param name="toCell">take cell</param>
        /// <returns>possible cells for turn </returns>
        private List<Cell> FilterAccordingFigureComposition(Cell cellForTurn, Cell fromCell = null, Cell toCell = null)
        {
            Figure figureForTurn = cellForTurn.Figure;
            List<List<Cell>> possibleTurns = cellForTurn.Figure.GetPossibleTurns().Select(possibleTurn => possibleTurn.Select(cell => this.Cells.Single(rec => rec.HorPosition == cell.HorPosition && rec.VertPosition == cell.VertPosition)).ToList()).ToList();

            List<Cell> cells = new List<Cell>();
            foreach (var possibleTurn in possibleTurns)
            {
                foreach (var cell in possibleTurn)
                {
                    if (fromCell != null && toCell != null)
                    {
                        if (cell.Figure != null && cell != fromCell && cell != toCell)
                        {
                            if (cell.Figure.IsWhite != figureForTurn.IsWhite && !(figureForTurn is PawnFigure))
                            {
                                cells.Add(cell);
                            }

                            break;
                        }

                        if (cell == toCell)
                        {
                            if (fromCell.Figure.IsWhite != figureForTurn.IsWhite && !(figureForTurn is PawnFigure))
                            {
                                cells.Add(cell);
                            }

                            break;
                        }
                    }
                    else
                    {
                        if (cell.Figure != null)
                        {
                            if (cell.Figure.IsWhite != figureForTurn.IsWhite && !(figureForTurn is PawnFigure))
                            {
                                cells.Add(cell);
                            }

                            break;
                        }
                    }

                    cells.Add(cell);
                }
            }

            //for PawnFigure
            if (cellForTurn.Figure is PawnFigure)
            {
                int hor = cellForTurn.HorPosition;
                int vert = cellForTurn.VertPosition;

                if ((hor + cellForTurn.Figure.Orientation < 8 && hor + cellForTurn.Figure.Orientation >= 0))
                {
                    if (vert + 1 < 8)
                    {
                        Cell cellWithFigure = this[hor + cellForTurn.Figure.Orientation, vert + 1];

                        if (cellWithFigure != fromCell && cellWithFigure.Figure != null
                            && cellWithFigure.Figure.IsWhite != figureForTurn.IsWhite)
                        {
                            cells.Add(cellWithFigure);
                        }

                        if (fromCell != null && toCell != null && cellWithFigure == toCell
                            && fromCell.Figure.IsWhite != figureForTurn.IsWhite)
                        {
                            cells.Add(cellWithFigure);
                        }
                    }

                    if (vert - 1 >= 0)
                    {
                        Cell cellWithFigure = this[hor + cellForTurn.Figure.Orientation, vert - 1];

                        if (cellWithFigure != fromCell &&  cellWithFigure.Figure != null
                            && cellWithFigure.Figure.IsWhite != figureForTurn.IsWhite)
                        {
                            cells.Add(cellWithFigure);
                        }

                        if (fromCell!= null && toCell != null && cellWithFigure == toCell
                            && fromCell.Figure.IsWhite != figureForTurn.IsWhite)
                        {
                            cells.Add(cellWithFigure);
                        }
                    }
                }
            }
            //

            return cells;
        }

        /// <summary>
        /// Say if we have check for player after his turn
        /// </summary>
        /// <param name="fromCell">turn from</param>
        /// <param name="toCell">turn to</param>
        /// <returns>is check after turn</returns>
        private bool IsCheckForTurn(Cell fromCell, Cell toCell)
        {
            var anotherSideFigures = (fromCell.Figure.IsWhite ? BlackFigureCells : WhiteFigureCells).Where(rec => rec != toCell);

            List<Cell> possibleTurns = new List<Cell>();
            foreach (var cell in anotherSideFigures)
            {
                possibleTurns.AddRange(FilterAccordingFigureComposition(cell, fromCell, toCell));
            }

            return possibleTurns.Any(turn => (turn.Figure is KingFigure && turn != fromCell) || (turn == toCell && fromCell.Figure is KingFigure));
        }

        /// <summary>
        /// Say if we have check for player
        /// </summary>
        /// <param name="isForWhite">player (true = white)</param>
        /// <returns>is check for player</returns>
        private bool IsCheckSituationOnField(bool isForWhite)
        {
            var anotherSideFigures = isForWhite ? BlackFigureCells : WhiteFigureCells;

            List<Cell> possibleTurns = new List<Cell>();
            foreach (var cell in anotherSideFigures)
            {
                possibleTurns.AddRange(FilterAccordingFigureComposition(cell));
            }

            return possibleTurns.Any(turn => turn.Figure is KingFigure);
        }

        /// <summary>
        /// Say if we have mate for player
        /// </summary>
        /// <param name="isForWhite">player color true = white</param>
        /// <returns>is check situation on field</returns>
        private bool IsMateSituation(bool isForWhite)
        {
            var thatSideFigures = isForWhite ? WhiteFigureCells : BlackFigureCells;

            return thatSideFigures.All(thatSideFigure => !FilterAccordingFigureComposition(thatSideFigure).Any(possibleTurn => !IsCheckForTurn(thatSideFigure, possibleTurn)));
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            Field newField = new Field();
            foreach (var cell in newField.Cells)
            {
                cell.Figure = null;
            }

            foreach (var cell in Cells.Where(rec => rec.Figure != null))
            {
                Cell newFieldCell = newField[cell.HorPosition, cell.VertPosition];

                newFieldCell.Figure = CloneFigure(cell.Figure, newFieldCell);
            }


            return newField;
        }
        private static Figure CloneFigure(Figure figure, Cell toCell)
        {
            Figure newFigure = new PawnFigure(figure.IsWhite, toCell);
            if (figure is KingFigure)
            {
                newFigure = new KingFigure(figure.IsWhite, toCell);
            }

            if (figure is QueenFigure)
            {
                newFigure = new QueenFigure(figure.IsWhite, toCell);
            }

            if (figure is BishopFigure)
            {
                newFigure = new BishopFigure(figure.IsWhite, toCell);
            }

            if (figure is KnightFigure)
            {
                newFigure = new KnightFigure(figure.IsWhite, toCell);
            }

            if (figure is RookFigure)
            {
                newFigure = new RookFigure(figure.IsWhite, toCell);
            }

            return newFigure;
        }
        #endregion ICloneable
    }
}
