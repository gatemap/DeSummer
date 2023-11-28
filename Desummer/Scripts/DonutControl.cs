using ScottPlot;
using ScottPlot.Plottable;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using System.Windows.Threading;

namespace Desummer.Scripts
{
    class DonutControl
    {
        WpfPlot temperatureDonut1;
        WpfPlot temperatureDonut2;
        WpfPlot temperatureDonut3;
        List<TemperatureData> datas = new List<TemperatureData>();
        private DispatcherTimer timer; // DispatcherTimer를 클래스 변수로 선언
        private int index = 0;
        private Color color1;
        private Color color2;

        public DonutControl(WpfPlot temperatureDonut1, WpfPlot temperatureDonut2, WpfPlot temperatureDonut3, List<TemperatureData> datas)
        {
            this.temperatureDonut1 = temperatureDonut1;
            this.temperatureDonut2 = temperatureDonut2;
            this.temperatureDonut3 = temperatureDonut3;
            this.datas = datas;
            Start();
        }
        public void Start()
        {
            // DispatcherTimer 초기화
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.075); // 0.075초마다
            timer.Tick += UpdateDonutCharts; // UpdateCharts 메서드 호출
            timer.Start(); // 타이머 시작
            
        }
        private void UpdateDonutCharts(object sender, EventArgs e)
        {
            if (index < datas.Count) // 아직 List의 끝에 도달하지 않았으면
            {
                TemperatureData data = datas[index];

                var plot1 = temperatureDonut1.Plot;
                var plot2 = temperatureDonut2.Plot;
                var plot3 = temperatureDonut3.Plot;

                // wpfPlot1 갱신
                SettingDoNut(temperatureDonut1.Plot, temperatureDonut1, "A");
                SettingDoNut(temperatureDonut2.Plot, temperatureDonut2, "B");
                SettingDoNut(temperatureDonut3.Plot, temperatureDonut3, "C");

                index++;
            }
            else // 파일 끝인 경우
            {
                timer.Stop(); // 타이머 정지
            }
        }
        void SettingDoNut(Plot plot, WpfPlot donutControl, string str)
        {
            Color color1 = Color.FromArgb(255, 0, 255, 0); // (투명도, R, G, B) values color
            Color color2 = Color.FromArgb(255, 240, 240, 240); // (투명도, R, G, B) Max values color

            TemperatureData data = datas[index];

            // 기존 Plot 초기화
            plot.Clear();
            plot.Title($"{str}로");
            PiePlot pie;
            if (str == "A")
            {
                pie = plot.AddPie(new double[] { data.A_temp, 2600 }); // 2600 = Max values
                pie.DonutLabel = (data.A_temp.ToString()) + '℃'; // 현재 온도를 표시할 label
            }
            else if (str == "B")
            {
                pie = plot.AddPie(new double[] { data.B_temp, 2600 }); // 2600 = Max values
                pie.DonutLabel = data.B_temp.ToString() + '℃'; // 현재 온도를 표시할 label}
            }
            else
            {
                pie = plot.AddPie(new double[] { data.C_temp, 2600 }); // 2600 = Max values
                pie.DonutLabel = data.C_temp.ToString() + '℃'; // 현재 온도를 표시할 label}
            }
            pie.DonutSize = .8;
            pie.CenterFont.Size = 12;
            pie.CenterFont.Color = color1; // font color
            pie.OutlineSize = 1;
            pie.SliceFillColors = new Color[] { color1, color2 };
            
            donutControl.Refresh(); // wpfPlot1 갱신
        }
    }
}
