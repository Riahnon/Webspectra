using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WebSpectra.Decoders
{
	public interface IMode : INotifyPropertyChanged
	{
		/// <summary>
		/// Name of the mode
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Status of the mode (i.e. synchronizing, idle, etc)
		/// </summary>
		string Status { get; }

		/// <summary>
		/// Parameters of the mode
		/// </summary>
		IEnumerable<IParameter> Parameters { get; }

		/// <summary>
		/// name based parameter accessor
		/// </summary>
		/// <param name="aParamName">Name of the parameter to retrieve</param>
		/// <returns>The parameter with the given name if found, null otherwise</returns>
		IParameter this[string aParamName] { get; }

	}
}
