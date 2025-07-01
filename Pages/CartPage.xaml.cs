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
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        public decimal TotalAmount { get; set; }

        public CartPage()
        {
            InitializeComponent();
            LoadCartItems();
            DataContext = this;
        }

        private void LoadCartItems()
        {
            try
            {
                if (CurrentUser.User == null) return;

                var orders = AppConnect.model2.zakaz
                    .Include("Product")
                    .Where(z => z.userID == CurrentUser.User.userID && z.statusID == 1) 
                    .ToList();

                if (orders != null && orders.Any())
                {
                    lvCartItems.ItemsSource = orders;
                    TotalAmount = orders.Sum(z => z.product?.price ?? 0); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке корзины: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int zakazID)
            {
                try
                {
                    var item = AppConnect.model2.zakaz.Find(zakazID);
                    if (item != null)
                    {
                        AppConnect.model2.zakaz.Remove(item);
                        AppConnect.model2.SaveChanges();
                        LoadCartItems();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.User == null)
            {
                MessageBox.Show("Для оформления заказа необходимо авторизоваться", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (lvCartItems.Items.Count == 0)
            {
                MessageBox.Show("Корзина пуста", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationService.Navigate(new AdminOrders());
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new UserDataOutput());
        private void BtnCart_Click(object sender, RoutedEventArgs e) { /* Уже на этой странице */ }
        private void BtnFavorites_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new PageLike());
        private void BtnAccount_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AccountPage());
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.User = null;
            NavigationService.Navigate(new Authorization());
        }

        private void BtnUserOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserOrders());
        }

    }

}
