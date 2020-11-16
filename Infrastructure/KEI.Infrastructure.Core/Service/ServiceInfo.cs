using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure.Service
{
    public interface IInitializable
    {
        public bool Initialize();
    }

    public interface IConfigurable
    {
        public string ConfigPath { get; }
        public bool LoadConfig();
        public bool StoreConfig(string path);
        public bool ResetConfig();
    }

    public class ServiceInfo
    {
        public TypeInfo ServiceType { get; set; }

        public TypeInfo ImplementationType { get; set; }

        public List<TypeInfo> Dependencies { get; set; }

        public bool IsRequired { get; set; } = true;

        public string Name { get; set; }

        private List<TypeInfo> availableImplementations;
        [XmlIgnore]
        public List<TypeInfo> AvailableImplementations
        {
            get
            {
                if (availableImplementations is null)
                {
                    availableImplementations = new List<TypeInfo>();

                    ImplementationsProvider.Instance.GetImplementations(ServiceType.GetUnderlyingType())?
                        .ForEach(x => availableImplementations.Add(new TypeInfo(x)));
                }

                return availableImplementations;
            }
        }

        public ServiceInfo(Type serviceType, ServiceAttribute sa)
        {
            ServiceType = new TypeInfo(serviceType);

            Name = sa.Name ?? serviceType.Name;
            IsRequired = sa.IsRequired;
            Dependencies = sa.Dependencies;
        }

        public ServiceInfo()
        {

        }
    }

    [AttributeUsage(validOn: AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsRequired { get; set; } = true;

        public List<TypeInfo> Dependencies { get; set; } = new List<TypeInfo>();

        public ServiceAttribute(string name)
        {
            Name = name;
        }

        public ServiceAttribute(string name, params Type[] dependencies)
        {
            Name = name;

            foreach (var type in dependencies)
            {
                Dependencies.Add(new TypeInfo(type));
            }
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Interface, Inherited = true)]
    public class OptionalServiceAttribute : ServiceAttribute
    {
        public OptionalServiceAttribute(string name)
            : base(name)
        {
            IsRequired = false;
        }

        public OptionalServiceAttribute(string name, params Type[] dependencies)
            : base(name, dependencies)
        {
            IsRequired = false;
        }

    }
}
