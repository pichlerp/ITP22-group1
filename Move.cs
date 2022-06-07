using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_UI
{
    public enum MoveType
    {
        Default,
        PromotionQueen,
        PromotionRook,
        PromotionBishop,
        PromotionKnight
    }
    public struct Move
    {
        public readonly Point StartSquare;
        public readonly Point EndSquare;
        public readonly MoveType MoveType;
        public Move(int startX, int startY, int endX, int endY)
        {
            StartSquare = new Point(startX, startY);
            EndSquare = new Point(endX, endY);
            MoveType = MoveType.Default;
        }
        public Move(int startX, int startY, int endX, int endY, MoveType Type)
        {
            StartSquare = new Point(startX, startY);
            EndSquare = new Point(endX, endY);
            MoveType = Type;
        }
    }
}
