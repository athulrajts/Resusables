using Prism.Mvvm;

namespace KEI.Infrastructure.UserManagement
{
    public class User : BindableBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserLevel Level { get; set; }
    }
}
