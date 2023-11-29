using Desummer.Scripts;
using System.Windows;
using System.Windows.Controls;

namespace Desummer.Views.Pages
{
    /// <summary>
    /// Interaction logic for DataGrid.xaml
    /// </summary>
    public partial class DataGrid : Page
    {
        ProcessData temperatrueProcessData;

        DataGridControl showOnDataGrid;
        public DataGrid()
        {
            InitializeComponent();

            temperatrueProcessData = new ProcessData();

            //실행했을때 데이터그리드가 비어있지 않게 전체 데이터에 대한 요약 출력
            showOnDataGrid = new DataGridControl();
            showOnDataGrid.MinMaxAvg(DataGrid_TempData, temperatrueProcessData.TemperatureTotalData());
        }

        private void Button_ShowData_Click(object sender, RoutedEventArgs e)
        {
            //선택된 month 체크
            ComboBoxItem SelectMonth = (ComboBoxItem)ComboBox_SelectMonth.SelectedValue;
            string SelectedMonth = (string)SelectMonth.Content;

            //선택된 week 체크
            ComboBoxItem SelectWeek = (ComboBoxItem)ComboBox_SelectWeek.SelectedValue;
            string SelectedWeek = (string)SelectWeek.Content;

            //ProcessData 클래스의 split 메서드 호출해 해당 week 데이터 가져오기
            ProcessData Month = new ProcessData();
            List<TemperatureData> Select_Month_Week = Month.SplitDataMonthly(SelectedMonth, SelectedWeek);

            //최대, 최소, 평균 출력하기
            showOnDataGrid = new DataGridControl();
            showOnDataGrid.MinMaxAvg(DataGrid_TempData, Select_Month_Week);
        }
    }
}
