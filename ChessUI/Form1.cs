using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        int form_width = 1000;
        int form_height = 1000;
        Color backgroundcolor = Color.FromArgb(220, 155, 55);
        ChessUI UI;
        ChessMenu Menu;

        public Form1()
        {
            InitializeComponent();
            initializeForm();
        }

        private void initializeForm()
        {
            this.BackColor = backgroundcolor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(form_width, form_height);

            loadMenu();
        }

        private void loadMenu()
        {
            Menu = new ChessMenu(this, menuButtonPress);
        }

        private void menuButtonPress(object sender, EventArgs e)
        {
            Menu.hideMenu();
            UI = new ChessUI(this, clickHandler);
        }

        private void clickHandler() // für den Clickhandler kann auch eine eigene Klasse erstellt werden
        {
            Console.WriteLine("Clickhandler not implemented yet");
        }
    }
}
