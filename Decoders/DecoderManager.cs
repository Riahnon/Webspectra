using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wcm.wspectra.decoders.RandomTest;


namespace wcm.wspectra.decoders
{
	public class DecoderManager
	{
		private static object m_locker = new object();
		/// <summary>
		/// Scans and returns a list of available decoders at the given host. Receivers can be accesible via network or just locally.
		/// Local decoder can only be scanned when "localhost" is provided as host.
		/// </summary>
		/// <param name="aHost"></param>
		/// <returns></returns>
		public static IEnumerable<IDecoder> GetAvailableDecoders(string aHost = "localhost", ushort aPort = 33264, uint aTimeout = 10000)
		{
			lock (m_locker)
			{
				List<IDecoder> lResult = new List<IDecoder>();
                lResult.Add(new RandomDecoder());
				return lResult;
			}
		}
	}
}
