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
    /// Логика взаимодействия для AdminWarehouse.xaml
    /// </summary>
    public partial class AdminWarehouse : Page
    {
        List<warehouse> itemlog1;
        public AdminWarehouse()
        {
            InitializeComponent();
            //itemlog1.ItemsSource = AppData.AppConnect.model1.warehouse;
            LoadWarehouseData();
        }

        // Пример загрузки данных (замените на реальные данные из БД)
        private void LoadWarehouseData()
        {
            //try
            //{
            //    var list = AppConnect.model1.warehouse
            //        .Include(warehouse => product.productID)
            //        .Include(product => wear.name)
            //        .Include(warehouse => warehouse.quantity)
            //        .ToList();

            //    itemlog1.Clear();
            //    foreach (var item in list)
            //        itemlog1.Add(item);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
            //        MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            try
            {
                itemlog1 = AppConnect.model2.warehouse.ToList();
                lvWarehouse.ItemsSource = itemlog1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчики кнопок
        private void BtnWarehouse_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminWarehouse()); // Переход на эту же страницу (пример)
        }

        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminSales()); // Переход на страницу продаж
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminOrders()); // Переход на страницу заказов
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminUsers()); // Переход на страницу пользователей
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Выход из системы (пример)
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new Authorization()); // Переход на страницу входа
            }
        }

        private void BtnEditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (lvWarehouse.SelectedItem is warehouse selected)
            {
                NavigationService.Navigate(new AdminDataOutput());
            }
        }
    }
}
