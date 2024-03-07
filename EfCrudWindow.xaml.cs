using ADO_KN_P_211.EfContext;
using ADO_KN_P_211.EfCrudViews;
using ADO_KN_P_211.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ADO_KN_P_211
{
    /// <summary>
    /// Interaction logic for EfCrudWindow.xaml
    /// </summary>
    public partial class EfCrudWindow : Window
    {
        private ICollectionView departmentsView;  // для налаштування відображення колекцій
        private readonly Predicate<Object> departmentsFilter = 
            obj => (obj as Department)?.DeleteDt == null;

        public EfCrudWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            LoadManagersData();
        }
        private void LoadManagersData()
        {
            ManagersListView.ItemsSource = null;
            App.EfDataContext.Managers.Load();
            ManagersListView.ItemsSource =
                App.EfDataContext.Managers.Local.ToObservableCollection();
        }
        private void LoadData()
        {
            DepartmentsListView.ItemsSource = null;

            App.EfDataContext.Departments.Load();

            DepartmentsListView.ItemsSource = 
                App.EfDataContext
                .Departments
                .Local
                .ToObservableCollection();

            departmentsView = CollectionViewSource.GetDefaultView(
                DepartmentsListView.ItemsSource);
            departmentsView.Filter = departmentsFilter;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item &&
                item.Content is Department department)
            {
                EfDepartmentCrudWindow dialog = 
                    new(DepartmentModel.FromEntity(department));
                dialog.ShowDialog();

                if(dialog.Action == CrudActions.Update)
                {
                    department.Name = dialog.Model.Name;
                    department.InternationalName = dialog.Model.InternationalName;
                    App.EfDataContext.SaveChanges();
                    LoadData();
                }
                if (dialog.Action == CrudActions.Delete)
                {
                    department.DeleteDt = DateTime.Now;
                    App.EfDataContext.SaveChanges();
                    LoadData();
                }
            }
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            Department department = new()
            {
                Id = Guid.NewGuid(),
            };
            EfDepartmentCrudWindow dialog =
                    new(DepartmentModel.FromEntity(department));
            dialog.ShowDialog();
            if (dialog.Action == CrudActions.Update)
            {
                department.Name = dialog.Model.Name;
                department.InternationalName = dialog.Model.InternationalName;
                App.EfDataContext.Add(department);
                App.EfDataContext.SaveChanges();
                LoadData();
            }
        }

        private void AllDepartmentsButton_Click(object sender, RoutedEventArgs e)
        {
            departmentsView.Filter = departmentsView.Filter == null
                ? departmentsFilter
                : null;
        }

        private void ListViewItem_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item &&
                item.Content is Manager manager)
            {
                EfManagerCrudWindow dialog = new(
                    new ManagerModel(manager)
                    {
                        Departments = App.EfDataContext
                            .Departments
                            .Select(d => d.Name)
                            .ToList(),
                        MainDep = manager.MainDepartment.Name
                    });
                dialog.ShowDialog();
            }
        }

        private void AddManagerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AllManagersButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
/*
 * Д.З. Реалізувати CRUD засоби для сутності Sale
 * - Перелік усіх продажів за "сьогодні" (ListView)
 * - Модель відображення з переліком товарів (назв)
 * - Вікно для редагування, у якому відображається перелік товарів
 * 
 * = Перелік менеджерів - поки не вимагається
 */
