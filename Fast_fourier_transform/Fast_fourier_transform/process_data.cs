using System;
using System.Collections.Generic;
using System.Linq;

namespace Fast_fourier_transform
{
    public class ProcessData
    {
        private List<ComplexNumber> inptSample = new List<ComplexNumber>();
        private List<double> outptFreq = new List<double>();
        private List<ComplexNumber> outptSignal = new List<ComplexNumber>();

        public List<double> FreqOutput => outptFreq;
        public List<ComplexNumber> FftOutput => outptSignal;

        public ProcessData(List<double> time, List<double> amplitude)
        {
            int i, sampleSize, sampleCount;
            sampleCount = amplitude.Count;
            sampleSize = amplitude.Count;

            for (i = 0; i < sampleCount; i++)
            {
                inptSample.Add(new ComplexNumber(amplitude[i], 0));
            }

            while (!IsPowerOfTwo(sampleCount))
            {
                sampleCount++;
                inptSample.Add(new ComplexNumber(0, 0));
            }

            int fftOutputLength;
            List<ComplexNumber> outptSignalShift = FFT(inptSample);
            fftOutputLength = outptSignalShift.Count;

            outptSignal = new List<ComplexNumber>();

            for (i = fftOutputLength / 2; i < fftOutputLength; i++)
            {
                outptSignal.Add(outptSignalShift[i]);
            }

            for (i = 0; i < fftOutputLength / 2; i++)
            {
                outptSignal.Add(outptSignalShift[i]);
            }

            double sampleTime = time[time.Count - 1] - time[0];
            double frequencyResolution = ((double)sampleSize / (double)(sampleTime * fftOutputLength));
            outptFreq = new List<double>();

            for (i = -(fftOutputLength / 2); i < (fftOutputLength / 2) + 1; i++)
            {
                outptFreq.Add(i * frequencyResolution);
            }
        }

        public static List<ComplexNumber> FFT(List<ComplexNumber> inptSignal)
        {
            int i;
            int N = inptSignal.Count;
            if (N == 1)
                return inptSignal;

            List<ComplexNumber> evenList = new List<ComplexNumber>();
            for (i = 0; i < N / 2; i++)
            {
                evenList.Add(inptSignal[2 * i]);
            }
            evenList = FFT(evenList);

            List<ComplexNumber> oddList = new List<ComplexNumber>();
            for (i = 0; i < N / 2; i++)
            {
                oddList.Add(inptSignal[(2 * i) + 1]);
            }
            oddList = FFT(oddList);

            ComplexNumber[] result = new ComplexNumber[N];

            for (i = 0; i < N / 2; i++)
            {
                double w = (-2.0 * i * Math.PI) / N;
                ComplexNumber wk = new ComplexNumber(Math.Cos(w), Math.Sin(w));
                ComplexNumber even = evenList[i];
                ComplexNumber odd = oddList[i];

                result[i] = even + (wk * odd);
                result[i + N / 2] = even - (wk * odd);
            }
            return result.ToList();
        }

        public static bool IsPowerOfTwo(int n)
        {
            return (n != 0) && (n & (n - 1)) == 0;
        }
    }
}
