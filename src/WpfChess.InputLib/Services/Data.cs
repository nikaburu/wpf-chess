using System.Runtime.Serialization;

namespace WpfChess.InputLib.Services
{
    [DataContract]
    public class Data
    {
        [DataMember]
        public int CelHorPosition { get; set; }

        [DataMember]
        public int CellVertPosition { get; set; }

        [DataMember]
        public string CellFigureName { get; set; }
    }
}
