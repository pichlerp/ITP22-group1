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
        new ChessMenu Menu;
        Chess_UI.Engine TheEngine = new Chess_UI.Engine();

        public Form1()
        {
            InitializeComponent();
            // Programmablauf fängt hier an
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.BackColor = backgroundcolor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(form_width, form_height);

            LoadMenu();
        }

        private void LoadMenu()
        {
            Menu = new ChessMenu(this, MenuStartButtonPress);
        }

        private void MenuStartButtonPress(object sender, EventArgs e)
        {
            Menu.HideMenu();
            UI = new ChessUI(this, ClickHandler);
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN());
        }

        int selectedY, selectedX;
        private void ClickHandler(int y, int x, bool piece_selected)
        {
            List<Move> moves = TheEngine.GenerateMoves();
            // Übersetzung der Moves für die richtige Anzeige in der UI (Koordinatentransformation)
            if (piece_selected)
            {
                if (TheEngine.IsValidMove(selectedX, selectedY, x, y))
                {
                    TheEngine.MakeMove(selectedX, selectedY, x, y);
                }
                UI.HidePossibleMoves();
            }
            else
            {
                if (!TheEngine.ValidColorSelected(x, y))
                {
                    return;
                }
                selectedX = x;
                selectedY = y;
                List<Point> possibleMoves = new List<Point>();
                TheEngine.GetPossibleMoves(x, y, ref possibleMoves);
                UI.ShowPossibleMoves(possibleMoves);
            }
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN());
            TheEngine.GetTheBoard();
        }
    }
}
