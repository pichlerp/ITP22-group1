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
        int totalLeafNodes = 0;

        public Perft(ref Engine in_engine)
        {
            engine = in_engine;
        }
        public void countMovesToDepth(int maxdepth)
        {
            if (maxdepth > 0)
            {
                string initial_pos = engine.FromPositionCreateFEN();

                Console.WriteLine("Counting moves to depth: " + maxdepth);

                // Divide: Die Anzahl der Blätter wird aufgeteilt auf den ersten Zug, der zu diesen Blättern geführt hat
                // Dazu werden alle Positionen getrennt betrachtet, die nach einem Zug aus der gegebenen Position entstehen
                List<string> positions = new List<string>();
                List<Move> moves = engine.GenerateMoves(engine.GetTurnColor());

                // Es wird eine Liste erstellt, die alle Positionen als FEN enthält, die durch einen Zug aus der gegebenen Position entstehen können
                foreach (Move move in moves)
                {
                    List<char> promotions = new List<char>() { 'q', 'r', 'b', 'n' };
                    engine.setBoardFromFEN(initial_pos);
                    engine.MakeMove(move/*, promotions*/);
                    positions.Add(engine.FromPositionCreateFEN());
                }

                totalLeafNodes = 0;
                int i = 0;

                Console.WriteLine("Divided by first move: ");
                // Es werden alle Blätter für jede Position der Liste berechnet, ausgegeben und zu einer Gesamtsumme addiert
                foreach (String fen in positions)
                {
                    int currentLeafNodes = 0;
                    if (maxdepth == 1)
                    {
                        currentLeafNodes++;
                    }
                    // Die übergebene Position ist bereits einen Zug tiefer als die ursprüngliche Position, daher wird maxdepth um 1 reduziert
                    // Der Zähler currentLeafNodes wird als Referenz übergeben
                    countMoves(maxdepth - 1, fen, ref currentLeafNodes);
                    // Übersetzung der Koordinaten in algebraische Schachnotation
                    char fileStart = (char)(moves[i].StartSquare.Y + 97);
                    char rankStart = (char)(moves[i].StartSquare.X + 49);
                    char fileEnd = (char)(moves[i].EndSquare.Y + 97);
                    char rankEnd = (char)(moves[i].EndSquare.X + 49);
                    char promotionChar = ' ';
                    if (moves[i].MoveType != MoveType.Default)
                    {
                        switch (moves[i].MoveType)
                        {
                            case MoveType.PromotionQueen:
                                promotionChar = 'q';
                                break;
                            case MoveType.PromotionRook:
                                promotionChar = 'r';
                                break;
                            case MoveType.PromotionBishop:
                                promotionChar = 'b';
                                break;
                            case MoveType.PromotionKnight:
                                promotionChar = 'n';
                                break;
                        }
                    }

                    // Blätter, die aus dem Zug entstehen, der zur betrachteten Position geführt hat, werden ausgegeben und zur Gesamtanzahl addiert
                    Console.WriteLine("  " + fileStart.ToString() + rankStart.ToString() + fileEnd.ToString() + rankEnd.ToString() + promotionChar.ToString() + " : " + currentLeafNodes);
                    totalLeafNodes += currentLeafNodes;

                    i++;
                }

                Console.WriteLine("Total number of leaf nodes: " + totalLeafNodes.ToString());

                engine.setBoardFromFEN(initial_pos);

            }
        }
        void countMoves(int depth, string position, ref int counter)
        {
            if (depth == 0) return;

            if (position != null)
            {
                engine.setBoardFromFEN(position);
            }

            string initial_pos = engine.FromPositionCreateFEN();

            List<Move> moves = engine.GenerateMoves(engine.GetTurnColor());
            if (depth == 1)
            {
                counter += moves.Count;
            }

            foreach (Move move in moves)
            {
                
                engine.setBoardFromFEN(initial_pos);

                engine.MakeMove(move);

                countMoves(depth - 1, null, ref counter);

            }
        }
    }
}