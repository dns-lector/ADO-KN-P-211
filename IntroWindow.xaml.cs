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
using System.Data;

namespace ADO_KN_P_211
{
    /// <summary>
    /// Interaction logic for IntroWindow.xaml
    /// </summary>
    public partial class IntroWindow : Window
    {
        private readonly String _msConnectionString;
        private readonly String _myConnectionString;
        SqlConnection? msConnection;
        MySqlConnection? mySqlConnection;

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
            msConnection = new(_msConnectionString);
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
            mySqlConnection = new(_myConnectionString);
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

        private void CreateMsButton_Click(object sender, RoutedEventArgs e)
        {
            // Виконання SQL запитів
            // Загальна схема: 
            using SqlCommand cmd = new();  // інструмент передачі команди (SQL)
            cmd.Connection = msConnection;
            cmd.CommandText = @"
                CREATE TABLE Users (
                    Id    UNIQUEIDENTIFIER PRIMARY KEY,
                    Name  NVARCHAR(64)     NOT NULL,
                    Login NVARCHAR(64)     NOT NULL,
                    PasswordHash CHAR(32)  NOT NULL
                )";
            try
            {
                cmd.ExecuteNonQuery();  // виконання команди. Без повернення результатів
                MsCreateStatusLabel.Content = "Execute OK";
            }
            catch (Exception ex)
            {
                MsCreateStatusLabel.Content = ex.Message;
            }
        }

        private void CreateMyButton_Click(object sender, RoutedEventArgs e)
        {
            using MySqlCommand cmd = new();
            cmd.Connection = mySqlConnection;
            cmd.CommandText = @"
                CREATE TABLE Users (
                    Id    CHAR(36)        PRIMARY KEY,
                    Name  VARCHAR(64)     NOT NULL,
                    Login VARCHAR(64)     NOT NULL,
                    PasswordHash CHAR(32) NOT NULL
                ) Engine = InnoDb, DEFAULT CHARSET = utf8mb4";
            try
            {
                cmd.ExecuteNonQuery();
                MyCreateStatusLabel.Content = "Create OK";
            }
            catch(Exception ex)
            {
                MyCreateStatusLabel.Content = ex.Message;
            }
        }

        private String? GetInputError()
        {
            if(String.IsNullOrEmpty(UserNameTextBox.Text))
            {
                return "Fill Name box";
            }
            if (String.IsNullOrEmpty(UserLoginTextBox.Text))
            {
                return "Fill Login box";
            }
            if (String.IsNullOrEmpty(UserPasswordTextBox.Password))
            {
                return "Fill Password box";
            }
            return null;
        }

        private String md5( String input )
        {
            return Convert.ToHexString(
                System.Security.Cryptography.MD5.HashData(
                    System.Text.Encoding.UTF8.GetBytes(input)));
        }
        
        private void InsertMyButton_Click(object sender, RoutedEventArgs e)
        {
            var errorMessage = GetInputError();
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage);
                return;
            }
            using var cmd = new MySqlCommand(
                $"INSERT INTO Users VALUES( UUID(), '{UserNameTextBox.Text}', '{UserLoginTextBox.Text}', '{md5(UserPasswordTextBox.Password)}' )",
                mySqlConnection);
            try
            {
                cmd.ExecuteNonQuery();
                InsertStatusLabel.Content = "Insert OK";
            }
            catch(Exception ex)
            {
                InsertStatusLabel.Content = ex.Message;
            }
        }

        private void InsertMsButton_Click(object sender, RoutedEventArgs e)
        {
            var errorMessage = GetInputError();
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage);
                return;
            }
            /*  // небезпечний підхід - вразливість до інжекцій
            using var cmd = new SqlCommand(
                $"INSERT INTO Users VALUES( NEWID(), N'{UserNameTextBox.Text}', N'{UserLoginTextBox.Text}', '{md5(UserPasswordTextBox.Password)}' )",
                msConnection);
            */
            using var cmd = new SqlCommand(
                $"INSERT INTO Users VALUES( NEWID(), @name, @login, '{md5(UserPasswordTextBox.Password)}' )",
                msConnection);
            cmd.Parameters.Add(new SqlParameter("@name", System.Data.SqlDbType.VarChar, 64)
            {
                Value = UserNameTextBox.Text
            });
            cmd.Parameters.Add(new SqlParameter("@login", System.Data.SqlDbType.VarChar, 64)
            {
                Value = UserLoginTextBox.Text
            });
            try
            {
                cmd.Prepare();  // підготовка запиту - компіляція без параметрів
                cmd.ExecuteNonQuery();  // виконання - передача даних у скомпільований запит
                InsertStatusLabel.Content = "Insert OK";
            }
            catch (Exception ex)
            {
                InsertStatusLabel.Content = ex.Message;
            }
        }

        private void SelectMsButton_Click(object sender, RoutedEventArgs e)
        {
            if(msConnection == null || 
                msConnection.State == System.Data.ConnectionState.Closed)
            {
                MessageBox.Show(
                    "Необхідно встановити підключення",
                    "Виконання зупинене", 
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            using SqlCommand cmd = new("SELECT * FROM Users", msConnection);
            try
            {
                using SqlDataReader reader = cmd.ExecuteReader();  // виконання команд з поверненням
                /* Reader - відображення таблиці (з довільною кількістю полів)
                    Його схема роботи: читання даних по одному рядку.
                    метод .Read() передає рядок даних у сам об'єкт reader,
                    після чого дані полів рядку доступні
                    а) за ключем reader["id"]  (-> object)
                    б) за допомогою get-терів reader.GetGuid("id") (-> Guid)
                Для переходу на наступний рядок знов подається команда .Read()
                Коли дані закінчаться, виклик Read() поверне false
                !! Після використання reader необхідно закрити, інакше він 
                блокує виконання інших команд (або додати using при створенні)
                 */
                SelectMsTextBlock.Text = "";
                while (reader.Read())
                {
                    var id    = reader.GetGuid("Id");
                    var name  = reader.GetString("Name");
                    var login = reader.GetString("Login");
                    var hash  = reader.GetString("PasswordHash");
                    SelectMsTextBlock.Text += $"{id.ToString()[..5]}... {name} {login} {hash[..5]}...\n";
                }
            }
            catch (Exception ex)
            {
                SelectMsTextBlock.Text = ex.Message;
            }
        }

        private void SelectMyButton_Click(object sender, RoutedEventArgs e)
        {
            if (mySqlConnection == null ||
                mySqlConnection.State == System.Data.ConnectionState.Closed)
            {
                MessageBox.Show(
                    "Необхідно встановити підключення",
                    "Виконання зупинене",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            using MySqlCommand cmd = new("SELECT * FROM Users", mySqlConnection);
            try
            {
                using MySqlDataReader reader = cmd.ExecuteReader();  // виконання команд з поверненням
                SelectMyTextBlock.Text = "";
                while (reader.Read())
                {
                    var id = reader.GetGuid("Id");
                    var name = reader.GetString("Name");
                    var login = reader.GetString("Login");
                    var hash = reader.GetString("PasswordHash");
                    SelectMyTextBlock.Text += $"{id.ToString()[..5]}... {name} {login} {hash[..5]}...\n";
                }
            }
            catch (Exception ex)
            {
                SelectMyTextBlock.Text = ex.Message;
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
 * 
 * Д.З. Виконання запитів. DDL
 * Додати кнопки Видалення таблиці Users та Оновлення - додавання
 * до неї поля birthdate
 * Додати скріншоти результатів роботи (UI)
 */
