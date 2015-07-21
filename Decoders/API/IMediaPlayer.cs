using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace wcm.wspectra.decoders
{
	public enum MediaPlayerFileFormat
	{
		WAV,
		PXGF
	}

	public enum MediaPlayerStatus
	{
		Stop,
		Rec,
		Play,
		RecPause,
		PlayPause,
	}
	/// <summary>
	/// Multimedia player and recorder that allows:
	/// -Play an audio file into the decoder so it is used as decoding input.
	/// -Record the signal that is currently being decoded by the decoder (when 
	/// </summary>
	public interface IMediaPlayer : INotifyPropertyChanged
	{
		/// <summary>
		/// Flag indicating if the playback will loop starting again from the begining when the end of the file is reached
		/// </summary>
		bool LoopPlayback { get; set; }

		/// <summary>
		/// Current status of the decoder
		/// </summary>
		MediaPlayerStatus Status { get; }

		/// <summary>
		/// Gets or sets the file format the media player will use for recording or opening the file when playing. The file format cannot be changed during playback
		/// </summary>
		MediaPlayerFileFormat FileFormat { get; set; }

		/// <summary>
		/// Target file of the media player. Used to record or to play depending on the status
		/// </summary>
		String File { get; set; }
		// Returns the carrier frequency (in micro Hz) when playing a pxgf file, otherwise returns 0 or the value set by calling SetPxGFCarrierFrequency
		// Sets the carrier frequency (in micro Hz) to store when recording a pxgf file. When playing a PXGF file the given value may be overwritten by the one stored in the file.
		UInt64 PxGFCarrierFrequency
		{
			get;
			set;
		}
		// Returns the timestamp when playing a pxgf file. The timestamp is in UNIX time (secs elapsed since 1.1.1970). Returns 0 or last timestamp otherwise
		Int64 PxGFTimeStamp
		{
			get;
		}
		// Returns the sample rate of the played file when when playing a pxgf file, returns 0 otherwise
		UInt32 SampleRate
		{
			get;
		}
		// Returns the audio channels of the played file when when playing a file, Otherwise returns 0
		Byte ChannelCount
		{
			get;
		}

		/// <summary>
		/// <summary>
		/// Starts recording operation. Returns true if succeeds, false otherwise
		/// </summary>
		/// <returns></returns>
		bool Rec ( );

		/// <summary>
		/// Starts playback operation. Returns true if command succeeds, false otherwise
		/// </summary>
		/// <returns></returns>
		bool Play ( );

		/// <summary>
		/// Pauses the current operation. Returns true if command succeeds, false otherwise
		/// </summary>
		/// <returns></returns>
		bool Pause ( );

		/// <summary>
		/// Stops the current operation. Returns true if command succeeds, false otherwise
		/// </summary>
		/// <returns></returns>
		bool Stop ( );

		/// <summary>
		/// If recording a file or playing a file, returns the audio recorded/played audio seconds. Returns 0 otherwise
		/// </summary>
		/// <returns></returns>
		double GetFileTime ( );

		/// <summary>
		/// Sets the playback position in secs. If given amount is larger than total length, last position will be set.
		/// Returns true if instance is currently in play or playpause mode and operation succeeds, false otherwise.
		/// </summary>
		/// <param name="aSecs"></param>
		void SetFileTime ( double aSecs );

		/// <summary>
		/// When playing a file returns the total audio length of the file in seconds, returns 0 otherwise
		/// </summary>
		/// <returns></returns>
		double GetTotalTime ( );

		/// <summary>
		/// If recording a file or playing a WAV file, returns the audio recorded/played audio bytes. Returns 0 otherwise
		/// </summary>
		/// <returns></returns>
		UInt64 GetCurrentBytes ( );
		/// <summary>
		/// When playing a file returns the total audio size of the file in bytes, returns 0 otherwise
		/// </summary>
		/// <returns></returns>
		UInt64 GetTotalBytes ( );
	}
}
