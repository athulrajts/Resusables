using System;
using System.Xml;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Dummy object to skip read <see cref="DataObject"/>s who hasn't
    /// registered itself to <see cref="DataObjectFactory"/>
    /// </summary>
    public class NotSupportedDataObject : DataObject
    {
        public override string Type => "404";

        public override Type GetDataType()
        {
            throw new NotImplementedException();
        }

        public override object GetValue()
        {
            throw new NotImplementedException();
        }

        public override bool SetValue(object value)
        {
            throw new NotImplementedException();
        }

        public override void ReadXml(XmlReader reader)
        {
            // read till end, so we can move on to the next node
            while(reader.EOF == false)
            {
                reader.Read();
            }
        }
    }
}
