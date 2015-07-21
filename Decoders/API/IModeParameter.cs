using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using wcm.wspectra.metadata;

namespace wcm.wspectra.decoders
{
	public interface IParameter : INotifyPropertyChanged, IMetadataHolder
	{
		string Name { get; }

		object RawValue { get; set; }
	}
}
