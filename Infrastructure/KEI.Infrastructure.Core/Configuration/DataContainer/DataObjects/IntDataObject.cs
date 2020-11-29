using System.IO;

namespace KEI.Infrastructure
{
    /// <summary>
    /// DataObject Implementation for <see cref="int"/>
    /// </summary>
    internal class IntDataObject : DataObject<int>, IWriteToBinaryStream
    {
        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "int";

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value)
        {
            return int.TryParse(value, out _);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            return int.TryParse(value, out int tmp)
                ? tmp
                : null;
        }

        /// <summary>
        /// Implementation for <see cref="IWriteToBinaryStream.WriteBytes(BinaryWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        public void WriteBytes(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.OnStringValueChanged(string)"/>
        /// </summary>
        /// <param name="value"></param>
        protected override void OnStringValueChanged(string value)
        {
            if(int.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }
        }
    }
}
