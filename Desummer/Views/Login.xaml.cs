using Desummer.Scripts;
using System.Windows;

namespace Desummer.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        // 로그인
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(userId.Text) || string.IsNullOrEmpty(userPassword.Text))
            {
                if (string.IsNullOrEmpty(userId.Text))
                    MessageBox.Show("아이디를 입력해주세요", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                else if(string.IsNullOrEmpty(userPassword.Text))
                    MessageBox.Show("패스워드를 입력해주세요", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }


            SqlControl sql = new SqlControl();
            
            if(sql.Login(userId.Text, userPassword.Password))
            {
                Container nextWindow = new Container();
                nextWindow.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("아이디 혹은 패스워드가 잘못되었습니다.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
