using WpfChess.ChessModel.Figures;

namespace WpfChess.ChessModel
{
    public sealed class Cell
    {
        #region Properties
        public int HorPosition { get; private set; }
        public int VertPosition { get; private set; }

        public Figure Figure { get; set; }
        #endregion

        #region Constructors
        public Cell(int horPosition, int vertPosition)
        {
            VertPosition = vertPosition;
            HorPosition = horPosition;
        }
        #endregion

        #region Equals operations
        public bool Equals(Cell other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.HorPosition == HorPosition && other.VertPosition == VertPosition;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Cell)) return false;
            return Equals((Cell) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (HorPosition*397) ^ VertPosition;
            }
        }

        public static bool operator ==(Cell left, Cell right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
