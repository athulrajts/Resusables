using System.Collections.Generic;
using System.ComponentModel;

namespace KEI.Infrastructure.UserManagement
{
    public interface IUserManager : INotifyPropertyChanged
    {
        List<IUser> Users { get; }
        IUser CurrentUser { get; }

        bool ValidateLogin(string username, string password);
    }

    public enum UserLevel
    {
        Operator,
        Engineer,
        Administrator
    }

}
