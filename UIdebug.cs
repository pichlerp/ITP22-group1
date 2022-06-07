using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_UI
{
    class UIdebug
    {
        public static bool CheckCoords(int y, int x)
        {
            return !(x < 0 || y < 0 || x > 7 || y > 7);
        }

        public static bool CheckCoords(int y, int x, int k, int i)
        {
            return !(x < 0 || y < 0 || x > 7 || y > 7 || i < 0 || k < 0 || i > 7 || k > 7) && (x != i || y != k);
        }
    }
}
