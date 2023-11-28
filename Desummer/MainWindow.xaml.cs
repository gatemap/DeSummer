using Desummer.Scripts;
using ScottPlot.Plottable;
using ScottPlot;
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

        private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (plotControl is null || !plotControl.pauseGraph) return;

            plotControl.CrosshairVisible(true);
        }

        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (plotControl is null || !plotControl.pauseGraph) return;

            plotControl.CrosshairVisible(false);
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (plotControl is null || !plotControl.pauseGraph) return;

            plotControl.ShowCrosshairData(e);
        }
    }
}