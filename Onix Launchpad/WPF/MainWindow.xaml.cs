using System.Windows;

namespace Onix_Launchpad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenPage_NewItem(object sender, RoutedEventArgs e)
        {
            NewItem netITem = new NewItem();
            netITem.Show();
            this.Hide();
        }
    }
}
