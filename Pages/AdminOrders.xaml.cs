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
    /// Логика взаимодействия для AdminOrders.xaml
    /// </summary>
    public partial class AdminOrders : Page
    {
        public AdminOrders()
        {
            InitializeComponent();

            orderDate.SelectedDate = DateTime.Today.AddDays(-7);
            dpToDate.SelectedDate = DateTime.Today;

            cbStatusFilter.ItemsSource = AppConnect.model2.status.ToList();
            cbStatusFilter.DisplayMemberPath = "name";
            cbStatusFilter.SelectedValuePath = "statusID";
            cbStatusFilter.SelectedIndex = 0;

            var statuses = AppConnect.model2.status.ToList();
            dgOrders.Tag = new { Statuses = statuses };

            LoadZakaz();
        }

        private void LoadZakaz()
        {
            try
            {
                var ordersQuery = AppConnect.model2.zakaz
                    .Include("users")
                    .OrderByDescending(z => z.zakazID)
                    .AsQueryable();

                if (cbStatusFilter.SelectedIndex > 0 && cbStatusFilter.SelectedItem is status selectedStatus)
                {
                    ordersQuery = ordersQuery.Where(z => z.statusID == selectedStatus.statusID);
                }

                if (orderDate.SelectedDate != null)
                {
                    ordersQuery = ordersQuery.Where(z => z.orderDate >= orderDate.SelectedDate);
                }


                zakaz.ItemsSource = ordersQuery.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadZakaz();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить выбранные заказы?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (var item in dgOrders.SelectedItems.Cast<zakaz>())
                    {
                        AppConnect.model2.zakaz.Remove(item);
                    }

                    try
                    {
                        AppConnect.model2.SaveChanges();
                        LoadZakaz();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении заказа: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId &&
                ((FrameworkElement)button.Parent).FindName("cbStatus") is ComboBox comboBox &&
                comboBox.SelectedItem is status selectedStatus)
            {
                try
                {
                    var order = AppConnect.model2.zakaz.FirstOrDefault(z => z.zakazID == orderId);
                    if (order != null)
                    {
                        order.statusID = selectedStatus.statusID;
                        AppConnect.model2.SaveChanges();
                        LoadZakaz();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при изменении статуса: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadZakaz();
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            LoadZakaz();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            cbStatusFilter.SelectedIndex = 0;
            orderDate.SelectedDate = DateTime.Today.AddDays(-7);
            dpToDate.SelectedDate = DateTime.Today;
            LoadZakaz();
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminDataOutput());
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductDetailsPage());
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminUsers());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.User = null;
            NavigationService.Navigate(new Authorization());
        }
    }
}
