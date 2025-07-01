using Microsoft.Win32;
using retail.AppData;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using Path = System.Windows.Shapes.Path;

namespace retail.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Page
    {
        private product Product;
        public event Action ProductUpdated;

        public EditProduct(product product)
        {
            InitializeComponent();
            this.Product = product ?? throw new ArgumentNullException(nameof(product));

            cbWearType.Text = product.wear.name;
            cbDepartment.Text = product.department.name;
            cbCategory.Text = product.category.name;
            cbBrand.Text = product.brand.name;
            cbCountry.Text = product.country.name;
            cbMaterial.Text = product.material.name;
            cbFastener.Text = product.fastener.name;
            cbColour.Text = product.colour.name;
            cbSize.Text = product.size.name;
            tbPrice.Text = product.price.ToString();

            LoadWears();
            LoadDeparts();
            LoadCategorys();
            LoadBrands();
            LoadCountrys();
            LoadMaterials();
            LoadFasteners();
            LoadColours();
            LoadSizes();

            cbWearType.SelectedItem = product.wear;
            cbDepartment.SelectedItem = product.department;
            cbCategory.SelectedItem = product.category;
            cbBrand.SelectedItem = product.brand;
            cbCountry.SelectedItem = product.country;
            cbMaterial.SelectedItem = product.material;
            cbFastener.SelectedItem = product.fastener;
            cbColour.SelectedItem = product.colour;
            cbSize.SelectedItem = product.size;
        }

        public EditProduct()
        {
            InitializeComponent();
            Product = new product();

            LoadWears();
            LoadDeparts();
            LoadCategorys();
            LoadBrands();
            LoadCountrys();
            LoadMaterials();
            LoadFasteners();
            LoadColours();
            LoadSizes();
        }

        private void LoadWears()
        {
            var wears = AppConnect.model2.wear.ToList();
            cbWearType.ItemsSource = wears;
            cbWearType.DisplayMemberPath = "name";
        }
        private void LoadDeparts()
        {
            var departs = AppConnect.model2.department.ToList();
            cbDepartment.ItemsSource = departs;
            cbDepartment.DisplayMemberPath = "name";
        }

        private void LoadCategorys()
        {
            var categories = AppConnect.model2.category.ToList();
            cbCategory.ItemsSource = categories;
            cbCategory.DisplayMemberPath = "name";
        }
        private void LoadBrands()
        {
            var brands = AppConnect.model2.brand.ToList();
            cbBrand.ItemsSource = brands;
            cbBrand.DisplayMemberPath = "name";
        }
        private void LoadCountrys()
        {
            var countrys = AppConnect.model2.country.ToList();
            cbCountry.ItemsSource = countrys;
            cbCountry.DisplayMemberPath = "name";
        }

        private void LoadMaterials()
        {
            var materials = AppConnect.model2.material.ToList();
            cbMaterial.ItemsSource = materials;
            cbMaterial.DisplayMemberPath = "name";
        }

        private void LoadFasteners()
        {
            var fasteners = AppConnect.model2.fastener.ToList();
            cbFastener.ItemsSource = fasteners;
            cbFastener.DisplayMemberPath = "name";
        }

        private void LoadColours()
        {
            var colours = AppConnect.model2.colour.ToList();
            cbColour.ItemsSource = colours;
            cbColour.DisplayMemberPath = "name";
        }
        private void LoadSizes()
        {
            var sizes = AppConnect.model2.size.ToList();
            cbSize.ItemsSource = sizes;
            cbSize.DisplayMemberPath = "name";
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            product product1 = new product();
            //product.name = EditProductName.Text;
            //product.description = EditDescription.Text;
            //product1.wears = (wear)cbWearType.SelectedItem;
            //product.departments = (department)cbDepartment.SelectedItem;
            //product.category = (category)cbCategory.SelectedItem;
            //product.brand = (brand)cbBrand.SelectedItem;
            //product.country = (country)cbCountry.SelectedItem;
            //product.material = (material)cbMaterial.SelectedItem;
            //product.fasteners = (fastener)cbFastener.SelectedItem;
            //product.colour = (colour)cbColour.SelectedItem;
            //product.size = (size)cbSize.SelectedItem;

            if (product1.productID == 0)
            {
                product1.wearID = AppData.AppConnect.model2.wear.FirstOrDefault(wear => wear.name == cbWearType.Text).wearID;
                product1.departID = AppData.AppConnect.model2.department.FirstOrDefault(department => department.name == cbDepartment.Text).departID;
                product1.categoryID = AppData.AppConnect.model2.category.FirstOrDefault(category => category.name == cbCategory.Text).categoryID;
                product1.brandID = AppData.AppConnect.model2.brand.FirstOrDefault(brand => brand.name == cbBrand.Text).brandID;
                product1.countryID = AppData.AppConnect.model2.country.FirstOrDefault(country => country.name == cbCountry.Text).countryID;
                product1.materialID = AppData.AppConnect.model2.material.FirstOrDefault(material => material.name == cbMaterial.Text).materialID;
                product1.fastenerID = AppData.AppConnect.model2.fastener.FirstOrDefault(fastener => fastener.name == cbFastener.Text).fastenerID;
                product1.colourID = AppData.AppConnect.model2.colour.FirstOrDefault(colour => colour.name == cbColour.Text).colourID;
                product1.sizeID = AppData.AppConnect.model2.size.FirstOrDefault(size => size.name == cbSize.Text).sizeID;
            }

            if (!decimal.TryParse(tbPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную цену.");
                return;
            }
            product1.price = (int)price;

            if (product1.productID == 0)
            {
                AppConnect.model2.product.Add(product1);
            }
            else
            {
                AppConnect.model2.Entry(product1).State = EntityState.Modified;
            }

            AppConnect.model2.SaveChanges();
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminDataOutput());
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            AppConnect.model2.SaveChanges();
        }
    }
}
