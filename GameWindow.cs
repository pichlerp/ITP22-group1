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
        Color backgroundcolor = Color.FromArgb(10, 10, 10);
        static ChessUI UI;
        bool playing_ai = false;
        PieceColor AIcolor = PieceColor.Empty;
        Chess_UI.PieceColor playerColor;
        new ChessMenu Menu;
        static Chess_UI.Engine TheEngine;
        InGameMenu InGameMenu;
        Timer timer = new Timer();
        AI ai;

        // Soundeffekte
        System.Media.SoundPlayer SFXmoveMade = new System.Media.SoundPlayer(Chess_UI.Properties.Resources.move_regular);
        System.Media.SoundPlayer SFXcheck = new System.Media.SoundPlayer(Chess_UI.Properties.Resources.move_check);
        System.Media.SoundPlayer SFXpieceTaken = new System.Media.SoundPlayer(Chess_UI.Properties.Resources.move_piecetaken);

        public GameWindow()
        {
            InitializeComponent();
            // Programmablauf fängt hier an
            InitializeForm();

            timer.Interval = 500;
            timer.Tick += new EventHandler(CheckIfAIShouldMove);
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
            Menu = new ChessMenu(this, MenuStartButtonPressW, MenuStartButtonPressB);
        }

        private void MenuStartButtonPressW(object sender, EventArgs e)
        {
            playerColor = Chess_UI.PieceColor.White;
            initGame(Chess_UI.PieceColor.White);

            if (playing_ai)
            {
                ai = new AI(Chess_UI.PieceColor.Black);
                AIcolor = PieceColor.Black;
            }
            else
            {
                AIcolor = PieceColor.Empty;
                timer.Stop();
            }
        }

        private void MenuStartButtonPressB(object sender, EventArgs e)
        {
            playerColor = Chess_UI.PieceColor.Black;
            initGame(Chess_UI.PieceColor.Black);

            if (playing_ai)
            {
                ai = new AI(Chess_UI.PieceColor.White);
                AIcolor = PieceColor.White;
            }
            else
            {
                AIcolor = PieceColor.Empty;
                timer.Stop();
            }
        }
        public void initGame(Chess_UI.PieceColor color)
        {
            TheEngine = new Chess_UI.Engine();
            Menu.HideMenu();
            UI = new ChessUI(this, ClickHandler, color);
            InGameMenu = new InGameMenu(ref UI, this, ref TheEngine);
            InGameMenu.show();
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
            playing_ai = Menu.play_ai;
            timer.Enabled = true;
        }

        public void returnToMenu()
        {
            UI.Hide();
            InGameMenu.hide();
            TheEngine.setBoardFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            gameOver = false;
            Menu.ShowMenu();
            Menu.play_ai = false;
            Menu.play_ai_bt.Text = "Spiel gegen KI";
            Menu.play_ai_bt.Enabled = true;

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

                    if (TheEngine.Board().Squares[selectedX, selectedY].Type == PieceType.Pawn && (x == 0 || x == 7))
                    {
                        InGameMenu.showPromotionMenu(selectedX, selectedY, x, y, turnColor);
                        return;
                    }

                    bool pieceTaken = TheEngine.Board().Squares[x, y].Color != PieceColor.Empty;
                    PieceType pieceType = TheEngine.Board().Squares[selectedX, selectedY].Type;

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

                    // Check, ob Figur geschlagen, oder Bauer bewegt -> Halbzugzähler dementsprechend anpassen
                    if (pieceTaken || pieceType == PieceType.Pawn)
                    {
                        TheEngine.Board().halfmoveClock = 1;
                    }
                    else
                    {
                        TheEngine.Board().halfmoveClock++;
                    }
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
            endOfMove(turnColor);
        }
        public void endOfMove(PieceColor turnColor)
        {
            // Die Engine serialisiert die Position und daraus wird das GUI gebildet
            UI.PositionFromFEN(TheEngine.FromPositionCreateFEN(), playerColor);
            // Zusätzlich wird das Brett in der Konsole ausgegeben.
            //TheEngine.GetTheBoard();

            // Die Farbe des Gegners in diesem Halbzug wird bestimmt.
            PieceColor opponentColor = (turnColor == Chess_UI.PieceColor.White) ? Chess_UI.PieceColor.Black : Chess_UI.PieceColor.White;

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
                case 3:
                    gameOver = true;
                    ShowDialog("Remis durch 75-Zug-Regel", "Ergebnis: 1/2 - 1/2");
                    break;
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
                PieceType pieceType = TheEngine.Board().Squares[AIMove.StartSquare.X, AIMove.StartSquare.Y].Type;

                Point p = new Point(AIMove.StartSquare.X, AIMove.StartSquare.Y);

                if (AIcolor == Chess_UI.PieceColor.White)
                {
                    int temp = p.Y;
                    p.Y = p.X;
                    p.X = 7 - temp;
                }
                else
                {
                    int temp = p.X;
                    p.X = p.Y;
                    p.Y = 7 - temp;
                }
                
                UI.AiSelect(p);

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

                // Check, ob Figur geschlagen, oder Bauer bewegt -> Halbzugzähler dementsprechend anpassen
                if (pieceTaken || pieceType == PieceType.Pawn)
                {
                    TheEngine.Board().halfmoveClock = 1;
                }
                else
                {
                    TheEngine.Board().halfmoveClock++;
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
                    case 3:
                        gameOver = true;
                        ShowDialog("Remis durch 75-Zug-Regel", "Ergebnis: 1/2 - 1/2");
                        break;
                }

                p = new Point(AIMove.EndSquare.X, AIMove.EndSquare.Y);

                if (AIcolor == Chess_UI.PieceColor.White)
                {
                    int temp = p.Y;
                    p.Y = p.X;
                    p.X = 7 - temp;
                }
                else
                {
                    int temp = p.X;
                    p.X = p.Y;
                    p.Y = 7 - temp;
                }

                UI.AiSelect(p);
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
