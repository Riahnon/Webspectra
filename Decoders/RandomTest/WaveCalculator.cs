using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpectra.Decoders.RandomTest
{
    public enum WaveCalculatorFunc { Sin, Cos };
    public class WaveCalculator
    {
        Func<double,double> mCalcFunc;
        public WaveCalculator(WaveCalculatorFunc aFunc, double aFrequency = 1, int aPointCount = 1000)
        {
            this.Func = aFunc;
            if (this.Func == WaveCalculatorFunc.Sin)
                mCalcFunc = new Func<double, double>(Math.Sin);
            else
                mCalcFunc = new Func<double, double>(Math.Cos);
            this.Frequency = aFrequency;
            mPointCount = aPointCount;
            Values = Enumerable.Repeat<double>(0, mPointCount).ToArray();
        }

        public WaveCalculatorFunc Func
        {
            get;
            private set;
        }

        public double[] Values { get; private set; }

        public double Frequency { get; set; }

        int mPointCount;
        public int PointCount
        {
            get { return mPointCount; }
            set
            {
                if (mPointCount == value)
                    return;
                mPointCount = value;
                Values = Enumerable.Repeat<double>(0, mPointCount).ToArray();
            }
        }


        public void Calc(double aElapsedSecs)
        {
            for (int i=0; i < Values.Length; ++i)
            {
                var lNormX = (double)i / (double)(Values.Length - 1);
                Values[i] = mCalcFunc((aElapsedSecs + lNormX) * this.Frequency * Math.PI * 2);
            }
        }
    }
}
