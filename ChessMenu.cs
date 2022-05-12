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
        Color menu_backcolor = Color.FromArgb(132, 86, 18);
        PictureBox menu_box;
        EventHandler buttonPress;
        Form form;
        Button play_button; // platzhalterbutton, bis mehr funktinalität da ist
        Label main_menu_label;

        public ChessMenu(Form in_form, EventHandler handler)
        {
            buttonPress = handler;
            form = in_form;
            menu_box = new PictureBox
            {
                Size = new Size(menu_box_width, menu_box_height),
                Location = new Point(menu_x, menu_y),
                BackColor = menu_backcolor
            };
            form.Controls.Add(menu_box);

            main_menu_label = new Label();
            form.Controls.Add(main_menu_label);
            main_menu_label.Parent = menu_box;
            main_menu_label.Text = "Chess";
            main_menu_label.Font = new Font("Broadway", 30, FontStyle.Bold);
            main_menu_label.Size = new Size(200, 50);
            main_menu_label.AutoSize = false;
            main_menu_label.TextAlign = ContentAlignment.MiddleCenter;
            main_menu_label.Location = new Point(menu_box_width / 2 - main_menu_label.Size.Width / 2, 30);

            play_button = new Button();
            form.Controls.Add(play_button);
            play_button.Click += buttonPress;
            play_button.Size = new Size(300, 50);
            play_button.Parent = menu_box;
            play_button.Location = new Point(menu_box_width / 2 - play_button.Width / 2, 150);
            play_button.Text = "Press to start Game";
            play_button.Font = new Font("Ariel", 16, FontStyle.Bold);
            play_button.FlatStyle = FlatStyle.Flat;
        }

        public void HideMenu()
        {
            form.Controls.Remove(menu_box);
        }
    }
}
