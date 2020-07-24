using System.Reflection;

namespace KEI.Infrastructure.Localizer
{
    public interface ILocalizationProvider
    {
        IStringLocalizer Provide();
        IStringLocalizer Provide(Assembly assembly);
    }

    public class LocalizationProvider : ILocalizationProvider
    {
        public IStringLocalizer Provide() 
            => Provide(Assembly.GetCallingAssembly());

        public IStringLocalizer Provide(Assembly assembly) 
            => LocalizationManager.Instance[assembly.GetName().Name];
    }
}
