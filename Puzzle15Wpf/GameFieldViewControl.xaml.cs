using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Puzzle15.Interfaces;
using System.Windows.Media.Animation;

namespace Puzzle15.Wpf
{
    /// <summary>
    /// Interaction logic for GameFieldViewControl.xaml
    /// </summary>
    public partial class GameFieldViewControl : UserControl, IView
    {
        int _cellWidth = 0;
        int _cellHeight = 0;
        ShowCellModeEnum _showCellMode = ShowCellModeEnum.DigitsAndPictures;

        private IModel _model;

        public GameFieldViewControl()
        {
            InitializeComponent();
        }

        public IModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        #region interface IView
        public event GameEpilogFinishedDelegate GameEpilogFinishedEvent;

        ICell IView.CreateCell(int cellNumber, int cellIndex, bool isEmptyCell)
        {
            var cell = new CellViewControl(cellNumber, cellIndex, _cellWidth, _cellHeight, isEmptyCell);
            (cell as ICell).ManipulationEvent += (sender, args) => OnCellClick(sender);
            ContentPanel.Children.Add(cell);
            return cell;
        }

        public void EndOfGame()
        {
            var storyboard = this.GameAreaBorder.FindResource("BlinkStoryboard") as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }

        public void CreateCells(int size)
        {
            ContentPanel.Children.Clear();
            _cellWidth = (int)((GameAreaBorder.ActualWidth / size) * 0.95);
            _cellHeight = (int)((GameAreaBorder.ActualHeight / size) * 0.95);
            _model.Init(size, this);
        }

        public void LoadImageToCells(BitmapImage bitmapImage)
        {
            var cellSize = Math.Min(bitmapImage.PixelHeight / _model.Rows, bitmapImage.PixelWidth / _model.Columns);
            var cellBitMapHeight = cellSize;
            var cellBitMapWidth = cellSize;

            // Calculate stride of source
            int stride = bitmapImage.PixelWidth * (bitmapImage.Format.BitsPerPixel / 8);
            // Create data array to hold source pixel data
            byte[] data = new byte[stride * bitmapImage.PixelHeight];

            int cellNum = 0;
            for (int i = 0; i < _model.Columns; i++)
            {
                for (int j = 0; j < _model.Rows; j++)
                {
                    cellNum++;
                    var cell = _model.FindCellByNumber(cellNum);

                    if (cell.IsEmptyCell) continue;

                    // Copy source image pixels to the data array
                    bitmapImage.CopyPixels(
                        new Int32Rect(
                            j * cellBitMapWidth,
                            i * cellBitMapHeight,
                            cellBitMapWidth,
                            cellBitMapHeight
                            ),
                            data,
                            stride,
                            0
                            );
                    // Create WriteableBitmap to copy the pixel data to.      
                    WriteableBitmap target = new WriteableBitmap(
                      cellBitMapWidth,
                      cellBitMapWidth,
                      bitmapImage.DpiX,
                      bitmapImage.DpiY,
                      bitmapImage.Format,
                      null
                      );

                    // Write the pixel data to the WriteableBitmap.
                    target.WritePixels(
                      new Int32Rect(0, 0, cellBitMapWidth, cellBitMapWidth),
                      data,
                      stride,
                      0
                      );

                    cell.Picture = target;
                }
            }
        }
        #endregion

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (GameEpilogFinishedEvent != null)
                GameEpilogFinishedEvent();
            //NewGame();
        }

        /// <summary>
        /// Click Event on all Grids,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellClick(object sender)
        {
            var item = sender as ICell;// (UIElement)e.OriginalSource;

            if (item == null) return;
            //if (!(item is Grid)) return;

            var to = _model.CanMove(item);

            if (to != null)
            {
                //_model.Moves++;
                _model.Swap(item, to);
                //MoveItem(item, to);
                //CheckBoard();
            }

            if (_model.AreCellsOrdered)
            {
                EndOfGame();
            }
        }

        public string ShowCellModeStr
        {
            get
            {
                return _showCellMode.ToString();
            }
            set
            {
                if (!_showCellMode.ToString().Equals(value))
                {
                    Enum.TryParse<ShowCellModeEnum>(value, out _showCellMode); 
                }
            }
        }

        public ShowCellModeEnum ShowCellMode
        {
            get
            {
                return _showCellMode;
            }
            set
            {
                if (_showCellMode != value)
                {
                    _showCellMode = value;

                    foreach (var cell in ContentPanel.Children.OfType<ICell>())
                    {
                        cell.ShowCellMode = _showCellMode;
                    }
                }
            }
        }
    }
}
