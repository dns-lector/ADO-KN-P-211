using ADO_KN_P_211.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ADO_KN_P_211.DAL.DAO
{
    internal class UserDao
    {
        public static List<User> GetAll()
        {
            using SqlCommand cmd = new(
                "SELECT * FROM Users", 
                App.MsSqlConnection);
            try
            {
                using SqlDataReader reader = cmd.ExecuteReader();
                List<User> users = [];
                while (reader.Read())
                {
                    users.Add(new(reader));
                }
                return users;
            }
            catch (Exception ex)
            {
                return null!;
            }
        }

        public static Boolean AddUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            using var cmd = new SqlCommand(
                $"INSERT INTO Users(Id, Name, Login, PasswordHash, Birthdate) " +
                $"VALUES( NEWID(), @name, @login, @passHash, @birthdate )",
                App.MsSqlConnection);
            cmd.Parameters.Add(new SqlParameter("@name", System.Data.SqlDbType.VarChar, 64)
            {
                Value = user.Name
            });
            cmd.Parameters.Add(new SqlParameter("@login", System.Data.SqlDbType.VarChar, 64)
            {
                Value = user.Login
            });
            cmd.Parameters.Add(new SqlParameter("@passHash", System.Data.SqlDbType.Char, 32)
            {
                Value = user.PasswordHash
            });
            cmd.Parameters.Add(new SqlParameter("@birthdate", System.Data.SqlDbType.DateTime)
            {
                Value = user.Birthdate
            });
            try
            {
                cmd.Prepare();  // підготовка запиту - компіляція без параметрів
                cmd.ExecuteNonQuery();  // виконання - передача даних у скомпільований запит
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static User? GetUserByCredentials(String login, String password)
        {
            ArgumentNullException.ThrowIfNull(login);
            ArgumentNullException.ThrowIfNull(password);
            using var cmd = new SqlCommand(
                "SELECT * FROM Users u WHERE u.login = @login",
                App.MsSqlConnection);
            cmd.Parameters.Add(new SqlParameter("@login", System.Data.SqlDbType.VarChar, 64)
            {
                Value = login
            });
            try
            {
                cmd.Prepare();  
                var reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    /* Д.З. UserDao::GetUserByCredentials
                     * забезпечити перевірку паролю у разі
                     * виявлення користувача за даними логіну.
                     * 
                     * https://datatracker.ietf.org/doc/html/rfc2898
                     * Ознайомитись зі стандартами автентифікації
                     * за паролем (пп.4-5 стандарту)
                     */
                    return new User(reader);
                }
                else   // User не знайдений
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
