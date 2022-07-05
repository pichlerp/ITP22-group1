using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chess;

namespace Chess_UI
{
    class InGameMenu
    {
        Button return_to_menu_button = new Button();
        Point menu_point;
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        int distance_from_board = 50;
        int promotion_menu_distance = 100;
        ChessUI UI;
        GameWindow window;
        Engine engine;
        char option = 'x';

        int a, b, c, d;
        PieceColor pc;

        // Promotion Menu
        PictureBox[] promotion_boxes = new PictureBox[4];

        public InGameMenu(ref ChessUI inUI, GameWindow inwindow, ref Engine in_engine)
        {
            engine = in_engine;
            UI = inUI;
            window = inwindow;
            menu_point = new Point(UI.getLocation().X + UI.getSize().X + distance_from_board, UI.getLocation().Y);

            return_to_menu_button.Click += returnToMenu;
            return_to_menu_button.Location = menu_point;
            return_to_menu_button.Size = new Size(200, 50);
            return_to_menu_button.Text = "Zurück zum Menü";
            return_to_menu_button.Font = new Font("Candara Bold", 12, FontStyle.Regular);
            return_to_menu_button.ForeColor = Color.FromArgb(223, 223, 223);
            return_to_menu_button.FlatStyle = FlatStyle.Flat;

            for (int i = 0; i < 4; i++)
            {
                promotion_boxes[i] = new PictureBox();
                promotion_boxes[i].BackColor = Color.Gray;
                promotion_boxes[i].Size = new Size(100, 100);
                promotion_boxes[i].Location = new Point(menu_point.X, menu_point.Y + i * 100 + promotion_menu_distance);
                promotion_boxes[i].Click += promotionClick;
            }
        }

        private void promotionClick(object sender, EventArgs e)
        {
            PictureBox box = (PictureBox)sender;
            int i = (box.Location.Y - menu_point.Y - promotion_menu_distance) / 100;

            Console.WriteLine("box: " + i);

            switch (i)
            {
                case 0:
                    option = 'q';
                    break;
                case 1:
                    option = 'n';
                    break;
                case 2:
                    option = 'r';
                    break;
                case 3:
                    option = 'b';
                    break;
                default:
                    option = 'q';
                    break;
            }

            hidePromotionMenu();

            engine.promotionoption = option;

            engine.MakeMove(a, b, c, d, MoveType.Default);

            if (pc == PieceColor.Black)
            {
                engine.IncrementTurncounter();
            }

            window.endOfMove(pc);
        }

        public void showPromotionMenu(int ia, int ib, int ic, int id, PieceColor ie)
        {
            a = ia; b = ib; c = ic; d = id; pc = ie;

            PieceColor color = engine.GetTurnColor();
            if(color == PieceColor.Black)
            {
                promotion_boxes[0].Image = Chess_UI.Properties.Resources.bq;
                promotion_boxes[1].Image = Chess_UI.Properties.Resources.bn;
                promotion_boxes[2].Image = Chess_UI.Properties.Resources.br;
                promotion_boxes[3].Image = Chess_UI.Properties.Resources.bb;
            }
            else if(color == PieceColor.White)
            {
                promotion_boxes[0].Image = Chess_UI.Properties.Resources.wq;
                promotion_boxes[1].Image = Chess_UI.Properties.Resources.wn;
                promotion_boxes[2].Image = Chess_UI.Properties.Resources.wr;
                promotion_boxes[3].Image = Chess_UI.Properties.Resources.wb;
            }        
           
            for (int i = 0; i < 4; i++)
            {
                window.Controls.Add(promotion_boxes[i]);
            }
        }

        public void hidePromotionMenu()
        {
            for (int i = 0; i < 4; i++)
            {
                window.Controls.Remove(promotion_boxes[i]);
            }
        }

        public void show()
        {
            UI.form.Controls.Add(return_to_menu_button);
        }

        public void hide()
        {
            UI.form.Controls.Remove(return_to_menu_button);
            UI.HideRanksAndFiles();
        }

        private void returnToMenu(object sender, EventArgs e)
        {
            hidePromotionMenu();
            window.returnToMenu();
        }
    }
}
