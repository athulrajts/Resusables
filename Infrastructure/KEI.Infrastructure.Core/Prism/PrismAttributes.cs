using KEI.Infrastructure.Screen;
using System;

namespace KEI.Infrastructure.Prism
{

    /// <summary>
    /// Should be used on UserControls
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class RegisterForNavigationAttribute : Attribute { }

    /// <summary>
    /// Should be used on UserControls
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class RegisterWithRegionAttribute : Attribute
    {
        public string RegionName { get; set; }

        public RegisterWithRegionAttribute(string regionName)
        {
            RegionName = regionName;
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class RegisterTypeAttribute : Attribute { }


    [AttributeUsage(validOn: AttributeTargets.Class, Inherited = true)]
    public class RegisterSingletonAttribute : Attribute
    {
        public bool NeedResolve { get; set; } = true;
    }

    /// <summary>
    /// Should be used on ViewModels of Screen
    /// which are based on E95 standard.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class ScreenAttribute : RegisterSingletonAttribute
    {
        public string ParentName { get; set; }
        public string DisplayName { get; set; }
        public string ScreenName { get; set; }
        public Icon Icon { get; set; } = Icon.None16x;

        public ScreenAttribute()
        {
            NeedResolve = false;
        }
    }
}
