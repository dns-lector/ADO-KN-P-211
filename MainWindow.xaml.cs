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

namespace ADO_KN_P_211
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

        private void IntroButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new IntroWindow().ShowDialog();
            this.Show();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new AuthWindow().ShowDialog();
            this.Show();
        }

        private void CrudButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new CrudWindow().ShowDialog();
            this.Show();
        }

        private void EfButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new EfWindow().ShowDialog();
            this.Show();
        }

        private void EfCrudButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new EfCrudWindow().ShowDialog();
            this.Show();
        }
    }
}