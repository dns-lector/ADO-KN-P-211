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
        public static bool DeleteUser(User user, bool hardMode = false)
        {
            ArgumentNullException.ThrowIfNull(user);
            if (user.Id == default)  // можна покращити і перевіряти наявність у БД
            {
                throw new ArgumentException(
                    "Id field value must not be default",
                    "user.Id");
            }
            using var cmd = new SqlCommand(null, App.MsSqlConnection);
            if (hardMode)
            {
                cmd.CommandText = $"DELETE FROM Users WHERE Id='{user.Id}' ";
            }
            else
            {
                cmd.CommandText = $"UPDATE Users SET DeleteDt = CURRENT_TIMESTAMP, Name='', Birthdate=NULL WHERE Id='{user.Id}' ";
            }
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                App.LogError(ex.Message);
                return false;
            }
        }

        public static bool UpdateUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            if(user.Id == default)  // можна покращити і перевіряти наявність у БД
            {
                throw new ArgumentException(
                    "Id field value must not be default",
                    "user.Id");
            }
            using var cmd = new SqlCommand(
                $"UPDATE Users SET Name=@name, Login=@login, PasswordHash=@passHash, Birthdate=@birthdate " +
                $" WHERE Id=@id",
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
                Value = (object?) user.Birthdate ?? DBNull.Value
            });
            cmd.Parameters.Add(new SqlParameter("@id", System.Data.SqlDbType.UniqueIdentifier)
            {
                Value = user.Id
            });
            try
            {
                cmd.Prepare();
                cmd.ExecuteNonQuery(); 
                return true;
            }
            catch (Exception ex)
            {
                App.LogError(ex.Message);
                return false;
            }
        }

        public static List<User> GetAll(bool showDeleted = false)
        {
            using SqlCommand cmd = new(
                "SELECT * FROM Users " + (showDeleted ? "" : " WHERE DeleteDt IS NULL"), 
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
                App.LogError(ex.Message);
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
                App.LogError(ex.Message);
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
                App.LogError(ex.Message);
                return null;
            }
        }
    }
}
/* Д.З. Реалізувати зміну паролю із внесенням до БД
 * ** Реорганізувати код проєкту таким чином, щоб
 * після змін (Update) даних користувача вони відображались
 * у переліку користувачів.
 */
