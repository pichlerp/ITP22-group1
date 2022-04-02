using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Chess_UI
{
    class ChessUI
    {
        static int height = 800;
        static int width = 800;
        Point lastClick;
        Point currentClick;
        bool pieceSelected;
        Action clickHandler;
        Form form;
        Bitmap board;
        PictureBox boardbox;
        PictureBox[,] piece_imageboxes = new PictureBox[8, 8];
        // Bilder müssen in projectDirectory/Images gespeichert werden
        // Name der Bilder muss erster Großbuchstabe der englischen Figurenname + 'w' oder 'b' (black/white) sein, außer König hat 'G' als Buchstabe
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

        public ChessUI(Form inputform, Action inputClickhandler)
        {
            form = inputform;
            clickHandler = inputClickhandler;
            lastClick = new Point(-1, -1);
            currentClick = new Point(-1, -1);
            pieceSelected = false;

            board = new Bitmap(width, height);
            boardbox = new PictureBox();

            boardbox.Location = new Point(100, 100);
            boardbox.Size = new Size(width, height);
            form.Controls.Add(boardbox);

            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < height; k++)
                {
                    if (i / 100 % 2 == 0 ^ k / 100 % 2 == 0)
                    {
                        board.SetPixel(i, k, Color.LightYellow);
                    }
                    else
                    {
                        board.SetPixel(i, k, Color.DarkSlateGray);
                    }
                }
            }
            boardbox.Image = board;

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

            for (int i = 0; i < 8; i++)
            {
                piece_imageboxes[1, i].Image = Image.FromFile(projectDirectory + "/Images/Pb.png");
                piece_imageboxes[6, i].Image = Image.FromFile(projectDirectory + "/Images/Pw.png");
            }
        }

        public void ChessUI_Click(object sender, EventArgs e)
        {
            // clickhandler übergibt alle nötigen daten an die Chessengine
            // muss um die entsprechenden parameter erweitert werden
            // clickHandler sollte wahrscheinlich auch einen Rückgabewert haben, um bei illegalen zügen die funktion zu beenden
            clickHandler();

            PictureBox box = (PictureBox)sender;

            if (box.Image == null && pieceSelected == false) return;

            int pY = box.Location.Y / box.Size.Height;
            int pX = box.Location.X / box.Size.Width;
            Console.WriteLine(pY + "  " + pX);

            lastClick = currentClick;
            currentClick = new Point(pX, pY);

            if (pieceSelected)
            {
                pieceSelected = false;
                piece_imageboxes[lastClick.Y, lastClick.X].BackColor = Color.Transparent;
                if (lastClick == currentClick) return;
                move(lastClick.Y, lastClick.X, currentClick.Y, currentClick.X);
            }
            else
            {
                pieceSelected = true;
                box.BackColor = Color.LightGreen;
            }
        }

        public void move(int y_from, int x_from, int y_to, int x_to)
        {
            Debug.Assert(y_from >= 0 && y_from < 8 && x_from >= 0 && x_from < 8);
            Debug.Assert(y_to >= 0 && y_to < 8 && x_to >= 0 && x_to < 8);
            Debug.Assert(y_from != y_to || x_from != x_to);

            piece_imageboxes[y_to, x_to].Image = piece_imageboxes[y_from, x_from].Image;
            piece_imageboxes[y_from, x_from].Image = null;
        }
    }
}
