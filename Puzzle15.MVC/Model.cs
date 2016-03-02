using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Puzzle15.Interfaces;

namespace Puzzle15.Model
{
    public class Model: IModel
    {
        private int[] _bordersNums; //= { 0, 4, 8, 12, 3, 7, 11, 15 };
        private int _moves;
        public Random _rnd { get; set; }

        public bool FirstLoad { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int Cells { get; set; }
        public int Moves { get { return _moves; } }

        private List<ICell> _cells = new List<ICell>();

        public Model()
        {
            Rows = 4;
            Columns = 4;
            FirstLoad = true;
            _rnd = new Random();
        }

        public void Init(int gameAreaSize, IView view)
        {
            _cells.Clear();
            Rows = gameAreaSize;
            Columns = gameAreaSize;
            Cells = Rows * Columns;
            var bordersNums = new List<int>(Cells);

            for (int i = 0; i < Cells; i++)
            {
                var remainder = (i + 1) % Columns;
                if (remainder == 0 || remainder == 1)
                {
                    bordersNums.Add(i);
                }
            }
            _bordersNums = bordersNums.ToArray();

            for (int i = 0; i < Cells; i++)
            {
                var cell = view.CreateCell(i + 1, i, i + 1 == Cells);
                _cells.Add(cell);
            }
        }

        public void NewGame()
        {
            _moves = 0;
            Scrambles();
            while (!CheckIfSolvable())
            {
                Scrambles();
            }
        }

        /// <summary>
        /// Check if the current Scramble is solveable.
        /// </summary>
        /// <returns></returns>
        bool CheckIfSolvable()
        {
            var n = 0;
            for (var i = 1; i < Cells; i++)
            {
                var num1 = FindItemValueByPosition(i);
                var num2 = FindItemValueByPosition(i - 1);

                if (num1 > num2)
                {
                    n++;
                }
            }

            var emptyPos = FindEmptyItemPosition();
            return n % 2 == (emptyPos + emptyPos / Columns) % 2 ? true : false;
        }

        /// <summary>
        /// Bug Fix - if both of the items you want to swipe are in the Board borders do nothing.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool IsBorderSwitch(int a, int b)
        {
            return _bordersNums.Contains(a) && _bordersNums.Contains(b);
        }

        /// <summary>
        /// Find the cell with a specific number
        /// </summary>
        public ICell FindCellByNumber(int cellNum)
        {
            return _cells.FirstOrDefault(c => c.CellNumber == cellNum);
        }

        /// <summary>
        /// Find the position of stackpanel without children.
        /// </summary>
        /// <returns></returns>
        private int FindEmptyItemPosition()
        {
            for (int i = 0; i < Cells; i++)
            {
                if ((_cells[i]).IsEmptyCell)
                    return i;
            }
            return 0;
        }

        public bool IsAnyMovementAllowed { get; set; }

        public bool AreCellsOrdered
        {
            get
            {
                foreach (var cell in _cells)
                {
                    var iCell = cell as ICell;
                    if (iCell == null)
                    {
                        return false;
                    }
                    if (iCell.CellIndex + 1 != iCell.CellNumber)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        int FindItemValueByPosition(int position)
        {
            return (_cells[position]).IsEmptyCell ?
                Convert.ToInt32((_cells[position]).CellNumber) : Cells;
        }

        /// <summary>
        /// Runs n times and generate random numbers from 1 to _cells, for each number find the current stackpanel that hold him.(FindStackPanelByTagId)
        /// If First and Second number are smaller then _cells then - swipe the Grids and tag values.
        /// If One of the values is _cells the swipe  - One Spackpanel will be cleared of Items.
        /// </summary>
        public void Scrambles()
        {
            var count = 0;
            while (count < (Rows + 1) * (Columns + 1))
            {
                var a = _rnd.Next(1, Cells + 1);
                var b = _rnd.Next(1, Cells + 1);

                if (a == b) continue;

                var cell1 = FindCellByNumber(a);
                var cell2 = FindCellByNumber(b);
                cell1.SwapWith(cell2);
                count++;
            }
        }

        /// <summary>
        /// Check if the Item Can move, Checking all panels around the specific item with -1 +1 -4 +4, if one of them is empty then he can move.
        /// </summary>
        /// <param name="cellToMove">The Item that has been click by user.</param>
        /// <returns></returns>
        public ICell CanMove(ICell cellToMove)
        {
            if (IsAnyMovementAllowed)
            {
                return _cells[FindEmptyItemPosition()];
            }

            if (cellToMove.IsEmptyCell)
            {
                return null;
            }

            int i = cellToMove.CellIndex;

            if (!IsBorderSwitch(i, i + 1) && (i + 1 < Cells) &&
                (_cells[i + 1]).IsEmptyCell)
            {
                return _cells[i + 1];
            }

            if (!IsBorderSwitch(i, i - 1) && (i - 1 > -1) && _cells[i - 1].IsEmptyCell)
            {
                return _cells[i - 1];
            }

            if (i + Columns <= Cells - 1 && _cells[i + Columns].IsEmptyCell)
            {
                return _cells[i + Columns];
            }

            if (i - Columns > -1 && _cells[i - Columns].IsEmptyCell)
            {
                return _cells[i - Columns];
            }

            return null;
        }


        public void Swap(ICell item, ICell to)
        {
            item.SwapWith(to);
            _moves++;
        }
    }
}
