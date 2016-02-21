using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puzzle15.Interfaces
{
    public interface IModel
    {
        Random _rnd { get; set; }

        int _rows { get; set; }
        int _columns { get; set; }
        int _cells { get; set; }
        int _moves { get; set; }
        bool _firstLoad { get; set; }

        void NewGame();
        void Init(int gameAreaSize);
        bool IsBorderSwich(int a, int b);
    }
}
