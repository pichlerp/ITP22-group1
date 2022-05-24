using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_UI
{
    public struct Move
    {
        public readonly Point StartSquare;
        public readonly Point EndSquare;
        public Move(int startX, int startY, int endX, int endY)
        {
            StartSquare = new Point(startX, startY);
            EndSquare = new Point(endX, endY); ;
        }
    }
}
