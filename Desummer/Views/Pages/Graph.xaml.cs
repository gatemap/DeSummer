using Desummer.Scripts;
using System.Windows;
using System.Windows.Controls;

namespace Desummer.Views.Pages
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : Page
    {
        ProcessData temperatrueProcessData;

        PlotControl plotControl;
        PlotControl donutControl;

        public Graph()
        {
            InitializeComponent();

            temperatrueProcessData = new ProcessData();
            PLC plc = new PLC();

            plotControl = new PlotControl(temperaturePlot, temperatrueProcessData.TemperatureTotalData(), currentDate);
            donutControl = new PlotControl(temperatureDonut1, temperatureDonut2, temperatureDonut3, temperatrueProcessData.TemperatureTotalData(), Donut1Value, Donut2Value, Donut3Value, this);

            Container.main.plotControl = donutControl;
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

        private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (plotControl is null || !plotControl.pauseGraph) return;

            plotControl.CrosshairVisible(true);
        }

        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (plotControl is null) return;

            plotControl.CrosshairVisible(false);
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (plotControl is null || !plotControl.pauseGraph) return;

            plotControl.ShowCrosshairData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            plotControl.PauseGraph();
            donutControl.PauseGraph();

            if (plotControl.pauseGraph)
                pauseButton.Content = "재생";
            else
                pauseButton.Content = "일시정지";
        }
    }
}
