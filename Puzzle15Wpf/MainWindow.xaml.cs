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

        private IModel _model;
        private IView _view;
        private DateTime _startTime;

        //Model is public for WPF binding.
        public IModel Model
        {
            get { return _model; }
        }

        //View is public for WPF binding.
        public IView View
        {
            get { return _view; }
        }

        public MainWindow()
        {
            InitializeComponent();
            _model = new Puzzle15.Model.Model();
            this.GameFieldViewControl.Model = _model;
            _view = this.GameFieldViewControl;
            _view.GameEpilogFinishedEvent += delegate() { NewGame(); };

            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = new TimeSpan(0, 0, 0, 1);
        }

        void TimerTick(object sender, EventArgs e)
        {
            var time = DateTime.Now - _startTime;
            txtTime.Text = string.Format(Const.TimeFormat, time.Hours, time.Minutes, time.Seconds);
        }

        //#region interface IView
        //ICell IView.CreateCell(int cellNumber, int cellIndex, bool isEmptyCell)
        //{
        //    var cell = new CellViewControl(cellNumber, cellIndex, _cellWidth, _cellHeight, isEmptyCell);
        //    (cell as ICell).ManipulationEvent += (sender, args) => OnCellClick(sender);
        //    ContentPanel.Children.Add(cell);
        //    return cell;
        //}
        //#endregion

        //void CreateCells(int size)
        //{
        //    ContentPanel.Children.Clear();
        //    _cellWidth = (int)((GameAreaBorder.ActualWidth / size) * 0.95);
        //    _cellHeight = (int)((GameAreaBorder.ActualHeight / size) * 0.95);
        //    _model.Init(size, this);
        //}

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
            
            txtMoves.Text = _model.Moves.ToString();
            txtTime.Text = Const.DefaultTimeValue;

            _startTime = DateTime.Now.AddSeconds(1);
            _timer.Start();
        }

        private void EndOfGame()
        {
            _timer.Stop();
            _view.EndOfGame();
            //var storyboard = this.GameAreaBorder.FindResource("BlinkStoryboard") as Storyboard;
            //if (storyboard != null)
            //{
            //    storyboard.Begin();
            //}
        }

        ///// <summary>
        ///// Each move the user do, perform a loop and checks values from 1 to _cells.
        ///// if the numbers are not in the correct order than nothing happed.
        ///// </summary>
        //void CheckBoard()
        //{
        //    var index = 1;
        //    for (var i = _model.Cells - 1; i > 0; i--)
        //    {
        //        if (FindItemValueByPosition(i) != index) return;
        //        index++;
        //    }

        //    _timer.Stop();
        //    WinGrid.Visibility = System.Windows.Visibility.Visible;
        //}

        ///// <summary>
        ///// Click Event on all Grids,
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void OnCellClick(object sender)
        //{
        //    var item = sender as ICell;// (UIElement)e.OriginalSource;

        //    if (item == null) return;
        //    //if (!(item is Grid)) return;

        //    var to = CanMove(item);

        //    if (to != null)
        //    {
        //        _model.Moves++;
        //        txtMoves.Text = _model.Moves.ToString();
        //        item.SwapWith(to);
        //        //MoveItem(item, to);
        //        //CheckBoard();
        //    }

        //    if (_model.AreCellsOrdered)
        //    {
        //        EndOfGame();
        //    }
        //}

        ///// <summary>
        ///// Check if the Item Can move, Checking all panels around the specific item with -1 +1 -4 +4, if one of them is empty then he can move.
        ///// </summary>
        ///// <param name="cellToMove">The Item that has been click by user.</param>
        ///// <returns></returns>
        //ICell CanMove(ICell cellToMove)
        //{
        //    if (AllowAnyMovementCheckBox.IsChecked.HasValue && AllowAnyMovementCheckBox.IsChecked.Value)
        //    {
        //        return ContentPanel.Children[_model.FindEmptyItemPosition()] as ICell;
        //    }

        //    if (cellToMove.IsEmptyCell)
        //    {
        //        return null;
        //    }

        //    int i = cellToMove.CellIndex;

        //    if (!_model.IsBorderSwich(i, i + 1) && i + 1 < _model.Cells &&
        //        ((ICell)ContentPanel.Children[i + 1]).IsEmptyCell)
        //    {
        //        return (ICell)(ContentPanel.Children[i + 1]);
        //    }

        //    if (!_model.IsBorderSwich(i, i - 1) &&
        //        i - 1 > -1 &&
        //        ((ICell)ContentPanel.Children[i - 1]).IsEmptyCell)
        //    {
        //        return (ICell)(ContentPanel.Children[i - 1]);
        //    }

        //    if (i + _model.Columns <= _model.Cells - 1 &&
        //        ((ICell)ContentPanel.Children[i + _model.Columns]).IsEmptyCell)
        //    {
        //        return (ICell)(ContentPanel.Children[i + _model.Columns]);
        //    }

        //    if (i - _model.Columns > -1 &&
        //        ((ICell)ContentPanel.Children[i - _model.Columns]).IsEmptyCell)
        //    {
        //        return (ICell)(ContentPanel.Children[i - _model.Columns]);
        //    }

        //    return null;
        //}

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_model.FirstLoad) return;
            _model.FirstLoad = false;

            GameFieldSizeComboBox.SelectedIndex = 1;
        }

        private void GameFieldSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                int size = 3;
                int.TryParse((comboBox.SelectedItem as ComboBoxItem).Tag.ToString(), out size);
                _view.CreateCells(size);
                NewGame();
            }
        }

        //TODO: в Controller
        private void LoadPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var pictureFilePath = GetPictureFilePath();
            if (string.IsNullOrEmpty(pictureFilePath)) return;
            
            var bitmapImage = new BitmapImage(new Uri(pictureFilePath));
            PreviewImage.Source = bitmapImage;
            _view.LoadImageToCells(bitmapImage);
            PicturePreviewPanel.Visibility = System.Windows.Visibility.Visible;
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

        #region Button press handlers
        private void BtnNoThanksManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            WinGrid.Visibility = System.Windows.Visibility.Collapsed;
            NewGame();
            e.Handled = true;
            e.Complete();
        }

        private void BtnHelpClick(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void BtnPlayClick(object sender, EventArgs e)
        {
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
        #endregion
    }
}
