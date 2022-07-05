using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Drawing2D;


namespace Chess_UI
{
    class ChessUI
    {

        static int height = Screen.PrimaryScreen.Bounds.Height - 200 > 800 ? 800 : Screen.PrimaryScreen.Bounds.Height - 200;
        static int width = height;

        static int findmiddlewidth = (Screen.PrimaryScreen.Bounds.Width - width) / 2;
        static int findmiddleheight = (Screen.PrimaryScreen.Bounds.Height - height - 62) / 2;

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

        Label[] files = new Label[8];
        Label[] ranks = new Label[8];

        public ChessUI(Form inputform, Action<int, int, bool> inputClickhandler, Chess_UI.PieceColor color)
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
                Location = new Point(findmiddlewidth, findmiddleheight),
                Size = new Size(width, height)
            };
            form.Controls.Add(boardbox);
            // Brett schwarz/weiß karriert machen
            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < height; k++)
                {
                    if ((i / (width / 8) % 2 == 0 ^ k / (height / 8) % 2 == 0))
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

            // Reihen und Linien werden beschriftet, erfordert Unterscheidung, ob Brett von Weiß oder Schwarz aus betrachtet wird
            if (color == Chess_UI.PieceColor.White)
            {
                // Betrachtung von Weiß: Beschriftung der Reihen von a aufsteigend (links nach rechts); Beschriftung der Zeilen von 8 absteigend (oben nach unten)
                char file = 'a';
                int rank = 8;
                for (int i = 0; i < 8; i++)
                {
                    files[i] = new Label();
                    form.Controls.Add(files[i]);
                    files[i].Parent = form;
                    files[i].Text = file.ToString();
                    file++;
                    files[i].Font = new Font("Candara Bold", 24, FontStyle.Regular);
                    files[i].Size = new Size(50, 50);
                    files[i].ForeColor = Color.FromArgb(223, 223, 223);
                    files[i].AutoSize = false;
                    files[i].TextAlign = ContentAlignment.MiddleCenter;
                    files[i].Location = new Point(findmiddlewidth + width / 16 - 25 + i * width / 8, findmiddleheight + height);

                    ranks[i] = new Label();
                    form.Controls.Add(ranks[i]);
                    ranks[i].Parent = form;
                    ranks[i].Text = rank.ToString();
                    rank--;
                    ranks[i].Font = new Font("Candara Bold", 24, FontStyle.Regular);
                    ranks[i].Size = new Size(50, 50);
                    ranks[i].ForeColor = Color.FromArgb(223, 223, 223);
                    ranks[i].AutoSize = false;
                    ranks[i].TextAlign = ContentAlignment.MiddleCenter;
                    ranks[i].Location = new Point(findmiddlewidth - width / 16, findmiddleheight + height / 16 - 25 + i * height / 8);
                }
            }
            // Betrachtung von Schwarz: Beschriftung der Reihen von h absteigend (links nach rechts); Beschriftung der Zeilen von 1 aufsteigend (oben nach unten)
            else if (color == Chess_UI.PieceColor.Black)
            {
                char file = 'h';
                int rank = 1;
                for (int i = 0; i < 8; i++)
                {
                    files[i] = new Label();
                    form.Controls.Add(files[i]);
                    files[i].Parent = form;
                    files[i].Text = file.ToString();
                    file--;
                    files[i].Font = new Font("Candara Bold", 24, FontStyle.Regular);
                    files[i].Size = new Size(50, 50);
                    files[i].ForeColor = Color.FromArgb(223, 223, 223);
                    files[i].AutoSize = false;
                    files[i].TextAlign = ContentAlignment.MiddleCenter;
                    files[i].Location = new Point(findmiddlewidth + width / 16 - 25 + i * width / 8, findmiddleheight + height);

                    ranks[i] = new Label();
                    form.Controls.Add(ranks[i]);
                    ranks[i].Parent = form;
                    ranks[i].Text = rank.ToString();
                    rank++;
                    ranks[i].Font = new Font("Candara Bold", 24, FontStyle.Regular);
                    ranks[i].Size = new Size(50, 50);
                    ranks[i].ForeColor = Color.FromArgb(223, 223, 223);
                    ranks[i].AutoSize = false;
                    ranks[i].TextAlign = ContentAlignment.MiddleCenter;
                    ranks[i].Location = new Point(findmiddlewidth - width / 16, findmiddleheight + height / 16 - 25 + i * height / 8);
                }
            }

            // Pictureboxes der Figuren initialisieren
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    piece_imageboxes[i, k] = new PictureBox
                    {
                        Size = new Size((width / 8), (height / 8)),
                        Location = new Point(k * (width / 8), i * (height / 8))
                    };
                    piece_imageboxes[i, k].Click += ChessUI_Click;
                    form.Controls.Add(piece_imageboxes[i, k]);
                    piece_imageboxes[i, k].Parent = boardbox;
                    piece_imageboxes[i, k].BackColor = Color.Transparent;

                }
            }

        }

        // Wenn ins Menü gewechselt wird, müssen die Beschriftungen entfernt werden
        internal void HideRanksAndFiles()
        {
            for (int i = 0; i < 8; i++)
            {
                this.form.Controls.Remove(files[i]);
                this.form.Controls.Remove(ranks[i]);
            }
        }

        public void Hide()
        {
            form.Controls.Remove(boardbox);
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    form.Controls.Remove(piece_imageboxes[i, k]);
                }
            }
        }

        public Point getSize()
        {
            return new Point(width, height);
        }

        public Point getLocation()
        {
            return new Point(findmiddlewidth, findmiddleheight);
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
            Image picture = null;
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
                                picture = Chess_UI.Properties.Resources.bk;
                                break;
                            case 'K':
                                picture = Chess_UI.Properties.Resources.wk;
                                break;
                            case 'q':
                                picture = Chess_UI.Properties.Resources.bq;
                                break;
                            case 'Q':
                                picture = Chess_UI.Properties.Resources.wq;
                                break;
                            case 'r':
                                picture = Chess_UI.Properties.Resources.br;
                                break;
                            case 'R':
                                picture = Chess_UI.Properties.Resources.wr;
                                break;
                            case 'n':
                                picture = Chess_UI.Properties.Resources.bn;
                                break;
                            case 'N':
                                picture = Chess_UI.Properties.Resources.wn;
                                break;
                            case 'b':
                                picture = Chess_UI.Properties.Resources.bb;
                                break;
                            case 'B':
                                picture = Chess_UI.Properties.Resources.wb;
                                break;
                            case 'p':
                                picture = Chess_UI.Properties.Resources.bp;
                                break;
                            case 'P':
                                picture = Chess_UI.Properties.Resources.wp;
                                break;
                            default:
                                picture = Chess_UI.Properties.Resources.wp;
                                break;
                        }
                        int newrank;
                        int newfile;
                        if (playerColor == PieceColor.White)
                        {
                            // Darstellung für Weiß
                            newrank = file;
                            newfile = 7 - rank;
                            piece_imageboxes[newfile, newrank].Image = resizeImage(picture, width / 8, height / 8);
                        }
                        else
                        {
                            // Darstellung für Schwarz
                            newrank = 7 - file;
                            newfile = rank;
                            piece_imageboxes[newfile, newrank].Image = resizeImage(picture, width / 8, height / 8);
                        }
                        file++;
                    }
                }
                counter++;
            }
        }
        public static Image resizeImage(Image image, int new_height, int new_width)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage((Image)new_image);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return new_image;
        }

        public void NextMoveMade()
        {
            HidePossibleMoves();
            piece_imageboxes[currentClick.Y, currentClick.X].BackColor = Color.FromArgb(230, 230, 100);
            piece_imageboxes[lastClick.Y, lastClick.X].BackColor = Color.FromArgb(230, 230, 100);
        }

        public void ChessUI_Click(object sender, EventArgs e)
        {
            // x und y der geklickten Picturebox bestimmen
            PictureBox box = (PictureBox)sender;
            int pY = box.Location.Y / box.Size.Height;
            int pX = box.Location.X / box.Size.Width;
            Debug.Assert(UIdebug.CheckCoords(pY, pX));

            // Funktion wird abgebrochen wenn Leeres Feld angedrückt wird, ohne eine ausgewählte Figur
            if (box.Image == null && piece_selected == false)
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

        public void AiSelect(Point p)
        {
            penultimateClick = lastClick;
            lastClick = currentClick;
            currentClick = p;

            clickHandler(p.Y, p.X, piece_selected);

            if (piece_selected)
            {
                piece_imageboxes[currentClick.Y, currentClick.X].BackColor = Color.FromArgb(230, 230, 100);

                piece_imageboxes[lastClick.Y, lastClick.X].BackColor = Color.Transparent;
                piece_imageboxes[penultimateClick.Y, penultimateClick.X].BackColor = Color.Transparent;

                piece_selected = false;
            }
            else
            {
                piece_selected = true;
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