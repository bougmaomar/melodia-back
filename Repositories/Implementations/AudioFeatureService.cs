using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using melodia_api.Repositories;
using NAudio.Wave;

namespace melodia_api.Services.Implementations
{
    public class AudioFeatureExtractor : IAudioFeatureExtractor
    {
        public Task<float[]> ExtractVectorAsync(string filePath)
        {
            var features = ExtractFeatures(filePath);
            var vector = features.Values.Select(v => (float)v).ToArray();
            return Task.FromResult(vector);
        }

        public static Dictionary<string, double> ExtractFeatures(string filePath)
        {
            using var reader = new AudioFileReader(filePath);
            int sampleRate = reader.WaveFormat.SampleRate;
            int channels = reader.WaveFormat.Channels;
            var samples = new float[reader.Length / sizeof(float)];
            int samplesRead = reader.Read(samples, 0, samples.Length);

            int frameLength = 2048;
            int hopLength = 512;

            var rmsValues = new List<double>();
            var zcrValues = new List<double>();
            var centroids = new List<double>();
            var bandwidths = new List<double>();
            var rolloffs = new List<double>();
            var chromaMatrix = new List<double[]>();
            var harmonicEnergies = new List<double>();
            var percussiveEnergies = new List<double>();
            var onsetFrames = new List<int>();

            for (int start = 0; start + frameLength <= samplesRead; start += hopLength)
            {
                double sumSquares = 0.0;
                int zeroCrossings = 0;
                var frame = new Complex[NextPowerOfTwo(frameLength)];

                for (int i = 0; i < frameLength; i++)
                {
                    float windowed = samples[start + i] * (float)HannWindow(i, frameLength);
                    frame[i] = new Complex(windowed, 0);
                    sumSquares += windowed * windowed;
                    if (i < frameLength - 1 && Math.Sign(samples[start + i]) != Math.Sign(samples[start + i + 1]))
                        zeroCrossings++;
                }

                FourierTransform(frame);
                var spectrum = frame.Take(frame.Length / 2).ToArray();

                rmsValues.Add(Math.Sqrt(sumSquares / frameLength));
                zcrValues.Add((double)zeroCrossings / frameLength);
                centroids.Add(SpectralCentroid(spectrum, sampleRate));
                bandwidths.Add(SpectralBandwidth(spectrum, centroids.Last(), sampleRate));
                rolloffs.Add(SpectralRolloff(spectrum, 0.85, sampleRate));
                chromaMatrix.Add(ComputeChroma(spectrum, sampleRate));

                double[] magnitudes = spectrum.Select(c => c.Magnitude).ToArray();
                double harmonicEnergy = magnitudes.Average();
                double percussiveEnergy = magnitudes.Select(v => Math.Abs(v - harmonicEnergy)).Average();
                harmonicEnergies.Add(harmonicEnergy);
                percussiveEnergies.Add(percussiveEnergy);

                if (rmsValues.Count >= 2)
                {
                    double diff = rmsValues[^1] - rmsValues[^2];
                    if (diff > 0.05)
                        onsetFrames.Add(start);
                }
            }

            double[] chromaMeanVec = new double[12];
            double[] chromaVarVec = new double[12];
            for (int i = 0; i < 12; i++)
            {
                var chromaValues = chromaMatrix.Select(row => row[i]).ToArray();
                chromaMeanVec[i] = chromaValues.Average();
                chromaVarVec[i] = Variance(chromaValues.ToList());
            }

            double chromaMean = chromaMeanVec.Average();
            double chromaVar = chromaVarVec.Average();
            double harmonicMean = harmonicEnergies.Average();
            double harmonicVar = Variance(harmonicEnergies);
            double percussiveMean = percussiveEnergies.Average();
            double percussiveVar = Variance(percussiveEnergies);
            double durationSeconds = samplesRead / (double)sampleRate;
            double tempo = (onsetFrames.Count / durationSeconds) * 60.0;

            var mfccMatrix = new List<double[]>();
            for (int start = 0; start + frameLength <= samplesRead; start += hopLength)
            {
                var frame = new Complex[NextPowerOfTwo(frameLength)];
                for (int i = 0; i < frameLength; i++)
                {
                    float windowed = samples[start + i] * (float)HannWindow(i, frameLength);
                    frame[i] = new Complex(windowed, 0);
                }

                FourierTransform(frame);
                var powerSpectrum = frame.Take(frame.Length / 2).Select(c => c.Magnitude * c.Magnitude).ToArray();
                var logSpectrum = powerSpectrum.Select(p => Math.Log(p + 1e-10)).ToArray();
                var mfcc = DCT(logSpectrum, 20);
                mfccMatrix.Add(mfcc);
            }

            var mfccMeans = new double[20];
            var mfccVars = new double[20];
            for (int i = 0; i < 20; i++)
            {
                var values = mfccMatrix.Select(m => m[i]).ToList();
                mfccMeans[i] = values.Average();
                mfccVars[i] = Variance(values);
            }

            var features = new Dictionary<string, double>
            {
                ["chroma_mean"] = chromaMean,
                ["chroma_var"] = chromaVar,
                ["rms_mean"] = rmsValues.Average(),
                ["rms_var"] = Variance(rmsValues),
                ["centroid_mean"] = centroids.Average(),
                ["centroid_var"] = Variance(centroids),
                ["bandwidth_mean"] = bandwidths.Average(),
                ["bandwidth_var"] = Variance(bandwidths),
                ["rolloff_mean"] = rolloffs.Average(),
                ["rolloff_var"] = Variance(rolloffs),
                ["zcr_mean"] = zcrValues.Average(),
                ["zcr_var"] = Variance(zcrValues),
                ["harmonic_mean"] = harmonicMean,
                ["harmonic_var"] = harmonicVar,
                ["percussive_mean"] = percussiveMean,
                ["percussive_var"] = percussiveVar,
                ["tempo"] = tempo
            };

            for (int i = 0; i < 20; i++)
            {
                features[$"mfcc_{i}_mean"] = mfccMeans[i];
                features[$"mfcc_{i}_var"] = mfccVars[i];
            }

            return features;
        }

        private static double[] ComputeChroma(Complex[] spectrum, int sampleRate)
        {
            double[] chroma = new double[12];
            for (int i = 1; i < spectrum.Length; i++)
            {
                double freq = i * sampleRate / (2.0 * spectrum.Length);
                if (freq < 20 || freq > 5000) continue;
                int midiNote = (int)Math.Round(69 + 12 * Math.Log2(freq / 440.0));
                int chromaIndex = ((midiNote % 12) + 12) % 12;
                chroma[chromaIndex] += spectrum[i].Magnitude;
            }
            return chroma;
        }

        private static double HannWindow(int n, int N) => 0.5 * (1 - Math.Cos(2 * Math.PI * n / (N - 1)));

        private static double Variance(List<double> values)
        {
            var mean = values.Average();
            return values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
        }

        private static int NextPowerOfTwo(int x) => (int)Math.Pow(2, Math.Ceiling(Math.Log(x) / Math.Log(2)));

        private static void FourierTransform(Complex[] buffer)
        {
            int n = buffer.Length;
            int bits = (int)Math.Log(n, 2);

            for (int j = 1; j < n / 2; j++)
            {
                int swapPos = BitReverse(j, bits);
                var temp = buffer[j];
                buffer[j] = buffer[swapPos];
                buffer[swapPos] = temp;
            }

            for (int N = 2; N <= n; N <<= 1)
            {
                for (int i = 0; i < n; i += N)
                {
                    for (int k = 0; k < N / 2; k++)
                    {
                        int evenIndex = i + k;
                        int oddIndex = i + k + N / 2;
                        var even = buffer[evenIndex];
                        var odd = buffer[oddIndex];
                        double term = -2 * Math.PI * k / N;
                        var exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;
                        buffer[evenIndex] = even + exp;
                        buffer[oddIndex] = even - exp;
                    }
                }
            }
        }

        private static int BitReverse(int x, int bits)
        {
            int y = 0;
            for (int i = 0; i < bits; i++)
            {
                y <<= 1;
                y |= x & 1;
                x >>= 1;
            }
            return y;
        }

        private static double SpectralCentroid(Complex[] spectrum, int sampleRate)
        {
            double weightedSum = 0.0, sum = 0.0;
            for (int i = 0; i < spectrum.Length; i++)
            {
                double mag = spectrum[i].Magnitude;
                weightedSum += i * mag;
                sum += mag;
            }
            return sum == 0 ? 0.0 : (weightedSum / sum) * sampleRate / (2 * spectrum.Length);
        }

        private static double SpectralBandwidth(Complex[] spectrum, double centroid, int sampleRate)
        {
            double sum = 0.0;
            for (int i = 0; i < spectrum.Length; i++)
            {
                double freq = i * sampleRate / (2.0 * spectrum.Length);
                double diff = freq - centroid;
                double mag = spectrum[i].Magnitude;
                sum += mag * diff * diff;
            }
            return Math.Sqrt(sum / spectrum.Length);
        }

        private static double[] DCT(double[] input, int numCoefficients)
        {
            int N = input.Length;
            var result = new double[numCoefficients];
            for (int k = 0; k < numCoefficients; k++)
            {
                double sum = 0;
                for (int n = 0; n < N; n++)
                {
                    sum += input[n] * Math.Cos(Math.PI * k * (2 * n + 1) / (2.0 * N));
                }
                result[k] = sum * Math.Sqrt(2.0 / N);
            }
            return result;
        }

        private static double SpectralRolloff(Complex[] spectrum, double rolloffPercent, int sampleRate)
        {
            double totalEnergy = spectrum.Sum(x => x.Magnitude);
            if (totalEnergy == 0) return 0.0;

            double threshold = rolloffPercent * totalEnergy;
            double cumulative = 0.0;
            for (int i = 0; i < spectrum.Length; i++)
            {
                cumulative += spectrum[i].Magnitude;
                if (cumulative >= threshold)
                    return i * sampleRate / (2.0 * spectrum.Length);
            }
            return 0.0;
        }
    }
}
