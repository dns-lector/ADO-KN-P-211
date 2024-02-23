using ADO_KN_P_211.DAL.DAO;
using ADO_KN_P_211.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CrudWindow.xaml
    /// </summary>
    public partial class CrudWindow : Window
    {
        public ObservableCollection<User> Users { get; set; }

        public CrudWindow()
        {
            Users = [];
            this.DataContext = this;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (User user in UserDao.GetAll())
            {
                Users.Add(user);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListViewItem item)  // item = sender as ListViewItem
            {
                if(item.Content is User user)
                {
                    UserWindow dialog = new(user);
                    dialog.ShowDialog();
                    switch(dialog.SelectedAction)
                    {
                        case CrudActions.Delete: DeleteUser(user); break;
                        case CrudActions.Update: UpdateUser(user); break;
                    }
                }
            }
        }

        private void DeleteUser(User user)
        {
            /* Видалення даних: особливості
             * - поділяється на "м'яке" та "жорстке"
             *    жорстке - повне видалення без можливості відновлення
             *    м'яке - часткове видалення зі збереженням "контейнера"
             * - регулюється законодавством 
             *    (наприклад, https://gdpr-info.eu/art-17-gdpr/)
             * Жорстке видалення дуже небезпечне, оскільки може порушити
             * зв'язки між даними. 
             * М'яке видалення - "помітка" запису як видаленого, що 
             * зберігає зв'язки та дозволяє відновлення у майбутньому.
             */
            if (UserDao.DeleteUser(user))
            {
                MessageBox.Show("Успішно видалено");
                Users.Remove(user);
            }
            else
            {
                MessageBox.Show("Операція скасована, повторіть пізніше");
            }
        }

        private void UpdateUser(User user)
        {
            if(UserDao.UpdateUser(user))
            {
                MessageBox.Show("Успішно оновлено");
            }
            else
            {
                MessageBox.Show("Операція скасована, повторіть пізніше");
            }
        }
    }
}
/* Підготувати ресурси для зміни паролю:
 * - поруч з полем "DK" додати кнопку "змінити пароль"
 * - За її натисканням відкривається нове вікно з 
 *     двома полями: пароль, повтор паролю
 *     та двома кнопками: зберегти та закрити
 */
