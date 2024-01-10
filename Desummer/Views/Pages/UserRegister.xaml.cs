using Desummer.Scripts;

using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Desummer.Views.Pages
{
    /// <summary>
    /// Interaction logic for UserRegister.xaml
    /// </summary>
    public partial class UserRegister : Page
    {
        SqlControl sqlControl;

        public UserRegister()
        {
            InitializeComponent();

            sqlControl = new SqlControl();

            ButtonStateChange("중복체크", true);
        }

        /// <summary>
        /// ID에 영어(소문자), 숫자만 들어올 수 있도록 함
        /// </summary>
        private void Id_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "^[a-z0-9]*$"))
                e.Handled = true;
        }

        private void Duplication_Check(object sender, RoutedEventArgs e)
        {
            if (!sqlControl.CheckDuplicateId(id.Text))
                ButtonStateChange("체크완료", false);
            else
                MessageBox.Show("중복된 아이디입니다", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 비밀번호 확인과 일치하는지 확인하고, 불일치하면 날려버린다
            if(!MatchPassword())
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                pwCheck.Clear();
                return;
            }

            bool registSuccess = true;
            sqlControl.RegistUser(name.Text, id.Text, pw.Password, out registSuccess);

            if(registSuccess)
            {
                name.Clear();
                id.Clear();
                pw.Clear();
                pwCheck.Clear();

                ButtonStateChange("중복체크", true);
            }
        }

        /// <summary>
        /// 패스워드 일치하는지 체크
        /// </summary>
        /// <returns>true=일치, false=불일치</returns>
        bool MatchPassword()
        {
            return pw.Password == pwCheck.Password;
        }

        /// <summary>
        /// 버튼 상태 변경
        /// </summary>
        /// <param name="content">중복체크 버튼 텍스트 변경</param>
        /// <param name="enabled">버튼 활성/비활성화</param>
        void ButtonStateChange(string content, bool enabled)
        {
            duplicateCheckButton.IsEnabled = enabled;
            duplicateCheckButton.Content = content;
            registButton.IsEnabled = !enabled;
        }
    }
}
