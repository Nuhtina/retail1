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
    /// Логика взаимодействия для UserOrders.xaml
    /// </summary>
    public partial class UserOrders : Page
    {
        public UserOrders()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            var userId = AppConnect.model1.users.FirstOrDefault()?.userID ?? 0;


            var ordersFromDb = AppConnect.model1.zakaz
                .Where(z => z.userID == userId)  
                .OrderByDescending(z => z.zakazID)
                .ToList();

            var formattedOrders = ordersFromDb.Select(z => new
            {
                z.zakazID,
                OrderDate = z.zakazID.ToString(), 
                TotalAmount = 0, 
                Status = z.status.name, 
                OrderItems = new List<object>
            {
                new
                {
                    ProductName = z.product.productID,
                    Quantity = 1, 
                    Price = 0 
                }
            }
            }).ToList();

        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Authorization());
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserDataOutput());
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }

        private void BtnFavorites_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageLike());
        }

        private void BtnAccount_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AccountPage());
        }

        private void BtnUserOrders_Click(object sender, RoutedEventArgs e)
        {
            // Уже на странице заказов
        }
    }
}
