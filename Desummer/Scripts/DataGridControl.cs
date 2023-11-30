using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Desummer.Scripts
{
    class DataGridControl
    {
        public string Way { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public double AverageValue { get; private set; }
        public double normal_rate { get; private set; }

        public void MinMaxAvg(System.Windows.Controls.DataGrid grid, List<TemperatureData> Select_Month_Week,
            ProgressRing progressRingA, ProgressRing progressRingB, ProgressRing progressRingC,
            TextBlock textBlockA, TextBlock textBlockB, TextBlock textBlockC)
        {
            //A, B, C로의 온도 리스트
            List<int> A_temp = Select_Month_Week.Select(item => item.A_temp).ToList();
            List<int> B_temp = Select_Month_Week.Select(item => item.B_temp).ToList();
            List<int> C_temp = Select_Month_Week.Select(item => item.C_temp).ToList();

            //680~750 정상 온도일 때의 데이터 갯수
            double normal_A = (from temp in A_temp
                               where temp >= 680 && temp <= 750
                               select temp).Count();
            double normal_B = (from temp in B_temp
                               where temp >= 680 && temp <= 750
                               select temp).Count();
            double normal_C = (from temp in C_temp
                               where temp >= 680 && temp <= 750
                               select temp).Count();

            //그리드에 데이터 리스트 넣기
            List<DataGridControl> list = new List<DataGridControl>();
            try
            {
                list.Add(new DataGridControl { Way = "A로", MinValue = A_temp.Min(), MaxValue = A_temp.Max(), AverageValue = Math.Round(A_temp.Average(), 2) });
                list.Add(new DataGridControl { Way = "B로", MinValue = B_temp.Min(), MaxValue = B_temp.Max(), AverageValue = Math.Round(B_temp.Average(), 2) });
                list.Add(new DataGridControl { Way = "C로", MinValue = C_temp.Min(), MaxValue = C_temp.Max(), AverageValue = Math.Round(C_temp.Average(), 2) });
            }
            catch
        {
                System.Windows.MessageBox.Show("데이터가 없습니다");
            }
            grid.ItemsSource = list;
            //progressRingA 색 조정
            progressRingA.Progress = Math.Round(normal_A / A_temp.Count() * 100, 2);
            if(Math.Round(normal_A / A_temp.Count() * 100, 2) > 66)
            {
                progressRingA.Foreground = new SolidColorBrush(Colors.SpringGreen);
            }
            else if (Math.Round(normal_A / A_temp.Count() * 100, 2) > 33)
            {
                progressRingA.Foreground = new SolidColorBrush(Colors.LightGoldenrodYellow);
            }
            else
            {
                progressRingA.Foreground = new SolidColorBrush(Colors.Red);
            }
            //progressRingB 색 조정
            progressRingB.Progress = Math.Round(normal_B / B_temp.Count() * 100, 2);
            if (Math.Round(normal_B / B_temp.Count() * 100, 2) > 66)
            {
                progressRingB.Foreground = new SolidColorBrush(Colors.SpringGreen);
            }
            else if (Math.Round(normal_B / B_temp.Count() * 100, 2) > 33)
            {
                progressRingB.Foreground = new SolidColorBrush(Colors.LightGoldenrodYellow);
            }
            else
            {
                progressRingB.Foreground = new SolidColorBrush(Colors.Red);
            }
            //progressRingC 색 조정
            progressRingC.Progress = Math.Round(normal_C / C_temp.Count() * 100, 2);
            if (Math.Round(normal_C / C_temp.Count() * 100, 2) > 66)
            {
                progressRingC.Foreground = new SolidColorBrush(Colors.SpringGreen);
            }
            else if (Math.Round(normal_C / C_temp.Count() * 100, 2) > 33)
            {
                progressRingC.Foreground = new SolidColorBrush(Colors.LightGoldenrodYellow);
            }
            else
            {
                progressRingC.Foreground = new SolidColorBrush(Colors.Red);
            }

            textBlockA.Text = "  A로\n" + Math.Round(normal_A / A_temp.Count() * 100, 2).ToString() + "%";
            textBlockB.Text = "  B로\n" + Math.Round(normal_B / B_temp.Count() * 100, 2).ToString() + "%";
            textBlockC.Text = "  C로\n" + Math.Round(normal_C / C_temp.Count() * 100, 2).ToString() + "%";
        }
    }
}
