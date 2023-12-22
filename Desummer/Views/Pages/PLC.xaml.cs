using Desummer.Scripts;
using System.Windows.Controls;

namespace Desummer.Views.Pages
{
    /// <summary>
    /// Interaction logic for PLC.xaml
    /// </summary>
    public partial class PLC : Page
    {
        public PLC()
        {
            InitializeComponent();
            
            Container.main.plcControl = new PLCControl(failureIndicationText, reconnectButton);
        }

        private void ButtonClick_Reconnect(object sender, System.Windows.RoutedEventArgs e)
        {
            Container.main.plotControl.SendCurrentData();
        }
    }
}
