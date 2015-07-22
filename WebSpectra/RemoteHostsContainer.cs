using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Timers;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;

namespace WebSpectra
{
	public class RemoteHostsContainer
	{
		
		const double TIMER_INTERVAL = 1000;
		private Timer m_tTimer;
		object m_tLocker = new object();
		List<StreamWriter> m_tItems = new List<StreamWriter>();
		public RemoteHostsContainer()
		{
			m_tTimer = new Timer(TIMER_INTERVAL);
			m_tTimer.Elapsed += (object source, ElapsedEventArgs e) => { _Ping(); };
		}
		public void Add(StreamWriter aWriter)
		{
			lock (m_tLocker)
			{
				m_tItems.Add(aWriter);
				if (m_tItems.Count == 1)
					m_tTimer.Start();

			}
		}
		public void Remove(StreamWriter aWriter)
		{
			lock (m_tLocker)
			{
				m_tItems.Remove(aWriter);
				if (m_tItems.Count == 0)
					m_tTimer.Stop();
			}
		}
		public StreamWriter[] GetAll()
		{
			lock (m_tLocker)
			{
				return m_tItems.ToArray();
			}
		}

		/// <summary>
		/// Send the PING by using the <see cref="MvcApplication.Pub"/> and 
		/// log the action using <see cref="MvcApplication.Logger"/>
		/// 
		/// The backup is done by <see cref="MvcApplication.Db"/>
		/// </summary>
		private void _Ping()
		{
			string lJSONStr = "data\n\n";
			var lHosts = MvcApplication.HostContainer.GetAll();
			foreach (var lHost in lHosts)
			{
				lock (lHost)
				{
					try
					{
						lHost.Write(lJSONStr);
						lHost.Flush();
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						MvcApplication.HostContainer.Remove(lHost);
					}
				}
			}
		}
	}
}