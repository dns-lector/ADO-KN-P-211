using Microsoft.EntityFrameworkCore;
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
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace ADO_KN_P_211
{
    /// <summary>
    /// Interaction logic for EfWindow.xaml
    /// </summary>
    public partial class EfWindow : Window
    {
        public EfWindow()
        {
            App.EfDataContext.Database.Migrate();  // контроль міграцій
            InitializeComponent();
        }
        /* Д.З. Реалізувати "монітор БД" - табличку
         * з назвами таблиць та кількістю записів у них
         * Managers: 60
         * Departments: 7
         * Products: ...
         * Sales: ...
         */
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayStatistics();
        }
        private void DisplayStatistics()
        {
            ManagersCountLabel.Content = App.EfDataContext.Managers.Count();
            SalesCountLabel.Content = App.EfDataContext.Sales.Count();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            /* Повний перелік даних - ітерація колекції DbSet */
            foreach(var dep in  App.EfDataContext.Departments)
            {
                ResultLabel.Content += $"{dep.Name} \n";
            }
            /* Додавання умов (фільтрів) */
            var query = App.EfDataContext
                .Departments
                .Where(d => d.InternationalName != null)
                .Take(10)
                .OrderBy(d => d.Name);
            // query ~ SELECT TOP 10 * FROM Departments
            // WHERE InternationalName IS NOT NULL
            // ORDER BY Name

            foreach (var dep in query)  // Execute(query)
            {
                ResultLabel.Content += $"int - {dep.InternationalName} \n";
            }

            var query2 = App.EfDataContext
                .Departments
                .Select(d => d.Name);  // SELECT Name FROM Departments

            ResultLabel.Content += query2.Min() + "\n";

            var query3 = App.EfDataContext
                .Departments  
                .OrderBy(d => d.Id)   //       IQueryable (to SQL)
                .AsEnumerable()       // поділ 
                .Select(d =>          //       IEnumerable (to Collection)
                    new { Card = d.Id.ToString()[..5] + d.Name })
                .OrderBy(a => a.Card)
                .First();
            ResultLabel.Content += query3.Card + "\n";
        }

        private void CheapButton_Click(object sender, RoutedEventArgs e)
        {
            // Визначити самий дешевий товар (Product), вивести назву і ціну
            ResultLabel.Content =
                App.EfDataContext
                .Products
                .OrderBy(p => p.Price)
                .Select(p => p.Name + " " + p.Price)
                .First();
            // Додати відомості про середню ціну товару
            
        }

        private void SalesButton_Click(object sender, RoutedEventArgs e)
        {
            AddSales().ContinueWith((_) => DisplayStatistics());
            SalesCountLabel.Content = "Updating...";
        }

        private async Task AddSales()
        {
            for (int i = 0; i < 10000; i++)
            {
                App.EfDataContext.Sales.Add(new EfContext.Sale()
                {
                    Id = Guid.NewGuid(),
                    ManagerId = App.EfDataContext.Managers.OrderBy(r => Guid.NewGuid()).First().Id,
                    ProductId = App.EfDataContext.Products.OrderBy(r => Guid.NewGuid()).First().Id,
                    Quantity = App.Random.Next(1, 10),
                    SaleDt = new DateTime(2023, 1, 1)
                                    .AddDays(App.Random.Next(364))
                                    .AddHours(App.Random.Next(9, 20))
                                    .AddMinutes(App.Random.Next(0, 59))
                                    .AddSeconds(App.Random.Next(0, 59)),
                });
            }
            // !! зміни у контексті (додавання, модифікація, видалення)
            // відразу не відображаються у БД. Необхідно подавати команду оновлення
            // Але не обов'язково зберігати після кожної зміни, можна накопичити
            await App.EfDataContext.SaveChangesAsync();
        }
    }
}
/* Д.З. Скласти EF(LINQ) запити на одержання наступних даних:
 * - самий молодий менеджер
 * - товар з найкоротшою назвою
 * - випадковий товар (за натиском кнопки дані оновлюються)
 * - випадковий менеджер
 * Також додати інформацію (вивести) про випадкову дату (у межах 2023 року)
 *  та час у межах 9:00 до 20:00
 */
/* Entity Framework Core - ORM-інструментарій для спрощення роботи з
 * даними, орієнтований на бази даних.
 * Entity - термін, синонім DTO - різновид об'єктів (класів) з певними
 * обмеженнями, спрямованими на "носій даних"
 * Мета - створити єдиний інтерфейс для роботи з даними
 * 
 * Встановлення: додаються щонайменше три NuGet пакети
 * - Microsoft.EntityFrameworkCore - основа (ядро) фреймворку
 * - Microsoft.EntityFrameworkCore.Tools - інструменти командного рядка
 * - Microsoft.EntityFrameworkCore.SqlServer або
 *    Pomelo.EntityFrameworkCore.MySql або інший - "драйвер" конкретної СУБД
 *   
 * EF використовують у двох підходах
 * "Data first" - коли існує БД і для неї потрібна програма на .NET
 * "Code first" - коли БД немає і проєкт починається з .NET
 * 
 * Центральним поняттям EF є контекст даних - відображення БД та її таблиць.
 * Контекст даних замінює DAL
 * У контексті описуються "колекції" DbSet та слухачі подій життєвого
 * циклу (створення та налаштування контексту)
 * 
 * Міграції.
 * це механізм створення скриптових виразів, які відображають
 * зміни у БД (або їх вносять, або контролюють). Можна провести
 * аналогію з комітами (git) у тому, що їх можна повертати до
 * попередніх станів.
 * Для управління міграціями вживається інструментарій командного рядка
 * Tools -> NuGet -> PM Console
 * Міграції аналізують нащадків DbContext, тому контекст має бути
 * створеним, а також збирають проєкт, тому не повинно бути помилок.
 * Вводимо у РМ консоль
 * PM> Add-Migration Initial
 * У разі успішної роботи з'являється каталог "Migrations", у ньому - 
 * файли з описом міграції.
 * Впроваджуємо міграцію до БД
 * PM> Update-Database
 * Підключаємось до створеної БД
 * Tools -> Connect to DB -> MS SQL -> 
 * Server name:  (LocalDB)\MSSQLLocalDB
 * Select or enter DB name: (вибрати з переліку) ado211ef
 * [Server explorer] -> ...ado211ef
 */
