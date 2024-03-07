using ADO_KN_P_211.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADO_KN_P_211.EfCrudViews
{
    /// <summary>
    /// Interaction logic for EfManagerCrudWindow.xaml
    /// </summary>
    public partial class EfManagerCrudWindow : Window
    {
        public ManagerModel Model { get; init; }
        public CrudActions Action { get; private set; }

        public EfManagerCrudWindow(ManagerModel model)
        {
            InitializeComponent();
            this.Model = model;
            this.DataContext = this;
            Action = CrudActions.None;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainDepComboBox.SelectedItem =
                this.Model.Departments.First(depName =>
                depName == Model.MainDep);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
