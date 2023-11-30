using ScottPlot;
using ScottPlot.Plottable;
using System.Drawing;
using System.Windows.Threading;

namespace Desummer.Scripts
{
    partial class PlotControl
    {
        WpfPlot temperatureDonut1;
        WpfPlot temperatureDonut2;
        WpfPlot temperatureDonut3;
        List<TemperatureData> datas = new List<TemperatureData>();

        private DispatcherTimer timer; // DispatcherTimer를 클래스 변수로 선언
        private int index = 50;

        public PlotControl(WpfPlot temperatureDonut1, WpfPlot temperatureDonut2, WpfPlot temperatureDonut3, List<TemperatureData> datas)
        {
            this.temperatureDonut1 = temperatureDonut1;
            this.temperatureDonut2 = temperatureDonut2;
            this.temperatureDonut3 = temperatureDonut3;
            this.datas = datas;

            DonutLiveStart();
        }

        void DonutLiveStart()
        {
            // DispatcherTimer 초기화
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(liveUpdateMilliseconds); // 500밀리세컨드
            timer.Tick += UpdateDonutCharts; // UpdateCharts 메서드 호출
            timer.Start(); // 타이머 시작
        }

        private void UpdateDonutCharts(object sender, EventArgs e)
        {
            // 일시정지 중이면 아무것도 안함
            if (pauseGraph)
                return;

            if (index < datas.Count) // 아직 List의 끝에 도달하지 않았으면
            {
                SettingDonut(temperatureDonut1.Plot, temperatureDonut1, "A");
                SettingDonut(temperatureDonut2.Plot, temperatureDonut2, "B");
                SettingDonut(temperatureDonut3.Plot, temperatureDonut3, "C");

                index++;
            }
            else // 파일 끝인 경우
            {
                timer.Stop(); // 타이머 정지
            }
        }
        
        /// <summary>
        /// 도넛차트 설정(셋팅)
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="donutControl"></param>
        /// <param name="str"></param>
        void SettingDonut(Plot plot, WpfPlot donutControl, string str)
        {
            Color color1 = Color.FromArgb(255, 85, 156, 228); // (투명도, R, G, B) values color
            Color color2 = Color.FromArgb(255, 50, 50, 50); // (투명도, R, G, B) Max values color

            TemperatureData data = datas[index];

            // 기존 Plot 초기화
            plot.Clear();
            plot.Title($"{str}로", color:Color.White);
            plot.Style(figureBackground: System.Drawing.Color.Transparent);
            plot.Style(dataBackground: System.Drawing.Color.Transparent);

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
            pie.DonutSize = .7;
            pie.CenterFont.Size = 20;
            pie.CenterFont.Color = color1; // font color
            pie.OutlineSize = 0;
            pie.SliceFillColors = new Color[] { color1, color2 };
            
            donutControl.Refresh(); // wpfPlot1 갱신
        }
    }
}
