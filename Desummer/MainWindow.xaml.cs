using Desummer.Scripts;
using System.Windows;

namespace Desummer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlotControl plotControl;
        ProcessData temperatrueProcessData;

        public MainWindow()
        {
            InitializeComponent();

            temperatrueProcessData = new ProcessData();
            donutControl = new DonutControl(temperatureDonut1, temperatureDonut2, temperatureDonut3, temperatrueProcessData.TemperatureTotalData());
            plotControl = new PlotControl(temperaturePlot, temperatrueProcessData.TemperatureTotalData());
        }

        private void MouseLeftDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            plotControl.PauseGraph();
        }

        private void ShowAthermalFurnace(object sender, RoutedEventArgs e)
        {
            if (plotControl is null) return;

            plotControl.ThermalFurnaceVisible(1, true);
        }

        private void HideAthermalFurnace(object sender, RoutedEventArgs e)
        {
            plotControl.ThermalFurnaceVisible(1, false);
        }

        private void ShowBthermalFurnace(object sender, RoutedEventArgs e)
        {
            if (plotControl is null) return;

            plotControl.ThermalFurnaceVisible(2, true);
        }

        private void HideBthermalFurnace(object sender, RoutedEventArgs e)
        {
            plotControl.ThermalFurnaceVisible(2, false);
        }

        private void ShowCthermalFurnace(object sender, RoutedEventArgs e)
        {
            if (plotControl is null) return;

            plotControl.ThermalFurnaceVisible(3, true);
        }

        private void HideCthermalFurnace(object sender, RoutedEventArgs e)
        {
            plotControl.ThermalFurnaceVisible(3, false);
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