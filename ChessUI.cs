using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;


namespace Chess_UI
{
    class ChessUI
    {
        static int height = 800;
        static int width = 800;
        Point lastClick;
        Point currentClick;
        Point penultimateClick;
        static Color dark = Color.FromArgb(128, 63, 130);
        static Color bright = Color.FromArgb(63, 130, 128);
        Color dark_highlight = Color.FromArgb(Math.Min((int)((double)dark.R * 1.34), 255), Math.Min((int)((double)dark.G * 1.34), 255), Math.Min((int)((double)dark.B * 1.34), 255));
        Color bright_highlight = Color.FromArgb(Math.Min((int)((double)bright.R * 1.34), 255), Math.Min((int)((double)bright.G * 1.34), 255), Math.Min((int)((double)bright.B * 1.34), 255));
        bool piece_selected;
        public bool acceptMove = false;
        Action<int, int, bool> clickHandler;
        public Form form;
        Bitmap board;
        PictureBox boardbox;
        public PictureBox[,] piece_imageboxes = new PictureBox[8, 8];
        // Bilder müssen in projectDirectory/Images gespeichert werden
        // Name der Bilder muss erster Großbuchstabe der englischen Figurenname + 'w' oder 'b' (black/white) sein, außer König hat 'G' als Buchstabe
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

        public ChessUI(Form inputform, Action<int, int, bool> inputClickhandler)
        {
            // Variablen werden initialisiert
            form = inputform;
            clickHandler = inputClickhandler;
            lastClick = new Point(0, 0);
            currentClick = new Point(0, 0);
            penultimateClick = new Point(0, 0);
            piece_selected = false;
            board = new Bitmap(width, height);
            boardbox = new PictureBox
            {
                Location = new Point(100, 100),
                Size = new Size(width, height)
            };
            form.Controls.Add(boardbox);
            // Brett schwarz/weiß karriert machen
            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < height; k++)
                {
                    if ((i / 100 % 2 == 0 ^ k / 100 % 2 == 0))
                    {
                        board.SetPixel(i, k, dark);
                    }
                    else
                    {
                        board.SetPixel(i, k, bright);
                    }
                }
            }
            boardbox.Image = board;
            // Pictureboxes der Figuren initialisieren
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    piece_imageboxes[i, k] = new PictureBox
                    {
                        Size = new Size(100, 100),
                        Location = new Point(k * 100, i * 100)
                    };
                    piece_imageboxes[i, k].Click += ChessUI_Click;
                    form.Controls.Add(piece_imageboxes[i, k]);
                    piece_imageboxes[i, k].Parent = boardbox;
                    piece_imageboxes[i, k].BackColor = Color.Transparent;

                }
            }

        }

        internal List<Point> TransformMovesBlack(List<Point> moves)
        {
            List<Point> newMoves = new List<Point>();
            for (int i = 0; i < moves.Count; i++)
            {
                int newX = 7 - moves[i].Y;
                int newY = moves[i].X;

                newMoves.Add(new Point(newX, newY));
            }
            return newMoves;
        }

        internal List<Point> TransformMovesWhite(List<Point> moves)
        {
            List<Point> newMoves = new List<Point>();
            for (int i = 0; i < moves.Count; i++)
            {
                int newX = moves[i].Y;
                int newY = 7 - moves[i].X;

                newMoves.Add(new Point(newX, newY));
            }
            return newMoves;
        }

        public void ClearBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    piece_imageboxes[i, j].Image = null;
                }
            }
        }

        public void PositionFromFEN(string fen, PieceColor playerColor)
        {
            ClearBoard();
            int file = 0;
            int rank = 7;
            string picturePath;
            string piecePlacement = fen.Split(' ')[0];
            int counter = 0;
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
                        switch (fen[counter])
                        {
                            case 'k':
                                picturePath = "/Images/Gb.png";
                                break;
                            case 'K':
                                picturePath = "/Images/Gw.png";
                                break;
                            case 'q':
                                picturePath = "/Images/Qb.png";
                                break;
                            case 'Q':
                                picturePath = "/Images/Qw.png";
                                break;
                            case 'r':
                                picturePath = "/Images/Rb.png";
                                break;
                            case 'R':
                                picturePath = "/Images/Rw.png";
                                break;
                            case 'n':
                                picturePath = "/Images/Kb.png";
                                break;
                            case 'N':
                                picturePath = "/Images/Kw.png";
                                break;
                            case 'b':
                                picturePath = "/Images/Bb.png";
                                break;
                            case 'B':
                                picturePath = "/Images/Bw.png";
                                break;
                            case 'p':
                                picturePath = "/Images/Pb.png";
                                break;
                            case 'P':
                                picturePath = "/Images/Pw.png";
                                break;
                            default:
                                picturePath = "/Images/Gw.png";
                                break;
                        }
                        int newrank;
                        int newfile;
                        if (playerColor == PieceColor.White)
                        {
                            // Darstellung für Weiß
                            newrank = file;
                            newfile = 7 - rank;
                            piece_imageboxes[newfile, newrank].Image = Image.FromFile(projectDirectory + picturePath);
                        }
                        else
                        {
                            // Darstellung für Schwarz
                            newrank = 7 - file;
                            newfile = rank;
                            piece_imageboxes[newfile, newrank].Image = Image.FromFile(projectDirectory + picturePath);
                        }
                        file++;
                    }
                }
                counter++;
            }
        }

        public void NextMoveMade()
        {
            HidePossibleMoves();
            piece_imageboxes[currentClick.Y, currentClick.X].BackColor = Color.LightGreen;
            piece_imageboxes[lastClick.Y, lastClick.X].BackColor = Color.LightGreen;
        }

        public void ChessUI_Click(object sender, EventArgs e)
        {
            // x und y der geklickten Picturebox bestimmen
            PictureBox box = (PictureBox)sender;
            int pY = box.Location.Y / box.Size.Height;
            int pX = box.Location.X / box.Size.Width;
            Console.WriteLine("Y: " + pY + "  X: " + pX);
            Debug.Assert(UIdebug.CheckCoords(pY, pX));

            // Funktion wird abgebrochen wenn Leeres Feld angedrückt wird, ohne eine ausgewählte Figur, oder wenn selbes Feld 2 mal gedrückt wird
            if (box.Image == null && piece_selected == false || (pX == currentClick.X && pY == currentClick.Y))
            {
                piece_imageboxes[currentClick.Y, currentClick.X].BackColor = Color.Transparent;
                HidePossibleMoves();
                piece_selected = false;
                return;
            }

            // nächsten Zug speichern
            penultimateClick = lastClick;
            lastClick = currentClick;
            currentClick = new Point(pX, pY);

            // clickhandler übergibt alle nötigen daten an die Chessengine
            clickHandler(pY, pX, piece_selected);

            // Neuen Klick speichern
            if (piece_selected)
            {
                // Highlighting, beim Klicken
                piece_imageboxes[lastClick.Y, lastClick.X].BackColor = Color.Transparent;
                piece_imageboxes[penultimateClick.Y, penultimateClick.X].BackColor = Color.Transparent;

                piece_selected = false;
            }
            else
            {
                piece_selected = true;
                box.BackColor = Color.FromArgb(230, 230, 100);
            }
        }

        public Point[] GetLastTwoPointsClicked()
        {
            Point[] points = new Point[2];
            points[0] = currentClick;
            points[1] = lastClick;
            return points;
        }

        public void ShowPossibleMoves(System.Collections.Generic.List<System.Drawing.Point> moves)
        {
            // Hintergrundfarben fürs Highlight ändern
            foreach (Point move in moves)
            {
                if (move.Y % 2 == 0 ^ move.X % 2 == 0)
                {
                    piece_imageboxes[move.Y, move.X].BackColor = dark_highlight;
                }
                else
                {
                    piece_imageboxes[move.Y, move.X].BackColor = bright_highlight;
                }
            }
        }

        public void HidePossibleMoves()
        {
            foreach (PictureBox box in piece_imageboxes)
            {
                box.BackColor = Color.Transparent;
            }
        }
        public void HideMenu()
        {
            form.Controls.Remove(boardbox);
        }
    }
}