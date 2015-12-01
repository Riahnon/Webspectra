using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WebSpectra.Decoders.RandomTest
{
    public class RandomMode : IMode
    {
        const int PARAMETER_COUNT_MIN = 6;
        const int PARAMETER_COUNT_MAX = 16;
        const int VALUESTRING_LENGTH_MIN = 7;
        const int VALUESTRING_LENGTH_MAX = 12;
        const int VALUE_COUNT_MIN = 6;
        const int VALUE_COUNT_MAX = 16;
        public event PropertyChangedEventHandler PropertyChanged;
        static object sInstanceCnt = (int)0;
        int m_nInstanceId;
        public RandomMode(Random aRnd)
        {
            lock (sInstanceCnt)
            {
                int lCnt = (int)sInstanceCnt;
                m_nInstanceId = lCnt;
                sInstanceCnt = (int)lCnt + 1;
            }

            this.Parameters = _GenerateParameters(aRnd);
        }


        public string Name
        {
            get { return "RandomMode" + m_nInstanceId; }
        }

        public string Status
        {
            get { return "ok"; }
        }

        public IEnumerable<IParameter> Parameters
        {
            get;
            private set;
        }

        public IParameter this[string aParamName]
        {
            get
            {
                var lResult = this.Parameters.FirstOrDefault((aParam) => aParam.Name == aParamName);
                return lResult;
            }
        }

        private List<IParameter> _GenerateParameters(Random aRnd)
        {
            var lResult = new List<IParameter>();
            var lParamCount = aRnd.Next(PARAMETER_COUNT_MAX - PARAMETER_COUNT_MIN) + PARAMETER_COUNT_MIN;
            for (int i = 0; i < lParamCount; ++i)
            {
                //Value types: int, float, string, ValueList, NamedValueList
                switch (aRnd.Next(4))
                {
                    case 0:

                        switch (aRnd.Next(3))
                        {
                            case 0:
                                lResult.Add(new RandomParameter<int>(aRnd.Next(1000)));
                                break;
                            case 1:
                                lResult.Add(new RandomParameter<float>(aRnd.Next(1000)));
                                break;
                            case 2:
                                lResult.Add(new RandomParameter<string>(_GenerateRandomString(3 + aRnd.Next(10))));
                                break;
                        }
                        break;

                    case 1:

                        switch (aRnd.Next(2))
                        {
                            case 0:
                                lResult.Add(new RandomRangedParameter<int>(10 + aRnd.Next(1000), 10, 1010));
                                break;
                            case 1:
                                lResult.Add(new RandomRangedParameter<float>(10 + aRnd.Next(1000), 10, 1010));
                                break;
                        }

                        break;
                    case 2:
                        switch (aRnd.Next(3))
                        {
                            case 0:
                                {
                                    var lValidValues = Enumerable.Range(0, 2 + aRnd.Next(10)).Select(aItem => (int)aRnd.Next(1000)).ToArray();
                                    lResult.Add(new RandomSelectionParameter<int>(lValidValues[0], lValidValues));
                                }

                                break;
                            case 1:
                                {
                                    var lValidValues = Enumerable.Range(0, 2+ aRnd.Next(10)).Select(aItem => (float)aRnd.Next(1000)).ToArray();
                                    lResult.Add(new RandomSelectionParameter<float>(lValidValues[0], lValidValues));
                                }
                                break;
                            case 2:
                                {
                                    var lValidValues = Enumerable.Range(0, 2+ aRnd.Next(10)).Select(aItem => _GenerateRandomString(3 + aRnd.Next(10))).ToArray();
                                    lResult.Add(new RandomSelectionParameter<string>(lValidValues[0], lValidValues));
                                }
                                break;
                        }
                        break;
                    case 3:
                        switch (aRnd.Next(3))
                        {
                            case 0:
                                {
                                    var lValidNamedValues = Enumerable.Range(0, 2+ aRnd.Next(10)).Select(aItem => (int)aRnd.Next(1000))
                                        .Select(aValue => new KeyValuePair<string, int>(_GenerateRandomString(3 + aRnd.Next(10)), aValue))
                                        .ToArray();
                                    lResult.Add(new RandomNamedSelectionParameter<int>(lValidNamedValues[0].Value, lValidNamedValues));
                                }

                                break;
                            case 1:
                                {
                                    var lValidNamedValues = Enumerable.Range(0, 2 +aRnd.Next(10)).Select(aItem => (float)aRnd.Next(1000))
                                        .Select(aValue => new KeyValuePair<string, float>(_GenerateRandomString(3 + aRnd.Next(10)), aValue))
                                        .ToArray();
                                    lResult.Add(new RandomNamedSelectionParameter<float>(lValidNamedValues[0].Value, lValidNamedValues));
                                }
                                break;
                            case 2:
                                {
                                    var lValidNamedValues = Enumerable.Range(0, 2 + aRnd.Next(10)).Select(aItem => (float)aRnd.Next(1000))
                                       .Select(aValue => new KeyValuePair<string, string>(_GenerateRandomString(3 + aRnd.Next(10)), _GenerateRandomString(3 + aRnd.Next(10))))
                                       .ToArray();
                                    lResult.Add(new RandomNamedSelectionParameter<string>(lValidNamedValues[0].Value, lValidNamedValues));
                                }
                                break;
                        }
                        break;
                }

            }
            return lResult;
        }

        private static string _GenerateRandomString(int aMaxLength = 10, Random aRnd = null)
        {
            const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            if (aRnd == null)
                aRnd = new Random();

            var lLength = aRnd.Next(aMaxLength + 1);
            var lChars = new char[lLength];

            for (int i = 0; i < lChars.Length; i++)
            {
                lChars[i] = CHARS[aRnd.Next(lChars.Length)];
            }

            return new String(lChars);
        }

        protected void _NotifyPropertyChanged(string aPropertyName)
        {
            var lHandler = PropertyChanged;
            if (lHandler != null)
                lHandler(this, new PropertyChangedEventArgs(aPropertyName));
        }
    }
}
