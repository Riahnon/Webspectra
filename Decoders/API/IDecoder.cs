using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace WebSpectra.Decoders
{
	public delegate void FFTDataReceivedEventHandler(double[] aData);

	public delegate void TextDataReceivedEventHandler(string aTextData, bool aIsErrorIndication);

	public interface IDecoder : INotifyPropertyChanged
	{
		/// <summary>
		/// Performs decoder initialization. If initialization succeeds IsStarted will return true
		/// as long as the decoder is still working
		/// </summary>
		/// <returns></returns>
		bool Start();

		/// <summary>
		/// Performs decoder stopping. After stopping the decoder IsStarted must return false.
		/// </summary>
		void Stop();

		/// <summary>
		/// A human friendly name to identify the decoder. i.e. string containing model and manufacturer.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// An identifier for the decoder. As unique as possible.
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// Returns true if decoder is started and working properly. Before calling Start this method must return false. After stoping the receiver
		/// this method will return false again.
		/// </summary>
		bool IsStarted
		{
			get;
		}

		/// <summary>
		/// Returns a list of modes supported by this decoder
		/// </summary>
		IEnumerable<IMode> SupportedModes { get; }

		/// <summary>
		/// Sets or gets the current running mode at the decoder
		/// </summary>
		IMode CurrentMode
		{
			get;
			set;
		}

		/// <summary>
		/// Whenever a new FFT line is ready, the decoder provides it calling this event handlers. Data consists on a timestamp and a 
		/// vector of normalized [0,1] X,Y Coordinates
		/// </summary>
		/// 
		event FFTDataReceivedEventHandler FFTDataReceived;

		/// <summary>
		/// Callback used by the decoder to notify text output
		/// </summary>
		event TextDataReceivedEventHandler TextDataReceived;
    }
}
