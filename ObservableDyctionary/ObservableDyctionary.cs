using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BaseTypes
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyPropertyChanged
    {
        #region Events        
        /// <summary>
        /// Occurs when [item added].
        /// </summary>
        public event EventHandler<NotifyKeyValueChangedEventArgs<TKey, TValue>> ItemAdded;
        /// <summary>
        /// Occurs when [item removed].
        /// </summary>
        public event EventHandler<NotifyKeyValueChangedEventArgs<TKey, TValue>> ItemRemoved;
        /// <summary>
        /// Occurs when [item changed].
        /// </summary>
        public event EventHandler<NotifyKeyValueChangedEventArgs<TKey, TValue>> ItemChanged;
        /// <summary>
        /// Occurs when [cleared].
        /// </summary>
        public event EventHandler<NotifyKeyValueChangedEventArgs<TKey, TValue>> Cleared;
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
        /// </summary>
        public ObservableDictionary()
        {
            _keys = [];
            _values = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="opacity">The opacity.</param>
        public ObservableDictionary(int opacity)
        {
            _keys = [.. new TKey[opacity]];
            _values = [.. new TValue[opacity]];
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                if (!ContainsKey(key))
                    throw new KeyNotFoundException($"Key [{key}] not found.");

                var index = _keys.IndexOf(key);
                return _values[index];
            }
            set => AddOrUpdate(key, value);
        }

        private ObservableCollection<TKey> _keys { get; }
        /// <inheritdoc/>
        public ICollection<TKey> Keys => _keys;

        private ObservableCollection<TValue> _values { get; }
        /// <inheritdoc/>
        public ICollection<TValue> Values => _values;

        /// <inheritdoc/>
        public int Count => _keys.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;
        #endregion

        #region Virtual Methods        
        /// <summary>
        /// Called when [item added].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual void OnItemAdded(TKey key, TValue value) => ItemAdded?
            .Invoke(this, new NotifyKeyValueChangedEventArgs<TKey, TValue>(NotifyKeyValueChangeAction.Add, key, value));
        /// <summary>
        /// Called when [item removed].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual void OnItemRemoved(TKey key, TValue value) => ItemRemoved?
            .Invoke(this, new NotifyKeyValueChangedEventArgs<TKey, TValue>(NotifyKeyValueChangeAction.Remove, key, value));
        /// <summary>
        /// Called when [item changed].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnItemChanged(TKey key, TValue oldValue, TValue newValue) => ItemChanged?
            .Invoke(this, new NotifyKeyValueChangedEventArgs<TKey, TValue>(NotifyKeyValueChangeAction.Change, key, oldValue, newValue));
        /// <summary>
        /// Called when [cleared].
        /// </summary>
        protected virtual void OnCleared() => Cleared?
            .Invoke(this, new NotifyKeyValueChangedEventArgs<TKey, TValue>(NotifyKeyValueChangeAction.Clear));
        #endregion

        #region Realization IDictionary<,> interface
        /// <inheritdoc/>
        public void Add(TKey key, TValue value) => Add(new KeyValuePair<TKey, TValue>(key, value));

        public void AddOrUpdate(TKey key, TValue value) => AddOrUpdate(new KeyValuePair<TKey, TValue>(key, value));

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;

            if (_keys.Contains(key))
                throw new ArgumentException($"Key [{key}] already exists.");

            _keys.Add(key);
            _values.Add(value);

            OnPropertyChanged(nameof(Count));
            OnItemAdded(key, value);
        }

        public void AddOrUpdate(KeyValuePair<TKey, TValue> item, Func<TKey, TValue, TValue> updateValueFactory = null)
        {
            var key = item.Key;
            var value = item.Value;

            if (_keys.Contains(key))
            {
                var index = _keys.IndexOf(key);
                var oldValue = _values[index];

                if (updateValueFactory != null)
                    value = updateValueFactory.Invoke(key, oldValue);

                _values[index] = value;

                if (!ReferenceEquals(value, oldValue))
                    OnItemChanged(key, oldValue, value);
                return;
            }

            _keys.Add(key);
            _values.Add(value);

            OnPropertyChanged(nameof(Count));
            OnItemAdded(key, value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _values.Clear();
            _keys.Clear();
            OnCleared();
            OnPropertyChanged(nameof(Count));
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)  => _keys.Contains(item.Key) && _values.Contains(item.Value);

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => _keys.Contains(key);

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            var index = _keys.IndexOf(key);
            if (index < 0) return false;

            _keys.RemoveAt(index);
            var value = _values[index];
            _values.RemoveAt(index);

            OnPropertyChanged(nameof(Count));
            OnItemRemoved(key, value);

            return true;
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }

            var index = _keys.IndexOf(key);
            value = _values[index];
            return true;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < _keys.Count; i++)
                yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertyChanged(this, propertyName);

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(object sender, [CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
    }
}