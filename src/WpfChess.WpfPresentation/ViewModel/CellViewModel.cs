using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WpfChess.ChessModel;
using WpfChess.ChessModel.Figures;
using WpfChess.WpfPresentation.Exceptions;
using WpfChess.WpfPresentation.Helpers;
using WpfChess.WpfPresentation.Localization;

namespace WpfChess.WpfPresentation.ViewModel
{
    public sealed class CellViewModel : ViewModelBase
    {
        #region Fields
        private readonly Cell _cell;
        private readonly FieldViewModel _fieldViewModel;

        private bool _isMarked;
        private bool _isSelected;
        #endregion

        #region Constructors
        public CellViewModel(Cell cell, FieldViewModel fieldViewModel)
        {
            _cell = cell;
            _fieldViewModel = fieldViewModel;
        }

        #endregion

        #region Properties
        public Figure Figure { get { return _cell.Figure; } }

        public Cell Cell { get { return _cell; } }

        public bool IsWhite
        {
            get
            {
                return (_cell.HorPosition % 2 == 0 && _cell.VertPosition % 2 != 0 ||
                        _cell.HorPosition % 2 != 0 && _cell.VertPosition % 2 == 0);
            }
        }

        public bool IsMarked
        {
            get { return _isMarked; }
            set
            {
                if (_isMarked != value)
                {
                    _isMarked = value;

                    RaisePropertyChanged(nameof(Background));
                }
            }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    RaisePropertyChanged(nameof(Background));
                }
            }
        }
        #endregion

        #region Commands
        private RelayCommand _makeTurn;

        public RelayCommand MakeTurn
        {
            get { return _makeTurn ?? (_makeTurn = new RelayCommand(MakeTurnExecute)); }
        }

        private RelayCommand _makeSelected;

        public RelayCommand MakeSelected
        {
            get { return _makeSelected ?? (_makeSelected = new RelayCommand(MakeSelectedExecute)); }
        }
        #endregion

        #region Publlic members
        public void CallFigurePropertyChanged()
        {
            RaisePropertyChanged(nameof(Figure));
        }

        public void TransformFigureOnCel(string typeName, bool isWhiteFigure)
        {
            Figure figure = null;
            switch (typeName)
            {
                case "RookFigure":
                    figure = new RookFigure(isWhiteFigure, _cell);
                    break;
                case "KnightFigure":
                    figure = new KnightFigure(isWhiteFigure, _cell);
                    break;
                case "BishopFigure":
                    figure = new BishopFigure(isWhiteFigure, _cell);
                    break;
                case "QueenFigure":
                    figure = new QueenFigure(isWhiteFigure, _cell);
                    break;
            }

            if (figure == null)
            {
                throw new ChessGameException(Messages.PawnTransformationFigureNotSelected);
            }
            _cell.Figure = figure;
            CallFigurePropertyChanged();
        }
        #endregion

        #region Private members
        private void MakeTurnExecute()
        {
            _fieldViewModel.MakeTurn(this);
        }

        private void MakeSelectedExecute()
        {
            _fieldViewModel.MarkCellsForTurn(this);
        }
        #endregion

        #region For View
        public SolidColorBrush Background
        {
            get
            {
                if (IsMarked)
                {
                    return new SolidColorBrush(GameSettingHelper.MarkedCellColor);
                }

                if (IsSelected)
                {
                    return new SolidColorBrush(GameSettingHelper.SelectedCellColor);
                }

                return IsWhite ? new SolidColorBrush(GameSettingHelper.WhiteCellColor) : new SolidColorBrush(GameSettingHelper.BlackCellColor);
            }

        }
        #endregion
    }
}
