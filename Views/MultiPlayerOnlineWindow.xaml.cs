using LabyrinthGame.ViewModels;
using System.Windows;

namespace LabyrinthGame
{
    public partial class MultiPlayerOnlineWindow : Window
    {
        public MultiPlayerOnlineWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DataContext = new MultiPlayerOnlineViewModel();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}