using MySqlConnector;
using System.Diagnostics;
using System.Windows;

namespace Desummer.Scripts
{
    internal class SqlControl
    {
        readonly string server = "127.0.0.1";
        readonly string localhost = "localhost";
        readonly string localConnect = "Server=localhost; Uid=desummer; Pwd = 1234";
        MySqlConnection conn;
        string sql = string.Empty;

        public SqlControl() 
        { 
            // sql 연결 세팅
            conn = new MySqlConnection(localConnect);

            /*
            conn.Open();

            Debug.WriteLine("연결 성공");
            Debug.WriteLine(conn.ConnectionString);

            MySqlCommand cmd = new MySqlCommand("", conn);
            cmd.CommandText = "SELECT * FROM sys.userdata";

            MySqlDataReader reader = cmd.ExecuteReader();
            
            while(reader.Read())
            {
                Debug.WriteLine($"userIndex : {reader["userIndex"]}, userId : {reader["userId"]}, userPassword : {reader["userPassword"]}," +
                    $" userName : {reader["userName"]}, admin : {reader["admin"]}");
            }

            reader.Close();
            */
        }

        public bool Login(string id, string pw)
        {
            // 로그인 성공 여부
            bool loginSuccess = false;

            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                // Id는 중복이 없음
                sql = $"SELECT * FROM sys.userdata WHERE userId='{id}' AND userPassword='{pw}'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Debug.WriteLine($"데이터 불러오기 성공\nuserIndex : {reader["userIndex"]}, userId : {reader["userId"]}, userPassword : {reader["userPassword"]}," +
                    $" userName : {reader["userName"]}, admin : {reader["admin"]}");

                    App.main.userData = new UserData(reader["userName"].ToString(), reader["userId"].ToString(), reader["admin"].ToString().Equals("True"));

                    if (App.main.userData.userId.Equals(id) && reader["userPassword"].ToString().Equals(pw))
                        loginSuccess = true;
                }
            }
            catch(Exception e) { Debug.WriteLine(e.Message); }
            finally { conn.Close(); }

            return loginSuccess;
            
        }

        public void RegistUser(string name, string id, string pw, out bool registSuccess)
        {
            registSuccess = false;

            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                sql = $"insert into sys.userdata (userId, userPassword, userName) values ('{id}', '{pw}', '{name}')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("회원 정보가 정상 등록되었습니다.", "알림", MessageBoxButton.OK);
                    registSuccess = true;
                }
                else
                {
                    MessageBox.Show("비정상 정보 등록", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    registSuccess = false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                registSuccess = false;
            }
            finally { conn.Close(); }
        }


        /// <summary>
        /// 아이디 중복 체크
        /// </summary>
        /// <param name="id">입력받은 id</param>
        /// <returns>true=중복임, false=중복아님</returns>
        public bool CheckDuplicateId(string id)
        {
            bool duplicate = false;

            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                sql = $"SELECT COUNT(*) FROM sys.userdata WHERE userId='{id}'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                duplicate = Convert.ToInt32(cmd.ExecuteScalar()) > 0 ? true : false;
            }
            catch (Exception e) { Debug.WriteLine(e.Message); }
            finally { conn.Close(); }

            return duplicate;
        }
    }
}
