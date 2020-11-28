using System.Collections.Generic;
using KEI.Infrastructure.Helpers;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Utils;

namespace KEI.Infrastructure.UserManagement
{
    public class UserManager : ConfigHolder<List<IUser>>, IUserManager
    {
        private IUser _currentUser;
        
        public List<IUser> Users => Config;
        public IUser CurrentUser
        {
            get { return _currentUser; }
            set { SetProperty(ref _currentUser, value); }
        }


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
                    return true;
                }
            }

            return false;
        }

        protected override void CreateDefaultConfig()
        {
            Config = new List<IUser>
            {
                new User
                {
                    Username = "a",
                    Password = EncryptionHelper.Encrypt("a"),
                    Level = UserLevel.Administrator,
                    UserPrefrences = GetDefaultUserPreferences("a")
                },
                new User
                {
                    Username = "o",
                    Password = EncryptionHelper.Encrypt("o"),
                    Level = UserLevel.Operator,
                    UserPrefrences = GetDefaultUserPreferences("o")
                },
                new User
                {
                    Username = "e",
                    Password = EncryptionHelper.Encrypt("e"),
                    Level = UserLevel.Engineer,
                    UserPrefrences = GetDefaultUserPreferences("e")
                },
            };
        }

        private IDataContainer GetDefaultUserPreferences(string username)
        {
            return DataContainerBuilder.Create(username).Build();
        }
    }
}
