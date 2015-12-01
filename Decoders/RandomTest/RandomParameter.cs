using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebSpectra.Decoders.RandomTest
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

        public override object Value
        {
            get
            {
                return m_tValue;
            }
            set
            {
                this._SetRawValue(value);
            }
        }

        private void _SetRawValue(object aValue)
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
            }
        }
    }

    internal class RandomRangedParameter<T> : RandomParameter<T>, IRangedParameter
    {
        T mMin;
        T mMax;
        public RandomRangedParameter(T aValue, T aMin, T aMax)
            : base(aValue)
        {
            if (!(aValue is IComparable<T>))
                throw new ArgumentException("The type of the parameter is not comparable");
            if (typeof(T).IsClass)
            {
                if (Object.ReferenceEquals(aValue, null))
                    throw new ArgumentException("The given value cannot be null");
                if (Object.ReferenceEquals(aMin, null))
                    throw new ArgumentException("The given value cannot be null");
                if (Object.ReferenceEquals(aMax, null))
                    throw new ArgumentException("The given value cannot be null");
            }


            var lValue = (IComparable<T>)aValue;
            var lMin = (IComparable<T>)aMin;
            var lMax = (IComparable<T>)aMax;
            if (lValue.CompareTo(aMin) < 0)
                throw new ArgumentException("The given value cannot be less than the mininum");
            if (lValue.CompareTo(aMax) > 0)
                throw new ArgumentException("The given value cannot be more than the maximum");

            mMin = aMin;
            mMax = aMax;
        }

        public object Max
        {
            get
            {
                return mMax; ;
            }
        }

        public object Min
        {
            get
            {
                return mMin;
            }
        }

        public override void SetValue(T aValue)
        {
            var lValue = (IComparable<T>)aValue;
            if (lValue.CompareTo((T)this.Min) < 0 || lValue.CompareTo((T)this.Max) > 0)
                return;

            base.SetValue(aValue);
        }
    }

    internal class RandomSelectionParameter<T> : RandomParameter<T>, ISelectionParameter
    {
        T[] mValidValues;
        public RandomSelectionParameter(T aValue, T[] aValidValues)
            : base(aValue)
        {
            mValidValues = aValidValues.ToArray();
        }

        public object[] ValidValues
        {
            get
            {
                return mValidValues.Select(aItem => (object)aItem).ToArray();
            }
        }

        public override void SetValue(T aValue)
        {
            if (!mValidValues.Contains(aValue))
                return;

            base.SetValue(aValue);
        }
    }

    internal class RandomNamedSelectionParameter<T> : RandomParameter<T>, INamedSelectionParameter
    {
        KeyValuePair<string,T>[] mValidNamedValues;
        public RandomNamedSelectionParameter(T aValue, KeyValuePair<string, T>[] aValidNamedValues)
            : base(aValue)
        {
            mValidNamedValues = aValidNamedValues.ToArray();
        }

        public KeyValuePair<string, object>[] ValidNamedValues
        {
            get
            {
                return mValidNamedValues.Select(aItem => new KeyValuePair<string,object>(aItem.Key, (object)aItem.Value)).ToArray();
            }
        }

        public override void SetValue(T aValue)
        {
            var lValid = mValidNamedValues.Select(aItem => aItem.Value).ToArray();
            if (!lValid.Contains(aValue))
                return;

            base.SetValue(aValue);
        }
    }
}
