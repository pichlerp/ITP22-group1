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
        // Bewertung des Spielbretts; hoher Wert -> Weiß ist im Vorteil, niedriger Wert -> Schwarz ist im Vorteil
        public int score;
        // Parameter, die für Suche und Spielende relevant sind
        public bool whiteCastled = false;
        public bool blackCastled = false;
        public bool whiteInCheck = false;
        public bool blackInCheck = false;
        public bool whiteLost = false;
        public bool blackLost = false;
        public bool stalemate = false;
        public static int moveCount;
        public int eCount;
        public static int eCountPast = 32;
        public static int moveCountSinceBeaten = 0;

        public Chess_UI.PieceType[,] Field = new Chess_UI.PieceType[8,8];

        public int MCSB1()
        {
            return moveCountSinceBeaten;
        }

        public int getECountPast()
        {
            return eCountPast;
        }

        public void falseFigure()
        {
            moveCount--;
            moveCountSinceBeaten--;
        }

        public void EmptyCountIncrease()
        {
            eCount++;
        }

        public void EmptyCountSave()
        {
            eCountPast = eCount;
            moveCountSinceBeaten = 0;

        }

        public bool FieldComprasion()
        {
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Field[i,j].ToString() != Squares[i,j].ToString())
	                {
                        //moveCount++;
                        return false;
	                }
                }
            }
            return true;
        }

        public Square[,] Squares { get; set; }

        public void MoveCount()
        {
            //int MoveCount1 = int.Parse(this.moveCount.ToString());
            moveCount++;
            moveCountSinceBeaten++;
            //turnCounter++;
            Console.WriteLine($"Counter: {moveCount}, {eCountPast}, {moveCountSinceBeaten}");
        }
        public Board(string FEN)
        {
            PositionFromFEN(FEN);
        }
        public int getScore()
        {
            return score;
        }
        public PieceType getPieceType(int x, int y)
        {
            return Squares[x, y].Type;
        }
        public PieceColor getColor(int x, int y)
        {
            return Squares[x, y].Color;
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
            string fenHalfmoveClock = moveCountSinceBeaten.ToString();
            this.halfmoveClock = Convert.ToInt32(fenHalfmoveClock);
            // 6. Substring: Nummer des aktuellen Zugs
            string fenTurnCounter = moveCount.ToString();
            this.turnCounter = Convert.ToInt32(fenTurnCounter);
        }

        // gibt Evaluirung für weiß zurück => eval für schwarz = -(eval weiß)
        int evalBoard()
        {
            int score = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (getColor(i, k) != PieceColor.Empty)
                    {
                        int val = pieceValue(getPieceType(i, k));
                        score += (getColor(i, k) == PieceColor.White) ? val : -val;
                    }
                }
            }
            return score;
        }

        int pieceValue(PieceType t)
        {
            switch (t)
            {
                case PieceType.Pawn: return 1;
                case PieceType.Bishop: return 3;
                case PieceType.Knight: return 3;
                case PieceType.Rook: return 5;
                case PieceType.Queen: return 9;
                case PieceType.King: return 100;
                default: return -1;
            }
        }
    }
}
