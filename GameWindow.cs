using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using static Chess_UI.Engine;
using Chess_UI;

namespace Chess
{
    public partial class GameWindow : Form
    {
        int form_width = 1000;
        int form_height = 1000;
        Color backgroundcolor = Color.FromArgb(10, 10, 10); //Color.FromArgb(126, 128, 128);
        ChessUI UI;
        new ChessMenu Menu;
        static Chess_UI.Engine TheEngine = new Chess_UI.Engine();
        Perft perft = new Perft(ref TheEngine); // performance test, move path enumeration

        public GameWindow()
        {
            InitializeComponent();
            // Programmablauf fängt hier an
            InitializeForm();
            perft.countMovesToDepth(10);
        }

        private void InitializeForm()
        {
            this.BackColor = backgroundcolor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(form_width, form_height);

            LoadMenu();
        }
        Chess_UI.PieceColor playerColor;
        private void LoadMenu()
        {
            Menu = new ChessMenu(this, MenuStartButtonPressW, MenuStartButtonPressB);
        }
        
        private void MenuStartButtonPressW(object sender, EventArgs e)
        {
            playerColor = Chess_UI.PieceColor.White;
            Menu.HideMenu();
            UI = new ChessUI(this, ClickHandler);
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
        }

        private void MenuStartButtonPressB(object sender, EventArgs e)
        {
            playerColor = Chess_UI.PieceColor.Black;
            Menu.HideMenu();
            UI = new ChessUI(this, ClickHandler);
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
        }

        private void play_buttonW_Click(object sender, EventArgs e)
        {
            Console.WriteLine(sender);
            Menu.HideMenu();
            UI = new ChessUI(this, ClickHandler);
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
        }

        int selectedY, selectedX;
        private void ClickHandler(int y, int x, bool piece_selected)
        {
            List<Move> moves = TheEngine.GenerateMoves();
            
            // Je nach Spielerfarbe werden Koordinaten transformiert
            if(playerColor == Chess_UI.PieceColor.White)
            {
                int temp = y;
                y = x;
                x = 7 - temp;
            }
            else
            {
                int temp = x;
                x = y;
                y = 7 - temp;
            }

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
                TheEngine.GetPossibleMoves(x, y, moves, ref possibleMoves);
                // Koordinaten der erhaltenen Züge müssen an Koordinatensystem der Spielfarbe angepasst werden
                if (playerColor == Chess_UI.PieceColor.White)
                {
                    UI.ShowPossibleMoves(UI.TransformMovesWhite(possibleMoves));
                }
                else
                {
                    UI.ShowPossibleMoves(UI.TransformMovesBlack(possibleMoves));
                }
            }
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
            TheEngine.GetTheBoard();
        }
    }
}
