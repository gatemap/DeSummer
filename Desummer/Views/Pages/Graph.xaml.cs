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
        private bool isAVisible = true;
        private bool isBVisible = true;
        private bool isCVisible = true;

        public Graph()
        {
            InitializeComponent();

            temperatrueProcessData = new ProcessData();

            plotControl = new PlotControl(temperaturePlot, temperatrueProcessData.TemperatureTotalData(), currentDate);
            donutControl = new PlotControl(temperatureDonut1, temperatureDonut2, temperatureDonut3, temperatrueProcessData.TemperatureTotalData(), Donut1Value, Donut2Value, Donut3Value, this);
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

        private void ToggleAthermalFurnace(object sender, RoutedEventArgs e)
        {
            if (plotControl is null) return;

            isAVisible = !isAVisible;
            plotControl.ThermalFurnaceVisible(1, isAVisible);
        }

        private void ToggleBthermalFurnace(object sender, RoutedEventArgs e)
        {
            if (plotControl is null) return;

            isBVisible = !isBVisible;
            plotControl.ThermalFurnaceVisible(2, isBVisible);
        }

        private void ToggleCthermalFurnace(object sender, RoutedEventArgs e)
        {
            if (plotControl is null) return;

            isCVisible = !isCVisible;
            plotControl.ThermalFurnaceVisible(3, isCVisible);
        }
    }
}
