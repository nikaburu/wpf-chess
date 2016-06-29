using System;
using System.IO;
using System.Xml.Serialization;

namespace WpfChess.WpfPresentation.Model
{
    public enum GameMode
    {
        OnePcMode, AiMode, NetworkMode
    }

    [SerializableAttribute]
    public struct GameConfigData
    {
        #region Properties
        public string ServiceUrl { get; set; }
        public string AdUrl { get; set; }
        public string EndpointAddress { get; set; }
        public bool IsFirstGo { get; set; }
        public GameMode GameMode { get; set; }
        #endregion

        public GameConfigData(string serviceUrl, string adUrl, string endpointAddress, bool isFirstGo, GameMode gameMode) : this()
        {
            ServiceUrl = serviceUrl;
            AdUrl = adUrl;
            EndpointAddress = endpointAddress;
            IsFirstGo = isFirstGo;
            GameMode = gameMode;
        }

        public static GameConfigData GetFromFile(Stream fileStream)
        {
            GameConfigData data;
            XmlSerializer serializer = new XmlSerializer(typeof(GameConfigData));
            using (StreamReader file = new StreamReader(fileStream))
            {
                data = (GameConfigData)serializer.Deserialize(file);
            }

            return data;
        }

        public static void SetToFile(Stream fileStream, GameConfigData data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameConfigData));
            using (StreamWriter file = new StreamWriter(fileStream))
            {
                serializer.Serialize(file, data);
            }
        }
    }
}
