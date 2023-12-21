namespace Desummer.Scripts
{
    public class TemperatureData
    {
        public string date {  get; private set; }
        public int A_temp {  get; private set; }
        public int B_temp { get; private set; }
        public int C_temp { get; private set; }

        public TemperatureData(string date, int a_temp, int b_temp, int c_temp)
        {
            this.date = date;
            A_temp = a_temp;
            B_temp = b_temp;
            C_temp = c_temp;
        }
    }
}
