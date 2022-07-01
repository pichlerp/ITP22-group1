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
        //bool locked = false;

        public List<Move> movesPlayerColor;
        // Spielaufbau - online Editor mit FEN https://lichess.org/editor

        // Startposition
        static readonly string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        // PERFT siehe https://www.chessprogramming.org/Perft_Results
        // PERFT 3: 97898 statt 97862 (Stockfish) -> Bauer bedroht Feld, durch das König bei Rochade zieht
        // static readonly string FEN = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";

        // static readonly string FEN = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/P1N2Q2/1PPBBPpP/R3K2R w KQkq - 0 1";

        // static readonly string FEN = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
        // static readonly string FEN = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
        // static readonly string FEN = "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10";

        // PERFT
        // static readonly string FEN = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
        // static readonly string FEN = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
        // static readonly string FEN = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";       

        // Test für Schachmatt
        // static readonly string FEN = "r2q4/8/8/8/8/8/8/4K3 w - - 0 1";
        // static readonly string FEN = "rk6/ppp4p/6p1/5p2/8/1P1R1NP1/PBPPPPBP/1K6 w - - 0 1";
        // static readonly string FEN = "1k6/ppp5/8/8/3r4/7R/4r3/1K6 w - - 0 1";
        


        // Test für Patt
        // static readonly string FEN = "1k6/3R4/8/5Q2/8/2R5/8/4K3 w - - 0 1"; 

        // Tests für Rochade
        // PERFT 4: 1288070 statt 1288065 (Stockfish)
        // static readonly string FEN = "r2qk2r/8/8/8/8/8/8/R2QK2R w KQkq - 0 1"; // alle vier Möglichkeiten

        // static readonly string FEN = "r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1"; // selbe Position, aber Rochaden nicht mehr erlaubt
        // static readonly string FEN = "4k2r/6r1/8/8/8/8/3R4/R3K3 w Qk - 0 1"; // weiß lange Rochade, schwarz kurze Rochade

        // Tests für Beförderung
        // static readonly string FEN = "8/8/8/4p1K1/3k1P2/8/8/8 b - - 0 1";

        // static readonly string FEN = "8/6KP/8/8/8/8/pk6/8 w - - 0 1";

        // Tests für En Passant
        // static readonly string FEN = "rnbqkbnr/ppppp1pp/8/2P5/5p2/8/PP1PPPPP/RNBQKBNR w KQkq - 0 1"; // kein En Passant in diesem Zug möglich, aber viele Varianten, die zu En Passant führen
        // static readonly string FEN = "rnbqkbnr/ppppp1pp/8/2P5/5pP1/8/PP1PPP1P/RNBQKBNR b KQkq g3 0 1"; // selbe Position, aber FEN gibt jetzt g3 als legales Ziel für En Passant an -> Bauer auf f4 hat direkt Möglichkeit für En Passant

        Board TheBoard = new Board(FEN);

        public Board Board()
        {
            return TheBoard;
        }

        public void setBoardFromFEN(string fen)
        {
            TheBoard = new Board(fen);
        }

        /*
        public void lockMoves()
        {
            locked = true;
        }
        public void unlockMoves()
        {
            locked = false;
        }
        */

        internal bool IsValidMove(int startX, int startY, int endX, int endY)
        {
            foreach (Move move in movesBothColors)
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

        internal PieceColor GetTurnColor()
        {
            return TheBoard.turnColor;
        }

        internal void GetTheBoard()
        {
            PrintBoard(TheBoard);
        }

        internal string FromPositionCreateFEN()
        {
            // 1. Substring: Position der Figuren 
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
            // 2. Substring: Farbe, die am Zug ist
            if (TheBoard.turnColor == PieceColor.White)
            {
                fen += " w";
            }
            else
            {
                fen += " b";
            }
            // 3. Substring: Übrige Möglichkeiten der Rochade
            fen += " ";
            if (TheBoard.whiteCastlingLongPossible)
            {
                fen += "Q";
            }
            if (TheBoard.whiteCastlingShortPossible)
            {
                fen += "K";
            }
            if (TheBoard.blackCastlingLongPossible)
            {
                fen += "q";
            }
            if (TheBoard.blackCastlingShortPossible)
            {
                fen += "k";
            }
            if (!(TheBoard.whiteCastlingLongPossible || TheBoard.whiteCastlingShortPossible || TheBoard.blackCastlingLongPossible || TheBoard.blackCastlingShortPossible))
            {
                fen += "-";
            }
            // 4. Substring: En passant Position in algebraischer Notation
            fen += " ";
            if (TheBoard.enPassantPosition.X == -1 && TheBoard.enPassantPosition.Y == -1)
            {
                fen += "-";
            }
            else
            {
                char rank = (char)(TheBoard.enPassantPosition.X + 97);
                char file = (char)(TheBoard.enPassantPosition.Y + 49);
                fen += rank.ToString() + file.ToString();
            }
            // 5. Substring: Anzahl an Zügen seit Schlagen oder Bauernzug
            fen += (" " + TheBoard.halfmoveClock.ToString());
            // 6. Substring: Nummer des aktuellen Zugs
            fen += (" " + TheBoard.turnCounter.ToString());
            //Console.WriteLine("Position converted to " + fen);
            return fen;
        }

        internal void IncrementTurncounter()
        {
            TheBoard.turnCounter++;
        }

        public void MakeMove(Move move)
        {
            MakeMove(move.StartSquare.X, move.StartSquare.Y, move.EndSquare.X, move.EndSquare.Y, move.MoveType);
        }
        internal bool LegalMovesExist(List<Move> moves)
        {
            if (moves.Count == 0)
            {
                return false;
            }
            return true;
        }

        internal bool KingInCheck(PieceColor opponentColor, List<Move> moves)
        {
            int kingX, kingY;
            kingX = kingY = -1;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (TheBoard.Squares[i, j].Color == opponentColor && TheBoard.Squares[i, j].Type == PieceType.King)
                    {
                        kingX = i;
                        kingY = j;
                    }
                }
            }

            foreach (Move move in moves)
            {
                if (move.EndSquare.X == kingX && move.EndSquare.Y == kingY)
                {
                    return true;
                }
            }

            return false;
        }

        internal void MakeMove(int startX, int startY, int endX, int endY, MoveType Type)
        {
            bool promotion = false;

            // En passant: Bauer schlägt Bauer hinter sich TheBoard.Squares[endX, endY].EnPassantPossible 
            if (TheBoard.enPassantPosition.X == endY && TheBoard.enPassantPosition.Y == endX && TheBoard.Squares[startX, startY].Type == PieceType.Pawn)
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
            TheBoard.enPassantPosition.Y = -1;
            TheBoard.enPassantPosition.X = -1;

            // En passant wird eventuell möglich - Flag wird gesetzt
            if (TheBoard.Squares[startX, startY].Type == PieceType.Pawn && (startX - endX) * (startX - endX) == 4)
            {
                if (TheBoard.Squares[startX, startY].Color == PieceColor.White)
                {
                    TheBoard.enPassantPosition.Y = startX + 1;
                    TheBoard.enPassantPosition.X = startY;
                }
                else
                {
                    TheBoard.enPassantPosition.Y = startX - 1;
                    TheBoard.enPassantPosition.X = startY;
                }
            }
            else
            {
                TheBoard.enPassantPosition.X = TheBoard.enPassantPosition.Y = -1;
            }

            // Beförderung von Bauern
            if (TheBoard.Squares[startX, startY].Type == PieceType.Pawn && (endX == 0 || endX == 7))
            {
                promotion = true;
                string input;
                // Wenn kein spezieller MoveType angegeben wurde, dann handelt es sich um einen manuellen Zug und dementsprechend wird nachgefragt
                if (Type == MoveType.Default)
                {
                    Console.WriteLine("Bauer wird befördert. Figur auswählen (q, r, b, n): ");
                    input = Console.ReadLine();
                    while (input == "" || !(input[0] == 'q' || input[0] == 'r' || input[0] == 'b' || input[0] == 'n'))
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
                // Es wurde ein spezieller MoveType angegeben, der hier übersetzt wird
                else
                {
                    PieceType PromotionType = PieceType.Queen;
                    switch (Type)
                    {
                        case MoveType.PromotionQueen:
                            PromotionType = PieceType.Queen;
                            break;
                        case MoveType.PromotionRook:
                            PromotionType = PieceType.Rook;
                            break;
                        case MoveType.PromotionBishop:
                            PromotionType = PieceType.Bishop;
                            break;
                        case MoveType.PromotionKnight:
                            PromotionType = PieceType.Knight;
                            break;
                    }
                    TheBoard.Squares[endX, endY].Type = PromotionType;
                }

            }
            // Rochade
            if (TheBoard.Squares[startX, startY].Type == PieceType.King)
            {
                // Lange Rochade weiß
                if (endX == 0 && endY == 2 && TheBoard.whiteCastlingLongPossible)
                {
                    TheBoard.Squares[0, 3].Type = PieceType.Rook;
                    TheBoard.Squares[0, 3].Color = PieceColor.White;
                    TheBoard.Squares[0, 0].Color = PieceColor.Empty;
                    TheBoard.whiteCastled = true;
                }
                // Kurze Rochade weiß
                else if (endX == 0 && endY == 6 && TheBoard.whiteCastlingShortPossible)
                {
                    TheBoard.Squares[0, 5].Type = PieceType.Rook;
                    TheBoard.Squares[0, 5].Color = PieceColor.White;
                    TheBoard.Squares[0, 7].Color = PieceColor.Empty;
                    TheBoard.whiteCastled = true;
                }
                // Lange Rochade schwarz
                else if (endX == 7 && endY == 2 && TheBoard.blackCastlingLongPossible)
                {
                    TheBoard.Squares[7, 3].Type = PieceType.Rook;
                    TheBoard.Squares[7, 3].Color = PieceColor.Black;
                    TheBoard.Squares[7, 0].Color = PieceColor.Empty;
                    TheBoard.blackCastled = true;
                }
                // Kurze Rochade schwarz
                else if (endX == 7 && endY == 6 && TheBoard.blackCastlingShortPossible)
                {
                    TheBoard.Squares[7, 5].Type = PieceType.Rook;
                    TheBoard.Squares[7, 5].Color = PieceColor.Black;
                    TheBoard.Squares[7, 7].Color = PieceColor.Empty;
                    TheBoard.blackCastled = true;
                }
            }
            // Weißer König wird bewegt -> Weiß verliert beide Rochaden
            if (startX == 0 && startY == 4)
            {
                TheBoard.whiteCastlingLongPossible = false;
                TheBoard.whiteCastlingShortPossible = false;
            }
            // Schwarzer König wird bewegt -> Schwarz verliert beide Rochaden
            else if (startX == 7 && startY == 4)
            {
                TheBoard.blackCastlingLongPossible = false;
                TheBoard.blackCastlingShortPossible = false;
            }
            // Weißer Turm (queenside) wird bewegt -> Weiß verliert lange R.
            else if (startX == 0 && startY == 0)
            {
                TheBoard.whiteCastlingLongPossible = false;
            }
            // Weißer Turm (kingside) wird bewegt -> Weiß verliert kurze R.
            else if (startX == 0 && startY == 7)
            {
                TheBoard.whiteCastlingShortPossible = false;
            }
            // Schwarzer Turm (queenside) wird bewegt -> Schwarz verliert lange R.
            else if (startX == 7 && startY == 0)
            {
                TheBoard.blackCastlingLongPossible = false;
            }
            // Schwarzer Turm (kingside) wird bewegt -> Schwarz verliert kurze R.
            else if (startX == 7 && startY == 7)
            {
                TheBoard.blackCastlingShortPossible = false;
            }
            // Farbe und Typ wird von Startfeld übernommen (bei Beförderung wurde Typ schon angepasst)
            PieceColor color = TheBoard.Squares[startX, startY].Color;
            PieceType type = TheBoard.Squares[startX, startY].Type;
            TheBoard.Squares[endX, endY].Color = color;
            if (!promotion)
            {
                TheBoard.Squares[endX, endY].Type = type;
            }
            // Ursprüngliches Feld wird leer
            TheBoard.Squares[startX, startY].Color = PieceColor.Empty;
            // Der Zug endet und gegnerischer Zug beginnt
            if (TheBoard.turnColor == PieceColor.White)
            {
                TheBoard.turnColor = PieceColor.Black;
            }
            else
            {
                TheBoard.turnColor = PieceColor.White;
            }
        }

        // Ausgabe der Spielsituation an der Konsole
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

        public List<Move> movesBothColors;
        public List<Move> movesAfter;

        public List<Move> GenerateMoves()
        {
            PieceColor color = GetTurnColor();

            string SaveState = FromPositionCreateFEN();

            movesBothColors = new List<Move>();
            movesAfter = new List<Move>();
            movesPlayerColor = new List<Move>();

            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    if (TheBoard.Squares[file, rank].Color != PieceColor.Empty)
                    {
                        Point start = new Point(file, rank);
                        GeneratePieceMove(start, TheBoard.Squares[file, rank].Color, TheBoard.Squares[file, rank].Type, movesBothColors);
                    }
                }
            }

            foreach (Move move in movesBothColors.ToList())
            {
                int startx, starty, endx, endy;
                startx = move.StartSquare.X;
                starty = move.StartSquare.Y;
                endx = move.EndSquare.X;
                endy = move.EndSquare.Y;
                if (TheBoard.Squares[startx, starty].Type == PieceType.King)
                {
                    if (TheBoard.Squares[startx, starty].Color == PieceColor.White)
                    {
                        if (startx == 0 && starty == 4)
                        {
                            if (endx == 0 && (endy == 2 || endy == 6))
                            {
                                if (KingInCheck(PieceColor.White, movesBothColors))
                                {
                                    movesBothColors.Remove(move);
                                    continue;
                                }
                                int endyCastling;
                                if (endy == 2)
                                {
                                    endyCastling = 3;
                                }
                                else
                                {
                                    endyCastling = 5;
                                }

                                foreach (Move move1 in movesBothColors)
                                {
                                    if (endyCastling == move1.EndSquare.Y && 0 == move1.EndSquare.X)
                                    {
                                        if (TheBoard.Squares[startx, starty].Color != TheBoard.Squares[move1.StartSquare.X, move1.StartSquare.Y].Color)
                                        {
                                            movesBothColors.Remove(move);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (TheBoard.Squares[startx, starty].Color == PieceColor.Black)
                    {
                        if (startx == 7 && starty == 4)
                        {
                            if (endx == 7 && (endy == 2 || endy == 6))
                            {
                                if (KingInCheck(PieceColor.Black, movesBothColors))
                                {
                                    movesBothColors.Remove(move);
                                    continue;
                                }

                                int endychastling;
                                if (endy == 2)
                                {
                                    endychastling = 3;
                                }
                                else
                                {
                                    endychastling = 5;
                                }

                                foreach (Move move1 in movesBothColors)
                                {
                                    if (endychastling == move1.EndSquare.Y && 7 == move1.EndSquare.X)
                                    {
                                        if (TheBoard.Squares[startx, starty].Color != TheBoard.Squares[move1.StartSquare.X, move1.StartSquare.Y].Color)
                                        {
                                            movesBothColors.Remove(move);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }

            foreach (Move move in movesBothColors.ToList())
            {
                movesAfter.Clear();
                bool cont = false;

                MakeMove(move);

                for (int file = 0; file < 8; file++)
                {
                    for (int rank = 0; rank < 8; rank++)
                    {
                        if (TheBoard.Squares[file, rank].Color != PieceColor.Empty)
                        {
                            Point start = new Point(file, rank);
                            GeneratePieceMove(start, TheBoard.Squares[file, rank].Color, TheBoard.Squares[file, rank].Type, movesAfter);
                        }
                    }
                }

                foreach (Move moveAfter in movesAfter)
                {
                    if (TheBoard.Squares[moveAfter.EndSquare.X, moveAfter.EndSquare.Y].Type == PieceType.King && TheBoard.Squares[moveAfter.EndSquare.X, moveAfter.EndSquare.Y].Color != PieceColor.Empty)
                    {
                        if (TheBoard.Squares[moveAfter.StartSquare.X, moveAfter.StartSquare.Y].Color != TheBoard.Squares[moveAfter.EndSquare.X, moveAfter.EndSquare.Y].Color)
                        {
                            if (TheBoard.Squares[moveAfter.StartSquare.X, moveAfter.StartSquare.Y].Color == TheBoard.turnColor)
                            {
                                movesBothColors.Remove(move);
                                cont = true;
                                break;
                            }
                        }

                    }
                }

                TheBoard.PositionFromFEN(SaveState);
                if (cont)
                {
                    continue;
                }

                if (TheBoard.Squares[move.StartSquare.X, move.StartSquare.Y].Color == color)
                {
                    movesPlayerColor.Add(move);
                }
            }

            return movesPlayerColor;
        }
        // Aufruf mit expliziter Farbe; kann verwendet werden, um Züge zu erhalten, als ob tempcolor gerade am Zug ist
        public List<Move> GenerateMoves(PieceColor tempcolor)
        {
            List<Move> result;
            PieceColor color = GetTurnColor();
            TheBoard.turnColor = tempcolor;
            result = GenerateMoves();
            TheBoard.turnColor = color;
            return result;
        }

        public void GeneratePieceMove(Point start, PieceColor color, PieceType type, List<Move> moves)
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
                // Königin vereint Fähigkeiten von Turm und Läufer
                GeneratePieceMove(start, color, PieceType.Rook, moves);
                GeneratePieceMove(start, color, PieceType.Bishop, moves);
            }
            if (type == PieceType.Pawn)
            {
                if (color == PieceColor.White)
                {
                    // Falls Bauer nach vorne nicht blockiert ist...
                    if (TheBoard.Squares[start.X + 1, start.Y].Color != PieceColor.White && TheBoard.Squares[start.X + 1, start.Y].Color != PieceColor.Black)
                    {
                        // Weißer Bauer, der noch nicht bewegt wurde, darf zwei Felder nach vorne ziehen
                        if (start.X == 1)
                        {
                            if (TheBoard.Squares[start.X + 2, start.Y].Color != PieceColor.White && TheBoard.Squares[start.X + 2, start.Y].Color != PieceColor.Black)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 2, start.Y));
                            }
                        }
                        // Weißer Bauer, der das Ende des Brettes erreicht, muss zu einer von vier Figuren befördert werden.
                        if (start.X == 6)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y, MoveType.PromotionQueen));
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y, MoveType.PromotionRook));
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y, MoveType.PromotionBishop));
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y, MoveType.PromotionKnight));
                        }
                        // Ansonsten normaler Zug.
                        else
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y));
                        }
                    }
                    // Schlagen in die eine Diagonale
                    if (start.Y + 1 < 8)
                    {
                        if (TheBoard.Squares[start.X + 1, start.Y + 1].Color == PieceColor.White || TheBoard.Squares[start.X + 1, start.Y + 1].Color == PieceColor.Black)
                        {
                            // Schlagen und Beförderung
                            if (start.X == 6)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1, MoveType.PromotionQueen));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1, MoveType.PromotionRook));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1, MoveType.PromotionBishop));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1, MoveType.PromotionKnight));
                            }
                            // Normales Schlagen
                            else
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1));
                            }

                        }
                        // En passant
                        if (TheBoard.enPassantPosition.Y == start.X + 1 && TheBoard.enPassantPosition.X == start.Y + 1)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y + 1));
                        }
                    }
                    // Schlagen in die andere Diagonale
                    if (start.Y - 1 >= 0)
                    {
                        if (TheBoard.Squares[start.X + 1, start.Y - 1].Color == PieceColor.White || TheBoard.Squares[start.X + 1, start.Y - 1].Color == PieceColor.Black)
                        {
                            // Schlagen und Beförderung
                            if (start.X == 6)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1, MoveType.PromotionQueen));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1, MoveType.PromotionRook));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1, MoveType.PromotionBishop));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1, MoveType.PromotionKnight));
                            }
                            // Normales Schlagen
                            else
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1));
                            }
                        }
                        // En passant
                        if (TheBoard.enPassantPosition.Y == start.X + 1 && TheBoard.enPassantPosition.X == start.Y - 1)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y - 1));

                        }
                    }
                }
                // Analog zu weißem Bauern
                else if (color == PieceColor.Black)
                {
                    if (TheBoard.Squares[start.X - 1, start.Y].Color != PieceColor.White && TheBoard.Squares[start.X - 1, start.Y].Color != PieceColor.Black)
                    {
                        if (start.X == 6)
                        {
                            if (TheBoard.Squares[start.X - 2, start.Y].Color != PieceColor.White && TheBoard.Squares[start.X - 2, start.Y].Color != PieceColor.Black)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 2, start.Y));
                            }
                        }
                        if (start.X == 1)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y, MoveType.PromotionQueen));
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y, MoveType.PromotionRook));
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y, MoveType.PromotionBishop));
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y, MoveType.PromotionKnight));
                        }
                        else
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y));
                        }

                    }
                    if (start.Y + 1 < 8)
                    {
                        if (TheBoard.Squares[start.X - 1, start.Y + 1].Color == PieceColor.White || TheBoard.Squares[start.X - 1, start.Y + 1].Color == PieceColor.Black)
                        {
                            if (start.X == 1)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1, MoveType.PromotionQueen));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1, MoveType.PromotionRook));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1, MoveType.PromotionBishop));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1, MoveType.PromotionKnight));
                            }
                            else
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1));
                            }
                        }
                        if (TheBoard.enPassantPosition.Y == start.X - 1 && TheBoard.enPassantPosition.X == start.Y + 1)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1));

                        }
                    }
                    if (start.Y - 1 >= 0)
                    {
                        if (TheBoard.Squares[start.X - 1, start.Y - 1].Color == PieceColor.White || TheBoard.Squares[start.X - 1, start.Y - 1].Color == PieceColor.Black)
                        {
                            if (start.X == 1)
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1, MoveType.PromotionQueen));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1, MoveType.PromotionRook));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1, MoveType.PromotionBishop));
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1, MoveType.PromotionKnight));
                            }
                            else
                            {
                                potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1));
                            }
                        }
                        if (TheBoard.enPassantPosition.Y == start.X - 1 && TheBoard.enPassantPosition.X == start.Y - 1)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y - 1));

                        }
                    }
                }
            }

            switch (type)
            {
                case PieceType.King:
                    // Züge in alle angrenzenden Felder
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
                            if (TheBoard.Squares[0, 1].Color == PieceColor.Empty && TheBoard.Squares[0, 2].Color == PieceColor.Empty && TheBoard.Squares[0, 3].Color == PieceColor.Empty)
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
                    // L-förmige Züge
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

        public void UpdateChecksAndGameOver(String FEN)
        {
            setBoardFromFEN(FEN);
            List<Move> moves = GenerateMoves();

            if(KingInCheck(PieceColor.White, moves))
            {
                TheBoard.whiteInCheck = true;
            }
            if (KingInCheck(PieceColor.Black, moves))
            {
                TheBoard.blackInCheck = true;
            }

            if (moves.Count == 0)
            {
                PieceColor turnColor = TheBoard.turnColor;
                PieceColor opponentColor = (turnColor == PieceColor.White) ? PieceColor.Black : PieceColor.White;
                TheBoard.turnColor = opponentColor;
                moves = GenerateMoves();
                /*
                foreach(Move move in moves)
                {
                    Console.WriteLine(move.StartSquare.X + " " + move.StartSquare.Y + " " + move.EndSquare.X + " " + move.EndSquare.Y);
                }
                */
                // Gegner ist im Schach und hat keinen legalen Zug -> Schachmatt
                if (KingInCheck(turnColor, moves))
                {
                    if (TheBoard.turnColor != PieceColor.White)
                    {
                        TheBoard.whiteLost = true;
                    }
                    else
                    {
                        TheBoard.blackLost = true;
                    }
                }
                // Gegner ist nicht im Schach und hat keinen legalen Zug -> Patt
                else
                {
                    TheBoard.stalemate = true;
                }
            }
        }

        public int CheckGameOver(String FEN)
        {
            UpdateChecksAndGameOver(FEN);
            if (TheBoard.whiteLost)
            {
                return 0;
            }
            if (TheBoard.blackLost)
            {
                return 1;
            }
            if (TheBoard.stalemate)
            {
                return 2;
            }
            return -1;
        }

    }
}
