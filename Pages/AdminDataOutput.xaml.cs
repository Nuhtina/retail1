using Microsoft.Win32;
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
        List<product> itemlog;

        public AdminDataOutput()
        {
            InitializeComponent();

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                //var list = AppConnect.model1.product;
                //ProductList.Clear();
                //foreach (var item in list)
                //    ProductList.Add(item);
                itemlog = AppConnect.model2.product.ToList();
                lvProducts.ItemsSource = itemlog;
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
            //if (lvProducts.SelectedItem is product selected)
            //{
            //    var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selected.name}'?",
            //        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //    if (result == MessageBoxResult.Yes)
            //    {
            //        try
            //        {
            //            // Удаляем связанные записи из склада и продаж
            //            var warehouseItems = AppConnect.model1.warehouse
            //                .Where(w => w.productID == selected.wearID)
            //                .ToList();

            //            //var salesItems = AppConnect.model1.sales
            //            //    .Where(s => s.productID == selected.wearID)
            //            //    .ToList();

            //            AppConnect.model1.warehouse.Remove(warehouseItems);
            //            //AppConnect.model1.sales.RemoveRange(salesItems);
            //            AppConnect.model1.product.Remove(selected);
            //            AppConnect.model1.SaveChanges();

            //            LoadProducts();
            //            MessageBox.Show("Товар успешно удален", "Успех",
            //                MessageBoxButton.OK, MessageBoxImage.Information);
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
            //                MessageBoxButton.OK, MessageBoxImage.Error);
            //        }
            //    }
            //}
            if (lvProducts.SelectedItem is product selected)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selected.productID}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем связанные записи из склада
                        //warehouse warehouseItems = AppConnect.model1.warehouse
                        //    .Where(w => w.productID == selected.productID).
                        //    ;

                        //AppConnect.model1.warehouse.Remove(warehouseItems);
                        AppConnect.model2.product.Remove(selected);
                        AppConnect.model2.SaveChanges();

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
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
                //lvProducts.ItemsSource = AppConnect.model1.product
                //    .Include(p => p.Category)
                //    .Include(p => p.Brand)
                //    .Include(p => p.Wear)
                //    .ToList();
                List<product> itemlog = AppConnect.model2.product.ToList();
                lvProducts.ItemsSource = itemlog;
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
            if (itemlog == null || itemlog.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ExportToCsv(itemlog.ToList());
        }

        private void ExportToCsv(List<product> items)
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
                        string wearName = item.wear != null ? item.wear.name : "";
                        string categoryName = item.category != null ? item.category.name : "";
                        string brandName = item.brand != null ? item.brand.name : "";
                        string materialName = item.material != null ? item.material.name : "";
                        string colourName = item.colour != null ? item.colour.name : "";
                        string sizeName = item.size != null ? item.size.name : "";

                        sb.AppendLine($"{item.productID};{wearName};{categoryName};{brandName};{materialName};{colourName};{sizeName}");
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

        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
