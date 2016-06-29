using WpfChess.ChessModel;
using WpfChess.ChessModel.Figures;

namespace WpfChess.InputLib.Input
{
    public sealed class OnePcInputController : InputController
    {
        public OnePcInputController()
            : base(false)
        {

        }

        public override void SendInput(Cell fromCell, Cell toCell, Figure figure = null)
        {
            IsWaiting = false;
        }
    }
}
