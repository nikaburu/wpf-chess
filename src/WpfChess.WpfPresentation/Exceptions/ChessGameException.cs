using System;
using System.Runtime.Serialization;

namespace WpfChess.WpfPresentation.Exceptions
{
    public class ChessGameException : Exception
    {
        public ChessGameException()
        {
        }

        public ChessGameException(string message) : base(message)
        {
        }

        public ChessGameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ChessGameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
