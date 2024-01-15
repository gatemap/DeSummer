namespace Desummer.Scripts
{
    public class UserData
    {
        public string userName { get; private set; }
        public string userId { get; private set; }
        public bool admin { get; private set; } = false;

        public UserData(string userName, string userId, bool admin)
        {
            this.userName = userName;
            this.userId = userId;
            this.admin = admin;
        }
    }
}
