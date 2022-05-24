using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_UI
{
    class Perft
    {
        Engine engine;
        ulong movecount = 0;

        public Perft(ref Engine in_engine)
        {
            engine = in_engine;
        }
        public void countMovesToDepth(int maxdepth)
        {
            // zum testen --------------------------------------------------
            maxdepth = 0;

            Console.WriteLine("Counting moves to depth: " + maxdepth);

            movecount = 0;
            countMoves(maxdepth);
            ulong temp = movecount;
            Console.WriteLine("Number of Moves: " + movecount.ToString());
        }
        void countMoves(int depth)
        {
            if (depth == 0) return;
            string initial_pos = engine.FromPositionCreateFEN();
            List<Move> moves = engine.GenerateMoves(engine.GetTurnColor());
            if (depth == 1)
            {
                movecount += (ulong)moves.Count;
            }


            foreach (Move move in moves)
            {
                engine.setBoardFromFEN(initial_pos);
                engine.MakeMove(move);
                countMoves(depth - 1);
            }
        }
    }
}
