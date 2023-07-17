using System.Collections.Generic;
using System.ComponentModel;

namespace BaseTypes
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            RaisePropertiesChanged();
        }

        public new bool Remove(TKey key)
        {
            bool removed = base.Remove(key);
            if (removed) RaisePropertiesChanged();
            return removed;
        }

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public new void Clear()
        {
            Items.Clear();
            RaisePropertiesChanged();
        }

        public Dictionary<TKey, TValue> Items => this;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertiesChanged()
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Items));
        }
    }
}