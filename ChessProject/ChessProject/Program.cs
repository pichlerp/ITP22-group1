using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChessProject.Board;

namespace ChessProject
{
    class Program
    {
        static readonly Board TheBoard = new();
        static void Main(string[] args)
        {
            Square currentSquare = AskUserCurrentSquare();

            currentSquare.Occupied = true;

            Console.WriteLine("");
            Console.WriteLine("King:");
            TheBoard.AddLegalMoves(currentSquare, PieceType.King, PieceColor.White);
            PrintLegalMoves(currentSquare, PieceType.King, PieceColor.White);
            Console.WriteLine("");
            PrintBoard(TheBoard);

            Console.WriteLine("");
            Console.WriteLine("Queen:");
            TheBoard.AddLegalMoves(currentSquare, PieceType.Queen, PieceColor.White);
            PrintLegalMoves(currentSquare, PieceType.Queen, PieceColor.White);
            Console.WriteLine("");
            PrintBoard(TheBoard);

            Console.WriteLine("");
            Console.WriteLine("Rook:");
            TheBoard.AddLegalMoves(currentSquare, PieceType.Rook, PieceColor.White);
            PrintLegalMoves(currentSquare, PieceType.Rook, PieceColor.White);
            Console.WriteLine("");
            PrintBoard(TheBoard);

            Console.WriteLine("");
            Console.WriteLine("Bishop:");
            TheBoard.AddLegalMoves(currentSquare, PieceType.Bishop, PieceColor.White);
            PrintLegalMoves(currentSquare, PieceType.Bishop, PieceColor.White);
            Console.WriteLine("");
            PrintBoard(TheBoard);

            Console.WriteLine("");
            Console.WriteLine("Knight:");
            TheBoard.AddLegalMoves(currentSquare, PieceType.Knight, PieceColor.White);
            PrintLegalMoves(currentSquare, PieceType.Knight, PieceColor.White);
            Console.WriteLine("");
            PrintBoard(TheBoard);

            Console.WriteLine("");
            Console.WriteLine("White Pawn:");
            TheBoard.AddLegalMoves(currentSquare, PieceType.Pawn, PieceColor.White);
            PrintLegalMoves(currentSquare, PieceType.Pawn, PieceColor.White);
            Console.WriteLine("");
            PrintBoard(TheBoard);

            Console.WriteLine("");
            Console.WriteLine("Black Pawn:");
            TheBoard.AddLegalMoves(currentSquare, PieceType.Pawn, PieceColor.Black);
            PrintLegalMoves(currentSquare, PieceType.Pawn, PieceColor.Black);
            Console.WriteLine("");
            PrintBoard(TheBoard);

        }

        private static Square AskUserCurrentSquare()
        {
            Console.WriteLine("Enter row of piece");
            string line = Console.ReadLine();
            int row = 0, col = 0;
            if (line != "" && line != null)
            {
                row = int.Parse(line);
            }
            Console.WriteLine("Enter column of piece");
            line = Console.ReadLine();
            if (line != "" && line != null)
            {
                col = int.Parse(line);
            }
            return TheBoard.Squares[row, col];
        }

        private static void PrintBoard(Board B)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (B.Squares[i, j].Occupied == true)
                    {
                        Console.Write("O ");
                    }
                    else if (B.Squares[i, j].LegalMove == true)
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
        }

        public static void PrintLegalMove(int row, int col)
        {
            if (row >= 0 && row <= 7 && col >= 0 && col <= 7)
            {
                Console.Write("(" + row + "," + col + ") ");
            }
        }

        public static void PrintLegalMoves(Square currSquare, PieceType type, PieceColor color)
        {
            switch (type)
            {
                case PieceType.King:
                    {
                        PrintLegalMove(currSquare.RowNum + 1, currSquare.ColNum + 1);
                        PrintLegalMove(currSquare.RowNum + 1, currSquare.ColNum - 1);
                        PrintLegalMove(currSquare.RowNum - 1, currSquare.ColNum + 1);
                        PrintLegalMove(currSquare.RowNum - 1, currSquare.ColNum - 1);
                        PrintLegalMove(currSquare.RowNum, currSquare.ColNum + 1);
                        PrintLegalMove(currSquare.RowNum, currSquare.ColNum - 1);
                        PrintLegalMove(currSquare.RowNum + 1, currSquare.ColNum);
                        PrintLegalMove(currSquare.RowNum - 1, currSquare.ColNum);
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Queen:
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            PrintLegalMove(currSquare.RowNum + i, currSquare.ColNum);
                            PrintLegalMove(currSquare.RowNum - i, currSquare.ColNum);
                            PrintLegalMove(currSquare.RowNum, currSquare.ColNum + i);
                            PrintLegalMove(currSquare.RowNum, currSquare.ColNum - i);
                        }
                        for (int i = 1; i < 8; i++)
                        {
                            PrintLegalMove(currSquare.RowNum + i, currSquare.ColNum + i);
                            PrintLegalMove(currSquare.RowNum + i, currSquare.ColNum - i);
                            PrintLegalMove(currSquare.RowNum - i, currSquare.ColNum + i);
                            PrintLegalMove(currSquare.RowNum - i, currSquare.ColNum - i);
                        }
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Rook:
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            PrintLegalMove(currSquare.RowNum + i, currSquare.ColNum);
                            PrintLegalMove(currSquare.RowNum - i, currSquare.ColNum);
                            PrintLegalMove(currSquare.RowNum, currSquare.ColNum + i);
                            PrintLegalMove(currSquare.RowNum, currSquare.ColNum - i);
                        }
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Bishop:
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            PrintLegalMove(currSquare.RowNum + i, currSquare.ColNum + i);
                            PrintLegalMove(currSquare.RowNum + i, currSquare.ColNum - i);
                            PrintLegalMove(currSquare.RowNum - i, currSquare.ColNum + i);
                            PrintLegalMove(currSquare.RowNum - i, currSquare.ColNum - i);
                        }
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Knight:
                    {
                        PrintLegalMove(currSquare.RowNum + 2, currSquare.ColNum + 1);
                        PrintLegalMove(currSquare.RowNum + 2, currSquare.ColNum - 1);
                        PrintLegalMove(currSquare.RowNum - 2, currSquare.ColNum + 1);
                        PrintLegalMove(currSquare.RowNum - 2, currSquare.ColNum - 1);
                        PrintLegalMove(currSquare.RowNum + 1, currSquare.ColNum + 2);
                        PrintLegalMove(currSquare.RowNum + 1, currSquare.ColNum - 2);
                        PrintLegalMove(currSquare.RowNum - 1, currSquare.ColNum + 2);
                        PrintLegalMove(currSquare.RowNum - 1, currSquare.ColNum - 2);
                        Console.WriteLine("");
                    }
                    break;
                case PieceType.Pawn:
                    {
                        if (color == PieceColor.White)
                        {
                            PrintLegalMove(currSquare.RowNum + 1, currSquare.ColNum);
                        }
                        if (color == PieceColor.Black)
                        {
                            PrintLegalMove(currSquare.RowNum - 1, currSquare.ColNum);
                        }
                        Console.WriteLine("");
                    }
                    break;
            }
        }
    }
}
