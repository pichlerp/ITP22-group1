using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chess_UI
{
    public class Board
    {
        // Entweder Weiß oder Schwarz ist am Zug
        public PieceColor turnColor;
        // Zu Beginn des Spiels sind alle Formen der Rochade erlaubt; im Verlauf des Spiels werden einzelne Möglichkeiten ungültig
        public bool whiteCastlingLongPossible;
        public bool whiteCastlingShortPossible;
        public bool blackCastlingLongPossible;
        public bool blackCastlingShortPossible;
        // Ein Zug besteht aus einem Halbzug von Weiß und einem Halbzug von Schwarz; sobald Schwarz eine Figur bewegt hat, endet der Zug
        public int turnCounter;
        // Für Remis werden Halbzüge gezählt, bei denen weder eine Figur geschlagen, noch ein Bauer bewegt wird; ansonsten wird Zähler zurückgesetzt
        public int halfmoveClock;
        // Wenn ein Bauer noch nicht bewegt wurde, dann kann dieser um zwei Felder bewegt werden. Das leere Feld, das dabei übersprungen wird, ist jetzt ein gültiges Ziel für gegnerische Bauern, als ob der Bauer dort stünde.
        public Point enPassantPosition;

        public Square[,] Squares { get; set; }

        public Board(string FEN)
        {
            PositionFromFEN(FEN);
        }
        // Forsyth–Edwards-Notation: String beschreibt Spielsituation komplett
        // Beispiel Startposition: rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
        public void PositionFromFEN(string fen)
        {
            Squares = new Square[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares[i, j] = new Square(i, j);
                }
            }
            // 1. Substring: Position der Figuren
            var typeFromSymbol = new Dictionary<char, PieceType>()
            {
                ['k'] = PieceType.King,
                ['q'] = PieceType.Queen,
                ['r'] = PieceType.Rook,
                ['n'] = PieceType.Knight,
                ['b'] = PieceType.Bishop,
                ['p'] = PieceType.Pawn
            };
            int file = 0;
            int rank = 7;
            string piecePlacement = fen.Split(' ')[0];
            foreach (char symbol in piecePlacement)
            {
                if (symbol == '/')
                {
                    file = 0;
                    rank--;
                }
                else
                {
                    if (char.IsDigit(symbol))
                    {
                        file += (int)char.GetNumericValue(symbol);
                    }
                    else
                    {
                        PieceColor color;
                        if (char.IsUpper(symbol))
                        {
                            color = PieceColor.White; ;
                        }
                        else
                        {
                            color = PieceColor.Black;
                        }
                        PieceType type = typeFromSymbol[char.ToLower(symbol)];
                        this.Squares[rank, file].Type = type;
                        this.Squares[rank, file].Color = color;
                        file++;
                    }
                }
            }
            // 2. Substring: Farbe, die am Zug ist
            string fenColor = fen.Split(' ')[1];
            if (fenColor == "w")
            {
                this.turnColor = PieceColor.White;
            }
            else
            {
                this.turnColor = PieceColor.Black;
            }
            // 3. Substring: Übrige Möglichkeiten der Rochade
            string fenCastling = fen.Split(' ')[2];
            this.whiteCastlingLongPossible = false;
            this.blackCastlingLongPossible = false;
            this.whiteCastlingShortPossible = false;
            this.blackCastlingShortPossible = false;
            if (fenCastling.Contains('Q'))
            {
                this.whiteCastlingLongPossible = true;
            }
            if (fenCastling.Contains('q'))
            {
                this.blackCastlingLongPossible = true;
            }
            if (fenCastling.Contains('K'))
            {
                this.whiteCastlingShortPossible = true;
            }
            if (fenCastling.Contains('k'))
            {
                this.blackCastlingShortPossible = true;
            }
            // 4. Substring: En passant Position in algebraischer Notation
            string fenEnPassant = fen.Split(' ')[3];
            if (fenEnPassant == "-")
            {
                enPassantPosition.X = enPassantPosition.Y = -1;
            }
            else
            {
                int row = (int)fenEnPassant[0] - 97;
                int col = (int)fenEnPassant[1] - 49;
                enPassantPosition.X = row;
                enPassantPosition.Y = col;
            }
            // 5. Substring: Anzahl an Zügen seit Schlagen oder Bauernzug
            string fenHalfmoveClock = fen.Split(' ')[4];
            this.halfmoveClock = Convert.ToInt32(fenHalfmoveClock);
            // 6. Substring: Nummer des aktuellen Zugs
            string fenTurnCounter = fen.Split(' ')[5];
            this.turnCounter = Convert.ToInt32(fenTurnCounter);
        }
    }
}
