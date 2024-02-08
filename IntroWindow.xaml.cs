using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ADO_KN_P_211
{
    /// <summary>
    /// Interaction logic for IntroWindow.xaml
    /// </summary>
    public partial class IntroWindow : Window
    {
        private readonly String _msConnectionString;
        private readonly String _myConnectionString;
        public IntroWindow()
        {
            InitializeComponent();

            var config = JsonSerializer.Deserialize<JsonElement>(
                System.IO.File.ReadAllText("appsettings.json"));

            var connectionStrings = config
                .GetProperty("ConnectionStrings");

            _msConnectionString = connectionStrings
                .GetProperty("LocalMS")
                .GetString()!;

            _myConnectionString = connectionStrings
               .GetProperty("LocalMy")
               .GetString()!;
        }

        private void ConnectMsButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection msConnection = new(_msConnectionString);
            try
            {
                msConnection.Open();
                MsConnectionStatusLabel.Content = "Connected";
            }
            catch(Exception ex)
            {
                MsConnectionStatusLabel.Content = ex.Message;
            }
        }

        private void ConnectMyButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection mySqlConnection = new(_myConnectionString);
            try
            {
                mySqlConnection.Open();
                MyConnectionStatusLabel.Content = "Connected";
            }
            catch (Exception ex)
            {
                MyConnectionStatusLabel.Content = ex.Message;
            }
        }
    }
}
/*
 * ADO.NET Вступ
 * ADO.NET - технологія доступу до даних, яка вводить єдиний інтерфейс
 * для роботи з різними джерелами даних (з різними СУБД)
 * - Сама БД: ПКМ (Project) - Add new item - Service based Database - OK
 * - Параметри підключення. Зазвичай, їх закладають в окремий файл
 *    з конфігурацією (appsettings.json)
 * - Драйвер підключення (конектори) - додаткові бібліотеки (NuGet), 
 *    які містять інструменти для з'єднання з відповідною СУБД
 *    SQL Server -- System.Data.SqlClient
 *    MySQL/MariaDB -- MySql.Data.MySqlClient;
 *    
 * Д.З. Забезпечити підключення програми до баз даних
 * Реалізувати контроль за активністю підключень - не відкривати
 * повторні, якщо є раніше відкриті (як варіант - заблокувати кнопку)
 * Додати кнопку примусового закриття підключень, також
 * запровадити її контроль (щоб не закривати закрите)
 */
