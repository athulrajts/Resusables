using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace KEI.Infrastructure.Configuration
{
    /// <summary>
    /// Generic class for storing and retrieving data which is capable of storing complex data
    /// structures using a set of primitive types.
    /// </summary>
    [XmlRoot("DataContainer")]
    public class DataContainer : DataContainerBase
    {
        #region Properties

        /// <summary>
        /// Contains a collection of values which are under this need
        /// Even though any type of date can be stored in this collection
        /// only primitive types are recommented here
        /// </summary>
        internal DataObjectCollection Data { get; set; } = new DataObjectCollection();

        public override int Count => Data.Count;

        #endregion

        /// <summary>
        /// Create <see cref="DataContainer"/> from XML serialized file
        /// </summary>
        /// <param name="path">Path to XML file</param>
        /// <returns><see cref="DataContainer"/> deserilized from path</returns>
        public static DataContainer FromFile(string path)
        {
            if (XmlHelper.Deserialize<DataContainer>(path) is DataContainer dc)
            {
                dc.FilePath = path;

                return dc;
            }

            return null;
        }


        /// <summary>
        /// Serializes DataContainer object to an XML file to the given path
        /// </summary>
        /// <param name="path">file path to store the config</param>
        /// <returns>Is Sucess</returns>
        public override bool Store(string path)
        {
            FilePath = path;

            try
            {
                XmlHelper.Serialize(this, path);
            }
            catch (Exception)
            {
                ViewService.Warn($"Unable to Write config \"{Name}\" ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a clone of this config
        /// </summary>
        /// <returns>Clone</returns>
        public DataContainer Clone()
            => XmlHelper.DeserializeFromString<DataContainer>(XmlHelper.Serialize(this));

        public override void Add(DataObject obj)
        {
            Data.Add(obj);
        }

        public override void Remove(DataObject obj)
        {
            Data.Remove(obj);
        }


        public override IEnumerator<DataObject> GetEnumerator() => Data.GetEnumerator();

        public override IEnumerable<string> GetKeys() => Data.Select(x => x.Name);

        public override DataObject Find(string key) => Data.FirstOrDefault(x => x.Name == key);

    }
}
