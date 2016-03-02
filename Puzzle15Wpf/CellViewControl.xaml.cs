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

namespace Puzzle15.Wpf
{
    /// <summary>
    /// Interaction logic for Cell.xaml
    /// </summary>
    public partial class CellViewControl : UserControl, ICell
    {
        int _сellNumber = 0;
        bool _isEmptyCell;
        ImageSource _picture = null;
        ShowCellModeEnum _showCellMode = ShowCellModeEnum.DigitsAndPictures;

        public event EventHandler<EventArgs> ManipulationEvent;

        public CellViewControl()
        {
            InitializeComponent();
            CellNumber = 1;
        }

        public CellViewControl(int cellNumber, int cellIndex, int width, int height, bool isEmptyCell)
            : this()
        {
            _isEmptyCell = isEmptyCell;
            CellNumber = cellNumber;
            CellIndex = cellIndex;
            Width = width;
            Height = height;
        }

        public int CellNumber 
        {
            get
            {
                return _сellNumber;
            }
            set
            {
                _сellNumber = value;
                UpdateView();
            }
        }

        public int CellIndex { get; private set; }

        public ImageSource Picture
        {
            get
            {
                return _picture;
            }
            set
            {
                _picture = value;
                ImageView.Source = _picture;
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

                    switch (_showCellMode)
                    {
                        case ShowCellModeEnum.DigitsOnly:
                            ImageView.Visibility = System.Windows.Visibility.Hidden;
                            NumberView.Visibility = System.Windows.Visibility.Visible;
                            break;
                        case ShowCellModeEnum.PicturesOnly:
                            ImageView.Visibility = System.Windows.Visibility.Visible;
                            NumberView.Visibility = System.Windows.Visibility.Hidden;
                            break;
                        case ShowCellModeEnum.DigitsAndPictures:
                        default:
                            ImageView.Visibility = System.Windows.Visibility.Visible;
                            NumberView.Visibility = System.Windows.Visibility.Visible;
                            break;
                    }
                }
            }
        }

        public void SwapWith(ICell other)
        {
            var tmpCellNumber = other.CellNumber;
            other.CellNumber = this.CellNumber;
            CellNumber = tmpCellNumber;

            var tmpImageSource = other.Picture;
            other.Picture = this.Picture;
            this.Picture = tmpImageSource;

            var tmpIsEmptyCell = other.IsEmptyCell;
            other.IsEmptyCell = this.IsEmptyCell;
            IsEmptyCell = tmpIsEmptyCell;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ManipulationEvent != null)
            {
                ManipulationEvent(this, new EventArgs());
            }
        }

        public bool IsEmptyCell 
        {
            get
            {
                return _isEmptyCell;
            }
            set
            {
                _isEmptyCell = value;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            if (_isEmptyCell)
            {
                NumberLabel.Content = String.Empty;
            }
            else
            {
                NumberLabel.Content = _сellNumber.ToString();
            }
        }

    }
}
