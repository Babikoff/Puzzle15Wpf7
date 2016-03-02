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
        int Moves { get; }
        bool FirstLoad { get; set; }
        bool IsBorderSwitch(int a, int b);
        bool AreCellsOrdered { get; }
        bool IsAnyMovementAllowed { get; set; }

        void NewGame();
        void Init(int gameAreaSize, IView view);
        void Scrambles();
        ICell FindCellByNumber(int cellNum);
        ICell CanMove(ICell cellToMove);

        void Swap(ICell item, ICell to);
    }
}
