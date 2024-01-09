using ScottPlot;
using ScottPlot.Plottable;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Desummer.Scripts
{
    public partial class PlotControl
    {
        WpfPlot wpfPlot;
        SignalPlot athermalFurnace, bthermalFurnace, cthermalFurnace;
        TextBlock dateText;

        Timer _updateDataTimer;
        DispatcherTimer _renderTimer;

        int[] aTempArr, bTempArr, cTempArr;
        double[] aTempCopy, bTempCopy, cTempCopy;

        DateTime dataFirstDay;

        int currentIndex = 0;

        public bool pauseGraph { get; private set; } = false;

        Crosshair crosshair;

        static object plotLockOjbect = new object();

        readonly int totalMax = 760, totalMin = 660;
        readonly int copyDataAmount = 50;
        readonly int liveUpdateMilliseconds = 2000;

        public PlotControl(WpfPlot wpfPlot, List<TemperatureData> datas, TextBlock textBlock)
        {
            this.wpfPlot = wpfPlot;
            dateText = textBlock;

            // 데이터 초기화
            InitTemperatureArray(datas);
            InitPlot();

            // Y축 최대최소 설정
            SetYAxisMax();

            dataFirstDay = new DateTime(2021, 1, 8);
            // 0~49번 데이터를 가져온 채로 그래프가 시작하기 때문에 그만큼을 시간에서 더해준다
            dataFirstDay = dataFirstDay.AddMinutes(5 * copyDataAmount);
            dateText.Text = dataFirstDay.ToString("MM-dd\nHH:mm");

            crosshair = this.wpfPlot.Plot.AddCrosshair(0, 0);
            crosshair.IsVisible = false;

            Start();
        }

        #region 데이터 초기화

        /// <summary>
        /// 온도 데이터 초기화
        /// </summary>
        /// <param name="datas"></param>
        void InitTemperatureArray(List<TemperatureData> datas)
        {
            // A로 온도 데이터, B로 온도 데이터, C로 온도 데이터
            List<int> aTempDatas = new List<int>();
            List<int> bTempDatas = new List<int>();
            List<int> cTempDatas = new List<int>();

            // 온도 데이터 값을 채워준다
            foreach (TemperatureData data in datas)
            {
                aTempDatas.Add(data.A_temp);
                bTempDatas.Add(data.B_temp);
                cTempDatas.Add(data.C_temp);
            }

            // 배열로 변환
            aTempArr = aTempDatas.ToArray();
            bTempArr = bTempDatas.ToArray();
            cTempArr = cTempDatas.ToArray();

            // 50개씩 잘라서 넣어줄 데이터
            aTempCopy = new double[copyDataAmount];
            bTempCopy = new double[copyDataAmount];
            cTempCopy = new double[copyDataAmount];

            Array.Copy(aTempArr, aTempCopy, copyDataAmount);
            Array.Copy(bTempArr, bTempCopy, copyDataAmount);
            Array.Copy(cTempArr, cTempCopy, copyDataAmount);
        }


        /// <summary>
        /// 그래프 초기화
        /// </summary>
        void InitPlot()
        {
            athermalFurnace = wpfPlot.Plot.AddSignal(aTempCopy, label: "A로 온도");
            bthermalFurnace = wpfPlot.Plot.AddSignal(bTempCopy, label: "B로 온도");
            cthermalFurnace = wpfPlot.Plot.AddSignal(cTempCopy, label: "C로 온도");

            double[] xPositions = { 3, 17, 33, 47 };
            string[] xLabels = { "1시간전", "40분전", "20분전", "현재" };
            
            wpfPlot.Plot.XAxis.ManualTickPositions(xPositions, xLabels);

            wpfPlot.Plot.Style(style: Style.Gray2);
            wpfPlot.Plot.Style(figureBackground: System.Drawing.Color.Transparent);

            wpfPlot.Plot.Title("보온로 온도 그래프");
            //wpfPlot.Plot.XLabel("시간");
            wpfPlot.Plot.YLabel("보온로 온도");

            var legend = wpfPlot.Plot.Legend();
            legend.Location = Alignment.LowerLeft;      // 범례 좌측 하단으로 옮김
            wpfPlot.Refresh();
        }

        #endregion

        /// <summary>
        /// Y축 최대, 최소값 설정
        /// </summary>
        void SetYAxisMax()
        {
            wpfPlot.Plot.SetAxisLimits(yMin: totalMin, yMax: totalMax);
            wpfPlot.Plot.XAxis.SetBoundary(0, 50);
            wpfPlot.Plot.YAxis.SetBoundary(totalMin, totalMax);
        }

        /// <summary>
        /// 실시간 그래프 시작
        /// </summary>
        public void Start() 
        { 
            // period가 500이므로 500밀리세컨드 마다 UpdateData()를 호출
            //_updateDataTimer = new Timer(_ => UpdateData(), null, 0, liveUpdateMilliseconds);

            _renderTimer = new DispatcherTimer();
            _renderTimer.Interval = TimeSpan.FromMilliseconds(liveUpdateMilliseconds);
            _renderTimer.Tick += Render;
            _renderTimer.Start();
        }

        void UpdateData() 
        {
            // 일시정지 중에는 그래프를 갱신해주지 않음
            if (pauseGraph)
                return;

            // 데이터의 최대값이 된다면 더이상 Update하지 않는다
            if (currentIndex + 1 >= aTempArr.Length)
            {
                _updateDataTimer.Dispose();
                _renderTimer.Stop();
                return;
            }

            // 그래프 갱신하기
            Array.Copy(aTempArr, currentIndex, aTempCopy, 0, aTempCopy.Length - 1);
            Array.Copy(bTempArr, currentIndex, bTempCopy, 0, bTempCopy.Length - 1);
            Array.Copy(cTempArr, currentIndex, cTempCopy, 0, cTempCopy.Length - 1);

            aTempCopy[aTempCopy.Length - 1] = aTempArr[currentIndex + copyDataAmount];
            bTempCopy[bTempCopy.Length - 1] = bTempArr[currentIndex + copyDataAmount];
            cTempCopy[cTempCopy.Length - 1] = cTempArr[currentIndex + copyDataAmount];

            dataFirstDay = dataFirstDay.AddMinutes(5);
            currentIndex++;
        }

        void Render(object sender, EventArgs e)
        {
            if (pauseGraph)
                return;

            UpdateData();

            lock (plotLockOjbect)
            {
                dateText.Text = dataFirstDay.ToString("MM-dd\nHH:mm");
                wpfPlot.Refresh();
            }
        }

        /// <summary>
        /// 그래프 멈추기
        /// </summary>
        public void PauseGraph()
        {
            pauseGraph = !pauseGraph;
        }

        /// <summary>
        /// 보온로 visible 설정
        /// </summary>
        /// <param name="thermalFurnaceType">보온로 타입. 1은 A보온로, 2는 B보온로, 3은 C보온로</param>
        /// <param name="visible"></param>
        public void ThermalFurnaceVisible(int thermalFurnaceType, bool visible)
        {
            if (wpfPlot is null) return;

            SignalPlot signalPlot;

            switch (thermalFurnaceType)
            {
                case 1:
                    signalPlot = athermalFurnace;
                    break;
                case 2:
                    signalPlot = bthermalFurnace;
                    break;
                case 3:
                    signalPlot = cthermalFurnace;
                    break;
                default:
                    signalPlot = athermalFurnace;
                    break;
            }

            signalPlot.IsVisible = visible;
            wpfPlot.Refresh();
        }

        /// <summary>
        /// crosshair 보여줄지 체크
        /// </summary>
        /// <param name="visible"></param>
        public void CrosshairVisible(bool visible)
        {
            if (wpfPlot is null) return;
            crosshair.IsVisible = visible;

            if(!visible)
                wpfPlot.Refresh();
        }

        /// <summary>
        /// crosshair 데이터 x,y축 값을 마우스 움직임에 따라 계속 갱신해주는 함수. 실제 갱신은 MainWindow에서 하고 있음
        /// </summary>
        /// <param name="e"></param>
        public void ShowCrosshairData()
        {
            (double coordinateX, double coordinateY) = wpfPlot.GetMouseCoordinates();

            crosshair.X = coordinateX; crosshair.Y = coordinateY;
            crosshair.VerticalLine.PositionFormatter = pos => DateTime.FromOADate(pos).ToString("MM-dd HH:mm");

            wpfPlot.Refresh();
        }
    }
}
