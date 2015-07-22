using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WebSpectra.MetaData;

namespace WebSpectra.Decoders
{
	public interface IParameter : INotifyPropertyChanged, IMetadataHolder
	{
		string Name { get; }

		object RawValue { get; set; }
	}
}
