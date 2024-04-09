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

namespace Akhmerova41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private User _user = null;
        public ProductPage(User user)
        {
            InitializeComponent();
            OrderButton.Visibility = Visibility.Hidden;
            var currentProducts = Akhmerova41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProducts;
            DiscountComboBox.SelectedIndex = 0;
            UpdateProductPage();
            _user = user;
            int ProductMaxCount = 0;
            foreach (Product product in currentProducts)
            {
                ProductMaxCount++;
            }
            ProductMaxCountTextBlock.Text = ProductMaxCount.ToString();
            if (user == null)
            {
                NameTextBlock.Text = "Гость";
                RoleTextBlock.Text = "Гость";
            }
               
            else
            {
                NameTextBlock.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;

                switch (user.UserRole)
                {
                    case 1: RoleTextBlock.Text = "Администратор"; break;
                    case 2: RoleTextBlock.Text = "Клиент"; break;
                    case 3: RoleTextBlock.Text = "Менеджер"; break;
                }
            }
            

        }
        

        private void UpdateProductPage()
        {
            var currentProducts = Akhmerova41Entities.GetContext().Product.ToList();

            //поиск
            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();

            //сортировка
            if (RadioButtonUp.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderBy(p => p.ProductCost).ToList();
            }
            if (RadioButtonDown.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            }

            //фильтрация
            if (DiscountComboBox.SelectedIndex == 2)
            {
                currentProducts = currentProducts.ToList();
            }
            if (DiscountComboBox.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => (p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount <=9.99)).ToList();
            }
            if (DiscountComboBox.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => (p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount <= 14.99)).ToList();
            }
            if (DiscountComboBox.SelectedIndex == 3)
            {
                currentProducts = currentProducts.Where(p => (p.ProductDiscountAmount >= 15)).ToList();
            }

            ProductListView.ItemsSource = currentProducts;

            int ProductCount = 0;
            foreach (Product product in currentProducts)
            {
                ProductCount++;
            }
            ProductCountTextBlock.Text = ProductCount.ToString();
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProductPage();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProductPage();
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            UpdateProductPage();
        }

        private void DiscountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProductPage();
        }

        private void RadioButtonDown_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButtonCancel_Checked(object sender, RoutedEventArgs e)
        {

        }

        List<OrderProduct> _orderProducts = new List<OrderProduct>();
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach(Product item in ProductListView.SelectedItems)
            {
                if(_orderProducts.Any(p=>p.ProductArticleNumber == item.ProductArticleNumber))
                {
                    _orderProducts.Find(p => p.ProductArticleNumber == item.ProductArticleNumber).Amount++;
                }
                else
                {
                    _orderProducts.Add(new OrderProduct { ProductArticleNumber = item.ProductArticleNumber, Amount = 1, Product = item});
                }
            }
            
            OrderButton.Visibility = Visibility.Visible;
            ProductListView.SelectedIndex = -1;
            
        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            
            OrderWindow orderWindow = new OrderWindow(_orderProducts, _user);
            orderWindow.ShowDialog();
        }
    }
}
