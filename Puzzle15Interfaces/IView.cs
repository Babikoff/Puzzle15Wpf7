using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puzzle15.Interfaces
{
    public interface IView
    {
        ICell CreateCell(int cellNumber, int cellIndex, bool isEmptyCell);
    }
}
