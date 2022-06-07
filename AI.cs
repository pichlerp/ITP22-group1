using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_UI
{
    class AI
    {
        const int maxdepth = 0;
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
    }
}
