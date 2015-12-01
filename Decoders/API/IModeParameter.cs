using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WebSpectra.Decoders
{
	public interface IParameter : INotifyPropertyChanged
	{
		string Name { get; }

		object Value { get; set; }
	}

    public interface IRangedParameter : IParameter
    {
        object Max { get; }
        object Min { get; }
    }

    public interface ISelectionParameter : IParameter
    {
        object[] ValidValues { get; }
    }

    public interface INamedSelectionParameter : IParameter
    {
        KeyValuePair<string,object>[] ValidNamedValues { get; }
    }
}
