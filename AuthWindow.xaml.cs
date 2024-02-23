using ADO_KN_P_211.DAL.DAO;
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
            try
            {
                if(UserDao.AddUser(new()
                {
                    Name = RegName.Text,
                    Login = RegLogin.Text,
                    PasswordHash = App.md5(RegPassword.Password),
                    Birthdate = DateTime.Now,
                }))
                {
                    MessageBox.Show("Insert OK");
                }
                else
                {
                    MessageBox.Show("Insert fails");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
/* CRUD - Create Read Update Delete
 * - життєвий цикл даних
 * - перелік необхідних "операцій" для повноти системи, що працює з даними.
 * CRUD-повнота - вимога до системи щодо її функціональності
 * 
 */
/* ORM. DAO. DAL.
 * Object Relation Mapping (ORM) - відображення (Mapping)
 * даних та зв'язків між ними (Relation) на об'єкти. Іншими
 * словами, створення об'єктів мови програмування які за 
 * структурою максимально наближені до даних, що надходять до
 * програми (БД, JSON, тощо). Такі об'єкти також відомі як
 * DTO (data transfer object) або Entity.
 * Робота з даними переводиться до роботи з об'єктами.
 * Утворюються "перехідні" засоби - DAO (data access object)
 * UserDao.CreateUser(UserDto user)
 * List<UserDto> UserDao.GetAll()
 * Сукупність DAO для різних даних утворює DAL (data access layer) -
 * архітектурний шар проєкту, також відомий як контекст даних.
 */
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
