using System;

namespace ChessPiece
{
    [Serializable]
    public enum TEAM
    {
        RED_TEAM = 0,
        BLACK_TEAM = 1,
    }

    public static class Border
    {
        public const int LEFT = 0;
        public const int RIGHT = 8;
        public const int TOP = 9;
        public const int BOTTOM = 0;
        public const int PALACE_LEFT = 3;
        public const int PALACE_RIGHT = 5;
        public const int PALACE_TOP_RED = 2;
        public const int PALACE_TOP_BLACK = 7;
        public const int RIVERBANK_RED = 4;
        public const int RIVERBANK_BLACK = 5;
    }
}