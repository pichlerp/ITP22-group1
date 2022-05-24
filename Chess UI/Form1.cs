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
            // Programmablauf fängt hier an
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
            Menu = new ChessMenu(this, menuStartButtonPress);
        }

        private void menuStartButtonPress(object sender, EventArgs e)
        {
            Menu.hideMenu();
            UI = new ChessUI(this, clickHandler);
        }

        private void clickHandler(int y, int x, bool piece_selected)
        {
            if (piece_selected)
            {
                /*
                if (Engine.isValidMove(y, x)){
                    Engine.makeMove(y, x);
                }
                */

                UI.hidePossibleMoves();
            }
            else
            {
                Point[] possible_moves; // = Engine.getPossibleMoves(y, x);

                // dummydaten
                possible_moves = new Point[5];
                for (int i = 0; i < 5; i++)
                {
                    possible_moves[i] = new Point(1, i + 2);
                }

                UI.showPossibleMoves(possible_moves);
            }
        }
    }
}
