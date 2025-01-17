namespace AuthService.Services
{
    public interface IUserService
    {
        public void AddUser(string username, string password, string role);
        public string Authenticate(string username, string password);
    }
}
