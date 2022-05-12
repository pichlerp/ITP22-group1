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

        // Wenn ein Bauer noch nicht bewegt wurde, dann kann dieser um zwei Felder bewegt werden. Das leere Feld, das dabei übersprungen wird, ist jetzt ein gültiges Ziel für gegnerische Bauern, als ob der Bauer dort stünde.
        public bool EnPassantPossible { get; set; }

        public PieceColor Color;

        public PieceType Type;

        public Square(int row, int col)
        {
            RowNum = row;
            ColNum = col;
            EnPassantPossible = false;
            Color = PieceColor.Empty;
        }
    }
}
