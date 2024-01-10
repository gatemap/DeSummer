using Desummer.Scripts;
using System.Windows;

namespace Desummer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App main = null;
        public UserData userData; 

        public App()
        {
            if (main == null)
                main = this;
        }
    }

}
