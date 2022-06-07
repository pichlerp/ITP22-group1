using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    public class Board
    {
        public enum PieceColor
        {
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

        public Square[,] Squares { get; set; }

        public Board()
        {
            Squares = new Square[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares[i, j] = new Square(i, j);
                }
            }
        }

        public void ClearLegalMoves()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares[i, j].Occupied = false;
                    Squares[i, j].LegalMove = false;
                }
            }
        }

        public void SetLegalMove(int row, int col)
        {
            if (row >= 0 && row <= 7 && col >= 0 && col <= 7)
            {
                this.Squares[row, col].LegalMove = true;
            }
        }


        public void AddLegalMoves(Square currSquare, PieceType type, PieceColor color)
        {
            ClearLegalMoves();

            currSquare.Occupied = true;

            switch (type)
            {
                case PieceType.King:
                    {
                        SetLegalMove(currSquare.RowNum + 1, currSquare.ColNum + 1);
                        SetLegalMove(currSquare.RowNum + 1, currSquare.ColNum - 1);
                        SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum + 1);
                        SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum - 1);
                        SetLegalMove(currSquare.RowNum, currSquare.ColNum + 1);
                        SetLegalMove(currSquare.RowNum, currSquare.ColNum - 1);
                        SetLegalMove(currSquare.RowNum + 1, currSquare.ColNum);
                        SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum);
                    }
                    break;
                case PieceType.Queen:
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            SetLegalMove(currSquare.RowNum + i, currSquare.ColNum);
                            SetLegalMove(currSquare.RowNum - i, currSquare.ColNum);
                            SetLegalMove(currSquare.RowNum, currSquare.ColNum + i);
                            SetLegalMove(currSquare.RowNum, currSquare.ColNum - i);
                        }
                        for (int i = 1; i < 8; i++)
                        {
                            SetLegalMove(currSquare.RowNum + i, currSquare.ColNum + i);
                            SetLegalMove(currSquare.RowNum + i, currSquare.ColNum - i);
                            SetLegalMove(currSquare.RowNum - i, currSquare.ColNum + i);
                            SetLegalMove(currSquare.RowNum - i, currSquare.ColNum - i);
                        }
                    }
                    break;
                case PieceType.Rook:
                    {
                        for(int i = 1; i < 8; i++)
                        {
                            SetLegalMove(currSquare.RowNum + i, currSquare.ColNum);
                            SetLegalMove(currSquare.RowNum - i, currSquare.ColNum);
                            SetLegalMove(currSquare.RowNum, currSquare.ColNum + i);
                            SetLegalMove(currSquare.RowNum, currSquare.ColNum - i);
                        }
                    }
                    break;
                case PieceType.Bishop:
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            SetLegalMove(currSquare.RowNum + i, currSquare.ColNum + i);
                            SetLegalMove(currSquare.RowNum + i, currSquare.ColNum - i);
                            SetLegalMove(currSquare.RowNum - i, currSquare.ColNum + i);
                            SetLegalMove(currSquare.RowNum - i, currSquare.ColNum - i);
                        }
                    }
                    break;
                case PieceType.Knight:
                    {
                        SetLegalMove(currSquare.RowNum + 2, currSquare.ColNum + 1);
                        SetLegalMove(currSquare.RowNum + 2, currSquare.ColNum - 1);
                        SetLegalMove(currSquare.RowNum - 2, currSquare.ColNum + 1);
                        SetLegalMove(currSquare.RowNum - 2, currSquare.ColNum - 1);
                        SetLegalMove(currSquare.RowNum + 1, currSquare.ColNum + 2);
                        SetLegalMove(currSquare.RowNum + 1, currSquare.ColNum - 2);
                        SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum + 2);
                        SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum - 2);
                    }
                    break;
                case PieceType.Pawn:
                    {
                        if (color == PieceColor.White)
                        {
                            SetLegalMove(currSquare.RowNum + 1, currSquare.ColNum);
                        }
                        if (color == PieceColor.Black)
                        {
                            SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum);
                        }
                    }
                    break;
            }
        }

    }

}
