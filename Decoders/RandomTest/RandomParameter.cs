using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using wcm.wspectra.metadata;

namespace wcm.wspectra.decoders.RandomTest
{
	internal class RandomParameter<T> : RandomParameterBase
	{
		protected T m_tValue;
        public RandomParameter(T aValue)
		{
			if (Object.ReferenceEquals(aValue, null))
				throw new ArgumentNullException("Parameter value cannot be null");
			m_tValue = aValue;
		}

		public T Value
		{
			get
			{
				return m_tValue;
			}
			set
			{
				this.SetValue(value);
			}
		}

		public override object RawValue
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.SetRawValue(value);
			}
		}

		public override void SetRawValue(object aValue)
		{
			if (aValue is T)
			{
				this.SetValue((T)aValue);
			}
			else
			{
				try
				{
					this.SetValue((T)Convert.ChangeType(aValue, typeof(T)));
				}
				catch { }
			}
		}

		public virtual void SetValue(T aValue)
		{
			//If the valid values are restricted, the given one must be one of them
			IEnumerable<T> lValidValues;
			if (this.TryGetMetadata(MetadataIDs<T>.VALID_VALUES, out lValidValues) && !lValidValues.Contains(aValue))
			{
				return;
			}

			IEnumerable<KeyValuePair<string, T>> lValidNamedValues;
			if (this.TryGetMetadata(MetadataIDs<T>.VALID_NAMED_VALUES, out lValidNamedValues) &&
				!lValidNamedValues.Any(aKvp => aKvp.Value.Equals(aValue)))
			{
				return;
			}
			//If there's a maximum value the given one cannot be bigger
			T lMaxValue;
			if (aValue is IComparable<T> && this.TryGetMetadata(MetadataIDs<T>.MAX_VALUE, out lMaxValue) &&
				((IComparable<T>)aValue).CompareTo(lMaxValue) > 0)
				aValue = lMaxValue;

			//If there's a minimum value the given one cannot be smaller
			T lMinValue;
			if (aValue is IComparable<T> && this.TryGetMetadata(MetadataIDs<T>.MIN_VALUE, out lMinValue) &&
				((IComparable<T>)aValue).CompareTo(lMinValue) < 0)
				aValue = lMinValue;

			//If they are the same reference there's nothing to do
			if (Object.ReferenceEquals(m_tValue, aValue))
			{
				return;
			}

			//One of them is null, since they are not the same reference they are for sure different and we have a value change.
			//If none of them is null, the value changed logic relies on the Equals method
			if (Object.ReferenceEquals(m_tValue, null) || Object.ReferenceEquals(aValue, null) || !m_tValue.Equals(aValue))
			{
				m_tValue = aValue;
				_NotifyPropertyChanged("Value");
				_NotifyPropertyChanged("RawValue");
			}
		}
	}
}
