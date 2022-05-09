using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using static Chess_UI.Engine;

namespace Chess
{
    public partial class Form1 : Form
    {
        int form_width = 1000;
        int form_height = 1000;
        Color backgroundcolor = Color.FromArgb(220, 155, 55);
        ChessUI UI;
        ChessMenu Menu;
        Chess_UI.Engine TheEngine = new Chess_UI.Engine();

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

        int selectedY, selectedX;
        private void clickHandler(int y, int x, bool piece_selected)
        {
            List<Move> moves = TheEngine.GenerateMoves();
            if (piece_selected)
            {
                if (TheEngine.IsValidMove(selectedX, selectedY, x, y))
                {
                    UI.acceptMove = true;
                    TheEngine.MakeMove(selectedX, selectedY, x, y);
                }
                UI.hidePossibleMoves();
            }
            else
            {
                if(!TheEngine.ValidColorSelected(x, y))
                {
                    return;
                }
                selectedX = x;
                selectedY = y;
                List<Point> possibleMoves = new List<Point>();
                TheEngine.GetPossibleMoves(x, y, ref possibleMoves);
                // dummydaten
                //possible_moves = new Point[5];
                //for (int i = 0; i < 5; i++)
                //{
                //    possible_moves[i] = new Point(1, i + 2);
                //}

                UI.showPossibleMoves(possibleMoves);
            }
            TheEngine.GetTheBoard();
        }
    }
}
