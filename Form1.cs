using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Chess_UI;

namespace Chess
{
    public partial class Form1 : Form
    {
        int form_width = 1000;
        int form_height = 1000;
        ChessUI UI;

        public Form1()
        {
            InitializeComponent();
            initializeForm();
        }

        private void initializeForm()
        {
            this.BackColor = Color.FromArgb(180, 80, 0);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(form_width, form_height);

            UI = new ChessUI(this, Clickhandler);
        }
    
        private void Clickhandler() // für den Clickhandler kann auch eine eigene Klasse erstellt werden
        {
            Console.WriteLine("Clickhandler not implemented yet");
        }
    }
}
