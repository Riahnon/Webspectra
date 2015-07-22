using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebSpectra.Decoders;

namespace WebSpectra
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	
	public class MvcApplication : System.Web.HttpApplication
	{
		private const int FFT_BLOCK_SIZE = 32;
		private const string EVENT_PATTERN = "event: {0}\ndata: {1}\n\n";
		private const string GET_MODE_NAME_URL = "/Home/GetCurrentModeName";
		private const string GET_MODE_PARAMS_URL = "/Home/GetCurrentModeParams";
		private const string GET_MODE_PARAM_URL = "/Home/GetCurrentModeParam";
		public static IDecoder Decoder { get; private set; }
		public static RemoteHostsContainer HostContainer { get; private set; }
        public static IHubContext DecoderHub { get; private set; }
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
			HostContainer = new RemoteHostsContainer();
			var lDecoders = DecoderManager.GetAvailableDecoders(aTimeout: 1000);
			Decoder = new DummyDecoder();
            DecoderHub = GlobalHost.ConnectionManager.GetHubContext<DecoderHub>();
			if (lDecoders.Count() > 0)
			{
				var lCandidate = lDecoders.First();
				if (lCandidate.Start())
				{
					if (Decoder != null)
					{
						Decoder.FFTDataReceived -= _OnFFTData;
						Decoder.TextDataReceived -= _OnTextData;
						Decoder.PropertyChanged -= _OnDecoderPropertyChanged;
					}

					Decoder = lCandidate;
					if (Decoder != null)
					{
						Decoder.FFTDataReceived += _OnFFTData;
						Decoder.TextDataReceived += _OnTextData;
						Decoder.PropertyChanged += _OnDecoderPropertyChanged;
                        Decoder.CurrentMode = Decoder.SupportedModes.FirstOrDefault();
					}
				}
			}
		}
		protected void Application_End()
		{
			var lHandlers = HostContainer.GetAll();
			foreach (var lHandler in lHandlers)
				HostContainer.Remove(lHandler);
		}

		private void _OnFFTData(double[] aData)
		{
			_SendEvent("fft", aData);
		}

		private void _OnTextData(String aText, bool aIsErrorIndication)
		{
			_SendEvent("text", aText);
		}
		IMode m_tCurrentMode;
		IParameter[] m_tCurrentModeParameters = Enumerable.Empty<IParameter>().ToArray();
		private void _OnDecoderPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "CurrentMode":
				if (m_tCurrentMode != null)
					m_tCurrentMode.PropertyChanged -= _OnCurrentModePropertyChanged;
				
				foreach(var lParam in m_tCurrentModeParameters)
					lParam.PropertyChanged -= _OnCurrentModeParamPropertyChanged;

				m_tCurrentMode = Decoder.CurrentMode;

				if (m_tCurrentMode != null)
				{
					m_tCurrentMode.PropertyChanged += _OnCurrentModePropertyChanged;
					m_tCurrentModeParameters = m_tCurrentMode.Parameters.ToArray();
				}
				else
				{
					m_tCurrentModeParameters = Enumerable.Empty<IParameter>().ToArray();
				}
				foreach (var lParam in m_tCurrentModeParameters)
					lParam.PropertyChanged += _OnCurrentModeParamPropertyChanged;

				_SendEvent("modechanged", new { modeurl = GET_MODE_NAME_URL });
				_SendEvent("paramschanged", new { paramsurl = GET_MODE_PARAMS_URL });
                _SendEvent("confidence", m_tCurrentMode.Confidence);
				break;


			}
		}

		private void _OnCurrentModePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "Confidence":
				_SendEvent("confidence", m_tCurrentMode.Confidence);
				break;
			case "Parameters":
				foreach (var lParam in m_tCurrentModeParameters)
					lParam.PropertyChanged -= _OnCurrentModeParamPropertyChanged;

				m_tCurrentModeParameters = m_tCurrentMode.Parameters.ToArray();

				foreach (var lParam in m_tCurrentModeParameters)
					lParam.PropertyChanged += _OnCurrentModeParamPropertyChanged;

				_SendEvent("paramschanged", new { paramsurl = GET_MODE_PARAMS_URL });
				break;
			}
		}

		private void _OnCurrentModeParamPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var lParam = (IParameter)sender;
			switch (e.PropertyName)
			{
			case "RawValue":
				_SendEvent("paramvaluechanged", new { paramname = lParam.Name, paramurl = GET_MODE_PARAM_URL });
				break;
			}
		}

		private void _SendEvent(string aEvent, object aData)
		{
            DecoderHub.Clients.All.OnServerEvent(aEvent, aData);
			/*var lHosts = MvcApplication.HostContainer.GetAll();
			Dictionary<string, object> lJSONDict = new Dictionary<string, object>(){
								{aEvent, aData}
            };
			var lJSONData = JsonConvert.SerializeObject(aData);
			foreach (var lHost in lHosts)
			{
				lock (lHost)
				{
					try
					{

						lHost.Write(EVENT_PATTERN, aEvent, lJSONData);
						lHost.Flush();

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						MvcApplication.HostContainer.Remove(lHost);
					}
				}
			}*/
		}

		private string _GetBaseUrl()
		{
			var request = HttpContext.Current.Request;
			var appUrl = HttpRuntime.AppDomainAppVirtualPath;

			if (!string.IsNullOrWhiteSpace(appUrl)) appUrl += "/";

			var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

			return baseUrl;
		}
		
	}
}