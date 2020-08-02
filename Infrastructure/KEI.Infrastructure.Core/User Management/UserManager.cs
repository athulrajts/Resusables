using System.Collections.Generic;
using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Utils;

namespace KEI.Infrastructure.UserManagement
{
    public class UserManager : ConfigHolder<List<User>>, IUserManager
    {
        public UserManager()
        {
            Users = Config;
        }

        public List<User> Users { get; set; }
        public User CurrentUser { get; set; }

        public override string ConfigPath => PathUtils.GetPath("Configs/users.xcfg");
        public override string ConfigName => @"Users";

        public bool ValidateLogin(string username, string password)
        {
            var encryptedPass = EncryptionHelper.Encrypt(password);

            foreach (var user in Users)
            {
                if (user.Password == encryptedPass && user.Username == username)
                {
                    CurrentUser = user;
                    RaisePropertyChanged(nameof(CurrentUser));
                    return true;
                }
            }

            return false;
        }

        protected override void CreateDefaultConfig()
        {
            Config = new List<User>
            {
                new User
                {
                    Username = "a",
                    Password = EncryptionHelper.Encrypt("a"),
                    Level = UserLevel.Administrator
                }
            };
        }
    }
}
