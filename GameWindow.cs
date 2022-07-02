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
        static ChessUI UI;
        bool playing_ai = false;
        PieceColor AIcolor = PieceColor.Empty;
        Chess_UI.PieceColor playerColor;
        new ChessMenu Menu;
        static Chess_UI.Engine TheEngine = new Chess_UI.Engine();
        AI ai;
        Perft perft = new Perft(ref TheEngine); // performance test, move path enumeration

        System.Media.SoundPlayer SFXmoveMade = new System.Media.SoundPlayer(Chess_UI.Properties.Resources.move_regular);
        System.Media.SoundPlayer SFXcheck = new System.Media.SoundPlayer(Chess_UI.Properties.Resources.move_check);
        System.Media.SoundPlayer SFXpieceTaken = new System.Media.SoundPlayer(Chess_UI.Properties.Resources.move_piecetaken);



        public GameWindow()
        {
            InitializeComponent();
            // Programmablauf fängt hier an
            InitializeForm();

            Timer timer = new Timer();
            timer.Interval = 500;
            timer.Tick += new EventHandler(CheckIfAIShouldMove);
            timer.Enabled = true;

            Console.WriteLine("Enter number as depth for PERFT or 'x' to skip: ");
            int depth;
            string input = "";
            while (input != "x")
            {
                input = Console.ReadLine();
                if (input == "x")
                {
                    break;
                }
                while (!Int32.TryParse(input, out depth))
                {
                    input = Console.ReadLine();
                    if (input == "x")
                    {
                        break;
                    }
                }
                perft.countMovesToDepth(depth);
            }

        }

        private void InitializeForm()
        {
            this.BackColor = backgroundcolor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(form_width, form_height);

            LoadMenu();
        }

        /*
        public void initAI(Chess_UI.PieceColor color)
        {
            ai = new AI(ref TheEngine, ref UI, color);
        }
        */

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
            playing_ai = Menu.play_ai;

            if (playing_ai)
            {
                ai = new AI(Chess_UI.PieceColor.Black);
                AIcolor = PieceColor.Black;
            }
        }

        private void MenuStartButtonPressB(object sender, EventArgs e)
        {
            playerColor = Chess_UI.PieceColor.Black;
            Menu.HideMenu();
            UI = new ChessUI(this, ClickHandler);
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
            playing_ai = Menu.play_ai;

            if (playing_ai)
            {
                ai = new AI(Chess_UI.PieceColor.White);
                AIcolor = PieceColor.White;
            }
        }

        private void play_buttonW_Click(object sender, EventArgs e)
        {
            Console.WriteLine(sender);
            Menu.HideMenu();
            UI = new ChessUI(this, ClickHandler);
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
        }

        int selectedY, selectedX;
        bool gameOver = false;
        private void ClickHandler(int y, int x, bool piece_selected)
        {
            TheEngine.UpdateChecksAndGameOver(TheEngine.FromPositionCreateFEN());

            // Wenn das Spiel vorbei ist, sollen Clicks verworfen werden.
            if (gameOver || TheEngine.GetTurnColor() == AIcolor)
            {
                return;
            }

            // Farbe des aktuellen Halbzugs wird aktualisiert.
            Chess_UI.PieceColor turnColor = TheEngine.GetTurnColor();

            // Die Farbe des Gegners in diesem Halbzug wird bestimmt.
            PieceColor opponentColor = (turnColor == Chess_UI.PieceColor.White) ? Chess_UI.PieceColor.Black : Chess_UI.PieceColor.White;

            // Eine Liste von allen legalen Zügen der Figuren der aktuellen Farbe wird erstellt.
            List<Move> moves = TheEngine.GenerateMoves();

            // Je nach Spielerfarbe werden Koordinaten transformiert
            if (playerColor == Chess_UI.PieceColor.White)
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

            // Falls vor dem Click bereits eine Figur ausgewählt war ...
            if (piece_selected)
            {
                // ... wird ermittelt, ob diese Figur mit dem aktuellen Click einen Zug machen kann.
                if (TheEngine.IsValidMove(selectedX, selectedY, x, y))
                {
                    UI.NextMoveMade();

                    /*
                    // ai macht zug
                    if (turnColor == AIcolor)
                    {
                        //ai.randomMove();
                        //ai.makeMove();
                        //String backup = TheEngine.FromPositionCreateFEN();
                        Move AIMove = ai.AlphaBetaRoot(TheEngine, 2);
                        //TheEngine.setBoardFromFEN(backup);
                        TheEngine.MakeMove(AIMove.StartSquare.X, AIMove.StartSquare.Y, AIMove.EndSquare.X, AIMove.EndSquare.Y, MoveType.Default);
                    }
                    else
                    {
                        
                    }
                    */

                    bool pieceTaken = TheEngine.Board().Squares[x, y].Color != PieceColor.Empty;

                    TheEngine.MakeMove(selectedX, selectedY, x, y, MoveType.Default);

                    if (TheEngine.KingInCheck(opponentColor, TheEngine.GenerateMoves(turnColor)))
                    {
                        // Soundeffekt, wenn König in Schach gesetzt wird
                        SFXcheck.Play();
                    }
                    else if (pieceTaken)
                    {
                        // Soundeffekt, wenn Figur geschlagen wird
                        SFXpieceTaken.Play();
                    }
                    else
                    {
                        // Sonst anderer Soundeffekt
                        SFXmoveMade.Play();
                    }

                    if (turnColor == PieceColor.Black)
                    {
                        TheEngine.IncrementTurncounter();
                    }

                    /* TODO: Check, ob Figur geschlagen, oder Bauer bewegt -> Halbzugzähler dementsprechend anpassen
                    if (TheEngine.PieceTakenOrPawnMoved())
                    {
                        TheEngine.ResetHalfmoveclock();
                    }
                    else
                    {
                        TheEngine.IncrementHalfmoveclock();
                    }
                    */

                }
                else
                {
                    UI.HidePossibleMoves();
                }
            }

            // Falls noch keine Figur ausgewählt worden war ...
            else
            {   // Clicks auf Felder, die keine Figur der gültigen Farbe enthalten, werden verworfen.
                if (!TheEngine.ValidColorSelected(x, y))
                {
                    return;
                }
                // Jetzt wissen wir, dass eine Figur korrekt angeclickt wurde, und die Koordinaten der ausgewählten Figur werden zugewiesen.
                selectedX = x;
                selectedY = y;

                // Eine Liste an Punkten wird erstellt, die alle Felder enthält, wohin die gewählte Figur ziehen kann.
                List<Point> possibleMoves = new List<Point>();
                TheEngine.GetPossibleMoves(x, y, moves, ref possibleMoves);

                // Diese Felder werden markiert.
                // Dabei müssen die Koordinaten der erhaltenen Züge an das Koordinatensystem der Spielfarbe angepasst werden
                if (playerColor == Chess_UI.PieceColor.White)
                {
                    UI.ShowPossibleMoves(UI.TransformMovesWhite(possibleMoves));
                }
                else
                {
                    UI.ShowPossibleMoves(UI.TransformMovesBlack(possibleMoves));
                }
            }
            // Die Engine serialisiert die Position und daraus wird das GUI gebildet
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
            // Zusätzlich wird das Brett in der Konsole ausgegeben.
            TheEngine.GetTheBoard();

            int gameStatus = TheEngine.CheckGameOver(TheEngine.FromPositionCreateFEN());

            switch (gameStatus)
            {
                case 0:
                    gameOver = true;
                    ShowDialog("Weiß ist Schachmatt", "Ergebnis: 0 - 1");
                    break;
                case 1:
                    gameOver = true;
                    ShowDialog("Schwarz ist Schachmatt", "Ergebnis: 1 - 0");
                    break;
                case 2:
                    gameOver = true;
                    ShowDialog("Remis durch Patt", "Ergebnis: 1/2 - 1/2");
                    break;
            }
            if(TheEngine.MovesSinceBeaten() == 150)
            {
                gameOver = true;
                ShowDialog("Remis durch 75-Zug-Regel", "Ergebnis: 1/2 - 1/2");

            }

        }
        private void CheckIfAIShouldMove(object Sender, EventArgs e)
        {
            PieceColor turnColor = TheEngine.GetTurnColor();
            if (turnColor == AIcolor && !gameOver)
            {
                PieceColor opponentColor = (turnColor == Chess_UI.PieceColor.White) ? Chess_UI.PieceColor.Black : Chess_UI.PieceColor.White;

                Move AIMove = AI.AlphaBetaRoot(TheEngine, 3);

                bool pieceTaken = TheEngine.Board().Squares[AIMove.EndSquare.X, AIMove.EndSquare.Y].Color != PieceColor.Empty;

                TheEngine.MakeMove(AIMove.StartSquare.X, AIMove.StartSquare.Y, AIMove.EndSquare.X, AIMove.EndSquare.Y, MoveType.PromotionQueen);

                if (TheEngine.KingInCheck(opponentColor, TheEngine.GenerateMoves(turnColor)))
                {
                    // Soundeffekt, wenn König in Schach gesetzt wird
                    SFXcheck.Play();
                }
                else if (pieceTaken)
                {
                    // Soundeffekt, wenn Figur geschlagen wird
                    SFXpieceTaken.Play();
                }
                else
                {
                    // Sonst anderer Soundeffekt
                    SFXmoveMade.Play();
                }

                if (turnColor == PieceColor.Black)
                {
                    TheEngine.IncrementTurncounter();
                }

                // Die Engine serialisiert die Position und daraus wird das GUI gebildet
                UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
                // Zusätzlich wird das Brett in der Konsole ausgegeben.
                TheEngine.GetTheBoard();

                int gameStatus = TheEngine.CheckGameOver(TheEngine.FromPositionCreateFEN());

                Console.WriteLine(gameStatus);

                switch (gameStatus)
                {
                    case 0:
                        gameOver = true;
                        ShowDialog("Weiß ist Schachmatt", "Ergebnis: 0 - 1");
                        break;
                    case 1:
                        gameOver = true;
                        ShowDialog("Schwarz ist Schachmatt", "Ergebnis: 1 - 0");
                        break;
                    case 2:
                        gameOver = true;
                        ShowDialog("Remis durch Patt", "Ergebnis: 1/2 - 1/2");
                        break;
                }

            }
        }

        public static void ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text };
            textLabel.Width = 150;
            textLabel.Height = 100;
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.ShowDialog();
        }
    }
}
