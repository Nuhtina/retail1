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
using static retail.Pages.PageLike;

namespace retail.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminUsers.xaml
    /// </summary>
    public partial class AdminUsers : Page
    {
        public System.Collections.Generic.List<role> Roles { get; set; }

        public AdminUsers()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            // Загрузка пользователей
            var users = AppConnect.model1.users.ToList();
            lvUsers.ItemsSource = users;

            // Загрузка ролей
            Roles = AppConnect.model1.role.ToList();
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
            {
                var comboBox = (ComboBox)sender;
                int userId = (int)comboBox.Tag;
                int newRoleId = ((role)e.AddedItems[0]).roleID;

                var user = AppConnect.model1.users.FirstOrDefault(u => u.userID == userId);
                if (user != null)
                {
                    user.roleID = newRoleId;
                    try
                    {
                        AppConnect.model1.SaveChanges();
                        MessageBox.Show("Роль пользователя успешно изменена", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Ошибка при изменении роли: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lvUsers.SelectedItem is users selectedUser)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {selectedUser.surname} {selectedUser.name}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем связанные записи
                        var cart = AppConnect.model1.cart.FirstOrDefault(c => c.UserId == selectedUser.userID);
                        if (cart != null) AppConnect.model1.cart.Remove(cart);

                        var favorites = AppConnect.model1.favorites.Where(f => f.UserId == selectedUser.userID).ToList();
                        AppConnect.model1.favorites.RemoveRange(favorites);

                        // Удаляем пользователя
                        AppConnect.model1.users.Remove(selectedUser);
                        AppConnect.model1.SaveChanges();

                        // Обновляем список
                        LoadData();

                        MessageBox.Show("Пользователь успешно удален", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnAttractions_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminDataOutput());
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminOrders());
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            // Уже на странице пользователей
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.User = null;
            NavigationService?.Navigate(new Authorization());
        }

        private void BtnSessions_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminSchedule());
        }
    }
}
