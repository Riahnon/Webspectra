using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WebSpectra.Decoders.RandomTest
{
	internal abstract class RandomParameterBase : IParameter
	{
		public event PropertyChangedEventHandler PropertyChanged;
		Dictionary<string, object> m_tMetaDatas = new Dictionary<string, object>();

        static object sInstanceCnt = (int)0;
        int m_nInstanceId;

        public RandomParameterBase()
        {
            lock (sInstanceCnt)
            {
                int lCnt = (int)sInstanceCnt;
                m_nInstanceId = lCnt;
                sInstanceCnt = (int)lCnt + 1;
            }
            this.Name = "RandomParameter" + m_nInstanceId;
        }


		public string Name
		{
			get;
			private set;
		}

		public abstract object Value
		{
			get;
			set;
		}

		protected void _NotifyPropertyChanged(string aPropertyName)
		{
			var lHandler = PropertyChanged;
			if (lHandler != null)
				lHandler(this, new PropertyChangedEventArgs(aPropertyName));
		}

		public override string ToString()
		{
			return this.Name + " = " + this.Value;
		}
	}
}
