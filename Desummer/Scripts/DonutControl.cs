using ScottPlot;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using System.Windows.Threading;

namespace Desummer.Scripts
{
    // MainWindow에서 아래 주석된 코드를 추가하면 실행
    // DonutControl donutControl = new DonutControl(temperatureDonut1, temperatureDonut2, temperatureDonut3);
    // donutControl.Start();
    class DonutControl
    {
        WpfPlot temperatureDonut1;
        WpfPlot temperatureDonut2;
        WpfPlot temperatureDonut3;

        public DonutControl(WpfPlot temperatureDonut1, WpfPlot temperatureDonut2, WpfPlot temperatureDonut3)
        {
            this.temperatureDonut1 = temperatureDonut1;
            this.temperatureDonut2 = temperatureDonut2;
            this.temperatureDonut3 = temperatureDonut3;
        }

        string filePath = "C:\\Users\\woong\\Downloads\\desummer.csv";
        private StreamReader sr; // StreamReader를 클래스 변수로 선언
        private DispatcherTimer timer; // DispatcherTimer를 클래스 변수로 선언
        public void Start()
        {
            sr = new StreamReader(filePath);
            sr.ReadLine();

            // DispatcherTimer 초기화
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2); // 2초마다
            timer.Tick += UpdateDonutCharts; // UpdateCharts 메서드 호출
            timer.Start(); // 타이머 시작
        }

        private void UpdateDonutCharts(object sender, EventArgs e)
        {
            if (!sr.EndOfStream) // 파일 끝이 아닌 경우
            {
                var line = sr.ReadLine(); // 다음 줄 읽기
                var csvline = line.Split(','); // CSV 파일의 각 열의 값을 분리
                if (csvline.Length >= 4) // 4열 이상의 데이터가 있는 경우
                {
                    // 각 열의 데이터를 double 형태로 변환하여 wpfPlot의 values로 설정
                    int wpfPlot1Value = int.Parse(csvline[1]);
                    int wpfPlot2Value = int.Parse(csvline[2]);
                    int wpfPlot3Value = int.Parse(csvline[3]);

                    // 도넛 색상 설정
                    Color color1 = Color.FromArgb(255, 0, 255, 0); // (투명도, R, G, B) values color
                    Color color2 = Color.FromArgb(255, 240, 240, 240); // (투명도, R, G, B) Max values color

                    // wpfPlot1 갱신
                    var plot1 = temperatureDonut1.Plot;
                    plot1.Clear(); // 기존 Plot 초기화
                    var pie1 = plot1.AddPie(new double[] { wpfPlot1Value, 2600 }); // 2600 = Max values
                    pie1.DonutSize = .8;
                    pie1.CenterFont.Size = 12;
                    pie1.CenterFont.Color = color1; // font color
                    pie1.OutlineSize = 1;
                    pie1.SliceFillColors = new Color[] { color1, color2 };
                    pie1.DonutLabel = wpfPlot1Value.ToString() + '℃'; // 현재 온도를 표시할 label
                    
                    temperatureDonut1.Refresh(); // wpfPlot1 갱신

                    // wpfPlot2 갱신
                    var plot2 = temperatureDonut2.Plot;
                    plot2.Clear(); // 기존 Plot 초기화
                    var pie2 = plot2.AddPie(new double[] { wpfPlot2Value, 2600 }); // 2600 = Max values
                    pie2.DonutSize = .8;
                    pie2.CenterFont.Size = 12;
                    pie2.CenterFont.Color = color1; // font color
                    pie2.OutlineSize = 1;
                    pie2.SliceFillColors = new Color[] { color1, color2 };
                    pie2.DonutLabel = wpfPlot2Value.ToString() + '℃'; // 현재 온도를 표시할 label
                    temperatureDonut2.Refresh(); // wpfPlot2 갱신

                    // wpfPlot3 갱신
                    var plot3 = temperatureDonut3.Plot;
                    plot3.Clear(); // 기존 Plot 초기화
                    var pie3 = plot3.AddPie(new double[] { wpfPlot3Value, 2600 }); // 2600 = Max values
                    pie3.DonutSize = .8;
                    pie3.CenterFont.Size = 12;
                    pie3.CenterFont.Color = color1; // font color
                    pie3.OutlineSize = 1;
                    pie3.SliceFillColors = new Color[] { color1, color2 };
                    pie3.DonutLabel = wpfPlot3Value.ToString() + '℃'; // 현재 온도를 표시할 label
                    temperatureDonut3.Refresh(); // wpfPlot3 갱신
                }
            }
            else // 파일 끝인 경우
            {
                timer.Stop(); // 타이머 정지
                sr.Close(); // 파일 닫기
            }
        }
    }
}
