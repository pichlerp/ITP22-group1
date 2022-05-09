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
        // Spielaufbau
        static readonly string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
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
        /*
        internal List<Point> getPossibleMoves(int x, int y, ref List<Point> legal_moves)
        {
            PieceColor color = TheBoard.getPieceColor(y, x);
            if (color == turnColor)
            {
                TheBoard.ClearLegalMoves();
                PieceType type = TheBoard.getPieceType(y, x);
                TheBoard.Squares[y, x].Selected = true;
                TheBoard.AddLegalMoves(TheBoard.Squares[y, x], type, color);
                Square square = TheBoard.getSquare(y, x);
                TheBoard.AppendLegalMoves(square.ColNum, square.RowNum, type, color, ref legal_moves);
            }
            return legal_moves;
        }
        */
        internal List<Point> GetPossibleMoves(int rank, int file, ref List<Point> legalMoves)
        {
            foreach (Move move in moves)
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

        internal void MakeMove(int startX, int startY, int endX, int endY)
        {
            PieceColor color = TheBoard.Squares[startX, startY].Color;
            PieceType type = TheBoard.Squares[startX, startY].Type;
            TheBoard.Squares[endX, endY].Color = color;
            TheBoard.Squares[endX, endY].Type = type;
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
        /*
        internal void makeMove(int y, int x)
        {
            Square selectedSquare = TheBoard.GetSelectedSquare();
            TheBoard.Squares[x, y].Color = selectedSquare.Color;
            TheBoard.Squares[x, y].Type = selectedSquare.Type;
            selectedSquare.Color = PieceColor.Empty;
            if (turnColor == PieceColor.White)
            {
                turnColor = PieceColor.Black;
            }
            else
            {
                turnColor = PieceColor.White;
            }
        }
        */
        /*
        internal void deselectPiece()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    {
                        TheBoard.Squares[i, j].Selected = false;

                    }
                }
            }

        }
        */


        // Testfälle
        //static readonly Board TheBoard = new("EnPassantTest");
        //static readonly Board TheBoard = new("PromotionTest");
        //static readonly Board TheBoard = new("RochadeTest");
        /*
        static void game()
        {
            string input;
            while (true)
            {
                PrintBoard(TheBoard);

                Console.WriteLine("Weiß am Zug. Weiße Figur auswählen: ");
                int PieceRow = 0;
                int PieceCol = 0;
                input = Console.ReadLine();
                while (!ParseInput(ref PieceRow, ref PieceCol, input))
                {
                    Console.WriteLine("Kein gültiges Feld gewählt. Weiße Figur auswählen: ");
                    input = Console.ReadLine();
                    ParseInput(ref PieceRow, ref PieceCol, input);
                }
                while (!TheBoard.SelectPiece(PieceRow, PieceCol, PieceColor.White))
                {
                    Console.WriteLine("Keine weiße Figur auf diesem Feld! Weiße Figur auswählen: ");
                    input = Console.ReadLine();
                    ParseInput(ref PieceRow, ref PieceCol, input);
                }
                Console.WriteLine("Gültige Züge für diese Figur: ");
                PrintLegalMoves(PieceRow, PieceCol, TheBoard);
                PrintBoard(TheBoard);
                Console.WriteLine("Zug auswählen: ");
                int MoveRow = 0;
                int MoveCol = 0;
                input = Console.ReadLine();
                while (!ParseInput(ref MoveRow, ref MoveCol, input))
                {
                    Console.WriteLine("Kein gültiger Zug gewählt. Zug auswählen: ");
                    input = Console.ReadLine();
                    ParseInput(ref MoveRow, ref MoveCol, input);
                }
                while (!TheBoard.SelectMove(MoveRow, MoveCol, PieceRow, PieceCol))
                {
                    Console.WriteLine("Kein gültiger Zug gewählt. Zug auswählen: ");
                    input = Console.ReadLine();
                    ParseInput(ref MoveRow, ref MoveCol, input);
                }
                TheBoard.ClearLegalMoves();

                PrintBoard(TheBoard);

                Console.WriteLine("Schwarz am Zug. Schwarze Figur auswählen: ");
                input = Console.ReadLine();
                while (!ParseInput(ref PieceRow, ref PieceCol, input))
                {
                    Console.WriteLine("Kein gültiges Feld gewählt. Schwarze Figur auswählen: ");
                    input = Console.ReadLine();
                    ParseInput(ref PieceRow, ref PieceCol, input);
                }
                while (!TheBoard.SelectPiece(PieceRow, PieceCol, PieceColor.Black))
                {
                    Console.WriteLine("Keine schwarze Figur auf diesem Feld! Schwarze Figur auswählen: ");
                    input = Console.ReadLine();
                    ParseInput(ref PieceRow, ref PieceCol, input);
                }
                Console.WriteLine("Gültige Züge für diese Figur: ");
                PrintLegalMoves(PieceRow, PieceCol, TheBoard);
                PrintBoard(TheBoard);
                Console.WriteLine("Zug auswählen: ");
                input = Console.ReadLine();
                while (!ParseInput(ref MoveRow, ref MoveCol, input))
                {
                    Console.WriteLine("Kein gültiger Zug gewählt. Zug auswählen: ");
                    input = Console.ReadLine();
                    ParseInput(ref MoveRow, ref MoveCol, input);
                }
                while (!TheBoard.SelectMove(MoveRow, MoveCol, PieceRow, PieceCol))
                {
                    Console.WriteLine("Kein gültiger Zug gewählt. Zug auswählen: ");
                    input = Console.ReadLine();
                    ParseInput(ref MoveRow, ref MoveCol, input);
                }
                TheBoard.ClearLegalMoves();
            }
        }*/
        /*
        static bool ParseInput(ref int RowNum, ref int ColNum, string input)
        {
            if (input.Length != 2)
            {
                return false;
            }

            char File = input[0];
            char Rank = input[1];

            if (File == 'h' || File == 'A')
            {
                ColNum = 0;
            }
            else if (File == 'g' || File == 'B')
            {
                ColNum = 1;
            }
            else if (File == 'f' || File == 'C')
            {
                ColNum = 2;
            }
            else if (File == 'e' || File == 'D')
            {
                ColNum = 3;
            }
            else if (File == 'd' || File == 'E')
            {
                ColNum = 4;
            }
            else if (File == 'c' || File == 'F')
            {
                ColNum = 5;
            }
            else if (File == 'b' || File == 'G')
            {
                ColNum = 6;
            }
            else if (File == 'a' || File == 'H')
            {
                ColNum = 7;
            }
            else
            {
                return false;
            }

            if (Rank == '1')
            {
                RowNum = 0;
            }
            else if (Rank == '2')
            {
                RowNum = 1;
            }
            else if (Rank == '3')
            {
                RowNum = 2;
            }
            else if (Rank == '4')
            {
                RowNum = 3;
            }
            else if (Rank == '5')
            {
                RowNum = 4;
            }
            else if (Rank == '6')
            {
                RowNum = 5;
            }
            else if (Rank == '7')
            {
                RowNum = 6;
            }
            else if (Rank == '8')
            {
                RowNum = 7;
            }
            else
            {
                return false;
            }

            return true;
        }
        */
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
                    else if (B.Squares[i, j].LegalMove)
                    {
                        Console.Write("X ");
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
        /*
        public static void PrintLegalMove(int row, int col)
        {
            if (row >= 0 && row <= 7 && col >= 0 && col <= 7)
            {
                if (col == 0)
                {
                    Console.Write("h");
                }
                else if (col == 1)
                {
                    Console.Write("g");
                }
                else if (col == 2)
                {
                    Console.Write("f");
                }
                else if (col == 3)
                {
                    Console.Write("e");
                }
                else if (col == 4)
                {
                    Console.Write("d");
                }
                else if (col == 5)
                {
                    Console.Write("c");
                }
                else if (col == 6)
                {
                    Console.Write("b");
                }
                else if (col == 7)
                {
                    Console.Write("a");
                }

                if (row == 0)
                {
                    Console.Write("1");
                }
                else if (row == 1)
                {
                    Console.Write("2");
                }
                else if (row == 2)
                {
                    Console.Write("3");
                }
                else if (row == 3)
                {
                    Console.Write("4");
                }
                else if (row == 4)
                {
                    Console.Write("5");
                }
                else if (row == 5)
                {
                    Console.Write("6");
                }
                else if (row == 6)
                {
                    Console.Write("7");
                }
                else if (row == 7)
                {
                    Console.Write("8");
                }
                Console.Write(" ");
            }
        }
        */
        /*
        public static void PrintLegalMoves(int row, int col, Board Board)
        {
            PieceType type = Board.Squares[row, col].Type;
            switch (type)
            {
                case PieceType.King:
                    {
                        PrintLegalMove(row + 1, col + 1);
                        PrintLegalMove(row + 1, col - 1);
                        PrintLegalMove(row - 1, col + 1);
                        PrintLegalMove(row - 1, col - 1);
                        PrintLegalMove(row, col + 1);
                        PrintLegalMove(row, col - 1);
                        PrintLegalMove(row + 1, col);
                        PrintLegalMove(row - 1, col);
                        if (!Board.Squares[row, col].Moved && Board.Squares[row, col].Color == PieceColor.White)
                        {
                            // Lange Rochade weiß
                            if (Board.Squares[0, 7].Moved == false && Board.Squares[0, 6].Color == PieceColor.Empty && Board.Squares[0, 5].Color == PieceColor.Empty && Board.Squares[0, 4].Color == PieceColor.Empty)
                            {
                                PrintLegalMove(row, col + 2);
                            }
                            // kurze Rochade weiß
                            if (Board.Squares[0, 0].Moved == false && Board.Squares[0, 1].Color == PieceColor.Empty && Board.Squares[0, 2].Color == PieceColor.Empty)
                            {
                                PrintLegalMove(row, col - 2);
                            }
                        }
                        else if (!Board.Squares[row, col].Moved && Board.Squares[row, col].Color == PieceColor.White)
                        {
                            // Lange Rochade schwarz
                            if (Board.Squares[7, 7].Moved == false && Board.Squares[7, 6].Color == PieceColor.Empty && Board.Squares[7, 5].Color == PieceColor.Empty && Board.Squares[7, 4].Color == PieceColor.Empty)
                            {
                                PrintLegalMove(row, col + 2);
                            }
                            // kurze Rochade schwarz
                            if (Board.Squares[7, 0].Moved == false && Board.Squares[7, 1].Color == PieceColor.Empty && Board.Squares[7, 2].Color == PieceColor.Empty)
                            {
                                PrintLegalMove(row, col - 2);
                            }
                        }
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Queen:
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            PrintLegalMove(row + i, col);
                            PrintLegalMove(row - i, col);
                            PrintLegalMove(row, col + i);
                            PrintLegalMove(row, col - i);
                        }
                        for (int i = 1; i < 8; i++)
                        {
                            PrintLegalMove(row + i, col + i);
                            PrintLegalMove(row + i, col - i);
                            PrintLegalMove(row - i, col + i);
                            PrintLegalMove(row - i, col - i);
                        }
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Rook:
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            PrintLegalMove(row + i, col);
                            PrintLegalMove(row - i, col);
                            PrintLegalMove(row, col + i);
                            PrintLegalMove(row, col - i);
                        }
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Bishop:
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            PrintLegalMove(row + i, col + i);
                            PrintLegalMove(row + i, col - i);
                            PrintLegalMove(row - i, col + i);
                            PrintLegalMove(row - i, col - i);
                        }
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Knight:
                    {
                        PrintLegalMove(row + 2, col + 1);
                        PrintLegalMove(row + 2, col - 1);
                        PrintLegalMove(row - 2, col + 1);
                        PrintLegalMove(row - 2, col - 1);
                        PrintLegalMove(row + 1, col + 2);
                        PrintLegalMove(row + 1, col - 2);
                        PrintLegalMove(row - 1, col + 2);
                        PrintLegalMove(row - 1, col - 2);
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Pawn:
                    {
                        if (Board.Squares[row, col].Color == PieceColor.White)
                        {
                            PrintLegalMove(row + 1, col);
                            if (row == 1)
                            {
                                PrintLegalMove(row + 2, col);
                            }
                        }
                        if (Board.Squares[row, col].Color == PieceColor.Black)
                        {
                            PrintLegalMove(row - 1, col);
                            if (row == 6)
                            {
                                PrintLegalMove(row - 2, col);
                            }
                        }
                        if (Board.Squares[row, col].Color == PieceColor.White && row == 4 && col + 1 <= 7)
                        {
                            if (Board.Squares[row + 1, col + 1].EnPassantPossible)
                            {
                                PrintLegalMove(row + 1, col + 1);
                            }
                        }
                        if (Board.Squares[row, col].Color == PieceColor.White && row == 4 && col - 1 >= 0)
                        {
                            if (Board.Squares[row + 1, col - 1].EnPassantPossible)
                            {
                                PrintLegalMove(row + 1, col - 1);
                            }
                        }
                        if (Board.Squares[row, col].Color == PieceColor.Black && row == 3 && col + 1 <= 7)
                        {
                            if (Board.Squares[row - 1, col + 1].EnPassantPossible)
                            {
                                PrintLegalMove(row - 1, col + 1);
                            }
                        }
                        if (Board.Squares[row, col].Color == PieceColor.Black && row == 3 && col - 1 >= 0)
                        {
                            if (Board.Squares[row - 1, col - 1].EnPassantPossible)
                            {
                                PrintLegalMove(row - 1, col - 1);
                            }
                        }
                        if (Board.Squares[row, col].Color == PieceColor.White && row == 6)
                        {
                            Console.WriteLine("Beförderung von weißem Bauern");
                        }
                        if (Board.Squares[row, col].Color == PieceColor.Black && row == 1)
                        {
                            Console.WriteLine("Beförderung von schwarzem Bauern");
                        }
                    }
                    break;
            }
            Console.WriteLine("");
        }
        */
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

        List<Move> moves;
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
            // Liste von Zügen ohne Kontrolle von Bounds
            List<Move> potentialMoves = new List<Move>();
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
                    potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y + 1));
                    // Rochade
                    if (color == PieceColor.White)
                    {
                        // Lange Rochade weiß
                        if (TheBoard.whiteCastlingLongPossible)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y + 2));
                        }
                        // Kurze Rochade weiß
                        if (TheBoard.whiteCastlingShortPossible)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y - 2));
                        }
                    }
                    else if (color == PieceColor.Black)
                    {
                        // Lange Rochade schwarz
                        if (TheBoard.blackCastlingLongPossible)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y + 2));
                        }
                        // Kurze Rochade schwarz
                        if (TheBoard.blackCastlingShortPossible)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y - 2));
                        }
                    }
                    break;
                case PieceType.Queen:
                    for (int i = 1; i < 8; i++)
                    {
                        potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y + i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y - i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y + i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y - i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y + i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y - i));
                    }
                    break;
                case PieceType.Rook:
                    for (int i = 1; i < 8; i++)
                    {
                        potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y + i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X, start.Y - i));
                    }
                    break;
                case PieceType.Bishop:
                    for (int i = 1; i < 8; i++)
                    {
                        potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y + i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X + i, start.Y - i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y + i));
                        potentialMoves.Add(new Move(start.X, start.Y, start.X - i, start.Y - i));
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
                case PieceType.Pawn:
                    if (color == PieceColor.White)
                    {
                        if (start.X == 1)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X + 2, start.Y));
                        }
                        potentialMoves.Add(new Move(start.X, start.Y, start.X + 1, start.Y));
                    }
                    else if (color == PieceColor.Black)
                    {
                        if (start.X == 6)
                        {
                            potentialMoves.Add(new Move(start.X, start.Y, start.X - 2, start.Y));
                        }
                        potentialMoves.Add(new Move(start.X, start.Y, start.X - 1, start.Y));
                    }
                    break;
            }
            // Nur die Züge, die innerhalb des Bretts bleiben, werden an moves angehängt
            foreach (Move move in potentialMoves)
            {
                if (!(move.EndSquare.X < 0 || move.EndSquare.Y < 0 || move.EndSquare.X > 7 || move.EndSquare.Y > 7))
                {
                    moves.Add(move);
                }
            }
        }
    }
}
