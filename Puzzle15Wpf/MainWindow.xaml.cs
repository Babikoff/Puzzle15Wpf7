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
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.IO;
using Puzzle15.Interfaces;

namespace Puzzle15.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private IModel _model = new Model();
        private DateTime _startTime;

        public MainWindow()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = new TimeSpan(0, 0, 0, 1);
        }

        void CreateCells(int size)
        {
            _model.Init(size);

            var cellWidth = (int)((GameAreaBorder.ActualWidth / _model._columns) * 0.95);
            var cellHeight = (int)((GameAreaBorder.ActualHeight / _model._rows) * 0.95);
            ContentPanel.Children.Clear();
            for (int i = 0; i < _model._cells; i++)
            {
                //TODO: определеться, кто должен создавать Cell (View, Model или кто?)
                var cell = new Cell(i + 1, i, cellWidth, cellHeight, i + 1 == _model._cells);
                (cell as ICell).ManipulationEvent += (sender, args) => OnCellClick(sender);
                ContentPanel.Children.Add(cell);    
            }
        }

        void TimerTick(object sender, EventArgs e)
        {
            var time = DateTime.Now - _startTime;
            txtTime.Text = string.Format(Const.TimeFormat, time.Hours, time.Minutes, time.Seconds);
        }

        /// <summary>
        /// The Range of all Stackpanels is between 15 and 0, when 15 is the first (top left) and 0 is last (right bottom).
        /// 15 , 14 , 13 , 12
        /// 11 , 10 , 09 , 08
        /// 07 , 06 , 05 , 04
        /// 03 , 02 , 01 , 00
        /// The values are 1 to _cells, meaning that 15 equals 1 and 00 equals _cells
        /// </summary>
        public void NewGame()
        {
            _model.NewGame();
            
            txtMoves.Text = _model._moves.ToString();
            txtTime.Text = Const.DefaultTimeValue;

            Scrambles();
            while (!CheckIfSolvable())
            {
                Scrambles();
            }

            _startTime = DateTime.Now.AddSeconds(1);
            _timer.Start();

            GridScrambling.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void EndOfGame()
        {
            _timer.Stop();

            var storyboard = this.GameAreaBorder.FindResource("BlinkStoryboard") as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }

        /// <summary>
        /// Find the cell with a specific number
        /// </summary>
        ICell FindCellByNumber(int cellNum)
        {
            return ContentPanel.Children.OfType<ICell>().FirstOrDefault(c => c.CellNumber == cellNum);
        }

        /// <summary>
        /// Find the position of stackpanel without children.
        /// </summary>
        /// <returns></returns>
        int FindEmptyItemPosition()
        {
            for (int i = 0; i < _model._cells; i++)
            {
                if (((ICell)ContentPanel.Children[i]).IsEmptyCell)
                    return i;
            }
            return 0;
        }

        bool AreCellsOrdered
        {
            get 
            { 
                foreach (var cell in ContentPanel.Children)
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

        /// <summary>
        /// Get the Tag value by StackPanel position.
        /// </summary>
        /// <param name="position">position of StackPanel</param>
        /// <returns>The Grid Tag value, if there is no Grids then returns - _cells</returns>
        int FindItemValueByPosition(int position)
        {
            return ((ICell)ContentPanel.Children[position]).IsEmptyCell ?
                Convert.ToInt32(((ICell)ContentPanel.Children[position]).CellNumber) : _model._cells;
        } 

        /// <summary>
        /// Runs n times and generate random numbers from 1 to _cells, for each number find the current stackpanel that hold him.(FindStackPanelByTagId)
        /// If First and Second number are smaller then _cells then - swipe the Grids and tag values.
        /// If One of the values is _cells the swipe  - One Spackpanel will be cleared of Items.
        /// </summary>
        void Scrambles()
        {
            var count = 0;
            while (count < 25)
            {
                var a = _model._rnd.Next(1, _model._cells + 1);
                var b = _model._rnd.Next(1, _model._cells + 1);

                if (a == b) continue;

                var cell1 = FindCellByNumber(a);
                var cell2 = FindCellByNumber(b);
                cell1.SwapWith(cell2);
                count++;
            }
        }

        /// <summary>
        /// Each move the user do, perform a loop and checks values from 1 to _cells.
        /// if the numbers are not in the correct order than nothing happed.
        /// </summary>
        void CheckBoard()
        {
            var index = 1;
            for (var i = _model._cells - 1; i > 0; i--)
            {
                if (FindItemValueByPosition(i) != index) return;
                index++;
            }

            _timer.Stop();
            WinGrid.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Check if the current Scramble is solveable.
        /// </summary>
        /// <returns></returns>
        bool CheckIfSolvable()
        {
            var n = 0;
            for (var i = 1; i < _model._cells; i++)
            {
                if (!(ContentPanel.Children[i] is ICell)) continue;

                var num1 = FindItemValueByPosition(i);
                var num2 = FindItemValueByPosition(i - 1);

                if (num1 > num2)
                {
                    n++;
                }
            }

            var emptyPos = FindEmptyItemPosition();
            return n % 2 == (emptyPos + emptyPos / _model._columns) % 2 ? true : false;
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

            var to = CanMove(item);

            if (to != null)
            {
                _model._moves++;
                txtMoves.Text = _model._moves.ToString();
                item.SwapWith(to);
                //MoveItem(item, to);
                CheckBoard();
            }

            if (AreCellsOrdered)
            {
                EndOfGame();
            }
        }

        /// <summary>
        /// Check if the Item Can move, Checking all panels around the specific item with -1 +1 -4 +4, if one of them is empty then he can move.
        /// </summary>
        /// <param name="cellToMove">The Item that has been click by user.</param>
        /// <returns></returns>
        ICell CanMove(ICell cellToMove)
        {
            if (AllowAnyMovementCheckBox.IsChecked.HasValue && AllowAnyMovementCheckBox.IsChecked.Value)
            {
                return ContentPanel.Children[FindEmptyItemPosition()] as ICell;
            }

            if (cellToMove.IsEmptyCell)
            {
                return null;
            }

            int i = cellToMove.CellIndex;

            if (!_model.IsBorderSwich(i, i + 1) && i + 1 < _model._cells &&
                ((ICell)ContentPanel.Children[i + 1]).IsEmptyCell)
            {
                return (ICell)(ContentPanel.Children[i + 1]);
            }

            if (!_model.IsBorderSwich(i, i - 1) &&
                i - 1 > -1 &&
                ((ICell)ContentPanel.Children[i - 1]).IsEmptyCell)
            {
                return (ICell)(ContentPanel.Children[i - 1]);
            }

            if (i + _model._columns <= _model._cells - 1 &&
                ((ICell)ContentPanel.Children[i + _model._columns]).IsEmptyCell)
            {
                return (ICell)(ContentPanel.Children[i + _model._columns]);
            }

            if (i - _model._columns > -1 &&
                ((ICell)ContentPanel.Children[i - _model._columns]).IsEmptyCell)
            {
                return (ICell)(ContentPanel.Children[i - _model._columns]);
            }

            return null;
        }

        private void BtnNoThanksManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            WinGrid.Visibility = System.Windows.Visibility.Collapsed;
            NewGame();
            e.Handled = true;
            e.Complete();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_model._firstLoad) return;

            _model._firstLoad = false;
            //CreateCells(3);

            GridScrambling.Visibility = System.Windows.Visibility.Visible;
            GameFieldSizeComboBox.SelectedIndex = 0;
        }

        private void BtnHelpClick(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void BtnPlayClick(object sender, EventArgs e)
        {
            GridScrambling.Visibility = System.Windows.Visibility.Visible;
            NewGame();
        }

        private void BtnAboutClick(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void BtnSettingsClick(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                CreateCells((int)comboBox.SelectedIndex + 3);
                NewGame();
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            NewGame();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var pictureFilePath = GetPictureFilePath();
            var bitmapImage = new BitmapImage(new Uri(pictureFilePath));
            PreviewImage.Source = bitmapImage;
            LoadImagetoCells(bitmapImage);
        }

        private void LoadImagetoCells(BitmapImage bitmapImage)
        {
            var cellSize = Math.Min(bitmapImage.PixelHeight / _model._rows, bitmapImage.PixelWidth / _model._columns);
            var cellBitMapHeight = cellSize;
            var cellBitMapWidth = cellSize;
            //var cellBitMapHeight = bitmapImage.PixelHeight / _rows;
            //var cellBitMapWidth = bitmapImage.PixelWidth / _columns;

            // Calculate stride of source
            int stride = bitmapImage.PixelWidth * (bitmapImage.Format.BitsPerPixel / 8);
            // Create data array to hold source pixel data
            byte[] data = new byte[stride * bitmapImage.PixelHeight];

            int cellNum = 0;
            for (int i = 0; i < _model._columns; i++)
            {
                for (int j = 0; j < _model._rows; j++)
                {
                    cellNum++;
                    var cell = FindCellByNumber(cellNum);

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

        string GetPictureFilePath()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var path = System.IO.Path.Combine(currentPath, "Pictures");
            if (!Directory.Exists(path))
            {
                path = currentPath;
            }
            var openFileDialog = 
                new System.Windows.Forms.OpenFileDialog 
                { 
                    Title = "Укажите файл с картинкой",
                    InitialDirectory = path,
                    Filter = "|*.bmp;*.jpg;*.png"
                };

            var openFileDialogResult =  openFileDialog.ShowDialog();
            if (openFileDialogResult == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog.FileName;
            }

            return null;
        }


    }
}
