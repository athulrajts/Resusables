namespace KEI.Infrastructure.UserManagement
{
    public interface IUserAccessManager
    {
        bool IsUserGrantedAcces(string featureName);
    }
}
