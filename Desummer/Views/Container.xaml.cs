using Desummer.Scripts;
using Desummer.Views.Pages;
using System.Windows;
using System.Windows.Controls;

using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Controls.Interfaces;

namespace Desummer.Views
{
    /// <summary>
    /// Interaction logic for Container.xaml
    /// </summary>
    public partial class Container : INavigationWindow
    {
        public static Container main;
        public PLCControl plcControl;
        
        public Container()
        {
            InitializeComponent();

            if(main == null)
                main = this;

            PLC plc = new PLC();
            plcControl = new PLCControl(plc.failureIndicationText, plc.reconnectButton);
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
