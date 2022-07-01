using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Text;
using System.IO;

namespace Chess
{
    class ChessMenu
    {
        static int menu_box_height = 500;
        static int menu_box_width = 700;
        int menu_x = 500 - menu_box_width / 2;
        int menu_y = 500 - menu_box_height / 2;
        static int findmiddlewidth = (Screen.PrimaryScreen.Bounds.Width - menu_box_width) / 2;
        static int findmiddleheight = (Screen.PrimaryScreen.Bounds.Height - menu_box_height - 62) / 2;
        public bool play_ai = false;
        Color menu_backcolor = Color.FromArgb(48, 52, 51);
        PictureBox menu_box;
        EventHandler buttonPressW;
        EventHandler buttonPressB;
        Button play_ai_bt;
        Form form;
        Button play_buttonW;
        Button play_buttonB;
        Label main_menu_label;

        public ChessMenu(Form in_form, EventHandler playW, EventHandler playB)
        {
            buttonPressW = playW;
            buttonPressB = playB;
            form = in_form;
            menu_box = new PictureBox
            {
                Size = new Size(menu_box_width, menu_box_height),
                Location = new Point(findmiddlewidth, findmiddleheight),
                BackColor = menu_backcolor
            };
            form.Controls.Add(menu_box);

            main_menu_label = new Label();
            form.Controls.Add(main_menu_label);
            main_menu_label.Parent = menu_box;
            main_menu_label.Text = "Chess";
            main_menu_label.Font = new Font("Ink Free", 64, FontStyle.Italic);
            main_menu_label.Size = new Size(400, 100);
            main_menu_label.ForeColor = Color.FromArgb(223, 223, 223);
            main_menu_label.AutoSize = false;
            main_menu_label.TextAlign = ContentAlignment.MiddleCenter;
            main_menu_label.Location = new Point(menu_box_width / 2 - main_menu_label.Size.Width / 2, 30);

            play_buttonW = new Button();
            form.Controls.Add(play_buttonW);
            play_buttonW.Click += buttonPressW;
            play_buttonW.Size = new Size(350, 50);
            play_buttonW.Parent = menu_box;
            play_buttonW.Location = new Point(menu_box_width / 2 - play_buttonW.Width / 2, 200);
            play_buttonW.Text = "Neues Spiel gegen Schwarz";
            play_buttonW.Font = new Font("Candara Bold", 16, FontStyle.Regular);
            play_buttonW.ForeColor = Color.FromArgb(223, 223, 223);
            play_buttonW.FlatStyle = FlatStyle.Flat;

            play_buttonB = new Button();
            form.Controls.Add(play_buttonB);
            play_buttonB.Click += buttonPressB;
            play_buttonB.Size = new Size(350, 50);
            play_buttonB.Parent = menu_box;
            play_buttonB.Location = new Point(menu_box_width / 2 - play_buttonB.Width / 2, 300);
            play_buttonB.Text = "Neues Spiel gegen Weiß";
            play_buttonB.Font = new Font("Candara Bold", 16, FontStyle.Regular);
            play_buttonB.ForeColor = Color.FromArgb(223, 223, 223);
            play_buttonB.FlatStyle = FlatStyle.Flat;

            play_ai_bt = new Button();
            form.Controls.Add(play_ai_bt);
            play_ai_bt.Click += playAI;
            play_ai_bt.Size = new Size(350, 50);
            play_ai_bt.Parent = menu_box;
            play_ai_bt.Location = new Point(menu_box_width / 2 - play_buttonB.Width / 2, 400);
            play_ai_bt.Text = "Spiel gegen KI";
            play_ai_bt.Font = new Font("Candara Bold", 16, FontStyle.Regular);
            play_ai_bt.ForeColor = Color.FromArgb(223, 223, 223);
            play_ai_bt.FlatStyle = FlatStyle.Flat;
        }
        public void playAI(object sender, EventArgs e)
        {
            play_ai_bt.Text = "Farbe wählen";
            play_ai_bt.Enabled = false;
            play_ai = true;
        }
        public void HideMenu()
        {
            form.Controls.Remove(menu_box);
        }
        public void ShowMenu()
        {
            form.Controls.Add(menu_box);
        }
    }
}
