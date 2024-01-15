using Desummer.Scripts;
using System.Windows;
using System.Windows.Controls;

namespace Desummer.Views.Pages
{
    /// <summary>
    /// Interaction logic for SendSerialCode.xaml
    /// </summary>
    public partial class SendSerialCode : Page
    {
        MailControl mail;

        public SendSerialCode()
        {
            InitializeComponent();

            mail = new MailControl();

            sendMailButton.IsEnabled = true;
            insertCodeButton.IsEnabled = false;
        }

        private void Send_SerialCode(object sender, RoutedEventArgs e)
        {
            // 메일 전송에 성공한 경우
            if (mail.SendMailSuccess(userId.Text, userEmail.Text))
            {
                sendMailButton.IsEnabled = false;
                insertCodeButton.IsEnabled = true;
            }
        }

        private void Insert_SerialCode(object sender, RoutedEventArgs e)
        {
            // 입력받은 시리얼코드 동일하면 화면 넘어감
            if (mail.MatchSerialCode(serialCode.Text))
            {
                ResetPasswordPage resetPage = new ResetPasswordPage(userId.Text);
                NavigationService.Navigate(resetPage);
            }
            else
                MessageBox.Show("보안코드가 일치하지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
