using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Puzzle15.Interfaces
{
    public interface IView
    {
        ICell CreateCell(int cellNumber, int cellIndex, bool isEmptyCell);
        event GameEpilogFinishedDelegate GameEpilogFinishedEvent;

        void EndOfGame();
        void CreateCells(int size);
        void LoadImageToCells(BitmapImage bitmapImage);

        ShowCellModeEnum ShowCellMode { get; set; }
    }
}
