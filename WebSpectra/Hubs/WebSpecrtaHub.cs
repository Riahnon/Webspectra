using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebSpectra.Decoders;

namespace WebSpectra.Hubs
{
    public class WebSpecrtaHub : Hub
    {
        static object mLocker = new object();
        static IDecoder mDecoder = null;
        public WebSpecrtaHub()
        {
            lock (mLocker)
            {
                if (mDecoder == null)
                {
                    Task.Factory.StartNew(_ScanDecoderTask);
                }
            }
        }

        private async void _ScanDecoderTask()
        {
            bool lExit = false;
            while (!lExit) //Until exit is set to true
            {
                lock (mLocker) //Exclusive access to the decoder
                {
                    //Still need to find a decoder
                    if (mDecoder == null)
                    {
                        //Attempt to fetch a decoder is done
                        var lDecoder = DecoderManager.GetAvailableDecoders().FirstOrDefault(aDecoder => aDecoder.Start());
                        //If a decoder is found
                        if (lDecoder != null)
                        {
                            //Decoder is set
                            mDecoder = lDecoder;
                            lDecoder.TextDataReceived += _OnDecoderText;
                            lDecoder.FFTDataReceived += _OnDecoderFFT;
                            lDecoder.PropertyChanged += _OnDecoderPropertyChanged;
                            //Exit task flag is set
                            lExit = true;
                        }
                    }//Decoder has been found, so we're done
                    else
                    {
                        lExit = true;
                    }
                }
                //We are gonna loop, so add a wait
                if (!lExit)
                    await Task.Delay(20);
            }

        }

        public void GetCurrentModeName()
        {
            lock (mLocker)
            {
                IMode lMode = null;
                lock (mLocker)
                {
                    lMode = mDecoder.CurrentMode;
                }
                if (lMode != null)
                {
                    Clients.Caller.setCurrentMode(lMode.Name);
                }
            }
        }

        public void SetCurrentMode(string aModeName)
        {
            lock (mLocker)
            {
                if (mDecoder == null)
                    return;

                var lMode = mDecoder.SupportedModes.FirstOrDefault(aMode => aMode.Name == aModeName);
                if (lMode == null)
                    return;
                else
                    mDecoder.CurrentMode = lMode;
            }
        }

        public void GetSupportedModes()
        {
            var lSupportedModes = _GetSupportedModes();
            if (lSupportedModes != null)
                Clients.Caller.setSupportedModes(lSupportedModes);
        }



        private void _OnDecoderText(string aTextData, bool aIsErrorIndication)
        {
            Clients.All.updateText(aTextData);
        }

        private void _OnDecoderFFT(double[] aData)
        {
            Clients.All.updateFFT(aData);
        }

        private void _OnDecoderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Confidence":
                    Clients.All.updateConfidence(mDecoder.Confidence);
                    break;
                case "IsStarted":
                    if (!mDecoder.IsStarted)
                    {
                        mDecoder.TextDataReceived -= _OnDecoderText;
                        mDecoder.FFTDataReceived -= _OnDecoderFFT;
                        mDecoder.PropertyChanged -= _OnDecoderPropertyChanged;
                        mDecoder = null;
                        Task.Factory.StartNew(_ScanDecoderTask);
                    }
                    break;
                case "CurrentMode":
                    {
                        IMode lMode = null;
                        lock (mLocker)
                        {
                            lMode = mDecoder.CurrentMode;
                        }
                        if (lMode != null)
                        {
                            Clients.All.setCurrentMode(lMode.Name);
                        }
                    }
                    break;
                case "SupportedModes":
                    {
                        var lSupportedModes = _GetSupportedModes();
                        if (lSupportedModes != null)
                            Clients.All.setSupportedModes(lSupportedModes);
                    }
                    break;
            }
        }

        private object _GetSupportedModes()
        {
            object lSupportedModes = null;
            lock (mLocker)
            {
                if (mDecoder == null)
                    return null;

                lSupportedModes = mDecoder.SupportedModes.Select(aMode =>
                new
                {
                    name = aMode.Name,
                    parameters = _GetModeParameters(aMode)
                }).ToArray();
            }
            return lSupportedModes;
        }

        private object _GetModeParameters(IMode aMode)
        {
            var lModeParameters = aMode.Parameters.Select(aParam =>
            {
                var lParamData = new Dictionary<string, object>();
                lParamData["name"] = aParam.Name;
                lParamData["value"] = aParam.Value;
                lParamData["type"] = "text";
                if (aParam is IRangedParameter)
                {
                    var lParam = (IRangedParameter)aParam;
                    lParamData["min"] = lParam.Min;
                    lParamData["min"] = lParam.Max;
                    lParamData["type"] = "range";
                }
                if (aParam is ISelectionParameter)
                {
                    var lParam = (ISelectionParameter)aParam;
                    lParamData["validvalues"] = lParam.ValidValues.ToArray();
                    lParamData["type"] = "select";
                }
                if (aParam is INamedSelectionParameter)
                {
                    var lParam = (INamedSelectionParameter)aParam;
                    lParamData["validnamedvalues"] = lParam.ValidNamedValues.ToArray();
                    lParamData["type"] = "namedselect";
                }
                return lParamData;
            }).ToArray();
            return lModeParameters;
        }
    }
}
