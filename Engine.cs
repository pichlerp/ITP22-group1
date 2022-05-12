using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_UI
{
    class Engine
    {
        // Spielaufbau - online Editor mit FEN https://lichess.org/editor

        // Startposition
        static readonly string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        // Tests für Rochade
        //static readonly string FEN = "r2qk2r/8/8/8/8/8/8/R2QK2R w KQkq - 0 1"; // alle vier Möglichkeiten
        //static readonly string FEN = "r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1"; // selbe Position, aber Rochaden nicht mehr erlaubt
        //static readonly string FEN = "4k2r/6r1/8/8/8/8/3R4/R3K3 w Qk - 0 1"; // weiß lange Rochade, schwarz kurze Rochade

        // Tests für Beförderung
        //static readonly string FEN = "8/8/8/4p1K1/3k1P2/8/8/8 b - - 0 1";

        // Tests für En Passant
        //static readonly string FEN = "rnbqkbnr/ppppp1pp/8/2P5/5p2/8/PP1PPPPP/RNBQKBNR w KQkq - 0 1";

        static readonly Board TheBoard = new Board(FEN);

        internal bool IsValidMove(int startX, int startY, int endX, int endY)
        {
            foreach (Move move in moves)
            {
                if (move.StartSquare.X == startX && move.StartSquare.Y == startY && move.EndSquare.X == endX && move.EndSquare.Y == endY)
                {
                    return true;
                }
            }
            return false;
        }

        internal List<Point> GetPossibleMoves(int rank, int file, List<Move> allMoves, ref List<Point> legalMoves)
        {
            foreach (Move move in allMoves)
            {
                if (move.StartSquare.X == rank && move.StartSquare.Y == file)
                {
                    Point p = new Point(move.EndSquare.X, move.EndSquare.Y);
                    legalMoves.Add(p);
                }
            }
            return legalMoves;
        }

        internal bool ValidColorSelected(int rank, int file)
        {
            PieceColor color = TheBoard.Squares[rank, file].Color;
            if (color == TheBoard.turnColor)
            {
                return true;
            }
            return false;
        }

        internal void GetTheBoard()
        {
            PrintBoard(TheBoard);
        }

        internal string FromPositionCreateFEN()
        {
            string fen = "";

            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    int emptyCounter = 0;

                    while (TheBoard.Squares[i, j].Color == PieceColor.Empty)
                    {
                        emptyCounter++;
                        if (j == 7)
                        {
                            break;
                        }
                        j++;
                    }
                    if (emptyCounter != 0)
                    {
                        fen += emptyCounter.ToString();
                    }

                    if (TheBoard.Squares[i, j].Color != PieceColor.Empty)
                    {
                        PieceType pieceType = TheBoard.Squares[i, j].Type;
                        char symbol;

                        var symbolFromType = new Dictionary<PieceType, char>()
                        {
                            [PieceType.King] = 'k',
                            [PieceType.Queen] = 'q',
                            [PieceType.Rook] = 'r',
                            [PieceType.Knight] = 'n',
                            [PieceType.Bishop] = 'b',
                            [PieceType.Pawn] = 'p',
                        };
                        if (TheBoard.Squares[i, j].Color == PieceColor.White)
                        {
                            symbol = char.ToUpper(symbolFromType[pieceType]);
                        }
                        else
                        {
                            symbol = symbolFromType[pieceType];
                        }
                        fen += symbol;
                    }
                    if (j == 7 && i != 0)
                    {
                        fen += '/';
                    }
                }
            }
            return fen;
        }

        internal void MakeMove(int startX, int startY, int endX, int endY)
        {
            if (TheBoard.Squares[endX, endY].EnPassantPossible && TheBoard.Squares[startX, startY].Type == PieceType.Pawn)
            {
                if (TheBoard.Squares[startX, startY].Color == PieceColor.White)
                {
                    TheBoard.Squares[endX - 1, endY].Color = PieceColor.Empty;
                }
                else
                {
                    TheBoard.Squares[endX + 1, endY].Color = PieceColor.Empty;
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    TheBoard.Squares[i, j].EnPassantPossible = false;
                }
            }

            if (startX == 0 && startY == 4)
            {
                TheBoard.whiteCastlingLongPossible = false;
                TheBoard.whiteCastlingShortPossible = false;
            }
            else if (startX == 7 && startY == 4)
            {
                TheBoard.blackCastlingLongPossible = false;
                TheBoard.blackCastlingShortPossible = false;
            }
            else if (startX == 0 && startY == 0)
            {
                TheBoard.whiteCastlingLongPossible = false;
            }
            else if (startX == 0 && startY == 7)
            {
                TheBoard.whiteCastlingShortPossible = false;
            }
            else if (startX == 7 && startY == 0)
            {
                TheBoard.blackCastlingLongPossible = false;
            }
            else if (startX == 7 && startY == 7)
            {
                TheBoard.blackCastlingShortPossible = false;
            }

            bool special = false;
            // En passant wird eventuell möglich - Flag wird gesetzt
            if (TheBoard.Squares[startX, startY].Type == PieceType.Pawn && (startX - endX) * (startX - endX) == 4)
            {
                if (TheBoard.Squares[startX, startY].Color == PieceColor.White)
                {
                    TheBoard.Squares[startX + 1, startY].EnPassantPossible = true;
                }
                else
                {
                    TheBoard.Squares[startX - 1, startY].EnPassantPossible = true;
                }
            }
            // Beförderung von Bauern
            if (TheBoard.Squares[startX, startY].Type == PieceType.Pawn && (endX == 0 || endX == 7))
            {
                special = true;
                Console.WriteLine("Bauer wird befördert. Figur auswählen (q, r, b, n): ");
                string input = Console.ReadLine();
                while (!(input[0] == 'q' || input[0] == 'r' || input[0] == 'b' || input[0] == 'n'))
                {
                    Console.WriteLine("Eingabe ungültig. Figur auswählen (q, r, b, n): ");
                    input = Console.ReadLine();
                }
                if (input[0] == 'q')
                {
                    TheBoard.Squares[endX, endY].Type = PieceType.Queen;
                }
                else if (input[0] == 'r')
                {
                    TheBoard.Squares[endX, endY].Type = PieceType.Rook;
                }
                else if (input[0] == 'b')
                {
                    TheBoard.Squares[endX, endY].Type = PieceType.Bishop;
                }
                else
                {
                    TheBoard.Squares[endX, endY].Type = PieceType.Knight;
                }
            }
            // Rochade
            if (TheBoard.Squares[startX, startY].Type == PieceType.King)
            {
                // Lange Rochade weiß
                if (endX == 0 && endY == 2)
                {
                    TheBoard.Squares[0, 3].Type = PieceType.Rook;
                    TheBoard.Squares[0, 3].Color = PieceColor.White;
                    TheBoard.Squares[0, 0].Color = PieceColor.Empty;
                }
                // Kurze Rochade weiß
                else if (endX == 0 && endY == 6)
                {
                    TheBoard.Squares[0, 5].Type = PieceType.Rook;
                    TheBoard.Squares[0, 5].Color = PieceColor.White;
                    TheBoard.Squares[0, 7].Color = PieceColor.Empty;
                }
                // Lange Rochade schwarz
                else if (endX == 7 && endY == 2)
                {
                    TheBoard.Squares[7, 3].Type = PieceType.Rook;
                    TheBoard.Squares[7, 3].Color = PieceColor.Black;
                    TheBoard.Squares[7, 0].Color = PieceColor.Empty;
                }
                // Kurze Rochade schwarz
                else if (endX == 7 && endY == 6)
                {
                    TheBoard.Squares[7, 5].Type = PieceType.Rook;
                    TheBoard.Squares[7, 5].Color = PieceColor.Black;
                    TheBoard.Squares[7, 7].Color = PieceColor.Empty;
                }
            }
            PieceColor color = TheBoard.Squares[startX, startY].Color;
            PieceType type = TheBoard.Squares[startX, startY].Type;
            TheBoard.Squares[endX, endY].Color = color;
            if (!special)
            {
                TheBoard.Squares[endX, endY].Type = type;
            }
            TheBoard.Squares[startX, startY].Color = PieceColor.Empty;
            if (TheBoard.turnColor == PieceColor.White)
            {
                TheBoard.turnColor = PieceColor.Black;
            }
            else
            {
                TheBoard.turnColor = PieceColor.White;
            }
        }

        public static void PrintBoard(Board B)
        {
            Console.WriteLine("");
            Console.Write("   ");
            for (int i = 7; i >= 0; i--)
            {
                if (i == 0)
                {
                    Console.Write("h ");
                }
                else if (i == 1)
                {
                    Console.Write("g ");
                }
                else if (i == 2)
                {
                    Console.Write("f ");
                }
                else if (i == 3)
                {
                    Console.Write("e ");
                }
                else if (i == 4)
                {
                    Console.Write("d ");
                }
                else if (i == 5)
                {
                    Console.Write("c ");
                }
                else if (i == 6)
                {
                    Console.Write("b ");
                }
                else if (i == 7)
                {
                    Console.Write("a ");
                }
            }
            Console.WriteLine("");
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (j == 0)
                    {
                        Console.Write(" ");
                        Console.Write(i + 1 + " ");
                    }
                    if (B.Squares[i, j].Color != PieceColor.Empty)
                    {
                        if (B.Squares[i, j].Color == PieceColor.White)
                        {
                            switch (B.Squares[i, j].Type)
                            {
                                case PieceType.King:
                                    { Console.Write("K "); }
                                    break;
                                case PieceType.Queen:
                                    { Console.Write("Q "); }
                                    break;
                                case PieceType.Rook:
                                    { Console.Write("R "); }
                                    break;
                                case PieceType.Bishop:
                                    { Console.Write("B "); }
                                    break;
                                case PieceType.Knight:
                                    { Console.Write("N "); }
                                    break;
                                case PieceType.Pawn:
                                    { Console.Write("p "); }
                                    break;
                            }
                        }
                        if (B.Squares[i, j].Color == PieceColor.Black)
                        {
                            switch (B.Squares[i, j].Type)
                            {
                                case PieceType.King:
                                    { Console.Write("k "); }
                                    break;
                                case PieceType.Queen:
                                    { Console.Write("q "); }
                                    break;
                                case PieceType.Rook:
                                    { Console.Write("r "); }
                                    break;
                                case PieceType.Bishop:
                                    { Console.Write("b "); }
                                    break;
                                case PieceType.Knight:
                                    { Console.Write("n "); }
                                    break;
                                case PieceType.Pawn:
                                    { Console.Write("p "); }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Console.Write("- ");
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

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

        public List<Move> moves;
        public List<Move> GenerateMoves()
        {
            moves = new List<Move>();
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    if (TheBoard.Squares[file, rank].Color != PieceColor.Empty)
                    {
                        Point start = new Point(file, rank);
                        GeneratePieceMove(start, TheBoard.Squares[file, rank].Color, TheBoard.Squares[file, rank].Type);
                    }
                }
            }
            return moves;
        }

        private void GeneratePieceMove(Point start, PieceColor color, PieceType type)
        {
            // Liste von allen möglichen Zügen, die teilweise out of bounds sind -> diese werden nicht in die Liste der gültigen Züge übernommen
            List<Move> potentialMoves = new List<Move>();

            if (type == PieceType.Rook)
            {
                for (int i = 1; (start.X + i) < 8; i++)
                {
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y));
                    if (TheBoard.Squares[start.X + i, start.Y].Color == PieceColor.White || TheBoard.Squares[start.X + i, start.Y].Color == PieceColor.Black)
                    {
                        break;
                    }
                }
                for (int i = 1; (start.X - i) >= 0; i++)
                {
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y));
                    if (TheBoard.Squares[start.X - i, start.Y].Color == PieceColor.White || TheBoard.Squares[start.X - i, start.Y].Color == PieceColor.Black)
                    {
                        break;
                    }
                }
                for (int i = 1; (start.Y + i) < 8; i++)
                {
                    potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y + i));
                    if (TheBoard.Squares[start.X, start.Y + i].Color == PieceColor.White || TheBoard.Squares[start.X, start.Y + i].Color == PieceColor.Black)
                    {
                        break;
                    }
                }
                for (int i = 1; (start.Y - i) >= 0; i++)
                {
                    potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y - i));
                    if (TheBoard.Squares[start.X, start.Y - i].Color == PieceColor.White || TheBoard.Squares[start.X, start.Y - i].Color == PieceColor.Black)
                    {
                        break;
                    }
                }
            }
            if (type == PieceType.Bishop)
            {
                for (int i = 1; ((start.X + i) < 8 && (start.Y + i) < 8); i++)
                {
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y + i));
                    if (TheBoard.Squares[start.X + i, start.Y + i].Color == PieceColor.White || TheBoard.Squares[start.X + i, start.Y + i].Color == PieceColor.Black)
                    {
                        break;
                    }
                }
                for (int i = 1; ((start.X + i) < 8 && (start.Y - i) >= 0); i++)
                {
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y - i));
                    if (TheBoard.Squares[start.X + i, start.Y - i].Color == PieceColor.White || TheBoard.Squares[start.X + i, start.Y - i].Color == PieceColor.Black)
                    {
                        break;
                    }
                }
                for (int i = 1; ((start.X - i) >= 0 && (start.Y + i) < 8); i++)
                {
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y + i));
                    if (TheBoard.Squares[start.X - i, start.Y + i].Color == PieceColor.White || TheBoard.Squares[start.X - i, start.Y + i].Color == PieceColor.Black)
                    {
                        break;
                    }
                }
                for (int i = 1; ((start.X - i) >= 0 && (start.Y - i) >= 0); i++)
                {
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y - i));
                    if (TheBoard.Squares[start.X - i, start.Y - i].Color == PieceColor.White || TheBoard.Squares[start.X - i, start.Y - i].Color == PieceColor.Black)
                    {
                        break;
                    }
                }
            }
            if (type == PieceType.Queen)
            {
                GeneratePieceMove(start, color, PieceType.Rook);
                GeneratePieceMove(start, color, PieceType.Bishop);
            }
            if (type == PieceType.Pawn)
            {
                if (color == PieceColor.White)
                {
                    if (TheBoard.Squares[start.X + 1, start.Y].Color != PieceColor.White && TheBoard.Squares[start.X + 1, start.Y].Color != PieceColor.Black)
                    {
                        potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y));

                        if (start.X == 1)
                        {
                            if (TheBoard.Squares[start.X + 2, start.Y].Color != PieceColor.White && TheBoard.Squares[start.X + 2, start.Y].Color != PieceColor.Black)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 2, start.Y));
                            }
                        }
                    }
                    if (start.Y + 1 < 8)
                    {
                        if (TheBoard.Squares[start.X + 1, start.Y + 1].Color == PieceColor.White || TheBoard.Squares[start.X + 1, start.Y + 1].Color == PieceColor.Black)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1));
                        }
                        if (TheBoard.Squares[start.X + 1, start.Y + 1].EnPassantPossible)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1));

                        }
                    }
                    if (start.Y - 1 >= 0)
                    {
                        if (TheBoard.Squares[start.X + 1, start.Y - 1].Color == PieceColor.White || TheBoard.Squares[start.X + 1, start.Y - 1].Color == PieceColor.Black)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1));
                        }
                        if (TheBoard.Squares[start.X + 1, start.Y - 1].EnPassantPossible)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1));

                        }
                    }

                }
                else if (color == PieceColor.Black)
                {
                    if (TheBoard.Squares[start.X - 1, start.Y].Color != PieceColor.White && TheBoard.Squares[start.X - 1, start.Y].Color != PieceColor.Black)
                    {
                        potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y));

                        if (start.X == 6)
                        {
                            if (TheBoard.Squares[start.X - 2, start.Y].Color != PieceColor.White && TheBoard.Squares[start.X - 2, start.Y].Color != PieceColor.Black)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 2, start.Y));
                            }

                        }
                    }
                    if (start.Y + 1 < 8)
                    {
                        if (TheBoard.Squares[start.X - 1, start.Y + 1].Color == PieceColor.White || TheBoard.Squares[start.X - 1, start.Y + 1].Color == PieceColor.Black)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1));
                        }
                        if (TheBoard.Squares[start.X - 1, start.Y + 1].EnPassantPossible)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1));

                        }
                    }
                    if (start.Y - 1 >= 0)
                    {
                        if (TheBoard.Squares[start.X - 1, start.Y - 1].Color == PieceColor.White || TheBoard.Squares[start.X - 1, start.Y - 1].Color == PieceColor.Black)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1));
                        }
                        if (TheBoard.Squares[start.X - 1, start.Y - 1].EnPassantPossible)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1));

                        }
                    }
                }
            }


            switch (type)
            {
                case PieceType.King:
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y + 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y - 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1));
                    // Rochade
                    if (color == PieceColor.White)
                    {
                        // Lange Rochade weiß
                        if (TheBoard.whiteCastlingLongPossible)
                        {
                            if(TheBoard.Squares[0, 1].Color == PieceColor.Empty && TheBoard.Squares[0, 2].Color == PieceColor.Empty && TheBoard.Squares[0, 3].Color == PieceColor.Empty)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y - 2));
                            }                                
                        }
                        // Kurze Rochade weiß
                        if (TheBoard.whiteCastlingShortPossible)
                        {
                            if (TheBoard.Squares[0, 6].Color == PieceColor.Empty && TheBoard.Squares[0, 5].Color == PieceColor.Empty)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y + 2));
                            }                            
                        }
                    }
                    else if (color == PieceColor.Black)
                    {
                        // Lange Rochade schwarz
                        if (TheBoard.blackCastlingLongPossible)
                        {
                            if (TheBoard.Squares[7, 1].Color == PieceColor.Empty && TheBoard.Squares[7, 2].Color == PieceColor.Empty && TheBoard.Squares[7, 3].Color == PieceColor.Empty)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y - 2));
                            }                            
                        }
                        // Kurze Rochade schwarz
                        if (TheBoard.blackCastlingShortPossible)
                        {
                            if (TheBoard.Squares[7, 6].Color == PieceColor.Empty && TheBoard.Squares[7, 5].Color == PieceColor.Empty)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y + 2));
                            }
                        }
                    }
                    break;

                case PieceType.Knight:
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + 2, start.Y + 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + 2, start.Y - 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - 2, start.Y + 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - 2, start.Y - 1));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 2));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 2));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 2));
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 2));
                    break;

            }
            // Nur die Züge, die innerhalb des Bretts bleiben, werden an moves angehängt
            foreach (Move move in potentialMoves)
            {
                if (!(move.EndSquare.X < 0 || move.EndSquare.Y < 0 || move.EndSquare.X > 7 || move.EndSquare.Y > 7))
                {
                    int x, y;
                    x = move.StartSquare.X;
                    y = move.StartSquare.Y;
                    if (TheBoard.Squares[x, y].Color != TheBoard.Squares[move.EndSquare.X, move.EndSquare.Y].Color)
                    {
                        moves.Add(move);
                    }
                }
            }
        }
    }
}
