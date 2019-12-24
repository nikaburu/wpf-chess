using System;

namespace WpfChess.WpfPresentation.Model
{
    [SerializableAttribute]
    public class GameConfig
    {
        public bool IsFirstGo { get; set; }
        public GameMode GameMode { get; set; }

        public GameConfig(bool isFirstGo, GameMode gameMode)
        {
            IsFirstGo = isFirstGo;
            GameMode = gameMode;
        }
    }
}
