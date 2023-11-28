using ScottPlot;
using ScottPlot.Plottable;
using System.Windows.Threading;

namespace Desummer.Scripts
{
    class PlotControl
    {
        WpfPlot wpfPlot;
        SignalPlot athermalFurnace, bthermalFurnace, cthermalFurnace;

        Timer _updateDataTimer;
        DispatcherTimer _renderTimer;

        int[] aTempArr, bTempArr, cTempArr;
        double[] aTempCopy, bTempCopy, cTempCopy;

        DateTime dataFirstDay;

        int currentIndex = 0;
        int totalMax = 0, totalMin = int.MaxValue;
        public bool pauseGraph { get; private set; } = false;
        bool aPlotVisible = true, bPlotVisible = true, cPlotVisible = true;

        Crosshair crosshair;

        readonly int copyDataAmount = 50;
        readonly double dayRate = 5 * 24 * 60;

        public PlotControl(WpfPlot wpfPlot, List<TemperatureData> datas)
        {
            this.wpfPlot = wpfPlot;

            // 데이터 초기화
            InitTemperatureArray(datas);
            InitPlot();

            // Y축 최대최소 설정
            SetYAxisMax();

            crosshair = this.wpfPlot.Plot.AddCrosshair(0, 0);

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
            athermalFurnace = wpfPlot.Plot.AddSignal(aTempCopy, sampleRate: dayRate * 60 * 1000, label: "A로 온도");
            bthermalFurnace = wpfPlot.Plot.AddSignal(bTempCopy, sampleRate: dayRate * 60 * 1000, label: "B로 온도");
            cthermalFurnace = wpfPlot.Plot.AddSignal(cTempCopy, sampleRate: dayRate * 60 * 1000, label: "C로 온도");

            dataFirstDay = new DateTime(2021, 1, 8);

            athermalFurnace.OffsetX = bthermalFurnace.OffsetX = cthermalFurnace.OffsetX = dataFirstDay.ToOADate();
            wpfPlot.Plot.XAxis.TickLabelFormat("MM-dd HH:mm", dateTimeFormat: true);

            wpfPlot.Plot.Title("보온로 온도 그래프");
            wpfPlot.Plot.XLabel("날짜");
            wpfPlot.Plot.YLabel("보온로 온도");
            wpfPlot.Plot.Legend();
            wpfPlot.Refresh();
        }

        #endregion

        /// <summary>
        /// Y축 최대, 최소값 설정
        /// </summary>
        void SetYAxisMax()
        {
            double[] tempMaxValues = { aTempCopy.Max(), bTempCopy.Max(), cTempCopy.Max() };
            double[] tempMinValues = { aTempCopy.Min(), bTempCopy.Min(), cTempCopy.Min() };

            // Y축의 max, min값을 설정해주기 위해 전체 max, min값을 체크한다
            totalMax = (int)tempMaxValues.Max() + 20;
            totalMin = (int)tempMinValues.Min() > 0 ? (int)tempMinValues.Min() : 0;

            wpfPlot.Plot.SetAxisLimits(yMin: totalMin, yMax: totalMax);
            //wpfPlot.Plot.XAxis.SetBoundary(0, 50);
            //wpfPlot.Plot.YAxis.SetBoundary(totalMin, totalMax);
        }

        /// <summary>
        /// 실시간 그래프 시작
        /// </summary>
        public void Start() 
        { 
            // period가 500이므로 500밀리세컨드 마다 UpdateData()를 호출
            _updateDataTimer = new Timer(_ => UpdateData(), null, 0, 500);

            _renderTimer = new DispatcherTimer();
            _renderTimer.Interval = TimeSpan.FromMilliseconds(500);
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

            aTempCopy[aTempCopy.Length - 1] = aTempArr[currentIndex];
            bTempCopy[bTempCopy.Length - 1] = bTempArr[currentIndex];
            cTempCopy[cTempCopy.Length - 1] = cTempArr[currentIndex];


            // x축값 갱신을 위해서 그래프를 아예 지우고
            wpfPlot.Plot.Clear();

            // 새로 그려서
            athermalFurnace = wpfPlot.Plot.AddSignal(aTempCopy, sampleRate: dayRate, label: "A로 온도");
            bthermalFurnace = wpfPlot.Plot.AddSignal(bTempCopy, sampleRate: dayRate, label: "B로 온도");
            cthermalFurnace = wpfPlot.Plot.AddSignal(cTempCopy, sampleRate: dayRate, label: "C로 온도");
            athermalFurnace.OffsetX = bthermalFurnace.OffsetX = cthermalFurnace.OffsetX = dataFirstDay.ToOADate();
            wpfPlot.Plot.XAxis.TickLabelFormat("MM-dd\nHH:mm", dateTimeFormat: true);
            wpfPlot.Plot.XAxis.ManualTickSpacing(5, ScottPlot.Ticks.DateTimeUnit.Minute);
            SetYAxisMax();

            currentIndex++;
            dataFirstDay = dataFirstDay.AddMinutes(5);

            athermalFurnace.IsVisible = aPlotVisible;
            bthermalFurnace.IsVisible = bPlotVisible;
            cthermalFurnace.IsVisible = cPlotVisible;

            crosshair = wpfPlot.Plot.AddCrosshair(0, 0);

            // 렌더한다
            wpfPlot.Plot.Render();
        }

        void Render(object sender, EventArgs e)
        {
            if (pauseGraph)
                return;

            wpfPlot.Refresh();
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
                    aPlotVisible = visible;
                    break;
                case 2:
                    signalPlot = bthermalFurnace;
                    bPlotVisible = visible;
                    break;
                case 3:
                    signalPlot = cthermalFurnace;
                    cPlotVisible = visible;
                    break;
                default:
                    signalPlot = athermalFurnace;
                    aPlotVisible = visible;
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
