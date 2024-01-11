using MySqlConnector;

namespace Desummer.Scripts
{
    internal class ColumnConverter
    {
        /// <summary>
        /// DB데이터 string 변환
        /// </summary>
        public static string GetColumnString(MySqlDataReader reader, string column)
        {
            if (reader == null || reader[column] == null)
                return string.Empty;

            return string.Format("{0}", reader[column]);
        }

        /// <summary>
        /// 데이터 int 변환
        /// </summary>
        public static int GetColumnInt(MySqlDataReader reader, string column) 
        {
            if (reader == null || reader[column] == null)
                return 0;

            int n = 0;

            if (int.TryParse(reader[column].ToString(), out n))
                return n;
            else
                return -1;
        }

        /// <summary>
        /// 데이터 bool 변환
        /// </summary>
        public static bool GetColumnBool(MySqlDataReader reader, string column)
        {
            if (reader == null || reader[column] == null)
                return false;

            if (GetColumnString(reader, column).Equals("True"))
                return true;
            else if (GetColumnString(reader, column).Equals("False"))
                return false;
            else
                return false;
        }
    }
}
