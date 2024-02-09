using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
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
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }
        private String? GetInputError()
        {
            if (String.IsNullOrEmpty(RegName.Text))
            {
                return "Fill Name box";
            }
            if (String.IsNullOrEmpty(RegLogin.Text))
            {
                return "Fill Login box";
            }
            if (String.IsNullOrEmpty(RegPassword.Password))
            {
                return "Fill Password box";
            }
            if (RegPassword.Password != RegRepeat.Password)
            {
                return "Passwords mismatch";
            }
            return null;
        }
        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            var errorMessage = GetInputError();
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage,
                    "Виконання зупинене",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            using var cmd = new SqlCommand(
                $"INSERT INTO Users VALUES( NEWID(), @name, @login, '{App.md5(RegPassword.Password)}' )",
                App.MsSqlConnection);
            cmd.Parameters.Add(new SqlParameter("@name", System.Data.SqlDbType.VarChar, 64)
            {
                Value = RegName.Text
            });
            cmd.Parameters.Add(new SqlParameter("@login", System.Data.SqlDbType.VarChar, 64)
            {
                Value = RegLogin.Text
            });
            try
            {
                cmd.Prepare();  // підготовка запиту - компіляція без параметрів
                cmd.ExecuteNonQuery();  // виконання - передача даних у скомпільований запит
                MessageBox.Show( "Insert OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
/* Підключення різних ресурсів (вікон)
 * Підключення до БД - достатньо складний ресурс і відкривати
 * декілька підключень до одної БД - витрата ресурсів.
 * = З одного боку, сама платформа .NET контролює підключення і
 *    при спробі відкрити нове підключення з тими ж даними, що є
 *    переліку відкритих підключень, поверне посилання на наявний
 *    об'єкт (не буде відкривати нове підключення)
 * = З іншого боку, не всі платформи забезпечують такий контроль
 *    і архітектура програми має від початку будуватись правильно
 *    без команд повторного відкриття підключень.
 */
