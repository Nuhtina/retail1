using retail.AppData;
using System;
using System.Collections;
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
    /// Логика взаимодействия для AdminDataOutput.xaml
    /// </summary>
    public partial class AdminDataOutput : Page
    {
        public IEnumerable ProductList { get; private set; }

        public AdminDataOutput()
        {
            InitializeComponent();
            lvProducts.ItemsSource = ProductList;
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var list = AppConnect.model1.product
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Wear)
                    .ToList();

                ProductList.Clear();
                foreach (var item in list)
                    ProductList.Add(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Навигация
        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserOrders());
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminUsers());
        }

        // Управление товарами
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditProduct());
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lvProducts.SelectedItem is product selected)
            {
                NavigationService.Navigate(new EditProduct());
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lvProducts.SelectedItem is product selected)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selected.name}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем связанные записи из склада и продаж
                        var warehouseItems = AppConnect.model1.warehouse
                            .Where(w => w.productID == selected.productID)
                            .ToList();

                        var salesItems = AppConnect.model1.sales
                            .Where(s => s.productID == selected.productID)
                            .ToList();

                        AppConnect.model1.warehouse.RemoveRange(warehouseItems);
                        AppConnect.model1.sales.RemoveRange(salesItems);
                        AppConnect.model1.product.Remove(selected);
                        AppConnect.model1.SaveChanges();

                        LoadProducts();
                        MessageBox.Show("Товар успешно удален", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Authorization());
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lvProducts.ItemsSource = AppConnect.model1.product
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.Wear)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnWarehouse_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminWarehouse());
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (ProductList == null || ProductList.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ExportToCsv(ProductList.ToList());
        }

        private void ExportToCsv(List<Product> items)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv",
                    FileName = "Товары.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var sb = new StringBuilder();

                    // Заголовки столбцов, разделённые точкой с запятой
                    sb.AppendLine("ID;Название;Тип одежды;Категория;Бренд;Материал;Цвет;Размер");

                    foreach (var item in items)
                    {
                        string wearName = item.Wear != null ? item.Wear.Name : "";
                        string categoryName = item.Category != null ? item.Category.Name : "";
                        string brandName = item.Brand != null ? item.Brand.Name : "";
                        string materialName = item.Material != null ? item.Material.Name : "";
                        string colourName = item.Colour != null ? item.Colour.Name : "";
                        string sizeName = item.Size != null ? item.Size.Name : "";

                        sb.AppendLine($"{item.ProductID};{item.Name};{wearName};{categoryName};{brandName};{materialName};{colourName};{sizeName}");
                    }

                    System.IO.File.WriteAllText(saveDialog.FileName, sb.ToString(), Encoding.UTF8);

                    MessageBox.Show("Файл успешно сохранён!", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
