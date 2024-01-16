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
            if (string.IsNullOrEmpty(userId.Text) || string.IsNullOrEmpty(userPassword.Password))
            {
                if (string.IsNullOrEmpty(userId.Text))
                    MessageBox.Show("아이디를 입력해주세요", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                else if(string.IsNullOrEmpty(userPassword.Password))
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

        // 비밀번호 재설정
        private void Reset_Password(object sender, RoutedEventArgs e)
        {
            ResetPassword resetPassword = new ResetPassword();
            resetPassword.Show();
        }

        private void Button_Click_Face(object sender, RoutedEventArgs e)
        {
            FaceRecognitions fp = new FaceRecognitions();
            fp.StartFaceRecognition();

            string fl = FaceRecognitions.face_login;

            if (fl == "Unknown")
            {
                MessageBox.Show("미등록 얼굴입니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else if (fl == "")
            {
                MessageBox.Show("시간 초과 얼굴 인식에 실패했습니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
            {
                Container nextWindow = new Container();
                nextWindow.Show();

                this.Close();
            }
        }
    }
}
