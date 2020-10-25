using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace KEI.Infrastructure.Server
{
    public class InputParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public InputParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

    public class InputParameterInfo
    {
        public string Name { get; set; }

        public Type Type { get; }

        public InputParameterInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }

    public class InputParameterInfoCollection : List<InputParameterInfo>
    {
        public InputParameterCollection Parse(byte[] inputBuffer)
        {
            // first 4 bytes is total length, skip those
            int bytesParsed = 4;

            var inputs = new InputParameterCollection();

            foreach (var inputInfo in this)
            {
                if(Parse(inputInfo, inputBuffer, ref bytesParsed) is InputParameter ip)
                {
                    inputs.Add(ip);
                }
            }

            return inputs;
        }

        public InputParameter Parse(InputParameterInfo inputInfo, byte[] inputBuffer, ref int bytesParsed)
        {
            InputParameter retValue = null;
            var t = inputInfo.Type;

            if(t == typeof(uint))
            {
                retValue = new InputParameter(inputInfo.Name, BitConverter.ToUInt32(inputBuffer, bytesParsed));
                bytesParsed += 4;
            }
            else if(t == typeof(float))
            {
                retValue = new InputParameter(inputInfo.Name, BitConverter.ToSingle(inputBuffer, bytesParsed));
                bytesParsed += 4;
            }
            else if (t == typeof(double))
            {
                retValue = new InputParameter(inputInfo.Name, BitConverter.ToDouble(inputBuffer, bytesParsed));
                bytesParsed += 8;
            }
            else if (t == typeof(string))
            {
                uint size = BitConverter.ToUInt32(inputBuffer, bytesParsed);
                var value = Encoding.ASCII.GetString(inputBuffer, bytesParsed + 4, (int)size);
                bytesParsed += 4 + value.Length;
                retValue = new InputParameter(inputInfo.Name, value);
            }

            return retValue;
        }

        public InputParameterInfoCollection Add<T>(string name)
        {
            Add(new InputParameterInfo(name, typeof(T)));
            return this;
        }
    }


    public class InputParameterCollection
    {
        private readonly ObservableCollection<InputParameter> inputs = new ObservableCollection<InputParameter>();

        public object this[int index] => inputs[index].Value;
        public object this[string name] => inputs.FirstOrDefault(x => x.Name == name)?.Value;

        public bool TryGetValue<T>(string name, out T value)
        {
            value = default;

            try
            {
                value = (T)inputs.FirstOrDefault(x => x.Name == name)?.Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T GetValue<T>(string name) => (T)inputs.FirstOrDefault(x => x.Name == name)?.Value;

        public void Add(InputParameter ip) => inputs.Add(ip);
    }
}
