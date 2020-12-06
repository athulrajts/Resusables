﻿using System;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;
using Prism.Mvvm;
using System.Data;

namespace KEI.Infrastructure.Database
{
    public interface IDatabaseContext { }
    
    public class DatabaseColumn : BindableBase
    {
        public DatabaseColumn(PropertyInfo propInfo)
        {
            TypeString = propInfo.PropertyType.FullName;
            Namespace = propInfo.DeclaringType.Name;
            DisplayName = propInfo.Name;
            Name = propInfo.Name;
        }

        public DatabaseColumn() { }

        [ReadOnly(true)]
        public string Name { get; set; }

        public string FullName => $"{Namespace}.{Name}";
        public string DisplayName { get; set; }
        public bool HasPassFailCriteria { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public Type Type => Type.GetType(TypeString);

        [ReadOnly(true)]
        public string TypeString { get; set; }

        [ReadOnly(true)]
        public string Namespace { get; set; }

        private string unit;
        public string Unit
        {
            get { return unit; }
            set { SetProperty(ref unit, value); }
        }

        public virtual string GetUnit() => string.IsNullOrEmpty(Unit) ? "NA" : Unit;

        public virtual string HeaderString
        {
            get
            {
                return new StringBuilder()
                    .AppendLine(DisplayName)
                    .AppendLine(string.IsNullOrEmpty(Unit) ? "NA" : Unit)
                    .AppendLine("Always")
                    .AppendLine("Always")
                    .ToString();
            }
        }

        public virtual bool IsValid(string value) => true;

        public virtual DataColumn GetDataTableColumn()
        {
            return new TaggedDataColumn
            {
                ColumnName = DisplayName,
                Tag = this,
                DataType = typeof(object),
                Caption = HeaderString
            };
        }

        public virtual bool Equals(DatabaseColumn other)
        {
            return other != null &&
                   Name == other.Name &&
                   FullName == other.FullName &&
                   DisplayName == other.DisplayName &&
                   HasPassFailCriteria == other.HasPassFailCriteria &&
                   Namespace == other.Namespace &&
                   Unit == other.Unit &&
                   HeaderString == other.HeaderString;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, FullName, DisplayName, HasPassFailCriteria, Namespace, Unit, HeaderString);
        }
    }

    public class TaggedDataColumn : DataColumn
    {
        public object Tag { get; set; }
    }
}
