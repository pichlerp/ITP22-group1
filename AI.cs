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

        static PieceColor AIcolor;
        static PieceColor opponentColor;
        static Engine engine1 = new Engine();
        static Engine engine2 = new Engine();
        static int nodesCounter = 0;
        public AI(PieceColor color)
        {
            AIcolor = color;
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

        static int GetPieceValue(PieceType type)
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
                        return 10000;
                    }
                default:
                    {
                        //Console.WriteLine("ERROR!");
                        return -1;
                    }
            }
        }

        static List<MoveAndScore> EvaluateMoves(String FEN)
        {
            List<MoveAndScore> result = new List<MoveAndScore>();
            engine1.setBoardFromFEN(FEN);
            PieceColor turnColor = engine1.GetTurnColor();
            List<Move> moves = engine1.GenerateMoves();
            List<Move> pieceMoves = new List<Move>();
            MoveAndScore mas = new MoveAndScore();
            //Console.WriteLine(FEN + " " + turnColor);
            engine1.setBoardFromFEN(FEN);
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Square square = engine1.Board().Squares[x, y];
                    // Leeres Feld bringt keine Punkte
                    // Figuren des Gegners bringen keine Punkte
                    if (square.Color == turnColor)
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
                            mas.SrcMove = move.StartSquare;
                            mas.DstMove = move.EndSquare;
                            mas.Score = 0;

                            int endX = mas.DstMove.X;
                            int endY = mas.DstMove.Y;
                            if (engine1.Board().Squares[endX, endY].Color == opponentColor)
                            {
                                PieceType attackedPiece = engine1.Board().Squares[endX, endY].Type;
                                // Punkte für das Schlagen einer gegnerischen Figur
                                int otherValue = GetPieceValue(attackedPiece);
                                mas.Score += otherValue;
                                // Mehr Punkte, falls gegnerische Figur höheren Wert hat
                                int thisValue = GetPieceValue(engine1.Board().Squares[mas.SrcMove.X, mas.SrcMove.Y].Type);
                                if (thisValue < otherValue)
                                {
                                    mas.Score += otherValue - thisValue;
                                }
                            }
                            result.Add(mas);
                        }

                    }
                }
            }
            return result;
        }

        internal static int CalculateBoardScore(String FEN)
        {
            engine2.setBoardFromFEN(FEN);
            engine2.UpdateChecksAndGameOver(FEN);

            /*
            // Bleibt 0, bei Remis
            if (engine2.Board().stalemate)
            {
                return 0;
            }            

            // Wenn Schwarz schachmatt ist ...
            if (engine2.Board().blackLost)
            {
                if (engine2.GetTurnColor() == PieceColor.Black)
                {
                    // und Schwarz am Zug ist -> sehr kleiner Wert
                    return 25000;
                }
                // und Weiß am Zug ist -> sehr großer Wert
                return -25000;
            }

            // Wenn Weiß schachmatt ist ...
            if (engine2.Board().whiteLost)
            {
                if (engine2.GetTurnColor() == PieceColor.Black)
                {
                    // und Schwarz am Zug ist -> sehr großer Wert
                    return -25000;
                }
                // und Weiß am Zug ist->sehr kleiner Wert
                return 25000;
            }

            */



            int score = 0;

            // Punkte für Schach
            if (engine2.Board().blackInCheck)
            {
                score += 75;
            }
            if (engine2.Board().whiteInCheck)
            {
                score -= 75;
            }

            /*
            // Bonus für Rochade                            
            if (engine1.Board().Squares[move.StartSquare.X, move.StartSquare.Y].Color != PieceColor.Empty && engine1.Board().Squares[move.StartSquare.X, move.StartSquare.Y].Type == PieceType.King)
            {
                if (move.StartSquare.X == 0 && move.StartSquare.X == 4 && engine1.Board().Squares[move.StartSquare.X, move.StartSquare.Y].Color == PieceColor.White)
                {
                    if (move.EndSquare.X == 0 && (move.EndSquare.Y == 6 || move.EndSquare.Y == 4))
                    {
                        mas.Score += 50;
                    }
                }
                else if (move.StartSquare.X == 7 && move.StartSquare.X == 4 && engine1.Board().Squares[move.StartSquare.X, move.StartSquare.Y].Color == PieceColor.Black)
                {
                    if (move.EndSquare.X == 7 && (move.EndSquare.Y == 6 || move.EndSquare.Y == 4))
                    {
                        mas.Score += 50;
                    }
                }
                else
                {
                    mas.Score -= 50;
                }
            }

            // Zu Beginn der Partie Bonus für Bauernzüge in der Mitte, die aber nicht in der gegnerischen Hälfte des Bretts enden 
            if (engine1.Board().Squares[move.StartSquare.X, move.StartSquare.Y].Type == PieceType.Pawn && engine1.Board().turnCounter < 5)
            {
                if (move.StartSquare.Y == 3 || move.StartSquare.Y == 4)
                {
                    mas.Score += 50;
                }
                if (engine1.Board().Squares[move.StartSquare.X, move.StartSquare.Y].Color == PieceColor.White)
                {
                    if (move.EndSquare.X > 3)
                    {
                        mas.Score -= 50;
                    }
                }
                if (engine1.Board().Squares[move.StartSquare.X, move.StartSquare.Y].Color == PieceColor.Black)
                {
                    if (move.EndSquare.X < 4)
                    {
                        mas.Score -= 50;
                    }
                }
            }
            */

            Square square;

            // Punkte für alle verbleibenden Figuren
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    square = engine2.Board().Squares[x, y];
                    PieceColor color = square.Color;
                    PieceType type = square.Type;

                    if (color == PieceColor.White)
                    {
                        score += GetPieceValue(type);
                        // Bauern sollen zu Beginn nicht in die gegnerische Hälfte des Bretts ziehen und zentrale Bauern zählen mehr
                        if (type == PieceType.Pawn && engine2.Board().turnCounter < 5)
                        {
                            if (x > 3)
                            {
                                score -= 50;
                            }
                            else if ((y == 3 || y == 4) && (x == 2 || x == 3))
                            {
                                score += 50;
                            }
                        }
                    }
                    if (color == PieceColor.Black)
                    {
                        score -= GetPieceValue(type);
                        if (type == PieceType.Pawn && engine2.Board().turnCounter < 5)
                        {
                            if (x < 4)
                            {
                                score += 50;
                            }
                            else if ((y == 3 || y == 4) && (x == 5 || x == 4))
                            {
                                score -= 50;
                            }
                        }
                    }
                }
            }

            // Position nach Rochade wird als gut gewertet, König soll nicht einfach so bewegt werden
            if (engine2.Board().Squares[0, 2].Type == PieceType.King && engine2.Board().Squares[0, 3].Type == PieceType.Rook && engine2.Board().turnCounter < 15)
            {
                score += 50;
            }
            else if (engine2.Board().Squares[0, 6].Type == PieceType.King && engine2.Board().Squares[0, 5].Type == PieceType.Rook && engine2.Board().turnCounter < 15)
            {
                score += 50;
            }
            else if (engine2.Board().Squares[0, 4].Type != PieceType.King && engine2.Board().turnCounter < 15)
            {
                score -= 50;
            }

            if (engine2.Board().Squares[7, 2].Type == PieceType.King && engine2.Board().Squares[7, 3].Type == PieceType.Rook && engine2.Board().turnCounter < 15)
            {
                score -= 50;
            }
            if (engine2.Board().Squares[7, 6].Type == PieceType.King && engine2.Board().Squares[7, 5].Type == PieceType.Rook && engine2.Board().turnCounter < 15)
            {
                score -= 50;
            }
            else if (engine2.Board().Squares[7, 4].Type != PieceType.King && engine2.Board().turnCounter < 15)
            {
                score += 50;
            }

            return score;
        }


        private static int AlphaBeta(String FEN, int depth, int alpha, int beta)
        {
            nodesCounter++;

            engine1.setBoardFromFEN(FEN);

            if (depth == 0)
            {
                engine1.Board().score = CalculateBoardScore(FEN);

                if (engine1.GetTurnColor() == PieceColor.Black)
                {
                    return -engine1.Board().score;
                }
                return engine1.Board().score;
            }

            engine1.setBoardFromFEN(FEN);
            engine1.UpdateChecksAndGameOver(FEN);

            // Bleibt 0, bei Remis
            if (engine1.Board().stalemate)
            {
                return 0;
            }

            // Wenn Schwarz schachmatt ist ...
            if (engine1.Board().blackLost)
            {
                if (engine1.GetTurnColor() == PieceColor.Black)
                {
                    // und Schwarz am Zug ist -> sehr kleiner Wert
                    return -25000;
                }
                // und Weiß am Zug ist -> sehr großer Wert
                return 25000;
            }

            // Wenn Weiß schachmatt ist ...
            if (engine1.Board().whiteLost)
            {
                if (engine1.GetTurnColor() == PieceColor.Black)
                {
                    // und Schwarz am Zug ist -> sehr großer Wert
                    return 25000;
                }
                // und Weiß am Zug ist->sehr kleiner Wert
                return -25000;
            }

            List<MoveAndScore> masList = EvaluateMoves(FEN);

            masList.Sort(SortMASByScore);

            int value;
            String fen;
            foreach (MoveAndScore mas in masList)
            {
                engine2.setBoardFromFEN(FEN);
                engine2.MakeMove(mas.SrcMove.X, mas.SrcMove.Y, mas.DstMove.X, mas.DstMove.Y, MoveType.PromotionQueen);
                //Console.WriteLine(engine2.Board().Squares[mas.SrcMove.X, mas.SrcMove.Y].Color);
                //List<Move> moves = engineCopy.GenerateMoves();
                //engine2.UpdateChecksAndGameOver();
                fen = engine2.FromPositionCreateFEN();
                value = -AlphaBeta(fen, depth - 1, -beta, -alpha);
                //Console.WriteLine(value);

                if (value >= beta)
                {
                    return beta;
                }

                if (value > alpha)
                {
                    alpha = value;
                }

                //Console.WriteLine(mas.SrcMove.X + " " + mas.SrcMove.Y + " " + mas.DstMove.X + " " + mas.DstMove.Y + " value: " + value + " alpha: " + alpha + " beta: " + alpha);

            }
            // Console.WriteLine("alpha " + alpha);
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

        public static Move AlphaBetaRoot(Engine theEngine, int depth)
        {
            int alpha = -10000000;
            const int beta = 10000000;
            engine1.setBoardFromFEN(theEngine.FromPositionCreateFEN());
            String FEN = engine1.FromPositionCreateFEN();

            List<FENAndLastMove> resultList = new List<FENAndLastMove>();

            List<Move> moves = engine1.GenerateMoves();
            List<Move> pieceMoves = new List<Move>();
            //FENAndLastMove best = new FENAndLastMove("", new Move(0, 0, 0, 0), -10000000);
            engine1.setBoardFromFEN(theEngine.FromPositionCreateFEN());

            System.Drawing.Point start;
            FENAndLastMove result;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Square square = engine1.Board().Squares[x, y];
                    PieceColor color = square.Color;
                    PieceType type = square.Type;

                    if (color == AIcolor)
                    {
                        start = new System.Drawing.Point(x, y);
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
                            engine1.setBoardFromFEN(FEN);
                            engine2.setBoardFromFEN(FEN);
                            engine2.MakeMove(move);
                            engine2.UpdateChecksAndGameOver(engine2.FromPositionCreateFEN());    
                            
                            if(AIcolor == PieceColor.White && engine2.Board().blackLost)
                            {
                                return move;
                            }

                            if (AIcolor == PieceColor.Black && engine2.Board().whiteLost)
                            {
                                return move;
                            }

                            engine2.Board().score = CalculateBoardScore(engine2.FromPositionCreateFEN());
                            if (AIcolor == PieceColor.Black)
                            {
                                engine2.Board().score = -engine2.Board().score;
                            }
                            result = new FENAndLastMove(engine2.FromPositionCreateFEN(), move, engine2.Board().score);
                            resultList.Add(result);
                        }
                    }

                }
            }
            resultList.Sort(SortFENAndLastMoveByScore);

            //FENAndLastMove f = resultList.First();

            //Console.WriteLine(f.LastMove.StartSquare.X + " " + f.LastMove.StartSquare.Y + " " + f.LastMove.EndSquare.X + " " + f.LastMove.EndSquare.Y + " " + f.Score);

            /*
            if (resultList.First().Score == 25000)
            {
                return new Move(resultList.First().LastMove.StartSquare.X, resultList.First().LastMove.StartSquare.Y, resultList.First().LastMove.EndSquare.X, resultList.First().LastMove.EndSquare.Y);
            }
            */


            depth--;
            int value;
            Move best = new Move(0, 0, 0, 0); ;

            foreach (FENAndLastMove falm in resultList)
            {
                //engineCopy.setBoardFromFEN(falm.FEN);
                value = -AlphaBeta(falm.FEN, depth, -beta, -alpha);
                //Console.WriteLine(falm.FEN);
                //Console.WriteLine("move: " + falm.LastMove.StartSquare.X + " " + +falm.LastMove.StartSquare.Y + " " + falm.LastMove.EndSquare.X + " " + +falm.LastMove.EndSquare.Y + " value: " + value + " alpha: " + alpha);

                if (value > alpha)
                {
                    alpha = value;
                    best = new Move(falm.LastMove.StartSquare.X, falm.LastMove.StartSquare.Y, falm.LastMove.EndSquare.X, falm.LastMove.EndSquare.Y);
                    Console.WriteLine("best move: " + best.StartSquare.X + " " + best.StartSquare.Y + " value: " + alpha);

                }

            }
            //Console.WriteLine("Current board evaluation: " + best.Score);
            Console.WriteLine("Nodes searched: " + nodesCounter);
            return best;
        }

    }
}
