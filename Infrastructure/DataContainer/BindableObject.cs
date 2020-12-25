using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KEI.Infrastructure
{
    public class BindableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public void RaisePropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string property = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value) == true)
            {
                return false;
            }

            storage = value;

            RaisePropertyChanged(property);

            return true;
        }
    }
}
