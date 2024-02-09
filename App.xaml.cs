using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Windows;

namespace ADO_KN_P_211
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static SqlConnection? _msConnection;
        public static SqlConnection MsSqlConnection { 
            get
            {
                if( _msConnection == null)
                {
                    _msConnection = new(
                        JsonSerializer.Deserialize<JsonElement>(
                            System.IO.File.ReadAllText("appsettings.json")
                        )
                        .GetProperty("ConnectionStrings")
                        .GetProperty("LocalMS")
                        .GetString()!
                    );
                    _msConnection.Open();
                }
                return _msConnection;
            }
        }
        public static String md5(String input)
        {
            return Convert.ToHexString(
                System.Security.Cryptography.MD5.HashData(
                    System.Text.Encoding.UTF8.GetBytes(input)));
        }

    }

}
/* Д.З. Розширити засоби реєстрації користувача - 
 * додати "дату народження", переконатись у її передачі
 * до бази даних.
 * До звіту з ДЗ додати скріншот таблиці БД із датами народження.
 * Також відобразити ці дані у результатів SELECT запитів
 * (IntroWindow), додати скріншоти
 */
