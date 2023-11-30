using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace Desummer.Views
{
    /// <summary>
    /// Interaction logic for Container.xaml
    /// </summary>
    public partial class Container : INavigationWindow
    {
        public Container()
        {
            InitializeComponent();
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
            Navigate(typeof(Pages.Home));
        }
    }
}
