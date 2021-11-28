using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TKHiLoader.Helper
{
    public static class WavHelper
    {
        private static float[] samplesZero = { 0.2421875f, 0.5703125f, 0.4609375f, 0.2421875f, -0.421875f, -0.640625f, -0.421875f, -0.203125f };
        private static float[] samplesUm = { 0.3515625f, 0.5703125f, 0.5703125f, 0.4609375f, 0.2421875f, 0.125f, 0.015625f, 0.015625f, -0.09375f, -0.203125f, -0.53125f, -0.7578125f, -0.7578125f, -0.53125f, -0.3125f };

        public static void GerarWav(string arquivoP, string destinoWav, bool useAlternativeLoader)
        {
            var ms = new MemoryStream();

            for (int i = 0; i < 4096 / 8; i++)
            {
                if (i == (4096 / 8) - 1)
                {
                    ms.WriteByte(1);
                }
                else
                {
                    ms.WriteByte(0);
                }
            }

            ms.WriteByte(0x43);

            var buff = File.ReadAllBytes(arquivoP);

            buff[0x27] = 0x87;

            ms.WriteByte(buff[11]);
            ms.WriteByte(buff[12]);

            byte checksum = 0;
            for (int i = 0; i < buff.Length; i++)
            {
                checksum += buff[i];
                ms.WriteByte(buff[i]);
            }
            ms.WriteByte(checksum);
            ms.WriteByte(checksum);

            ms.Position = 0;

            var waveFormat = new WaveFormat(44100, 16, 1);

            var fullBuf = ms.ToArray();

            using (WaveFileWriter writer = new WaveFileWriter(destinoWav, waveFormat))
            {
                if (useAlternativeLoader)
                {
                    using (WaveFileReader reader = new WaveFileReader(GetHiLoaderWav()))
                    {
                        byte[] buffer = new byte[1024];
                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, read);
                        }
                    }
                }

                for (int i = 0; i < waveFormat.SampleRate; i++)
                {
                    writer.WriteSample(0f);
                }

                for (int i = 0; i < samplesZero.Length; i++)
                {
                    writer.WriteSample((float)(samplesZero[i]));
                }
                for (int i = 0; i < samplesZero.Length; i++)
                {
                    writer.WriteSample((float)(samplesZero[i]));
                }

                foreach (var b in fullBuf)
                {
                    foreach (var bit in ToBinary(b))
                    {
                        if (bit == '0')
                        {
                            for (int i = 0; i < samplesZero.Length; i++)
                            {
                                writer.WriteSample((float)(samplesZero[i]));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < samplesUm.Length; i++)
                            {
                                writer.WriteSample((float)(samplesUm[i]));
                            }
                        }
                    }
                }
            }
            ms.Dispose();
        }

        private static string ToBinary(int n)
        {
            return Convert.ToString(n, 2).PadLeft(8, '0');
        }

        private static Stream GetHiLoaderWav()
        {
            var fileName = "TKHiLoader.Resource.HILOAD.WAV";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(fileName);

            return stream;
        }
    }
}