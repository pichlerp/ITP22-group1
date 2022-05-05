using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

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
        static Color bright = Color.FromArgb(255,240,100);
        Color dark_highlight = Color.FromArgb(Math.Min((int)((double)dark.R * 2), 255), Math.Min((int)((double)dark.G * 2), 255), Math.Min((int)((double)dark.B * 2), 255));
        Color bright_highlight = Color.FromArgb(Math.Min((int)((double)bright.R * 2), 255), Math.Min((int)((double)bright.G * 2), 255), Math.Min((int)((double)bright.B * 2), 255));
        bool piece_selected;
        Action<int, int, bool> clickHandler;
        Form form;
        Bitmap board;
        PictureBox boardbox;
        PictureBox[,] piece_imageboxes = new PictureBox[8, 8];
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
            boardbox = new PictureBox();
            boardbox.Location = new Point(100, 100);
            boardbox.Size = new Size(width, height);
            form.Controls.Add(boardbox);

            // Brett schwarz/weiß karriert machen
            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < height; k++)
                {
                    if (!(i / 100 % 2 == 0 ^ k / 100 % 2 == 0))
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
                    piece_imageboxes[i, k] = new PictureBox();
                    piece_imageboxes[i, k].Size = new Size(100, 100);
                    piece_imageboxes[i, k].Location = new Point(k * 100, i * 100);
                    piece_imageboxes[i, k].Click += ChessUI_Click;
                    form.Controls.Add(piece_imageboxes[i, k]);
                    piece_imageboxes[i, k].Parent = boardbox;
                    piece_imageboxes[i, k].BackColor = Color.Transparent;
                }
            }

            // Bilder in Figurenpictureboxes einfügen
            piece_imageboxes[0, 4].Image = Image.FromFile(projectDirectory + "/Images/Gb.png");
            piece_imageboxes[7, 4].Image = Image.FromFile(projectDirectory + "/Images/Gw.png");

            piece_imageboxes[0, 3].Image = Image.FromFile(projectDirectory + "/Images/Qb.png");
            piece_imageboxes[7, 3].Image = Image.FromFile(projectDirectory + "/Images/QW.png");

            piece_imageboxes[0, 0].Image = Image.FromFile(projectDirectory + "/Images/Rb.png");
            piece_imageboxes[0, 7].Image = Image.FromFile(projectDirectory + "/Images/Rb.png");
            piece_imageboxes[7, 0].Image = Image.FromFile(projectDirectory + "/Images/Rw.png");
            piece_imageboxes[7, 7].Image = Image.FromFile(projectDirectory + "/Images/Rw.png");

            piece_imageboxes[0, 1].Image = Image.FromFile(projectDirectory + "/Images/Kb.png");
            piece_imageboxes[0, 6].Image = Image.FromFile(projectDirectory + "/Images/Kb.png");
            piece_imageboxes[7, 1].Image = Image.FromFile(projectDirectory + "/Images/Kw.png");
            piece_imageboxes[7, 6].Image = Image.FromFile(projectDirectory + "/Images/Kw.png");

            piece_imageboxes[0, 2].Image = Image.FromFile(projectDirectory + "/Images/Bb.png");
            piece_imageboxes[0, 5].Image = Image.FromFile(projectDirectory + "/Images/Bb.png");
            piece_imageboxes[7, 2].Image = Image.FromFile(projectDirectory + "/Images/Bw.png");
            piece_imageboxes[7, 5].Image = Image.FromFile(projectDirectory + "/Images/Bw.png");

            // Bilder in bauernpictureboxes einfügen
            for (int i = 0; i < 8; i++)
            {
                piece_imageboxes[1, i].Image = Image.FromFile(projectDirectory + "/Images/Pb.png");
                piece_imageboxes[6, i].Image = Image.FromFile(projectDirectory + "/Images/Pw.png");
            }
        }

        public void ChessUI_Click(object sender, EventArgs e)
        {
            // x und y der geklickten Picturebox bestimmen
            PictureBox box = (PictureBox)sender;
            int pY = box.Location.Y / box.Size.Height;
            int pX = box.Location.X / box.Size.Width;
            Console.WriteLine("Y: " + pY + "  X: " + pX);
            Debug.Assert(UIdebug.checkCoords(pY, pX));

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
                move(lastClick.Y, lastClick.X, currentClick.Y, currentClick.X);
                piece_imageboxes[currentClick.Y, currentClick.X].BackColor = Color.LightGreen;
                piece_imageboxes[lastClick.Y, lastClick.X].BackColor = Color.LightGreen;

                hidePossibleMoves();
            }
            else
            {
                piece_selected = true;
                box.BackColor = Color.Green;
            }
        }

        public void move(int y_from, int x_from, int y_to, int x_to)
        {
            Debug.Assert(UIdebug.checkCoords(y_from, x_from, y_to, x_to));

            // Zug wird ausgeführt durch Tauschen der Bilder
            piece_imageboxes[y_to, x_to].Image = piece_imageboxes[y_from, x_from].Image;
            piece_imageboxes[y_from, x_from].Image = null;
        }

        public Point[] getLastTwoPointsClicked()
        {
            Point[] points = new Point[2];
            points[0] = currentClick;
            points[1] = lastClick;
            return points;
        }

        public void showPossibleMoves(Point[] moves)
        {
            // Hintergrundfarben fürs Highlight ändern
            foreach(Point move in moves)
            {
                if(move.Y % 2 == 0 ^ move.X % 2 == 0)
                {
                    piece_imageboxes[move.Y, move.X].BackColor = dark_highlight;
                }
                else
                {
                    piece_imageboxes[move.Y, move.X].BackColor = bright_highlight;
                }
            }
        }

        public void hidePossibleMoves()
        {
            foreach(PictureBox box in piece_imageboxes)
            {
                box.BackColor = Color.Transparent;
            }
        }
    }
}