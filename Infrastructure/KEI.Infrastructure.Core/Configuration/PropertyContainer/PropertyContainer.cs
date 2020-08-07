using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Generic class for storing and retrieving data which is capable of storing complex data
    /// structures using a set of primitive types and provies support for real time data validation
    /// property change notification and data binding to CLR properties
    /// </summary>
    [XmlRoot("PropertyContainer")]
    public class PropertyContainer : PropertyContainerBase
    {
        internal PropertyObjectCollection Data { get; set; } = new PropertyObjectCollection();

        public override int Count => Data.Count;

        #region Constructor
        internal PropertyContainer()
        {
            Data.CollectionChanged += Data_CollectionChanged;
        }

        ~PropertyContainer()
        {
            Data.CollectionChanged -= Data_CollectionChanged;
        }

        public static PropertyContainer Create() => new PropertyContainer();

        public static PropertyContainer FromFile(string path)
        {
            if (XmlHelper.Deserialize<PropertyContainer>(path) is PropertyContainer dc)
            {
                dc.FilePath = path;

                return dc;
            }

            return null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a clone
        /// </summary>
        /// <returns>Cloned instance</returns>
        public override object Clone()
            => XmlHelper.DeserializeFromString<PropertyContainer>(XmlHelper.Serialize(this));

        public override void Add(DataObject obj)
        {
            if (obj is PropertyObject p)
            {
                Data.Add(p);
            }
            else
            {
                var editor = obj.Type == null ? EditorType.String : obj.Type.GetType().GetEditorType();
                Add(new PropertyObject
                {
                    Name = obj.Name,
                    Value = obj.Value,
                    Editor = editor
                });
            }
        }

        public override void Remove(DataObject obj)
        {
            RemoveProperty(obj.Name);
        }


        public override IEnumerator<DataObject> GetEnumerator() => Data.GetEnumerator();

        public override IEnumerable<string> GetKeys() => Data.Select(x => x.Name);

        #endregion

        #region Implicit Type Conversions

        public static implicit operator DataContainer(PropertyContainer pc) => GetDataContainer(pc);
        public static implicit operator PropertyContainer(DataContainer dc) => GetPropertyContainer(dc);

        /// <summary>
        /// Casts a <see cref="PropertyContainer"/> into a <see cref="DataContainer"/>
        /// </summary>
        /// <param name="pc"><see cref="PropertyContainer"/> instance</param>
        /// <returns><see cref="DataContainer"/> instance</returns>
        private static DataContainer GetDataContainer(PropertyContainer pc)
        {
            if (pc == null)
                return null;

            DataContainer dc = new DataContainer();
            dc.Name = pc.Name;

            foreach (var item in pc)
            {
                if (item.Value is PropertyContainer cpc)
                {
                    dc.Put(item.Name, GetDataContainer(cpc));
                }
                else
                {
                    dc.Add(item);
                }
            }

            return dc;
        }

        /// <summary>
        /// Casts a <see cref="DataContainer"/> into a <see cref="PropertyContainer"/>
        /// </summary>
        /// <param name="dc"><see cref="DataContainer"/> instance</param>
        /// <returns><see cref="PropertyContainer"/> instance</returns>
        private static PropertyContainer GetPropertyContainer(DataContainer dc)
        {
            if (dc == null)
                return null;

            PropertyContainer pc = Create();
            pc.Name = dc.Name;

            foreach (var item in dc)
            {
                if (item.Value is DataContainer cdc)
                {
                    pc.Put(item.Name, GetPropertyContainer(cdc));
                }
                else
                {
                    pc.AddProperty(new PropertyObject
                    {
                        Name = item.Name,
                        Value = item.Value,
                        BrowseOption = BrowseOptions.Browsable,
                        Editor = item.Value.GetType().GetEditorType()
                    });
                }
            }

            return pc;
        }

        #endregion

        #region Private/Internal Methods

        /// <summary>
        /// Adds a <see cref="PropertyObject"/> this instance
        /// </summary>
        /// <param name="item"><see cref="PropertyObject"/> to add</param>
        internal void AddProperty(PropertyObject item) => Add(item);

        /// <summary>
        /// Remove a <see cref="PropertyObject"/> from this instance having key
        /// </summary>
        /// <param name="name">name of the <see cref="PropertyObject"/> to remove</param>
        internal void RemoveProperty(string name)
        {
            if (Data.FirstOrDefault(x => x.Name == name) is PropertyObject p)
            {
                Data.Remove(p);
            }
        }

        /// <summary>
        /// Removes <see cref="PropertyObject"/> by reference
        /// </summary>
        /// <param name="p">reference of <see cref="PropertyObject"/> to remove</param>
        internal void RemoveProperty(PropertyObject p) => Data.Remove(p);

        public override DataObject Find(string key) => Data.FirstOrDefault(x => x.Name == key);

        #endregion
    }
}
