using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    public class Board
    {
        public Square[,] Squares { get; set; }

        public Board()
        {
            Squares = new Square[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares[i, j] = new(i, j);
                    Squares[i, j].Color = PieceColor.Empty;
                }
            }
            for (int i = 0; i < 8; i++)
            {
                Squares[1, i].Type = PieceType.Pawn;
                Squares[1, i].Color = PieceColor.White;
                Squares[0, i].Color = PieceColor.White;
                Squares[6, i].Type = PieceType.Pawn;
                Squares[6, i].Color = PieceColor.Black;
                Squares[7, i].Color = PieceColor.Black;
            }

            Squares[0, 0].Type = PieceType.Rook;
            Squares[0, 1].Type = PieceType.Knight;
            Squares[0, 2].Type = PieceType.Bishop;
            Squares[0, 3].Type = PieceType.King;
            Squares[0, 4].Type = PieceType.Queen;
            Squares[0, 5].Type = PieceType.Bishop;
            Squares[0, 6].Type = PieceType.Knight;
            Squares[0, 7].Type = PieceType.Rook;

            Squares[7, 0].Type = PieceType.Rook;
            Squares[7, 1].Type = PieceType.Knight;
            Squares[7, 2].Type = PieceType.Bishop;
            Squares[7, 3].Type = PieceType.King;
            Squares[7, 4].Type = PieceType.Queen;
            Squares[7, 5].Type = PieceType.Bishop;
            Squares[7, 6].Type = PieceType.Knight;
            Squares[7, 7].Type = PieceType.Rook;
        }

        public Board(string type)
        {
            Squares = new Square[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares[i, j] = new(i, j);
                    Squares[i, j].Color = PieceColor.Empty;
                }
            }

            if (type == "EnPassantTest")
            {
                Squares[1, 0].Type = PieceType.Pawn;
                Squares[1, 0].Color = PieceColor.White;
                Squares[3, 1].Type = PieceType.Pawn;
                Squares[3, 1].Color = PieceColor.Black;
            }

            if (type == "PromotionTest")
            {
                Squares[6, 0].Type = PieceType.Pawn;
                Squares[6, 0].Color = PieceColor.White;
                Squares[1, 1].Type = PieceType.Pawn;
                Squares[1, 1].Color = PieceColor.Black;
            }

            if (type == "RochadeTest")
            {
                Squares[0, 3].Type = PieceType.King;
                Squares[0, 7].Type = PieceType.Rook;
                Squares[0, 0].Type = PieceType.Rook;
                Squares[0, 4].Type = PieceType.Queen;
                Squares[0, 3].Color = PieceColor.White;
                Squares[0, 7].Color = PieceColor.White;
                Squares[0, 0].Color = PieceColor.White;
                Squares[0, 4].Color = PieceColor.White;

                Squares[7, 3].Type = PieceType.King;
                Squares[7, 7].Type = PieceType.Rook;
                Squares[7, 0].Type = PieceType.Rook;
                Squares[7, 3].Color = PieceColor.Black;
                Squares[7, 7].Color = PieceColor.Black;
                Squares[7, 0].Color = PieceColor.Black;
            }

        }

        internal bool SelectMove(int MoveRow, int MoveCol, int PieceRow, int PieceCol)
        {
            if (!this.Squares[MoveRow, MoveCol].LegalMove)
            {
                return false;
            }
            else
            {
                bool special = false;
                // En passant wird eventuell möglich - Flag wird gesetzt
                if (this.Squares[PieceRow, PieceCol].Type == PieceType.Pawn && (MoveRow - PieceRow) * (MoveRow - PieceRow) == 4)
                {
                    if (this.Squares[PieceRow, PieceCol].Color == PieceColor.White)
                    {
                        this.Squares[PieceRow + 1, PieceCol].EnPassantPossible = true;
                    }
                    else
                    {
                        this.Squares[PieceRow - 1, PieceCol].EnPassantPossible = true;
                    }
                }
                // Beförderung von Bauern
                if (this.Squares[PieceRow, PieceCol].Type == PieceType.Pawn && (MoveRow == 0 || MoveRow == 7))
                {
                    special = true;
                    Console.WriteLine("Bauer wird befördert. Figur auswählen (q, r, b, n): ");
                    string input = Console.ReadLine()!;
                    while (!(input[0] == 'q' || input[0] == 'r' || input[0] == 'b' || input[0] == 'n'))
                    {
                        Console.WriteLine("Eingabe ungültig. Figur auswählen (q, r, b, n): ");
                        input = Console.ReadLine()!;
                    }
                    if (input[0] == 'q')
                    {
                        this.Squares[MoveRow, MoveCol].AddPiece(this.Squares[PieceRow, PieceCol].Color, PieceType.Queen);
                    }
                    else if (input[0] == 'r')
                    {
                        this.Squares[MoveRow, MoveCol].AddPiece(this.Squares[PieceRow, PieceCol].Color, PieceType.Rook);
                    }
                    else if (input[0] == 'b')
                    {
                        this.Squares[MoveRow, MoveCol].AddPiece(this.Squares[PieceRow, PieceCol].Color, PieceType.Bishop);
                    }
                    else
                    {
                        this.Squares[MoveRow, MoveCol].AddPiece(this.Squares[PieceRow, PieceCol].Color, PieceType.Knight);
                    }
                }
                // Rochade
                if (this.Squares[PieceRow, PieceCol].Type == PieceType.King && this.Squares[PieceRow, PieceCol].Moved == false)
                {
                    // Lange Rochade weiß
                    if (MoveRow == 0 && MoveCol == 5)
                    {
                        this.Squares[0, 4].AddPiece(PieceColor.White, PieceType.Rook);
                        this.Squares[0, 7].RemovePiece();
                    }
                    // Kurze Rochade weiß
                    else if (MoveRow == 0 && MoveCol == 1)
                    {
                        this.Squares[0, 2].AddPiece(PieceColor.White, PieceType.Rook);
                        this.Squares[0, 0].RemovePiece();
                    }
                    // Lange Rochade schwarz
                    else if (MoveRow == 7 && MoveCol == 5)
                    {
                        this.Squares[7, 4].AddPiece(PieceColor.Black, PieceType.Rook);
                        this.Squares[7, 7].RemovePiece();
                    }
                    // Kurze Rochade schwarz
                    else if (MoveRow == 7 && MoveCol == 1)
                    {
                        this.Squares[7, 2].AddPiece(PieceColor.Black, PieceType.Rook);
                        this.Squares[7, 0].RemovePiece();
                    }
                }
                if (!special)
                {
                    this.Squares[MoveRow, MoveCol].AddPiece(this.Squares[PieceRow, PieceCol].Color, this.Squares[PieceRow, PieceCol].Type);
                }
                this.Squares[PieceRow, PieceCol].Moved = true;
                this.Squares[MoveRow, MoveCol].Moved = true;
                this.Squares[PieceRow, PieceCol].RemovePiece();
                return true;
            }
        }

        internal bool SelectPiece(int row, int col, PieceColor color)
        {
            if (row >= 0 && row <= 7 && col >= 0 && col <= 7)
            {
                if (this.Squares[row, col].Color == color)
                {
                    this.AddLegalMoves(this.Squares[row, col], this.Squares[row, col].Type, this.Squares[row, col].Color);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void ClearLegalMoves()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
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
                        if (!this.Squares[currSquare.RowNum, currSquare.ColNum].Moved && color == PieceColor.White)
                        {
                            // Lange Rochade weiß
                            if (this.Squares[0, 7].Moved == false && this.Squares[0, 6].Color == PieceColor.Empty && this.Squares[0, 5].Color == PieceColor.Empty && this.Squares[0, 4].Color == PieceColor.Empty)
                            {
                                SetLegalMove(currSquare.RowNum, currSquare.ColNum + 2);
                            }
                            // kurze Rochade weiß
                            if (this.Squares[0, 0].Moved == false && this.Squares[0, 1].Color == PieceColor.Empty && this.Squares[0, 2].Color == PieceColor.Empty)
                            {
                                SetLegalMove(currSquare.RowNum, currSquare.ColNum - 2);
                            }
                        }
                        else if (!this.Squares[currSquare.RowNum, currSquare.ColNum].Moved && color == PieceColor.Black)
                        {
                            // Lange Rochade schwarz
                            if (this.Squares[7, 7].Moved == false && this.Squares[7, 6].Color == PieceColor.Empty && this.Squares[7, 5].Color == PieceColor.Empty && this.Squares[7, 4].Color == PieceColor.Empty)
                            {
                                SetLegalMove(currSquare.RowNum, currSquare.ColNum + 2);
                            }
                            // kurze Rochade schwarz
                            if (this.Squares[7, 0].Moved == false && this.Squares[7, 1].Color == PieceColor.Empty && this.Squares[7, 2].Color == PieceColor.Empty)
                            {
                                SetLegalMove(currSquare.RowNum, currSquare.ColNum - 2);
                            }
                        }
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
                        for (int i = 1; i < 8; i++)
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
                            if (currSquare.RowNum == 1)
                            {
                                SetLegalMove(currSquare.RowNum + 2, currSquare.ColNum);
                            }
                        }
                        if (color == PieceColor.Black)
                        {
                            SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum);
                            if (currSquare.RowNum == 6)
                            {
                                SetLegalMove(currSquare.RowNum - 2, currSquare.ColNum);
                            }
                        }
                        if (color == PieceColor.White && currSquare.RowNum == 4 && currSquare.ColNum + 1 <= 7)
                        {
                            if (this.Squares[currSquare.RowNum + 1, currSquare.ColNum + 1].EnPassantPossible)
                            {
                                SetLegalMove(currSquare.RowNum + 1, currSquare.ColNum + 1);
                            }
                        }
                        if (color == PieceColor.White && currSquare.RowNum == 4 && currSquare.ColNum - 1 >= 0)
                        {
                            if (this.Squares[currSquare.RowNum + 1, currSquare.ColNum - 1].EnPassantPossible)
                            {
                                SetLegalMove(currSquare.RowNum + 1, currSquare.ColNum - 1);
                            }
                        }
                        if (color == PieceColor.Black && currSquare.RowNum == 3 && currSquare.ColNum + 1 <= 7)
                        {
                            if (this.Squares[currSquare.RowNum - 1, currSquare.ColNum + 1].EnPassantPossible)
                            {
                                SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum + 1);
                            }
                        }
                        if (color == PieceColor.Black && currSquare.RowNum == 3 && currSquare.ColNum - 1 >= 0)
                        {
                            if (this.Squares[currSquare.RowNum - 1, currSquare.ColNum - 1].EnPassantPossible)
                            {
                                SetLegalMove(currSquare.RowNum - 1, currSquare.ColNum - 1);
                            }
                        }
                    }
                    break;
            }
        }

    }

}
