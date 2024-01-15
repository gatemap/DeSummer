using System.Text;
using System.Net.Mail;
using System.Windows;
using System.Diagnostics;

namespace Desummer.Scripts
{
    internal class MailControl
    {
        string serialCode = string.Empty;


        public bool SendMailSuccess(string userId, string email)
        {
            SqlControl sqlControl = new SqlControl();

            // 존재하지 않는 아이디인 경우
            if (!sqlControl.CheckDuplicateId(userId))
            {
                MessageBox.Show("존재하지 않는 사용자입니다", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // To : 수신, From : 발신(회신), Subject : 메일 제목, Body : 메일 내용
            MailMessage mail = new MailMessage();
            mail.To.Add(email + "@gmail.com");
            mail.From = new MailAddress("");
            mail.Subject = "[Desummer]Serial Code";

            serialCode = CreateRandomSerial();
            mail.Body = serialCode;

            // HTML 사용, 메일 발송 실패시 알려줌
            mail.IsBodyHtml = true;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            // 메일 제목, 내용 인코딩 UTF-8
            mail.SubjectEncoding = Encoding.UTF8;
            mail.BodyEncoding = Encoding.UTF8;

            // stmpClient 사용을 위한 smtp 객체 생성
            SmtpClient smtp = new SmtpClient();

            // smtp 메일 서버 주소, 포트 주소 입력
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.Timeout = 10000;

            // 서버 기본인증 미사용해야함
            // 사용자(보내는 사람 구글) 아이디와 앱 비밀번호를 입력할 것이기 떄문에
            smtp.UseDefaultCredentials = false;

            // smtp ssl 보안설정, 이메일 네트워크를 통해 smtp 서버로 전송하기
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            // 사용자의 구글 아이디와 비밀번호 입력하기
            smtp.Credentials = new System.Net.NetworkCredential("", "");

            try
            {
                // smtp 객체를 통해 mail 발송
                smtp.Send(mail);
                mail.Dispose();

                MessageBox.Show("전송 완료");
                return true;
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 랜덤 영문+숫자 6자리 문자열 생성
        /// </summary>
        /// <returns></returns>
        string CreateRandomSerial()
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] charsArr = new char[6];
            Random rnd = new Random();

            for (int i = 0; i < charsArr.Length; i++)
                charsArr[i] = characters[rnd.Next(characters.Length)];

            Debug.WriteLine($"생성된 시리얼 코드 : {new String(charsArr)}");

            return new String(charsArr);
        }

        public bool MatchSerialCode(string serialCode)
        {
            return this.serialCode.Equals(serialCode);
        }

    }
}