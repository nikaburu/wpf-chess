using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WpfChess.ChessModel;
using WpfChess.ChessModel.Events;
using WpfChess.InputLib.Events;
using WpfChess.InputLib.Input;
using WpfChess.WpfPresentation.Exceptions;
using WpfChess.WpfPresentation.Localization;

namespace WpfChess.WpfPresentation.ViewModel
{
    public sealed class FieldViewModel : ViewModelBase
    {
        #region Fields
        private readonly Field _field;
        private CellViewModel _selectedCell;
        private readonly InputController _inputController;
        private bool _makeTurnFromInputController;
        private bool _isPawnTransformation;
        #endregion

        #region Properties
        public bool IsMatePopup { get; set; }
        public bool IsWhiteGo
        {
            get { return _field.IsWhiteGo; }
        }
        public bool IsUnderCheck
        {
            get
            {
                bool isCheck = _field.IsCheckOnField != null;
                if (isCheck && _field.IsMateOnField)
                {
                    IsMatePopup = true;
                    RaisePropertyChanged(nameof(IsMatePopup));
                }
                return isCheck;
            }
        }

        public bool IsWaiting
        {
            get { return _inputController.IsWaiting; }
        }
        public List<CellViewModel> Cells { get; private set; }

        public PawnTransformationViewModel PawnTransformationViewModel { get; private set; }
        #endregion

        #region Commands
        private RelayCommand _hideMatePopupCommand;

        public RelayCommand HideMatePopupCommand
        {
            get { return _hideMatePopupCommand ?? (_hideMatePopupCommand = new RelayCommand(HideMatePopupCommandExecute)); }
        }

        private void HideMatePopupCommandExecute()
        {
            IsMatePopup = false;
            RaisePropertyChanged(nameof(IsMatePopup));
        }

        #endregion

        #region Constructors
        public FieldViewModel(Field field, InputController inputController)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (inputController == null)
                throw new ArgumentNullException(nameof(inputController));

            _field = field;
            _field.PawnFigureTransformationEvent += this.PawnFigureNearBoard;

            Cells = _field.Cells.Select(cell => new CellViewModel(cell, this)).ToList();

            _inputController = inputController;
            _inputController.CameInputDataEvent += this.InputInformationExecute;
            _inputController.IsWaitingChangedEvent += new EventHandler(IsWaitingChangedEventExecute);
        }

        #endregion

        #region Public members
        public void MarkCellsForTurn(CellViewModel fromCell)
        {
            if (fromCell == null)
                throw new ArgumentNullException(nameof(fromCell));

            bool isWasSelected = fromCell.IsSelected;
            ClearAllCellsFromSelection();

            fromCell.IsSelected = !isWasSelected;
            if (fromCell.IsSelected)
            {
                _selectedCell = fromCell;

                if (fromCell.Figure != null)
                {
                    MarkCells(PrepareFieldViewModelCells(_field.PossibleTurns(fromCell.Cell)));
                }
            }
        }

        public bool MakeTurn(CellViewModel toCell)
        {
            if (toCell == null)
                throw new ArgumentNullException(nameof(toCell));

            if (_inputController.IsWaiting) return false;

            _makeTurnFromInputController = false;
            if (toCell.IsMarked && MakeTurnExecute(_selectedCell, toCell) && !_field.IsMateOnField)
            {
                if (!_isPawnTransformation)
                {
                    SendTurnToInputController(toCell);

                    RaisePropertyChanged(nameof(IsWaiting));
                }

                return true;
            }

            return false;
        }

        public bool MakeTurnExecute(CellViewModel fromCell, CellViewModel toCell)
        {
            if (toCell == null)
                throw new ArgumentNullException(nameof(toCell));
            
            if (fromCell != null && fromCell.Figure.IsWhite == IsWhiteGo && _field.MakeTurn(fromCell.Cell, toCell.Cell))
            {
                fromCell.CallFigurePropertyChanged();
                toCell.CallFigurePropertyChanged();

                RaisePropertyChanged(nameof(IsWhiteGo));
                RaisePropertyChanged(nameof(IsUnderCheck));

                ClearAllCellsFromSelection();
                return true;
            }

            return false;
        }

        #endregion

        #region Private members

        private void InputInformationExecute(object source, InputDataEventArgs args)
        {
            CellViewModel fromCell = Cells.Single(rec => rec.Cell == args.FromCell);
            CellViewModel toCell = Cells.Single(rec => rec.Cell == args.ToCell);

            _makeTurnFromInputController = true;
            if (!MakeTurnExecute(fromCell, toCell))
            {
                throw new ChessGameException(Messages.InputWithSyncError);
            }

            if (args.FigureName != null)
            {
                toCell.TransformFigureOnCel(args.FigureName, toCell.Figure.IsWhite);
            }

            RaisePropertyChanged(nameof(IsWaiting));
        }

        private void ClearAllCellsFromSelection()
        {
            foreach (var cell in Cells)
            {
                cell.IsMarked = false;
                cell.IsSelected = false;
            }
        }

        private IEnumerable<CellViewModel> PrepareFieldViewModelCells(IEnumerable<Cell> cells)
        {
            return Cells.Where(cell => cells.Any(rec => cell.Cell == rec)).ToList();
        }

        private static void MarkCells(IEnumerable<CellViewModel> cellViewModels)
        {
            foreach (var cellViewModel in cellViewModels)
            {
                cellViewModel.IsMarked = true;
            }
        }

        private void PawnFigureNearBoard(object source, PawnFigureTransformationEventArgs args)
        {
            if (!_makeTurnFromInputController)
            {
                PawnTransformationViewModel = new PawnTransformationViewModel(IsWhiteGo, this.Cells.First(rec => rec.Cell == args.Cell));
                PawnTransformationViewModel.PawnTransformationDoneEvent += SendTurnToInputController;
                _isPawnTransformation = true;
                RaisePropertyChanged(nameof(PawnTransformationViewModel));
            }
        }

        private void SendTurnToInputController(object o, EventArgs args = null)
        {
            CellViewModel toCell = (CellViewModel)o;

            var uiThread = Thread.CurrentThread;
            ThreadStart start = delegate ()
                                    {
                                        try
                                        {
                                            _inputController.SendInput(_selectedCell.Cell, toCell.Cell, _isPawnTransformation ? toCell.Figure : null);
                                        }
                                        catch (Exception exception)
                                        {
                                            Dispatcher uiDispatcher = Dispatcher.FromThread(uiThread) ?? Dispatcher.CurrentDispatcher;

                                            Action act = delegate ()
                                                             {
                                                                 throw exception;
                                                             };

                                            uiDispatcher.Invoke(DispatcherPriority.Normal, act);
                                        }

                                        //Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                                        //                                    new Action<Cell, Cell, Figure>(
                                        //                                        _inputController.SendInput),
                                        //                                    _selectedCell.Cell, toCell.Cell,
                                        //                                    _isPawnTransformation ? toCell.Figure : null);
                                    };

            (new Thread(start)).Start();

            _isPawnTransformation = false;
        }

        private void IsWaitingChangedEventExecute(object sender, EventArgs e)
        {
            //Dispatcher uiDispatcher = Dispatcher.FromThread(uiThread) ?? Dispatcher.CurrentDispatcher;
            //Action act = () => RaisePropertyChanged("IsWaiting");
            //uiDispatcher.Invoke(DispatcherPriority.Normal, act);

            RaisePropertyChanged(nameof(IsWaiting));
        }
        #endregion
    }
}
