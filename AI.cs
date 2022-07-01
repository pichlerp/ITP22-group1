using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_UI
{
    class AI
    {
        /*
        const int maxdepth = 1;
        Engine real_engine;
        Engine imaginary_board;
        ChessUI ui;
        bool color;
        Random random = new Random();

        class Mabs // Move And Best Score
        {
            public Move move;
            public int score = 0; // bestermöglicher score nach dem Zug
            public Mabs(Move inmove)
            {
                move = inmove;
            }
        }

        public AI(ref Engine in_engine, ref ChessUI in_ui, Chess_UI.PieceColor in_color)
        {
            real_engine = in_engine;
            ui = in_ui;
            color = in_color == PieceColor.White ? true : false;
            real_engine.lockMoves();
            imaginary_board = new Engine();
        }

        public void randomMove()
        {
            List<Move> ai_moves = real_engine.GenerateMoves();
            if (ai_moves.Count == 0) return;
            int rnd = random.Next() % ai_moves.Count;
            real_engine.MakeMove(ai_moves[rnd]);
            ui.NextMoveMade();
        }

        public void makeMove()
        {
            Mabs bestmabs = bestMabsToDepth(maxdepth);
            real_engine.MakeMove(bestmabs.move);
        }

        Mabs bestMabsToDepth(int depth)
        {
            string state = imaginary_board.FromPositionCreateFEN();
            List<Mabs> mabslist = moveListToMabsList(imaginary_board.GenerateMoves());

            int maxindex = 0;
            for (int i = 0; i < mabslist.Count; i++)
            {
                imaginary_board.setBoardFromFEN(state);
                imaginary_board.MakeMove(mabslist[i].move);

                if(depth == 0)
                {
                    int val = imaginary_board.Board().getScore();
                    mabslist[i].score = (color == (imaginary_board.GetTurnColor() == PieceColor.White)) ? val : -val;
                }
                else
                {
                    mabslist[i].score = bestMabsToDepth(depth - 1).score;
                }

                if(mabslist[i].score > mabslist[maxindex].score)
                {
                    maxindex = i;
                }
            }

            return mabslist[maxindex];
        }

        List<Mabs> moveListToMabsList(List<Move> moves)
        {
            List<Mabs> mabs = new List<Mabs>(moves.Count);
            for(int i = 0; i < moves.Count; i++)
            {
                mabs.Add(new Mabs(moves[i]));
            }
            return mabs;
        }

        public int getScore()
        {
            return color ? imaginary_board.Board().getScore() : -imaginary_board.Board().getScore();
        }
        */

        PieceColor color;
        PieceColor opponentColor;
        Engine engine = new Engine();
        Engine engineCopy = new Engine();
        public AI(PieceColor AIcolor)
        {
            color = AIcolor;
            /*
            engineCopy.setBoardFromFEN(FEN);
            engine.setBoardFromFEN(FEN);
            */
            if (color == PieceColor.Black)
            {
                opponentColor = PieceColor.White;
            }
            else
            {
                opponentColor = PieceColor.Black;
            }
        }

        private struct MoveAndScore
        {
            internal System.Drawing.Point SrcMove;
            internal System.Drawing.Point DstMove;
            internal int Score;
        }

        private static int SortMASByScore(MoveAndScore s2, MoveAndScore s1)
        {
            return (s1.Score).CompareTo(s2.Score);
        }

        int GetPieceValue(PieceType type)
        {
            switch (type)
            {
                case PieceType.Pawn:
                    {
                        return 100;
                    }
                case PieceType.Knight:
                    {
                        return 320;
                    }
                case PieceType.Bishop:
                    {
                        return 325;
                    }
                case PieceType.Rook:
                    {
                        return 500;
                    }
                case PieceType.Queen:
                    {
                        return 975;
                    }
                case PieceType.King:
                    {
                        return 1000000;
                    }
                default:
                    {
                        return -1;
                    }
            }
        }

        List<MoveAndScore> EvaluateMoves(Engine engine)
        {
            List<MoveAndScore> result = new List<MoveAndScore>();
            List<Move> moves = engine.GenerateMoves();
            List<Move> pieceMoves = new List<Move>();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Square square = engine.Board().Squares[x, y];
                    // Leeres Feld bringt keine Punkte
                    // Figuren des Gegners bringen keine Punkte
                    if (square.Color == engine.GetTurnColor())
                    {
                        // Figur der AI befindet sich jetzt am betrachteten Feld
                        System.Drawing.Point start = new System.Drawing.Point(x, y);
                        pieceMoves.Clear();
                        // Die Züge mit dieser Figur werden aus der Liste aller Züge ausgelesen
                        foreach (Move move in moves)
                        {
                            if (move.StartSquare == start)
                            {
                                pieceMoves.Add(move);
                            }
                        }
                        foreach (Move move in pieceMoves)
                        {
                            MoveAndScore mas = new MoveAndScore();
                            mas.SrcMove = move.StartSquare;
                            mas.DstMove = move.EndSquare;
                            mas.Score = 0;
                            int endX = mas.DstMove.X;
                            int endY = mas.DstMove.Y;
                            if (engine.Board().Squares[endX, endY].Color == opponentColor)
                            {
                                PieceType attackedPiece = engine.Board().Squares[endX, endY].Type;
                                // Punkte für das Schlagen einer gegnerischen Figur
                                mas.Score += GetPieceValue(attackedPiece);
                                // Mehr Punkte, falls gegnerische Figur höheren Wert hat
                                if (GetPieceValue(engine.Board().Squares[mas.SrcMove.X, mas.SrcMove.Y].Type) < GetPieceValue(attackedPiece))
                                {
                                    mas.Score += GetPieceValue(attackedPiece) - GetPieceValue(engine.Board().Squares[mas.SrcMove.X, mas.SrcMove.Y].Type);
                                }
                            }
                            result.Add(mas);
                        }

                    }
                }
            }
            return result;
        }

        internal void CalculateBoardScore(Engine engine)
        {
            engine.UpdateChecksAndGameOver();
            // Wird bei jeder Evaluierung auf 0 gesetzt
            engine.Board().score = 0;
            // Bleibt 0, bei Remis
            if (engine.Board().stalemate)
            {
                return;
            }
            // Wird >> 1, wenn Schwarz schachmatt ist
            if (engine.Board().blackLost)
            {
                engine.Board().score = -1000000;
                return;
            }
            // Wird << 1, wenn Weiß schachmatt ist
            if (engine.Board().whiteLost)
            {
                engine.Board().score = 1000000;
                return;
            }
            // Punkte für Schach
            if (engine.Board().blackInCheck)
            {
                engine.Board().score -= 75;
            }
            if (engine.Board().whiteInCheck)
            {
                engine.Board().score += 75;
            }
            // Punkte für alle verbleibenden Figuren
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Square square = engine.Board().Squares[x, y];
                    PieceColor color = square.Color;
                    PieceType type = square.Type;
                    if (color == PieceColor.White && type != PieceType.King)
                    {
                        engine.Board().score -= GetPieceValue(type);
                    }
                    if (color == PieceColor.Black && type != PieceType.King)
                    {
                        engine.Board().score += GetPieceValue(type);
                    }
                }
            }
        }


        private int AlphaBeta(Engine engine, int depth, int alpha, int beta)
        {
            String FEN = engine.FromPositionCreateFEN();
            if (depth == 0)
            {
                CalculateBoardScore(engineCopy);
                if (color == PieceColor.Black)
                {
                    return -engineCopy.Board().score;
                }
                return engineCopy.Board().score;
            }
            List<MoveAndScore> masList = EvaluateMoves(engine);

            masList.Sort(SortMASByScore);

            foreach (MoveAndScore mas in masList)
            {
                engineCopy.setBoardFromFEN(FEN);
                engineCopy.MakeMove(mas.SrcMove.X, mas.SrcMove.Y, mas.DstMove.X, mas.DstMove.Y, MoveType.PromotionQueen);
                List<Move> moves = engineCopy.GenerateMoves();
                engineCopy.UpdateChecksAndGameOver();
                int value = -AlphaBeta(engineCopy, depth - 1, -beta, -alpha);
                if (value >= beta)
                {
                    return beta;
                }
                if (value > alpha)
                {
                    alpha = value;
                }
            }
            //Console.WriteLine("alpha " + alpha);
            //engineCopy.setBoardFromFEN(FEN);
            return alpha;

        }

        private struct FENAndLastMove
        {
            internal String FEN;
            internal Move LastMove;
            internal int Score;
            public FENAndLastMove(String f, Move m, int s)
            {
                FEN = f;
                LastMove = m;
                Score = s;
            }
        }

        private static int SortFENAndLastMoveByScore(FENAndLastMove f2, FENAndLastMove f1)
        {
            return (f1.Score).CompareTo(f2.Score);
        }

        public Move AlphaBetaRoot(Engine theEngine, int depth)
        {
            int alpha = -10000000;
            int beta = 10000000;

            engine = theEngine;
            String FEN = engine.FromPositionCreateFEN();

            List<FENAndLastMove> resultList = new List<FENAndLastMove>();

            List<Move> moves = engine.GenerateMoves();
            List<Move> pieceMoves = new List<Move>();
            FENAndLastMove best = new FENAndLastMove("", new Move(0, 0, 0, 0), 0);
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Square square = engine.Board().Squares[x, y];
                    PieceColor color = square.Color;
                    PieceType type = square.Type;

                    if (color == engine.Board().turnColor)
                    {
                        System.Drawing.Point start = new System.Drawing.Point(x, y);
                        pieceMoves.Clear();
                        foreach (Move move in moves)
                        {
                            if (move.StartSquare == start)
                            {
                                pieceMoves.Add(move);
                            }
                        }
                        foreach (Move move in pieceMoves)
                        {
                            engineCopy.setBoardFromFEN(FEN);
                            engineCopy.MakeMove(move);
                            CalculateBoardScore(engineCopy);
                            if (color == PieceColor.Black)
                            {
                                engineCopy.Board().score = -engineCopy.Board().score;
                            }
                            FENAndLastMove result = new FENAndLastMove(engineCopy.FromPositionCreateFEN(), move, engineCopy.Board().score);
                            resultList.Add(result);
                        }
                    }

                }
            }
            resultList.Sort(SortFENAndLastMoveByScore);
            /*
            foreach (FENAndLastMove f in resultList)
            {
                Console.WriteLine(f.LastMove.StartSquare.X + " " + f.LastMove.StartSquare.Y + " " + f.LastMove.EndSquare.X + " " + f.LastMove.EndSquare.Y + " " + f.Score);
            }
            */

            depth--;
            int value;
            foreach (FENAndLastMove falm in resultList)
            {
                engineCopy.setBoardFromFEN(falm.FEN);
                value = -AlphaBeta(engineCopy, depth, -beta, -alpha);
                //Console.WriteLine("value: " + best.Score);
                if (value > alpha)
                {
                    //Console.WriteLine("value2: " + best.Score);
                    //Console.WriteLine(value);
                    alpha = value;
                    best = falm;
                }
            }
            Console.WriteLine("Current board evaluation: " + best.Score);
            return best.LastMove;
        }

    }
}
