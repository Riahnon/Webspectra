using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WebSpectra.MetaData;

namespace WebSpectra.Decoders
{
	public class DummyDecoder : IDecoder
	{
		IMode m_tCurrentMode;
		public DummyDecoder()
		{
			m_tCurrentMode = new DummyMode();
			var lSupported = new List<IMode>();
			lSupported.Add(m_tCurrentMode);
			this.SupportedModes = lSupported;
		}

		public bool Start()
		{
			this.IsStarted = true;
			return true;
		}

		public void Stop()
		{
			this.IsStarted = false;
		}

		public string Name
		{
			get { return "Dummy Decoder"; }
		}

		public string Id
		{
			get { return "Dummy Decoder ID"; }
		}

		bool m_boIsStarted;
		public bool IsStarted
		{
			get { return m_boIsStarted; }
			private set
			{
				if (m_boIsStarted != value)
				{
					m_boIsStarted = value;
					_NotifyPropertyChanged("IsStarted");
				}
			}
		}

		public IEnumerable<IMode> SupportedModes
		{
			get;
			private set;
		}

		public IMode CurrentMode
		{
			get { return m_tCurrentMode; }
			set { }
		}

        public int Confidence
        {
            get { return 0; }
        }
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		public event FFTDataReceivedEventHandler FFTDataReceived;

		public event TextDataReceivedEventHandler TextDataReceived;

		private void _NotifyPropertyChanged(string aPropertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(aPropertyName));
		}
	}

	public class DummyMode : IMode
	{
		public readonly static DummyMode Default = new DummyMode();

		public string Name
		{
			get { return "no-mode"; }
		}

		public string Status
		{
			get { return "Idle"; }
		}

		public int Confidence
		{
			get { return -1; }
		}

		public IEnumerable<IParameter> Parameters
		{
			get { return Enumerable.Empty<IParameter>(); }
		}

		public IParameter this[string aParameterName]
		{
			get
			{
				return null;
			}
		}

		public override string ToString()
		{
			return "no-mode";
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class DummyParameter : IParameter
	{
		public readonly static DummyParameter Default = new DummyParameter();

		public event PropertyChangedEventHandler PropertyChanged;

		public DummyParameter(string aName = "DummyParameter")
		{
			this.Name = aName;
		}

		public string Name
		{
			get;
			protected set;
		}

		public virtual object RawValue { get; set; }

		public IEnumerable<KeyValuePair<string, object>> MetadatasRaw { get { return Enumerable.Empty<KeyValuePair<string, object>>(); } }

		public bool HasMetadataRaw(MetadataID aId) { return false; }

		public bool TryGetMetadataRaw(MetadataID aId, out object MetaData) { MetaData = null; return false; }

		public object GetMetaDataRaw(MetadataID aId) { throw new KeyNotFoundException(); }

		public void SetMetadataRaw(MetadataID aId, object aMetaData) { }

		public void RemoveMetadataRaw(MetadataID aId) { }

		public bool HasMetadata<T>(MetadataID<T> aId) { return false; }

		public bool TryGetMetadata<T>(MetadataID<T> aId, out T aMetaData) { aMetaData = default(T); return false; }

		public T GetMetadata<T>(MetadataID<T> aId) { throw new KeyNotFoundException(); }

		public void SetMetadata<T>(MetadataID<T> aId, T aMetaData) { }

		public void RemoveMetadata<T>(MetadataID<T> aId) { }

		protected void _NotifyPropertyChanged(string aPropertyName)
		{
			var lHandler = PropertyChanged;
			if (lHandler != null)
				lHandler(this, new PropertyChangedEventArgs(aPropertyName));
		}
	}

	public static class DummyExtensions
	{
		public static bool IsDummy(this IDecoder aDecoder)
		{
			return !Object.ReferenceEquals(aDecoder, null) && aDecoder is DummyDecoder;
		}

		public static bool IsDummy(this IMode aMode)
		{
			return aMode != null && aMode is DummyMode;
		}

		public static bool IsDummy(this IParameter aParam)
		{
			return aParam != null && aParam is DummyParameter;
		}
	}
}
