using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Akhmerova41
{
    public partial class AuthorizationPage : Page
    {
        
        public AuthorizationPage()
        {

            InitializeComponent();
            var currentUser = Akhmerova41Entities.GetContext().User.ToList();
            
        }

        
        
        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            User user = null;
            Manager.MainFrame.Navigate(new ProductPage(user));
            LoginTextBox.Text = "";
            PasswordTextBox.Text = "";
            
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string Login = LoginTextBox.Text;
            string Password = PasswordTextBox.Text;
            if((Login=="") || (Password == ""))
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            User user = Akhmerova41Entities.GetContext().User.ToList().Find(p=>p.UserLogin==Login && p.UserPassword==Password);
            if (user != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                LoginTextBox.Text = "";
                PasswordTextBox.Text = "";
            }
            else
            {
                
                MessageBox.Show("Неверный логин или пароль");
                SignInButton.IsEnabled = false;
                await Task.Delay(TimeSpan.FromSeconds(10));
                SignInButton.IsEnabled = true;
            }

        }
    }
}
