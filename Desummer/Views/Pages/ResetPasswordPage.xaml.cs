using Desummer.Scripts;
using System.Windows;
using System.Windows.Controls;

namespace Desummer.Views.Pages
{
    /// <summary>
    /// Interaction logic for ResetPasswordPage.xaml
    /// </summary>
    public partial class ResetPasswordPage : Page
    {
        string userId = string.Empty;

        public ResetPasswordPage(string id)
        {
            InitializeComponent();

            userId = id;
        }

        private void Change_Password(object sender, RoutedEventArgs e)
        {
            if(!userPw.Password.Equals(userPwCheck.Password))
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                userPwCheck.Clear();
            }
            else
            {
                SqlControl sqlControl = new SqlControl();
                
                // 비밀번호 변경에 성공하면
                if(sqlControl.ChangePassword(userId, userPw.Password))
                {
                    // 현재 창 닫기
                    Window currentWindow = Window.GetWindow(this);
                    currentWindow.Close();
                }
            }
        }
    }
}
