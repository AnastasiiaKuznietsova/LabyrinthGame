using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace LabyrinthGame
{
    public partial class FirstPlayingFieldWindow : Window
    {
        public FirstPlayingFieldWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void OnCardDrag(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var image = sender as Image;
                if (image != null)
                {
                    var card = image.DataContext as Card;
                    var viewModel = DataContext as FirstPlayingFieldViewModel;
                    viewModel?.OnCardDrag(card, image);
                }
            }
        }

        private void OnCardDrop(object sender, DragEventArgs e)
        {
            var viewModel = DataContext as FirstPlayingFieldViewModel;
            viewModel?.OnCardDrop(e);
        }
    }

    public class ItemCountToGridSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int itemCount)
            {
                int gridSize = (int)Math.Ceiling(Math.Sqrt(itemCount));
                return gridSize;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}