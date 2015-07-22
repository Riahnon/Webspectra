using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WebSpectra.MetaData;

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
        public RandomMode (Random aRnd)
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
            var lParamCount = aRnd.Next(PARAMETER_COUNT_MAX-PARAMETER_COUNT_MIN) + PARAMETER_COUNT_MIN;
            for (int i = 0; i < lParamCount; ++i)
            {
                //Value types: int, float, string, ValueList, NamedValueList
                switch (aRnd.Next(5))
                {
                    case 0:
                        {
                            lResult.Add(new RandomParameter<int>(aRnd.Next(1000)));
                            break;
                        }
                    case 1:
                        {
                            lResult.Add(new RandomParameter<double>(aRnd.NextDouble() * 5000));
                        }
                        break;
                    case 2:
                        lResult.Add(new RandomParameter<string>(_GenerateRandomString(aRnd.Next(VALUESTRING_LENGTH_MAX - VALUESTRING_LENGTH_MIN) + VALUESTRING_LENGTH_MIN, aRnd)));
                        break;
                    case 3:
                        {
                            var lValidValuesCount = aRnd.Next(VALUE_COUNT_MAX-VALUE_COUNT_MIN) + VALUE_COUNT_MIN;
                            var lValidValues = new List<int>();
                            for( int j=0;j<lValidValuesCount; ++j)
                                lValidValues.Add(aRnd.Next(10000));

                            var lParameter = new RandomParameter<int>(lValidValues[0]);
                            lParameter.SetMetadataRaw(MetadataIDs<int>.VALID_VALUES, lValidValues);
                            lResult.Add(lParameter);
                        }
                        break;
                    case 4:
                        {
                            var lValidValuesCount = aRnd.Next(VALUE_COUNT_MAX - VALUE_COUNT_MIN) + VALUE_COUNT_MIN;
                            var lValidNamedValues = new List<KeyValuePair<string,int>>();
                            for (int j = 0; j < lValidValuesCount; ++j)
                                lValidNamedValues.Add(new KeyValuePair<string,int>("Value" + j.ToString(), aRnd.Next(10000)));

                            var lParameter = new RandomParameter<int>(lValidNamedValues[0].Value);
                            lParameter.SetMetadataRaw(MetadataIDs<int>.VALID_NAMED_VALUES, lValidNamedValues);
                            lResult.Add(lParameter);
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
