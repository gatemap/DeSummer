using System.Windows.Controls;

namespace Desummer.Views.Pages
{
    /// <summary>
    /// Interaction logic for PLC.xaml
    /// </summary>
    public partial class PLC : Page
    {
        static bool initialize = false;

        public PLC()
        {
            if (initialize)
                return;

            InitializeComponent();

            initialize = true;
        }
    }
}
