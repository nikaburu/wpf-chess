using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WpfChess.ChessModel;
using WpfChess.ChessModel.Figures;

namespace WpfChess.WpfPresentation.ViewModel
{
    public class PawnTransformationViewModel : ViewModelBase
    {
        #region Fields
        private readonly CellViewModel _cell;
        #endregion

        #region Properties
        public bool IsWhite { get; private set; }
        
        private bool _isPawnTransformationPopup = true;
        public bool IsPawnTransformationPopup
        {
            get { return _isPawnTransformationPopup; }
            set
            {
                if (_isPawnTransformationPopup != false)
                {
                    _isPawnTransformationPopup = value;
                }
            }
        }

        public Cell RookFigureCell { get; private set; }
        public Cell KnightFigureCell { get; private set; }
        public Cell BishopFigureCell { get; private set; }
        public Cell QueenFigureCell { get; private set; }
        #endregion

        #region Constructors
        public PawnTransformationViewModel(bool isWhite, CellViewModel cell)
        {
            IsWhite = isWhite;
            _cell = cell;
            //IsPawnTransformationPopup = true;

            RookFigureCell = new Cell(0,0);
            RookFigureCell.Figure = new RookFigure(IsWhite, RookFigureCell);

            KnightFigureCell = new Cell(0, 0);
            KnightFigureCell.Figure = new KnightFigure(IsWhite, KnightFigureCell);

            BishopFigureCell = new Cell(0, 0);
            BishopFigureCell.Figure = new BishopFigure(IsWhite, BishopFigureCell);

            QueenFigureCell = new Cell(0, 0);
            QueenFigureCell.Figure = new QueenFigure(IsWhite, QueenFigureCell);
        }
        #endregion

        #region Events
        public event EventHandler PawnTransformationDoneEvent;
        private void InvokePawnTransformationDoneEvent()
        {
            EventHandler handler = PawnTransformationDoneEvent;
            if (handler != null) handler(_cell, null);
        }
        #endregion

        #region Commands
        private RelayCommand<string> _figureSelectionCommand;

        public RelayCommand<string> FigureSelectionCommand
        {
            get { return _figureSelectionCommand ?? (_figureSelectionCommand = new RelayCommand<string>(FigureSelectionCommandExecute)); }
        }

        private void FigureSelectionCommandExecute(string figureType)
        {
            _cell.TransformFigureOnCel(figureType, _cell.Figure.IsWhite);

            IsPawnTransformationPopup = false;
            RaisePropertyChanged("IsPawnTransformationPopup");
            InvokePawnTransformationDoneEvent();
        }

        #endregion
    }
}