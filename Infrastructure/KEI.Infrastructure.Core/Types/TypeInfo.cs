using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace KEI.Infrastructure.Types
{
    [XmlRoot("UnderlyingType")]
    public class TypeInfo
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Namespace { get; set; }

        [XmlAttribute]
        public string Assembly { get; set; }

        public string FullName => $"{Namespace}.{Name}";

        [XmlElement(IsNullable = false)]
        public List<TypeInfo> GenericTypeArgs { get; set; }

        [XmlIgnore]
        public bool IsGenericType { get; set; }

        public TypeInfo() { }

        public TypeInfo(Type type)
        {
            Name = type.Name;
            Namespace = type.Namespace;
            Assembly = type.Assembly.GetName().Name;

            if(type.IsGenericType)
            {
                IsGenericType = true;
                GenericTypeArgs = new List<TypeInfo>();
                foreach (var t in type.GenericTypeArguments)
                {
                    GenericTypeArgs.Add(new TypeInfo(t));
                }
            }
        }

        public override string ToString() => IsGenericType
            ? $"{Name.Split('`').FirstOrDefault()}<{string.Join(",", GenericTypeArgs.Select(x => x.Name))}>"
            : Name;

        public Type GetUnderlyingType()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == Assembly);

            if (assembly == null)
                return null;

            Type t = assembly.GetType(FullName);

            if(t.IsGenericType)
            {
                return t.MakeGenericType(GenericTypeArgs.Select(x => x.GetUnderlyingType()).ToArray());
            }

            return t;
        }

    }
}
