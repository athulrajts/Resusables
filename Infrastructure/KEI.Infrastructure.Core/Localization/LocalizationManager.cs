using System.Collections.Generic;

namespace KEI.Infrastructure.Localizer
{
    public class LocalizationManager
    {
        private static LocalizationManager localizationManager;
        public static LocalizationManager Instance => localizationManager ?? (localizationManager = new LocalizationManager());

        private readonly Dictionary<string, IStringLocalizer> _localizers = new Dictionary<string, IStringLocalizer>();
        public IStringLocalizer this[string assemblyName] => _localizers.ContainsKey(assemblyName) ? _localizers[assemblyName] : null;

        public void AddLocalizer(IStringLocalizer stringLocalizer)
        {
            if (_localizers.ContainsKey(stringLocalizer.Name) == false)
            {
                _localizers.Add(stringLocalizer.Name, stringLocalizer);
            }
        }

        public void Reload(string twoLetterISOLanguageName)
        {
            foreach (var item in _localizers)
            {
                item.Value.Reload(twoLetterISOLanguageName);
            }
        }

        public LocalizationManager()
        {
            _localizers = new Dictionary<string, IStringLocalizer>();
        }

        /// <summary>
        /// Dummy functions to force creation
        /// </summary>
        public void Initialize() { }

        #if DEBUG
        public void WriteKeys()
        {
            foreach (var item in _localizers)
            {
                if(item.Value is ResourceManagerStringLocalizer rs)
                {
                    rs.WriteKeys();
                }
            }
        }
        #endif
    }
}
