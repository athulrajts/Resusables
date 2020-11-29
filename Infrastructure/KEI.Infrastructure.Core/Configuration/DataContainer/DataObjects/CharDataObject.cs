using System.IO;

namespace KEI.Infrastructure
{
    /// <summary>
    /// DataObject implementation for <see cref="char"/>
    /// </summary>
    internal class CharDataObject : DataObject<char>, IWriteToBinaryStream
    {
        /// <summary>
        /// Implementation for <see cref="DataObject.Type"/>
        /// </summary>
        public override string Type => "char";

        /// <summary>
        /// Implementation for <see cref="DataObject.CanConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanConvertFromString(string value)
        {
            return char.TryParse(value, out _);
        }

        /// <summary>
        /// Implementation for <see cref="DataObject.ConvertFromString(string)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value)
        {
            return char.TryParse(value, out char tmp)
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
            if(char.TryParse(value, out _value))
            {
                RaisePropertyChanged(nameof(Value));
            }
        }

    }
}
