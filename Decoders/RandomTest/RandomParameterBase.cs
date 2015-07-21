using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using wcm.wspectra.metadata;

namespace wcm.wspectra.decoders.RandomTest
{
	internal abstract class RandomParameterBase : IParameter
	{
		public event PropertyChangedEventHandler PropertyChanged;
		Dictionary<string, object> m_tMetaDatas = new Dictionary<string, object>();

        static object sInstanceCnt = (int)0;
        int m_nInstanceId;

        public RandomParameterBase()
        {
            lock (sInstanceCnt)
            {
                int lCnt = (int)sInstanceCnt;
                m_nInstanceId = lCnt;
                sInstanceCnt = (int)lCnt + 1;
            }
            this.Name = "RandomParameter" + m_nInstanceId;
        }


		public string Name
		{
			get;
			private set;
		}

		public abstract object RawValue
		{
			get;
			set;
		}

		public abstract void SetRawValue(object aValue);

        
		public IEnumerable<KeyValuePair<string, object>> MetadatasRaw
		{
			get { return m_tMetaDatas; }
		}


		public bool HasMetadataRaw(MetadataID aId)
		{
			return m_tMetaDatas.ContainsKey(aId);
		}

		public bool TryGetMetadataRaw(MetadataID aId, out object aProperty)
		{
			return m_tMetaDatas.TryGetValue(aId, out aProperty);
		}

		public object GetMetaDataRaw(MetadataID aId)
		{
			return m_tMetaDatas[aId];
		}

		public void SetMetadataRaw(MetadataID aId, object aMetaData)
		{
			m_tMetaDatas[aId] = aMetaData;
		}

		public void RemoveMetadataRaw(MetadataID aId)
		{
			m_tMetaDatas.Remove(aId);
		}

		public bool HasMetadata<T>(MetadataID<T> aId)
		{
			object lDummy;
			return m_tMetaDatas.TryGetValue(aId.ToString(), out lDummy) && lDummy is T;
		}

		public bool TryGetMetadata<T>(MetadataID<T> aId, out T aProperty)
		{
			aProperty = default(T);
			object lDummy;
			if (m_tMetaDatas.TryGetValue(aId.ToString(), out lDummy) && lDummy is T)
			{
				aProperty = (T)lDummy;
				return true;
			}
			return false;
		}

		public T GetMetadata<T>(MetadataID<T> aId)
		{
			return (T)m_tMetaDatas[aId.ToString()];
		}

		public void SetMetadata<T>(MetadataID<T> aId, T aMetaData)
		{
			SetMetadataRaw(aId.ToString(), aMetaData);
		}

		public void RemoveMetadata<T>(MetadataID<T> aId)
		{
			m_tMetaDatas.Remove(aId.ToString());
		}

		protected void _NotifyPropertyChanged(string aPropertyName)
		{
			var lHandler = PropertyChanged;
			if (lHandler != null)
				lHandler(this, new PropertyChangedEventArgs(aPropertyName));
		}

		public override string ToString()
		{
			return this.Name + " = " + this.RawValue;
		}
	}
}
