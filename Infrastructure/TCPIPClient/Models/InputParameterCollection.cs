using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace TCPClient.Models
{
    public class InputParameterCollection : ObservableCollection<InputParameter>
    {
        public uint MessageLength { get; private set; }

        public InputParameterCollection()
        {
            CollectionChanged += InputParameterCollection_CollectionChanged;
        }

        private void InputParameterCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var param in e.NewItems.OfType<InputParameter>())
                {
                    if (param.Type == typeof(string))
                    {
                        param.PropertyChanged += Param_PropertyChanged; 
                    }
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var param in e.OldItems.OfType<InputParameter>())
                {
                    if (param.Type == typeof(string))
                    {
                        param.PropertyChanged -= Param_PropertyChanged; 
                    }
                }
            }
        }

        private void Param_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MessageLength = 0;

            foreach (var input in this)
            {
                if (input.Type == typeof(string))
                {
                    MessageLength += (uint)input.Value.Length;
                }
                else if (input.Type == typeof(int) ||
                    input.Type == typeof(uint) ||
                    input.Type == typeof(float))
                {
                    MessageLength += 4;
                }
                else if (input.Type == typeof(double))
                {
                    MessageLength += 8;
                }
                else if (input.Type == typeof(bool) ||
                    input.Type == typeof(byte))
                {
                    MessageLength += 1;
                }
            }
        }

        public void WriteBytes(Stream stream)
        {
            foreach (var input in this)
            {
                input.WriteBytes(stream);
            }
        }

        public override string ToString()
        {
            return string.Join(" ", this.Select(x => string.Format("\"{0}\"", x.Value)));
        }

        public bool IsValid() => !this.Any(x => x.IsValid() == false);
    }
}
