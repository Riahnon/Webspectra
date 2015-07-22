using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSpectra.Decoders;

namespace WebSpectra.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View(MvcApplication.Decoder);
		}

		[HttpGet]
		public ActionResult GetCurrentModeName()
		{
			if (MvcApplication.Decoder.CurrentMode != null)
				return Content(MvcApplication.Decoder.CurrentMode.Name);

			return new EmptyResult();
		}

		[HttpGet]
		public ActionResult GetCurrentModeParams()
		{
			if (MvcApplication.Decoder.CurrentMode != null)
				return PartialView("_ModeParameters", MvcApplication.Decoder.CurrentMode);

			return new EmptyResult();
		}

		[HttpGet]
		public ActionResult GetCurrentModeParam(string paramname = null)
		{
			if (MvcApplication.Decoder.CurrentMode != null)
			{
				var lParam = MvcApplication.Decoder.CurrentMode[paramname];
				if (lParam != null)
					return PartialView("_ModeParameter", lParam);
			}

			return new EmptyResult();
		}

		[HttpPost] //post back - get selected ddl value and refresh
		public ActionResult SetMode(string mode = null)
		{
			string lModeName = mode;
			var lNewMode = MvcApplication.Decoder.SupportedModes.FirstOrDefault(aMode => aMode.Name == lModeName);
			if (lNewMode != null)
				MvcApplication.Decoder.CurrentMode = lNewMode;

			return new EmptyResult();
		}

		[HttpPost]
		public ActionResult SetParamValue(string param, string value)
		{
			var lParam = MvcApplication.Decoder.CurrentMode.Parameters.FirstOrDefault(aParam => aParam.Name == param);
			if (lParam != null)
			{
				var lValue = lParam.RawValue;
				try
				{
					lValue = Convert.ChangeType(value, lParam.RawValue.GetType());
					lParam.RawValue = lValue;
				}
				catch { }
				//Ajax response if the parameter seems not to be set properly
				if (Request.IsAjaxRequest() && value != lParam.RawValue.ToString())
				{
					return PartialView("_ModeParameter", lParam);
				}
			}
			return new EmptyResult();
		}

		[HttpPost]
		public ActionResult SetFlagParamValue(string param, int value)
		{
			var lParam = MvcApplication.Decoder.CurrentMode.Parameters.FirstOrDefault(aParam => aParam.Name == param);
			if (lParam != null)
			{
				var lValue = lParam.RawValue;
				try
				{
					lValue = Enum.ToObject(lParam.RawValue.GetType(), value);
					lParam.RawValue = lValue;
				}
				catch { }
				//Ajax response if the parameter seems not to be set properly
				if (Request.IsAjaxRequest() && !lValue.Equals(lParam.RawValue))
				{
					return PartialView("_ModeParameter", lParam);
				}
			}
			return new EmptyResult();
		}
	
	}
}
