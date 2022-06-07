using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    public class Square
    {
        public int RowNum { get; set; }
        public int ColNum { get; set; }
        public bool LegalMove { get; set; }

        public bool Occupied { get; set; }

        public Square(int row, int col)
        {
            RowNum = row;
            ColNum = col;
        }

    }
}
