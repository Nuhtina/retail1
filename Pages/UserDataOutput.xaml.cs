using retail.AppData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для UserDataOutput.xaml
    /// </summary>
    public partial class UserDataOutput : Page
    {
        private ObservableCollection<product> _allProducts;
        private ObservableCollection<category> _categories;
        private ObservableCollection<brand> _brands;

        public UserDataOutput()
        {
            InitializeComponent();
            Loaded += UserDataOutput_Loaded;
        }

        private void UserDataOutput_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Загрузка всех товаров
                _allProducts = new ObservableCollection<product>(AppConnect.model1.product.ToList());
                lvProducts.ItemsSource = _allProducts;

                // Категории (с "Все категории")
                _categories = new ObservableCollection<category>(
                    new[] { new category { categoryID = 0, name = "Все категории" } }
                    .Concat(AppConnect.model1.category.ToList())
                );
                cbCategories.ItemsSource = _categories;
                cbCategories.DisplayMemberPath = "Name";
                cbCategories.SelectedIndex = 0;

                // Бренды (с "Все бренды")
                _brands = new ObservableCollection<brand>(
                    new[] { new brand { brandID = 0, name = "Все бренды" } }
                        .Concat(AppConnect.model1.brand.ToList())
                );
                cbBrands.ItemsSource = _brands;
                cbBrands.DisplayMemberPath = "Name";
                cbBrands.SelectedIndex = 0; // Выбираем "Все бренды" по умолчанию

                // Заполнение сортировки
                cbSort.ItemsSource = new List<string>
                {
                    "По умолчанию",
                    "Сначала дешевле",
                    "Сначала дороже"
                };
                cbSort.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_allProducts == null) return;

            var filtered = _allProducts.AsEnumerable();

            // Фильтрация по категории
            if (cbCategories.SelectedItem is category selectedCategory && selectedCategory.categoryID != 0)
            {
                filtered = filtered.Where(p => p.categoryID == selectedCategory.categoryID);
            }

            // Фильтрация по бренду
            if (cbBrands.SelectedItem is brand selectedBrand && selectedBrand.brandID != 0)
            {
                filtered = filtered.Where(p => p.brandID == selectedBrand.brandID);
            }

            // Фильтрация по цене
            if (decimal.TryParse(tbMinPrice.Text, out decimal minPrice))
            {
                filtered = filtered.Where(p => p.price >= minPrice);
            }

            if (decimal.TryParse(tbMaxPrice.Text, out decimal maxPrice))
            {
                filtered = filtered.Where(p => p.price <= maxPrice);
            }

            // Поиск по названию (например, по полю Image или другой строке)
            if (!string.IsNullOrWhiteSpace(tbSearch.Text))
            {
                filtered = filtered.Where(p => p.image != null && p.image.IndexOf(tbSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Сортировка
            switch (cbSort.SelectedItem?.ToString())
            {
                case "Сначала дешевле":
                    filtered = filtered.OrderBy(p => p.price);
                    break;
                case "Сначала дороже":
                    filtered = filtered.OrderByDescending(p => p.price);
                    break;
            }

            lvProducts.ItemsSource = filtered.ToList();
        }

        private void CbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CbBrands_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void PriceFilter_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.users = null;
            NavigationService.Navigate(new Authorization());
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productId)
            {
                NavigationService.Navigate(new ProductDetailsPage(productId));
            }
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productId)
            {
                try
                {
                    var product = _allProducts.FirstOrDefault(p => p.productID == productId);
                    if (product != null)
                    {
                        MessageBox.Show($"{product.image} добавлен в корзину", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Добавьте свою логику корзины здесь
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnUserOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserOrders());
        }
    }
}
