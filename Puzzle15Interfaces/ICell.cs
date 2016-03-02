using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;

namespace Puzzle15.Interfaces
{
    public interface ICell
    {
        void SwapWith(ICell cell);
        int CellNumber { get; set; }
        bool IsEmptyCell { get; set; }
        int CellIndex { get; }
        ImageSource Picture { get; set; }
        ShowCellModeEnum ShowCellMode { get; set; }

        event EventHandler<EventArgs> ManipulationEvent;
    }
}
