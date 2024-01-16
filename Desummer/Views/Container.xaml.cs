using Desummer.Views.Pages;
using System.Windows;
using System.Windows.Controls;

using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Controls.Interfaces;
using System.Diagnostics;

namespace Desummer.Views
{
    /// <summary>
    /// Interaction logic for Container.xaml
    /// </summary>
    public partial class Container : Window, INavigationWindow
    {

        public Container()
        {
            InitializeComponent();

            //Debug.WriteLine($"로그인 정보 : \n사용자 이름 : {App.userData.userName}, 사용자 아이디 : {App.userData.userId}, 관리자인가요? {App.userData.admin}");

            // 관리자가 아니면 숨긴다
            if(!App.userData.admin)
                userResister.Visibility = Visibility.Hidden;

        }

        #region INavigation interface Method

        public Frame GetFrame() => RootFrame;

        public INavigation GetNavigation() => navigation;

        public bool Navigate(Type pageType) => navigation.Navigate(pageType);

        public void SetPageService(IPageService pageService) =>
            navigation.PageService = pageService;

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        #endregion

        private void navigation_Loaded(object sender, RoutedEventArgs e)
        {
            Navigate(typeof(Home));
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
