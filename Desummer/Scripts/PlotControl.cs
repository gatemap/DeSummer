using ScottPlot;
using ScottPlot.Plottable;
using System.Diagnostics;
using System.Windows.Threading;

namespace Desummer.Scripts
{
    class PlotControl
    {
        WpfPlot wpfPlot;
        ScatterPlot athermalFurnace, bthermalFurnace, cthermalFurnace;

        Stopwatch sw = Stopwatch.StartNew();
        Timer _updateDataTimer;
        DispatcherTimer _renderTimer;

        int[] aTempArr, bTempArr, cTempArr;
        double[] aTempCopy, bTempCopy, cTempCopy;
        double[] days;
        double[] daysCopy;
        int currentIndex = 1;
        int totalMax = 0, totalMin = int.MaxValue;
        readonly int copyDataAmount = 50;
        bool pauseGraph = false;

        public PlotControl(WpfPlot wpfPlot, List<TemperatureData> datas)
        {
            this.wpfPlot = wpfPlot;

            InitTemperatureArray(datas);
            InitDayData();
            InitPlot();
            SetYAxisMax();
            
            Start();
        }

        /// <summary>
        /// 온도 데이터 초기화
        /// </summary>
        /// <param name="datas"></param>
        void InitTemperatureArray(List<TemperatureData> datas)
        {
            // A로 온도 데이터
            List<int> aTempDatas = new List<int>();
            foreach (TemperatureData data in datas)
                aTempDatas.Add(data.A_temp);

            // B로 온도 데이터
            List<int> bTempDatas = new List<int>();
            foreach (TemperatureData data in datas)
                bTempDatas.Add(data.B_temp);

            // C로 온도 데이터
            List<int> cTempDatas = new List<int>();
            foreach (TemperatureData data in datas)
                cTempDatas.Add(data.C_temp);

            aTempArr = aTempDatas.ToArray();
            bTempArr = bTempDatas.ToArray();
            cTempArr = cTempDatas.ToArray();

            // 50개씩 잘라서 넣어줄 데이터
            aTempCopy = new double[copyDataAmount];
            bTempCopy = new double[copyDataAmount];
            cTempCopy = new double[copyDataAmount];

            // 전체 데이터에서 50개를 복사해서 넣어줌
            Array.Copy(aTempArr, aTempCopy, copyDataAmount);
            Array.Copy(bTempArr, bTempCopy, copyDataAmount);
            Array.Copy(cTempArr, cTempCopy, copyDataAmount);
        }

        void InitDayData()
        {
            days = new double[aTempArr.Length];
            DateTime day = new DateTime(2021, 9, 8);
            for (int i = 0; i < days.Length; i++)
                days[i] = day.AddMinutes(5 * i).ToOADate();

            daysCopy = new double[copyDataAmount];
            Array.Copy(days, daysCopy, copyDataAmount);
        }

        /// <summary>
        /// 그래프 초기화
        /// </summary>
        void InitPlot()
        {
            //wpfPlot.Plot.AddSignal(aTempArr, label: "A로 온도");
            //wpfPlot.Plot.AddSignal(bTempArr, label: "B로 온도");
            //wpfPlot.Plot.AddSignal(cTempArr, label: "C로 온도");
            
            athermalFurnace = wpfPlot.Plot.AddScatter(daysCopy, aTempCopy, label: "A로 온도");
            bthermalFurnace = wpfPlot.Plot.AddScatter(daysCopy, bTempCopy, label: "B로 온도");
            cthermalFurnace = wpfPlot.Plot.AddScatter(daysCopy, cTempCopy, label: "C로 온도");
            //wpfPlot.Plot.XAxis.TickLabelFormat("MM-dd HH:mm", dateTimeFormat: true);
            wpfPlot.Plot.Title("보온로 온도 그래프");
            wpfPlot.Plot.XLabel("날짜");
            wpfPlot.Plot.YLabel("보온로 온도");
            wpfPlot.Plot.Legend();
            wpfPlot.Refresh();
        }

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

        public void Start() 
        { 
            sw = Stopwatch.StartNew();

            // period가 50이므로 50밀리세컨드 마다 UpdateData()를 호출
            _updateDataTimer = new Timer(_ => UpdateData(), null, 0, 50);

            _renderTimer = new DispatcherTimer();
            _renderTimer.Interval = TimeSpan.FromMilliseconds(75);
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
            //Array.Copy(days, currentIndex, daysCopy, 0, daysCopy.Length - 1);

            aTempCopy[aTempCopy.Length - 1] = aTempArr[currentIndex + 1];
            bTempCopy[bTempCopy.Length - 1] = bTempArr[currentIndex + 1];
            cTempCopy[cTempCopy.Length - 1] = cTempArr[currentIndex + 1];
            //daysCopy[daysCopy.Length - 1] = days[currentIndex + 1];
            SetYAxisMax();

            currentIndex++;
        }

        void Render(object sender, EventArgs e)
        {
            if (pauseGraph)
                return;

            //wpfPlot.Plot.XAxis.TickLabelFormat("MM-dd HH:mm:", dateTimeFormat: true);
            //wpfPlot.Plot.XAxis.DateTimeFormat(true);
            wpfPlot.Refresh();
        }

        public void PauseGraph()
        {
            pauseGraph = !pauseGraph;
        }

        public void ThermalFurnaceVisible(int thermalFurnaceType, bool visible)
        {
            if (wpfPlot is null) return;

            ScatterPlot scatter;

            switch (thermalFurnaceType)
            {
                case 1:
                    scatter = athermalFurnace;
                    break;
                case 2:
                    scatter = bthermalFurnace;
                    break;
                case 3:
                    scatter = cthermalFurnace;
                    break;
                default:
                    scatter = athermalFurnace;
                    break;
            }

            scatter.IsVisible = visible;
            wpfPlot.Refresh();
        }
    }
}
