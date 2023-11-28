using System.Windows.Controls;

namespace Desummer.Scripts
{
    class DataGridControl
    {
        public string Way { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public double AverageValue { get; private set; }
        public double normal_rate { get; private set; }

        public void MinMaxAvg(DataGrid grid, List<TemperatureData> Select_Month_Week)
        {
            //A, B, C로의 온도 리스트
            List<int> A_temp = Select_Month_Week.Select(item => item.A_temp).ToList();
            List<int> B_temp = Select_Month_Week.Select(item => item.B_temp).ToList();
            List<int> C_temp = Select_Month_Week.Select(item => item.C_temp).ToList();

            //660~750 정상 온도일 때의 데이터 갯수
            double normal_A = (from temp in A_temp
                               where temp >= 660 && temp <= 750
                               select temp).Count();
            double normal_B = (from temp in B_temp
                               where temp >= 660 && temp <= 750
                               select temp).Count();
            double normal_C = (from temp in C_temp
                               where temp >= 660 && temp <= 750
                               select temp).Count();

            //그리드에 데이터 리스트 넣기
            List<DataGridControl> list = new List<DataGridControl>();
            try
            {
                list.Add(new DataGridControl { Way = "A로", MinValue = A_temp.Min(), MaxValue = A_temp.Max(), AverageValue = Math.Round(A_temp.Average(), 2), normal_rate = Math.Round(normal_A / A_temp.Count() * 100, 2) });
                list.Add(new DataGridControl { Way = "B로", MinValue = B_temp.Min(), MaxValue = B_temp.Max(), AverageValue = Math.Round(B_temp.Average(), 2), normal_rate = Math.Round(normal_B / B_temp.Count() * 100, 2) });
                list.Add(new DataGridControl { Way = "C로", MinValue = C_temp.Min(), MaxValue = C_temp.Max(), AverageValue = Math.Round(C_temp.Average(), 2), normal_rate = Math.Round(normal_C / C_temp.Count() * 100, 2) });
            }
            catch
        {
                MessageBox.Show("데이터가 없습니다");
            }
            grid.ItemsSource = list;
        }
    }
}
