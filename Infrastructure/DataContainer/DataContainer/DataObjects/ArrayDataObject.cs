using System;
using System.Xml;
using System.Text;
using System.ComponentModel;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Base class for storing <see cref="System.Array"/> of primitive types
    /// </summary>
    internal abstract class ArrayDataObject : DataObject
    {
        /// <summary>
        /// Holds array object
        /// </summary>
        private Array _value;
        public Array Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        /// <summary>
        /// Element type of array
        /// </summary>
        public Type ElementType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ArrayDataObject(string name, Array value)
        {
            Name = name;
            Value = value;
            ElementType = value.GetType().GetElementType();
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetDataType"/>
        /// </summary>
        /// <returns></returns>
        public override Type GetDataType()
        {
            return Value.GetType();
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.GetValue"/>
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
            if (value is not Array)
            {
                return false;
            }

            Value = value as Array;

            return true;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ReadXmlElement(string, XmlReader)"/>
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlElement(string elementName, XmlReader reader)
        {
            if (base.ReadXmlElement(elementName, reader))
            {
                return true;
            }

            if (elementName == nameof(Value))
            {
                reader.Read();

                if (reader.NodeType == XmlNodeType.CDATA)
                {
                    StringValue = reader.Value;
                }
            }
            else if (elementName == nameof(TypeInfo))
            {
                ElementType = reader.ReadObjectXml<TypeInfo>();
            }

            return false;
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.CanWriteValueAsXmlAttribute"/>
        /// </summary>
        /// <returns></returns>
        protected override bool CanWriteValueAsXmlAttribute() { return false; }
    }

    /// <summary>
    /// Property Object implementation to store <see cref="Array"/> of dimension 1
    /// </summary>
    internal class Array1DDataObject : ArrayDataObject
    {
        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Array1D;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Array1DDataObject(string name, Array value) : base(name, value) { }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            base.WriteXmlContent(writer);

            if (Value is not null)
            {
                StringBuilder sb = new StringBuilder();

                int length = Value.GetLength(0);

                // convert array to string
                // each value separated by ','
                for (int i = 0; i < length; i++)
                {
                    sb.Append(Value.GetValue(i));

                    if (i != length - 1)
                    {
                        sb.Append(',');
                    }
                }

                // write xml
                writer.WriteStartElement(nameof(Value));
                writer.WriteCData(sb.ToString());
                writer.WriteEndElement();

                // Write element type
                writer.WriteObjectXml(new TypeInfo(Value.GetType().GetElementType()));
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            var converter = TypeDescriptor.GetConverter(ElementType);

            var values = StringValue.Split(',');
            Value = Array.CreateInstance(ElementType, values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                Value.SetValue(converter.ConvertFromString(values[i]), i);
            }
        }
    }

    /// <summary>
    /// Property Object implementation to store <see cref="Array"/> of dimension 2
    /// </summary>
    internal class Array2DDataObject : ArrayDataObject
    {
        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => DataObjectType.Array2D;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Array2DDataObject(string name, Array value) : base(name, value) { }

        /// <summary>
        /// Implementation for <see cref="DataObject.WriteXmlContent(XmlWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(XmlWriter writer)
        {
            base.WriteXmlContent(writer);

            if (Value is not null)
            {
                StringBuilder sb = new StringBuilder();

                int rows = Value.GetLength(0);
                int columns = Value.GetLength(1);

                // convert array to string
                // each row is separated by '\n'
                // each column separated by ','
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        sb.Append(Value.GetValue(i, j));

                        if (j != columns - 1)
                        {
                            sb.Append(',');
                        }
                    }

                    sb.Append('\n');
                }

                // write array string
                writer.WriteStartElement(nameof(Value));
                writer.WriteCData(sb.ToString());
                writer.WriteEndElement();

                // Write element type
                writer.WriteObjectXml(new TypeInfo(Value.GetType().GetElementType()));
            }
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnXmlReadingCompleted"/>
        /// </summary>
        protected override void OnXmlReadingCompleted()
        {
            var converter = TypeDescriptor.GetConverter(ElementType);

            var rows = StringValue.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int rowCount = rows.Length;
            int colCount = rows[0].Split(',').Length;

            Value = Array.CreateInstance(ElementType, rowCount, colCount);

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var cols = rows[rowIndex].Split(',');

                for (int colIndex = 0; colIndex < colCount; colIndex++)
                {
                    Value.SetValue(converter.ConvertFromString(cols[colIndex]), rowIndex, colIndex);
                }
            }
        }

    }
}
