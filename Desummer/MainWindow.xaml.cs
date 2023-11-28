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
            //선택된 month 체크
            ComboBoxItem SelectMonth = (ComboBoxItem)ComboBox_SelectMonth.SelectedValue;
            string SelectedMonth = SelectMonth.Content.ToString();

            //선택된 week 체크
            ComboBoxItem SelectWeek = (ComboBoxItem)ComboBox_SelectWeek.SelectedValue;
            string SelectedWeek = SelectWeek.Content.ToString();

            //ProcessData 클래스의 split 메서드 호출해 해당 week 데이터 가져오기
            ProcessData Month = new ProcessData();
            List<TemperatureData> Select_Month_Week = Month.SplitDataMonthly(SelectedMonth, SelectedWeek);

            //최대, 최소, 평균 출력하기
            DataGridControl ShowOnDataGrid = new DataGridControl();
            ShowOnDataGrid.MinMaxAvg(DataGrid_TempData, Select_Month_Week);
        }
    }
}