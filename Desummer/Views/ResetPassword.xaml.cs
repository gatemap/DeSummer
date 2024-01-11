using System.Windows;

namespace Desummer.Views
{
    /// <summary>
    /// Interaction logic for ResetPassword.xaml
    /// </summary>
    public partial class ResetPassword : Window
    {
        public ResetPassword()
        {
            InitializeComponent();

            resetPasswordPage.Source = new Uri("Pages/SendSerialCode.xaml", UriKind.Relative);
        }
    }
}
