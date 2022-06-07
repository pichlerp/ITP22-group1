using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chess_UI
{
    public class Board
    {
        // Folgende Parameter bestimmen neben den Positionen der Spielfiguren die Spielsituation
        public int halfmoveClock;
        public PieceColor turnColor;
        public bool whiteCastlingLongPossible;
        public bool whiteCastlingShortPossible;
        public bool blackCastlingLongPossible;
        public bool blackCastlingShortPossible;
        public int turnCounter;
        public Point enPassantPosition;
        int score;

        public Square[,] Squares { get; set; }

        public Board(string FEN)
        {
            PositionFromFEN(FEN);
            score = evalBoard();
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
            if (fenCastling.Contains('K'))
            {
                this.whiteCastlingLongPossible = true;
            }
            if (fenCastling.Contains('k'))
            {
                this.blackCastlingLongPossible = true;
            }
            if (fenCastling.Contains('Q'))
            {
                this.whiteCastlingShortPossible = true;
            }
            if (fenCastling.Contains('q'))
            {
                this.blackCastlingShortPossible = true;
            }
            // 4. Substring: En passant Position
            // TODO
            // 5. Substring: Anzahl an Zügen seit Schlagen oder Bauernzug
            string fenHalfmoveClock = fen.Split(' ')[4];
            this.halfmoveClock= Convert.ToInt32(fenHalfmoveClock);
            // 6. Substring: Nummer des aktuellen Zugs
            string fenTurnCounter = fen.Split(' ')[5];
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
