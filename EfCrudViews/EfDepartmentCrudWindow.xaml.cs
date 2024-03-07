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
    /// Interaction logic for EfDepartmentCrudWindow.xaml
    /// </summary>
    public partial class EfDepartmentCrudWindow : Window
    {
        public DepartmentModel Model { get; init; }
        public CrudActions Action { get; private set; }

        public EfDepartmentCrudWindow(DepartmentModel model)
        {
            InitializeComponent();
            this.Model = model;
            this.DataContext = this;
            Action = CrudActions.None;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Action = CrudActions.Delete;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Action = CrudActions.Update;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Action = CrudActions.None;
            Close();
        }
    }
}
