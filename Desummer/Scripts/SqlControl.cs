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
                sql = $"SELECT * FROM sys.userdata WHERE userId='{id}' AND userPassword='{PasswordEncryption.SHA256Hash(pw)}'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Debug.WriteLine($"데이터 불러오기 성공\nuserIndex : {reader["userIndex"]}, userId : {reader["userId"]}, userPassword : {reader["userPassword"]}," +
                    $" userName : {reader["userName"]}, admin : {reader["admin"]}");
                    Debug.WriteLine($"입력된 비밀번호의 암호화 : {PasswordEncryption.SHA256Hash(pw)}");

                    App.userData = new UserData(ColumnConverter.GetColumnString(reader, "userName"), ColumnConverter.GetColumnString(reader, "userId"), ColumnConverter.GetColumnBool(reader, "admin"));

                    if (App.userData.userId.Equals(id) && ColumnConverter.GetColumnString(reader, "userPassword").Equals(PasswordEncryption.SHA256Hash(pw)))
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

                sql = $"insert into sys.userdata (userId, userPassword, userName) values ('{id}', '{PasswordEncryption.SHA256Hash(pw)}', '{name}')";
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

        /// <summary>
        /// 비밀번호 변경
        /// </summary>
        /// <param name="id">보안코드 입력할 때, 입력했던 아이디</param>
        /// <param name="pw">변경하려고 하는 비밀번호</param>
        /// <returns></returns>
        public bool ChangePassword(string id, string pw)
        {
            bool changeSuccess = false;

            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                // 패스워드 갱신
                sql = $"update sys.userdata set userPassword='{PasswordEncryption.SHA256Hash(pw)}' where userId='{id}'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("비밀번호 변경 완료", "알림", MessageBoxButton.OK);
                    changeSuccess = true;
                }
                else
                {
                    MessageBox.Show("DB 업데이트 오류", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    changeSuccess = false;
                }
            }
            catch (Exception e) { Debug.WriteLine(e.Message); }
            finally { conn.Close(); }

            return changeSuccess;
        }
    }
}
