using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_UI
{

    public enum PieceColor
    {
        Empty,
        White,
        Black
    }
    public enum PieceType
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    public class Square
    {
        public int RowNum { get; set; }

        public int ColNum { get; set; }
        public bool LegalMove { get; set; }

        public bool EnPassantPossible { get; set; }

        public bool Moved;

        public PieceColor Color;

        public PieceType Type;

        public bool Selected;

        public Square(int row, int col)
        {
            RowNum = row;
            ColNum = col;
            EnPassantPossible = false;
            Moved = false;
        }
        /*
        internal void RemovePiece()
        {
            this.Color = PieceColor.Empty;
        }

        internal void AddPiece(PieceColor color, PieceType type)
        {
            this.Color = color;
            this.Type = type;
        }
        */
    }
}
