
namespace Desummer.Scripts
{
    class ProcessData
    {
        string data = string.Empty;
        string csvFilePath = @"pack://application,,,/Resources";

        List<TemperatureData> JanData = new List<TemperatureData>();
        List<TemperatureData> FebData = new List<TemperatureData>();
        List<TemperatureData> MarData = new List<TemperatureData>();
        List<TemperatureData> AprData = new List<TemperatureData>();
        List<TemperatureData> Week_Data = new List<TemperatureData>();
        public ProcessData()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>제한된 갯수의 데이터 리스트를 반환한다</returns>
        public List<TemperatureData> TemperatureLimitData()
        {
            data = dataResource.termalFurnace;
            List<TemperatureData> dataList = new List<TemperatureData>();

            bool firstLine = true;

            foreach(var row in data.Split('\n'))
            {
                if(firstLine)
                {
                    firstLine = false;
                    continue;
                }

                if (string.IsNullOrEmpty(row))
                    break;

                var columns = row.Split(',');
                dataList.Add(new TemperatureData(columns[0], int.Parse(columns[1]), int.Parse(columns[2]), int.Parse(columns[3])));

                if (dataList.Count > 100)
                    break;
            }

            return dataList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>모든 데이터를 반환</returns>
        public List<TemperatureData> TemperatureTotalData()
        {
            //Uri uri = new Uri(csvFilePath + "/termalFurnace.csv", UriKind.Absolute);
            // 리소스 파일에 등록한 csv 파일을 내용을 가져옴
            data = dataResource.termalFurnace;
            List<TemperatureData> dataList = new List<TemperatureData>();

            bool firstLine = true;

            foreach (var row in data.Split('\n'))
            {
                // 첫째 라인에는 컬럼 헤더값이 들어있으므로 넘긴다
                if (firstLine)
                {
                    firstLine = false;
                    continue;
                }

                // 공백이 들어오면, 해당 라인을 넘긴다
                if (string.IsNullOrEmpty(row))
                    continue;

                var columns = row.Split(',');
                dataList.Add(new TemperatureData(columns[0], int.Parse(columns[1]), int.Parse(columns[2]), int.Parse(columns[3])));

                if (columns[0].Contains("2021-01"))
                    JanData.Add(new TemperatureData(columns[0], int.Parse(columns[1]), int.Parse(columns[2]), int.Parse(columns[3])));
                else if(columns[0].Contains("2021-02"))
                    FebData.Add(new TemperatureData(columns[0], int.Parse(columns[1]), int.Parse(columns[2]), int.Parse(columns[3])));
                else if (columns[0].Contains("2021-03"))
                    MarData.Add(new TemperatureData(columns[0], int.Parse(columns[1]), int.Parse(columns[2]), int.Parse(columns[3])));
                else if (columns[0].Contains("2021-04"))
                    AprData.Add(new TemperatureData(columns[0], int.Parse(columns[1]), int.Parse(columns[2]), int.Parse(columns[3])));
            }

            return dataList;

        }
        public List<TemperatureData> SplitDataMonthly(string month, string week)
        {
            data = dataResource.termalFurnace;
            bool firstLine = true;
            foreach (var row in data.Split('\n'))
            {
                // 첫째 라인에는 컬럼 헤더값이 들어있으므로 넘긴다
                if (firstLine)
                {
                    firstLine = false;
                    continue;
                }
                // 공백이 들어오면, 해당 라인을 넘긴다
                if (string.IsNullOrEmpty(row))
                    continue;
                var columns = row.Split(',');

                //시간 년,월,일,시,분 으로 쪼개기
                DateTime date = DateTime.ParseExact(columns[0], "yyyy-MM-dd H:mm", null);

                int Selected_month = 0;
                switch (month)
                {
                    case "21년 1월":
                        Selected_month = 1;
                        break;
                    case "21년 2월":
                        Selected_month = 2;
                        break;
                    case "21년 3월":
                        Selected_month = 3;
                        break;
                    default:
                        Selected_month = 4;
                        break;
                }
                
                int Selected_week = 0;
                switch (week)
                {
                    case "1주차":
                        Selected_week = 7;
                        break;
                    case "2주차":
                        Selected_week = 14;
                        break;
                    case "3주차":
                        Selected_week = 21;
                        break;
                    default:
                        Selected_week = 31;
                        break;
                }
                
                if (date.Month == Selected_month)
                {
                    if (date.Day <= Selected_week && date.Day > Selected_week - 7)
                    {
                        Week_Data.Add(new TemperatureData(columns[0], int.Parse(columns[1]), int.Parse(columns[2]), int.Parse(columns[3])));
                    }
                }
            }
            return Week_Data;
        }
    }
}