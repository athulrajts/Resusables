using System.IO;
using Prism.Mvvm;
using System.Resources;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using KEI.Infrastructure.Helpers;
using System;

namespace KEI.Infrastructure.Localizer
{
    public class ResourceManagerStringLocalizer : BindableBase, IStringLocalizer
    {

#if DEBUG
        private List<string> _keys = new List<string>();
        private bool newKeysAdded = false;
        private readonly string fileName;
#endif

        public string this[string name] => GetLocalizedString(name);

        public string this[string name, params object[] args] => GetLocalizedString(name, args);

        public string GetLocalizedString(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

#if DEBUG
            if (_keys.Contains(name) == false)
            {
                _keys.Add(name);
                newKeysAdded = true;
            }
#endif

            if (currentCulture == "en")
                return name;

            var key = name.Replace(" ", "_");

            var result = _resourceManager?.GetString(key);

            return string.IsNullOrEmpty(result) ? name : result;
        }

        public string GetLocalizedString(string name, params object[] args)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

#if DEBUG
            if (_keys.Contains(name) == false)
            {
                _keys.Add(name);
                newKeysAdded = true;
            }
#endif

            if (currentCulture == "en")
                return string.Format(name, args);

            var key = name.Replace(" ", "_");

            var result = _resourceManager.GetString(key);

            return string.Format(string.IsNullOrEmpty(result) ? name : result, args);
        }

        public string Name { get; set; }

        private ResourceManager _resourceManager;
        private readonly Assembly _assembly;
        private string currentCulture;

        public ResourceManagerStringLocalizer(Assembly assembly)
        {

#if DEBUG

            fileName = $"../../Localization/{assembly.GetName().Name}_Resources-en.txt";
            FileHelper.CreateDirectoryIfNotExist(fileName);
            if (File.Exists(fileName))
            {
                foreach (var item in File.ReadAllLines(fileName))
                {
                    if (string.IsNullOrEmpty(item)) continue;

                    _keys.Add(item);
                }
            }

#endif

            _assembly = assembly;

            Name = assembly.GetName().Name;

            currentCulture = CultureInfo.DefaultThreadCurrentUICulture?.TwoLetterISOLanguageName ?? "en";

            var resources = _assembly.GetManifestResourceNames();
            bool hasResource = false;
            foreach (var resource in resources)
            {
                if (resource.Contains(currentCulture))
                {
                    hasResource = true;
                    break;
                }
            }

            currentCulture = hasResource ? currentCulture : "en";

            if (currentCulture != "en")
            {
                var resourceName = $"{assembly.GetName().Name}.Properties.Resources-{currentCulture}";

                _resourceManager = new ResourceManager(resourceName, assembly) { IgnoreCase = true };
            }

            LocalizationManager.Instance.AddLocalizer(this);
        }

        public void Reload(string twoLetterISOLanguageName)
        {
            var resources = _assembly.GetManifestResourceNames();

            bool hasResource = false;
            foreach (var resource in resources)
            {
                if (resource.Contains(twoLetterISOLanguageName))
                {
                    hasResource = true;
                    break;
                }
            }

            currentCulture = hasResource ? twoLetterISOLanguageName : "en";

            if (currentCulture != "en")
            {
                var newResource =  $"{_assembly.GetName().Name}.Properties.Resources-{currentCulture}";

                _resourceManager = new ResourceManager(newResource, _assembly) { IgnoreCase = true };
            }

            RaisePropertyChanged("Item[]");
        }

#if DEBUG
        public void WriteKeys()
        {
            if (_keys.Count > 0 && newKeysAdded)
            {
                File.WriteAllText(fileName, string.Join(Environment.NewLine, _keys));
            }
        }
#endif

    }

}
