using Microsoft.Win32;
using retail.AppData;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace retail.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Page
    {
        private Product product;
        public event Action ProductUpdated;

        public EditProduct(Product product)
        {
            InitializeComponent();
            this.product = product ?? throw new ArgumentNullException(nameof(product));

            EditProductName.Text = product.name;
            EditDescription.Text = product.description;
            EditPrice.Text = product.price.ToString();

            LoadBrands();
            LoadCategories();
            LoadMaterials();

            EditBrand.SelectedItem = product.Brand;
            EditCategory.SelectedItem = product.Category;
            EditMaterial.SelectedItem = product.Material;
        }

        public EditProduct()
        {
            InitializeComponent();
            product = new product();

            LoadBrands();
            LoadCategories();
            LoadMaterials();
        }

        private void LoadBrands()
        {
            var brands = AppConnect.model1.brand.ToList();
            EditBrand.ItemsSource = brands;
            EditBrand.DisplayMemberPath = "name";
        }

        private void LoadCategories()
        {
            var categories = AppConnect.model1.category.ToList();
            EditCategory.ItemsSource = categories;
            EditCategory.DisplayMemberPath = "name";
        }

        private void LoadMaterials()
        {
            var materials = AppConnect.model1.material.ToList();
            EditMaterial.ItemsSource = materials;
            EditMaterial.DisplayMemberPath = "name";
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            product.name = EditProductName.Text;
            product.description = EditDescription.Text;
            product.Brand = (Brand)EditBrand.SelectedItem;
            product.Category = (Category)EditCategory.SelectedItem;
            product.Material = (Material)EditMaterial.SelectedItem;

            if (!decimal.TryParse(EditPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную цену.");
                return;
            }
            product.price = price;

            if (product.productID == 0)
            {
                AppConnect.model1.product.Add(product);
            }
            else
            {
                AppConnect.model1.Entry(product).State = EntityState.Modified;
            }

            AppConnect.model1.SaveChanges();
            ProductUpdated?.Invoke();
            NavigationService.GoBack();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
            {
                NavigationService.Navigate(new ProductDetailsPage());
            }
            else
            {
                MessageBox.Show("Не удалось выполнить навигацию. Попробуйте еще раз.");
            }
        }

        private void LoadImageButton(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                dialog.InitialDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Images"));

                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                dialog.Title = "Выберите изображение";

                if (dialog.ShowDialog() == true)
                {
                    string photoName = System.IO.Path.GetFileName(dialog.FileName);
                    product.image = photoName;
                    MessageBox.Show("Изображение загружено: " + photoName, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Изображение не выбрано.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
