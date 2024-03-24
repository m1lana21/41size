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
using System.Windows.Shapes;

namespace Akhmerova41
{
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct = new OrderProduct(); 
        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO)
        {
            InitializeComponent();
            var currentPickups = Akhmerova41Entities.GetContext().OrderPickupPoint.ToList();
            PickupCombo.ItemsSource = currentPickups;
            ClientName.Text = FIO;
            OrderIDTextBox.Text = selectedOrderProducts.First().OrderID.ToString();
            OrderListView.ItemsSource = selectedProducts;
            foreach (Product p in selectedProducts)
            {
                p.ProductQuantityInStock = 1;
                foreach (OrderProduct q in selectedOrderProducts) 
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.ProductQuantityInStock = q.Amount;
                }
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            OrderDate.Text = DateTime.Now.ToShortDateString();
            SetDeliveryDate();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var newOrderID = Akhmerova41Entities.GetContext().Order.Select(p => p.OrderID).Max()+1;
            
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.ProductQuantityInStock++;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].Amount++;
            SetDeliveryDate();
            
            OrderListView.Items.Refresh();
        
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            if (prod.ProductQuantityInStock > 0)
            {
                prod.ProductQuantityInStock--;
                var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
                int index = selectedOrderProducts.IndexOf(selectedOP);
                selectedOrderProducts[index].Amount--;
                SetDeliveryDate();
                OrderListView.Items.Refresh();
            }
        }

        private void OrderListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SetDeliveryDate()
        {
            bool ifLessThanOnStock = false;
            DateTime deliveryDate = DateTime.Now;
            var currentProductQuantity = Akhmerova41Entities.GetContext().Product.ToList();
            foreach(var item in selectedProducts)
            {
                foreach (var item1 in selectedOrderProducts)
                {
                    if(item1.Amount>item.ProductQuantityInStock || item.ProductQuantityInStock < 3)
                    {
                        ifLessThanOnStock = true;
                    }
                }
            }
        }
    }
}
