using ScottPlot;
using System.Diagnostics;
using System.Windows.Threading;

namespace Desummer.Scripts
{
    class PlotControl
    {
        WpfPlot wpfPlot;

        Stopwatch sw = Stopwatch.StartNew();
        Timer _updateDataTimer;
        DispatcherTimer _renderTimer;

        public PlotControl(WpfPlot wpfPlot, List<Data> datas)
        { 
            this.wpfPlot = wpfPlot;

            List<double> aTempDatas = new List<double>();
            foreach(Data data in datas) 
                aTempDatas.Add(data.A_temp);

            double[] cutData = new double[50];
            Array.Copy(aTempDatas.ToArray(), 0, cutData, 0, 50);

            double[] days = new double[cutData.Length];
            DateTime day = new DateTime(2021, 1, 8);
            for (int i = 0; i < days.Length; i++)
                days[i] = day.AddMinutes(1).AddMinutes(i).ToOADate();

            List<int> bTempDatas = new List<int>();
            foreach (Data data in datas)
                aTempDatas.Add(data.B_temp);

            /*
            List<int> cTempDatas = new List<int>();
            foreach (Data data in datas
                aTempDatas.Add(data.C_temp);
            */

            /*
            this.wpfPlot.Plot.AddScatter(days, cutData);
            this.wpfPlot.Plot.XAxis.TickLabelFormat("yy-MM-dd HH:mm", dateTimeFormat: true);
            this.wpfPlot.Plot.Title("보온로 온도 그래프");
            this.wpfPlot.Plot.XLabel("날짜");
            this.wpfPlot.Plot.YLabel("보온로 온도");
            */

            

            this.wpfPlot.Plot.AddSignal(aTempDatas.ToArray(), label: "A로 온도");
            //this.wpfPlot.Plot.AddSignal(bTempDatas.ToArray(), label: "B로 온도");
            //this.wpfPlot.Plot.AddSignal(cTempDatas.ToArray(), label: "C로 온도");
        }

        public void Start() 
        { 
            sw = Stopwatch.StartNew();

            // period가 5이므로 5밀리세컨드 마다 UpdateData()를 호출
            _updateDataTimer = new Timer(_ => UpdateData(), null, 0, 5);

            _renderTimer = new DispatcherTimer();
            _renderTimer.Interval = TimeSpan.FromMilliseconds(10);
            _renderTimer.Tick += Render;
            _renderTimer.Start();
        }

        void UpdateData() 
        { 
        
        }

        void Render(object sender, EventArgs e)
        {
            wpfPlot.Refresh();
        }
    }
}
