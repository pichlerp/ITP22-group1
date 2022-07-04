using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Text;
using System.IO;
using Chess_UI;

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
        Form form;
        public Button play_ai_bt;
        Button play_buttonW;
        Button play_buttonB;
        Button exit_button;
        Button perft_button;
        Label credits_label;
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
            credits_label = new Label();
            form.Controls.Add(credits_label);
            credits_label.Parent = menu_box;
            credits_label.Text = "Peter Pichler, Gabriel Helm, Gerrit Kreuzer und Pavels Tuliss";
            credits_label.Font = new Font("Candara Bold", 8, FontStyle.Regular);
            credits_label.Size = new Size(400, 20);
            credits_label.ForeColor = Color.FromArgb(223, 223, 223);
            credits_label.AutoSize = false;
            credits_label.TextAlign = ContentAlignment.MiddleCenter;
            credits_label.Location = new Point(menu_box_width / 2 - main_menu_label.Size.Width / 2, 470);

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

            exit_button = new Button();
            form.Controls.Add(exit_button);
            exit_button.Click += exitClick;
            exit_button.Size = new Size(60, 40);
            exit_button.Parent = menu_box;
            exit_button.Location = new Point(menu_box_width - exit_button.Width - 10, menu_box_height - exit_button.Height - 10);
            exit_button.Text = "Exit";
            exit_button.Font = new Font("Candara Bold", 12, FontStyle.Regular);
            exit_button.ForeColor = Color.FromArgb(223, 223, 223);
            exit_button.FlatStyle = FlatStyle.Flat;

            perft_button = new Button();
            form.Controls.Add(perft_button);
            perft_button.Click += perftClick;
            perft_button.Size = new Size(75, 40);
            perft_button.Parent = menu_box;
            perft_button.Location = new Point(10, menu_box_height - exit_button.Height - 10);
            perft_button.Text = "PERFT";
            perft_button.Font = new Font("Candara Bold", 12, FontStyle.Regular);
            perft_button.ForeColor = Color.FromArgb(223, 223, 223);
            perft_button.FlatStyle = FlatStyle.Flat;
        }
        public void exitClick(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
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
        public void perftClick(object sender, EventArgs e)
        {
            MessageBox.Show("Bitte zur Konsole wechseln!\nMehr Informationen zu PERFT in Benutzeranleitung.");
            Perft perft = new Perft();
            string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            Console.WriteLine("PERFormance Test:");
            while (true)
            {
                Console.WriteLine("1. Start PERFT\n2. Position ändern\n3. Beenden");
                string s = Console.ReadLine();
                if (s == "1")
                {
                    Console.Write("Tiefe eingeben: ");
                    int input = Convert.ToInt32(Console.ReadLine());
                    perft.countMovesToDepth(input, FEN);
                }
                if (s == "2")
                {
                    Console.Write("FEN der gewünschten Position eingeben: ");
                    FEN = Console.ReadLine();
                }
                if (s == "3")
                {
                    Console.WriteLine("Bitte wieder zum Fenster wechseln.");
                    break;
                }
            }
        }
    }
}
