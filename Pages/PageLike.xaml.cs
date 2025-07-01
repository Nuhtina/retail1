using retail.AppData;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace retail.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageLike.xaml
    /// </summary>
    public partial class PageLike : Page
    {
        public PageLike()
        {
            InitializeComponent();
            LoadFavorites();
        }
        //public static class CurrentUser
        //{
        //    public static object users { get; internal set; }
        //}
        public class User // или users, в зависимости от вашей модели
        {
            public int userID { get; set; }
            // другие свойства...
        }

        // 2. Измените CurrentUser на строго типизированную версию
        public static class CurrentUser
        {
            public static User User { get; set; } // Указываем конкретный тип User вместо object
        }

        private void LoadFavorites()
        {
            if (CurrentUser.User == null) return;

            // Получаем избранные товары пользователя
            var favorites = AppConnect.model2.zakaz
                .Where(z => z.userID == CurrentUser.User.userID && z.statusID == 1) // statusID=1 для избранного (предположение)
                .Join(AppConnect.model2.product,
                    z => z.productID,
                    p => p.productID,
                    (z, p) => p)
                .ToList();

            FavoritesItemsControl.ItemsSource = favorites;

            // Показываем сообщение, если избранное пусто
            TxtNoFavorites.Visibility = favorites.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void BtnRemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.User == null) return;

            var button = (Button)sender;
            int productId = (int)button.CommandParameter;

            // Находим запись в заказах (избранном)
            var favorite = AppConnect.model2.zakaz
                .FirstOrDefault(z => z.userID == CurrentUser.User.userID &&
                                    z.productID == productId &&
                                    z.statusID == 1); // statusID=1 для избранного

            if (favorite != null)
            {
                try
                {
                    // Удаляем из избранного
                    AppConnect.model2.zakaz.Remove(favorite);
                    AppConnect.model2.SaveChanges();

                    // Обновляем список
                    LoadFavorites();

                    MessageBox.Show("Товар удален из избранного", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении из избранного: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Навигационные методы (остаются без изменений)
        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new UserDataOutput());
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CartPage());
        }

        private void BtnFavorites_Click(object sender, RoutedEventArgs e)
        {
            // Уже на странице избранного
        }

        private void BtnAccount_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AccountPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Authorization());
        }

        private void BtnUserOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserOrders());
        }

    }
}
