using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace retail.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
            Loaded += (s, e) => tbLogin.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            tbError.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(tbLogin.Text) || pbPassword.Password.Length == 0)
            {
                ShowError("Введите email и пароль");
                return;
            }

            try
            {
                using (var context = new saleEntities())
                {
                    var user = context.users
                        .FirstOrDefault(u => u.email == tbLogin.Text && u.password == pbPassword.Password);

                    if (user == null)
                    {
                        ShowError("Неверный email или пароль");
                        return;
                    }

                    CurrentUser.User = user;

                    // Перенаправление в зависимости от роли
                    if (user.roleID == 1) // Администратор
                    {
                        NavigationService.Navigate(new AdminDataOutput());
                    }
                    else // Обычный пользователь
                    {
                        NavigationService.Navigate(new UserDataOutput());
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка подключения к базе данных");
                // Логирование ошибки
                Debug.WriteLine(ex.Message);
            }
        }

        private void ShowError(string message)
        {
            tbError.Text = message;
            tbError.Visibility = Visibility.Visible;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Registration());
        }
    }

    // Класс для хранения текущего пользователя
    public static class CurrentUser
    {
        public static users User { get; set; }
    }
}
