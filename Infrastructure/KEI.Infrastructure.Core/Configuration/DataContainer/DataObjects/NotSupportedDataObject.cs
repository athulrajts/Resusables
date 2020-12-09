using System;
using System.Xml;

namespace KEI.Infrastructure
{
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
            while(reader.EOF == false)
            {
                reader.Read();
            }
        }
    }
}
