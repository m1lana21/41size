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
        private List<OrderProduct> _orderList = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        User _user = null;
        private int orderNumber = -1;
        public OrderWindow(List<OrderProduct> orderList, User user)
        {
            InitializeComponent();
            _orderList = orderList;
            _user = user;
            //if (user == null) ClientName.Text = "Гость";
            //else ClientName.Text = _user.UserSurname + " " + _user.UserName + " " + _user.UserPatronymic;


            if (_user == null)
            {
                ClientName.Text = "Гость";
            }
            else ClientName.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;


            OrderListView.ItemsSource = _orderList;
            
            orderNumber = Akhmerova41Entities.GetContext().Order.ToList().Select(p => p.OrderID).Max() + 1;
            OrderIDTextBox.Text = orderNumber.ToString();
            PickupCombo.ItemsSource = Akhmerova41Entities.GetContext().OrderPickupPoint.ToList();
            OrderDate.SelectedDate = DateTime.Now;
            UpdateDeliveryDate();


        }

        private void UpdateDeliveryDate()
        {
            if(_orderList.Any(p=>p.Product.ProductQuantityInStock<3) ||
                _orderList.Any(p => p.Amount > p.Product.ProductQuantityInStock))
            {
                OrderDeliveryDateTB.SelectedDate = OrderDate.SelectedDate.Value + TimeSpan.FromDays(6);
            }
            else
            {
                OrderDeliveryDateTB.SelectedDate = OrderDate.SelectedDate.Value + TimeSpan.FromDays(3);
            }
            double total = 0;
            foreach(var item in _orderList)
            {
                total+= item.Price * item.Amount;
            }
            PriceButton.Text = total.ToString();
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            _orderList.Find(p => p.ProductArticleNumber == ((sender as Button).DataContext as OrderProduct).ProductArticleNumber).Amount++;
            OrderListView.Items.Refresh();
            UpdateDeliveryDate();
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as OrderProduct;
            if (--_orderList.Find(p => p.ProductArticleNumber == prod.ProductArticleNumber).Amount <= 0)
            {
                _orderList.RemoveAll(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            }
            else prod.Amount--;
            OrderListView.Items.Refresh();
            UpdateDeliveryDate();

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(OrderDeliveryDateTB.SelectedDate==null || OrderDate.SelectedDate == null)
            {
                MessageBox.Show("Неверная дата доставки или заказа");
                return;
            }
            var order = new Order
            {
                User = _user,
                OrderReceiveCode = new Random().Next(100, 999).ToString(),
                OrderID = orderNumber,
                OrderPickupPoint = (PickupCombo.SelectedItem as OrderPickupPoint).OrderPickupPointID,
                OrderStatus = "Новый",
                OrderProduct = _orderList,
                OrderDate = OrderDate.SelectedDate.Value,
            };
            _orderList.ForEach(p => p.OrderID = orderNumber);
            Akhmerova41Entities.GetContext().OrderProduct.Concat(_orderList);
            Akhmerova41Entities.GetContext().Order.Add(order);
            try
            {
                Akhmerova41Entities.GetContext().SaveChanges();
                MessageBox.Show(String.Format("Заказ сохранен, код получения - {0}",order.OrderReceiveCode));

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
        }
    }
}
