using System.Windows;

namespace Desummer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_ShowData_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem SelectMonth = (ComboBoxItem)ComboBox_SelectMonth.SelectedValue;
            string SelectedMonth_name = SelectMonth.Content.ToString();

            ComboBoxItem SelectedWeek = (ComboBoxItem)ComboBox_SelectWeek.SelectedValue;
            string SelectedWeek_name = SelectedWeek.Content.ToString();

            ProcessData Month = new ProcessData();
            List<TemperatureData> Select_Month = Month.SplitDataMonthly(SelectedMonth_name, SelectedWeek_name);

            DataGrid_TempData.ItemsSource = Select_Month;
        }
    }
}