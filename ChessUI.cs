using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace Chess
{
    class ChessUI
    {
        static int height = 800;
        static int width = 800;
        Point lastClick;
        Point currentClick;
        Point penultimateClick;
        static Color dark = Color.DarkSlateGray;
        static Color bright = Color.FromArgb(255, 240, 100);
        Color dark_highlight = Color.FromArgb(Math.Min((int)((double)dark.R * 2), 255), Math.Min((int)((double)dark.G * 2), 255), Math.Min((int)((double)dark.B * 2), 255));
        Color bright_highlight = Color.FromArgb(Math.Min((int)((double)bright.R * 2), 255), Math.Min((int)((double)bright.G * 2), 255), Math.Min((int)((double)bright.B * 2), 255));
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
                        board.SetPixel(i, k, bright);
                    }
                    else
                    {
                        board.SetPixel(i, k, dark);
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

        public void PositionFromFEN(string fen)
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
                        piece_imageboxes[file, rank].Image = Image.FromFile(projectDirectory + picturePath);
                        file++;
                    }
                }
                counter++;
            }
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
            if (box.Image == null && piece_selected == false || (pX == currentClick.X && pY == currentClick.Y)) return;

            // Highlighting, beim Klicken
            if (piece_selected)
            {
                piece_imageboxes[lastClick.Y, lastClick.X].BackColor = Color.Transparent;
                piece_imageboxes[penultimateClick.Y, penultimateClick.X].BackColor = Color.Transparent;
            }

            // clickhandler übergibt alle nötigen daten an die Chessengine
            clickHandler(pY, pX, piece_selected);

            // Neuen Klick speichern
            penultimateClick = lastClick;
            lastClick = currentClick;
            currentClick = new Point(pX, pY);

            if (piece_selected)
            {
                piece_selected = false;
                piece_imageboxes[currentClick.Y, currentClick.X].BackColor = Color.LightGreen;
                piece_imageboxes[lastClick.Y, lastClick.X].BackColor = Color.LightGreen;

                HidePossibleMoves();
            }
            else
            {
                piece_selected = true;
                box.BackColor = Color.Green;
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
    }
}