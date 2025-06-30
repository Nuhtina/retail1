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

            dpFromDate.SelectedDate = DateTime.Today.AddDays(-7);
            dpToDate.SelectedDate = DateTime.Today;

            cbStatusFilter.ItemsSource = AppConnect.model1.status.ToList();
            cbStatusFilter.DisplayMemberPath = "name";
            cbStatusFilter.SelectedValuePath = "statusID";
            cbStatusFilter.SelectedIndex = 0;

            var statuses = AppConnect.model1.status.ToList();
            dgOrders.Tag = new { Statuses = statuses };

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                var ordersQuery = AppConnect.model1.zakaz
                    .Include("users")
                    .OrderByDescending(o => o.zakazID)
                    .AsQueryable();

                if (cbStatusFilter.SelectedIndex > 0 && cbStatusFilter.SelectedItem is status selectedStatus)
                {
                    ordersQuery = ordersQuery.Where(o => o.statusID == selectedStatus.statusID);
                }

                if (dpFromDate.SelectedDate != null)
                {
                    ordersQuery = ordersQuery.Where(o => o.zakazID >= dpFromDate.SelectedDate);
                }

                if (dpToDate.SelectedDate != null)
                {
                    var endDate = dpToDate.SelectedDate.Value.AddDays(1);
                    ordersQuery = ordersQuery.Where(o => o.zakazID < endDate);
                }

                dgOrders.ItemsSource = ordersQuery.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
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
                        AppConnect.model1.zakaz.Remove(item);
                    }

                    try
                    {
                        AppConnect.model1.SaveChanges();
                        LoadOrders();
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
                    var order = AppConnect.model1.zakaz.FirstOrDefault(o => o.zakazID == orderId);
                    if (order != null)
                    {
                        order.statusID = selectedStatus.statusID;
                        AppConnect.model1.SaveChanges();
                        LoadOrders();
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
            LoadOrders();
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            cbStatusFilter.SelectedIndex = 0;
            dpFromDate.SelectedDate = DateTime.Today.AddDays(-7);
            dpToDate.SelectedDate = DateTime.Today;
            LoadOrders();
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
