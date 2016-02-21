using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Puzzle15.Interfaces;

namespace Puzzle15.Wpf
{
    internal class Model: IModel
    {
        private int[] _bordersNums; //= { 0, 4, 8, 12, 3, 7, 11, 15 };

        public Random _rnd { get; set; }

        public bool _firstLoad { get; set; }
        public int _rows { get; set; }
        public int _columns { get; set; }
        public int _cells { get; set; }
        public int _moves { get; set; }

        public Model()
        {
            _rows = 4;
            _columns = 4;
            _firstLoad = true;
            _rnd = new Random();
        }

        public void Init(int gameAreaSize)
        {
            _rows = gameAreaSize;
            _columns = gameAreaSize;
            _cells = _rows * _columns;
            var bordersNums = new List<int>(_cells);

            for (int i = 0; i < _cells; i++)
            {
                var remainder = (i + 1) % _columns;
                if (remainder == 0 || remainder == 1)
                {
                    bordersNums.Add(i);
                }
            }
            _bordersNums = bordersNums.ToArray();
        }

        public void NewGame()
        {
            _moves = 0;
        }

        /// <summary>
        /// Bug Fix - if both of the items you want to swipe are in the Board borders do nothing.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool IsBorderSwich(int a, int b)
        {
            return _bordersNums.Contains(a) && _bordersNums.Contains(b);
        }
    }
}
