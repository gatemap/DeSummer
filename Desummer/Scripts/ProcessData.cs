namespace Desummer.Scripts
{
    class ProcessData
    {
        string data = string.Empty;
        string csvFilePath = @"pack://application,,,/Resources";

        public ProcessData()
        {

        }

        public void LoadTemperatureData()
        {
            Uri uri = new Uri(csvFilePath + "/termalFurnace.csv", UriKind.Absolute);
            
        }
    }
}
