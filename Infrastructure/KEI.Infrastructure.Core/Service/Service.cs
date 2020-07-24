using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure.Service
{
    public class Service
    {
        public TypeInfo ServiceType { get; set; }

        public TypeInfo ImplementationType { get; set; }

        [XmlIgnore]
        public List<TypeInfo> AvailableImplementations { get; set; }

        public string Name { get; set; }

        public Service(Type serviceType, List<Type> availableImplementations, ServiceAttribute sa)
        {
            AvailableImplementations = availableImplementations.Select(x => new TypeInfo(x)).ToList();
            ServiceType = new TypeInfo(serviceType);

            if(sa.DefaultImplementation is Type t)
            {
                ImplementationType = AvailableImplementations.Find(x => x.FullName == t.FullName);
            }
            else
            {
                ImplementationType = AvailableImplementations.FirstOrDefault();
            }

            Name = sa.Name ?? serviceType.Name;
        }

        public Service()
        {

        }
    }

    [AttributeUsage(validOn: AttributeTargets.Interface)]
    public class ServiceAttribute : Attribute
    {
        public Type DefaultImplementation { get; set; }

        public string Name { get; set; }

        public ServiceAttribute(string name, Type defaultImplementation)
        {
            DefaultImplementation = defaultImplementation;
            Name = name;
        }
    }
}
