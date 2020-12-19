using System;
using System.IO;
using System.Xml;
using System.Collections;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    /// <summary>
    /// <see cref="DataObject"/> implementation to store <see cref="IList"/> of non primitive types
    /// </summary>
    internal class CollectionDataObject : DataObject
    {
        private IDataContainer readingHelper;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public CollectionDataObject(string name, IList value)
        {
            Name = name;
            Value = value;
            CollectionType = value.GetType();
        }

        /// <summary>
        /// Type of <see cref="IList"/>
        /// </summary>
        public Type CollectionType { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        private IList _value;
        public IList Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }


        /// <summary>
        /// Imlementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "dcl";

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return Value.GetType();
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetValue()"/>
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.SetValue(object)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetValue(object value)
        {
            if (Value.GetType() != value.GetType())
            {
                return false;
            }

            Value = (IList)value;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetStartElementName"/>
        /// </summary>
        /// <returns></returns>
        protected override string GetStartElementName()
        {
            return ContainerDataObject.DC_START_ELEMENT_NAME;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.CanWriteValueAsXmlAttribute"/>
        /// </summary>
        /// <returns></returns>
        protected override bool CanWriteValueAsXmlAttribute() => false;

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            // Write base impl
            base.WriteXmlContent(writer);

            writer.WriteObjectXml(new TypeInfo(Value.GetType()));

            // Write values
            int count = 0;
            foreach (var item in Value)
            {
                new ContainerPropertyObject($"{Name}[{count++}]", item).WriteXml(writer);
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            // call base
            if(base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            /// Read DataObject implementation
            if (elementName == ContainerDataObject.DC_START_ELEMENT_NAME)
            {
                var obj = DataObjectFactory.GetDataObject("dc");

                if (obj is not null)
                {
                    using var newReader = XmlReader.Create(new StringReader(reader.ReadOuterXml()));

                    newReader.Read();

                    obj.ReadXml(newReader);

                    readingHelper.Add(obj);
                }

                return true;
            }

            /// Read type info
            else if(elementName == nameof(TypeInfo))
            {
                CollectionType = reader.ReadObjectXml<TypeInfo>();
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            Value = (IList)Activator.CreateInstance(CollectionType);

            foreach (var item in readingHelper)
            {
                Value.Add(item.GetValue());
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.InitializeObject"/>
        /// </summary>
        protected override void InitializeObject()
        {
            readingHelper = new DataContainer();
        }
    }
}
