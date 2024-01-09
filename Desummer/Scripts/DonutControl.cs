using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Drawing.Color;

using ScottPlot;
using ScottPlot.Plottable;

using Desummer.Views.Pages;
using System.Diagnostics;

namespace Desummer.Scripts
{
    public partial class PlotControl
    {
        WpfPlot temperatureDonut1;
        WpfPlot temperatureDonut2;
        WpfPlot temperatureDonut3;
        private TextBlock donut1Value;
        private TextBlock donut2Value;
        private TextBlock donut3Value;
        List<TemperatureData> datas = new List<TemperatureData>();

        private DispatcherTimer timer; // DispatcherTimer를 클래스 변수로 선언
        private int index = 49;

        // A,B,C 보온로 이전, 현재 정상체크
        bool prevATempNormal, currATempNormal = false;
        bool prevBTempNormal, currBTempNormal = false;
        bool prevCTempNormal, currCTempNormal = false;

        static object donutLockOjbect = new object();

        Graph graph;
        PLCControl plcControl;
        MediaPlayer MP3 = new MediaPlayer();
        Uri uri = new Uri("C:\\Users\\o\\Downloads\\DeSummer\\Desummer\\Resources\\alarm.mp3");

        public PlotControl(WpfPlot temperatureDonut1, WpfPlot temperatureDonut2, WpfPlot temperatureDonut3, List<TemperatureData> datas,
            TextBlock donut1Value, TextBlock donut2Value, TextBlock donut3Value, Graph graph)
        {
            this.temperatureDonut1 = temperatureDonut1;
            this.temperatureDonut2 = temperatureDonut2;
            this.temperatureDonut3 = temperatureDonut3;
            this.datas = datas;
            this.donut1Value = donut1Value;
            this.donut2Value = donut2Value;
            this.donut3Value = donut3Value;
            this.graph = graph;

            plcControl = new PLCControl();

            DonutLiveStart();
        }

        void DonutLiveStart()
        {
            SettingDonut(temperatureDonut1.Plot, temperatureDonut1, "A");
            SettingDonut(temperatureDonut2.Plot, temperatureDonut2, "B");
            SettingDonut(temperatureDonut3.Plot, temperatureDonut3, "C");

            index++;

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
                lock (donutLockOjbect)
                {
                    SettingDonut(temperatureDonut1.Plot, temperatureDonut1, "A");
                    SettingDonut(temperatureDonut2.Plot, temperatureDonut2, "B");
                    SettingDonut(temperatureDonut3.Plot, temperatureDonut3, "C");

                    SendCurrentData();

                    index++;
                }
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
            plot.Title($"{str}로", color:Color.White, size:15, bold: true);
            plot.Style(figureBackground: Color.Transparent);
            plot.Style(dataBackground: Color.Transparent);

            RadialGaugePlot gauges;
            if (str == "A")
            {
                gauges = plot.AddRadialGauge(new double[] { data.A_temp, 2600 - data.A_temp }); // 2600 = Max values
                donut1Value.Text = $"{data.A_temp}℃"; // 현재 온도를 표시할 TextBlock
            }
            else if (str == "B")
            {
                gauges = plot.AddRadialGauge(new double[] { data.B_temp, 2600 - data.A_temp }); // 2600 = Max values
                donut2Value.Text = $"{data.B_temp}℃"; // 현재 온도를 표시할 TextBlock
            }
            else
            {
                gauges = plot.AddRadialGauge(new double[] { data.C_temp, 2600 - data.A_temp }); // 2600 = Max values
                donut3Value.Text = $"{data.C_temp}℃"; // 현재 온도를 표시할 TextBlock
            }

            // 정상 온도가 아닐 때, 페이지를 붉은색으로 바꿔주고, 소리를 재생시킨다
            if (data.A_temp < 680 || data.A_temp > 750 || data.B_temp < 680 || data.B_temp > 750 || data.C_temp < 680 || data.C_temp > 750)
            {
                graph.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 180, 0, 0));
                MP3.Open(uri);
                MP3.Play();
            }
            else
            {
                graph.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
            }

            gauges.GaugeMode = RadialGaugeMode.SingleGauge;
            gauges.MaximumAngle = 360;
            gauges.StartingAngle = 180;
            gauges.ShowLevels = false;
            gauges.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            gauges.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            gauges.Colors = new Color[] { Color.FromArgb(255, 85, 156, 228), Color.FromArgb(255, 50, 50, 50) }; // {1번 value 색상, 2번 value 색상} (투명도, R, G, B)
            
            donutControl.Refresh(); // wpfPlot1 갱신
        }

        /// <summary>
        /// 현재 데이터 PLC로 전송하기
        /// </summary>
        void SendCurrentData()
        {
            TemperatureData data = datas[index];

            // 현재 정상동작인지 체크
            currATempNormal = data.A_temp >= 680;
            currBTempNormal = data.B_temp >= 680;
            currCTempNormal = data.C_temp >= 680;

            //currATempNormal = data.A_temp >= 680 && data.A_temp <= 750;
            //currBTempNormal = data.B_temp >= 680 && data.B_temp <= 750;
            //currCTempNormal = data.C_temp >= 680 && data.C_temp <= 750;

            // 이전 값과 비교해서 다르면 통신한다
            if (currATempNormal != prevATempNormal || currBTempNormal != prevBTempNormal || currCTempNormal != prevCTempNormal)
                plcControl.SendData(currATempNormal, currBTempNormal, currCTempNormal);

            prevATempNormal = currATempNormal;
            prevBTempNormal = currBTempNormal;
            prevCTempNormal = currCTempNormal;
        }
    }
}