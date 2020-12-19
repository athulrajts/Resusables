using KEI.Infrastructure.Configuration;
using System.Xml.Serialization;

namespace KEI.Infrastructure.UserManagement
{
    public interface IUser
    {
        public string Username { get; }
        public string Password { get; }
        public UserLevel Level { get; }
        public IDataContainer UserPrefrences { get; }
        public bool GetProperty<T>(string key, ref T value);
    }

    public class User : IUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserLevel Level { get; set; }
        public IDataContainer UserPrefrences { get; set; }
        
        public bool GetProperty<T>(string key, ref T value)
            => UserPrefrences.GetValue(key, ref value);
    }
}
