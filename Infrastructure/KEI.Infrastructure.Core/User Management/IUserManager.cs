using System.Collections.Generic;
using System.ComponentModel;

namespace KEI.Infrastructure.UserManagement
{
    public interface IUserManager : INotifyPropertyChanged
    {
        List<User> Users { get; set; }
        User CurrentUser { get; set; }

        bool ValidateLogin(string username, string password);
    }

    public enum UserLevel
    {
        Operator,
        Engineer,
        Administrator
    }

}
