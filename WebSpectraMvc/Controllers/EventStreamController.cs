using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Http;

namespace WebSpectraMvc.Controllers
{
	public class EventStreamController : ApiController
	{
		/// <summary>
		/// Prepare the response headers for the EventStream
		/// </summary>
		/// <param name="request">The request from which the response will be generated</param>
		/// <returns>Returns <see cref="HttpResponseMessage"/> whose content will be changed by <see cref="MvcApplication.Pub"/></returns>

		public HttpResponseMessage Get(HttpRequestMessage request)
		{
			HttpResponseMessage response = request.CreateResponse();

			response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");

			response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
			response.Headers.CacheControl.NoCache = true;

			return response;
		}
		/// <summary>
		/// Registers the connection as a client to <see cref="MvcApplication.Pub"/>
		/// </summary>
		/// <param name="stream">The stream from which the StreamWriter will be created</param>
		/// <param name="content"></param>
		/// <param name="context"></param>
		/// <remarks>This is a callback for <see cref="PushStreamContent"/></remarks>
		private void OnStreamAvailable(Stream stream, HttpContent content, TransportContext context)
		{
			StreamWriter streamWriter = new StreamWriter(stream);
			MvcApplication.HostContainer.Add(streamWriter);
			
		}
	}
}
