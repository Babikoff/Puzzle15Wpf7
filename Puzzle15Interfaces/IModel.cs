using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puzzle15.Interfaces
{
    public interface IModel
    {
        int Rows { get; set; }
        int Columns { get; set; }
        int Cells { get; set; }
        int Moves { get; set; }
        bool FirstLoad { get; set; }
        bool IsBorderSwich(int a, int b);
        bool AreCellsOrdered { get; }

        void NewGame();
        void Init(int gameAreaSize, IView view);
        void Scrambles();

        int FindEmptyItemPosition();
        ICell FindCellByNumber(int cellNum);
    }
}
