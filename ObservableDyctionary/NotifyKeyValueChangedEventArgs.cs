using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseTypes
{

    public class NotifyKeyValueChangedEventArgs<TKey, TValue>(NotifyKeyValueChangeAction action) : EventArgs
    {
        public NotifyKeyValueChangeAction Action { get; } = action;
        public TKey Key { get; }
        public TValue OldValue { get; }
        public TValue NewValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyKeyValueChangedEventArgs{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public NotifyKeyValueChangedEventArgs(NotifyKeyValueChangeAction action, TKey key, TValue value)
            : this(action)
        {
            Key = key;
            NewValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyKeyValueChangedEventArgs{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="key">The key.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public NotifyKeyValueChangedEventArgs(NotifyKeyValueChangeAction action, TKey key, TValue oldValue, TValue newValue)
            : this(action)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}