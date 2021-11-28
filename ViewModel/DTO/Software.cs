using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKHiLoader.DTO
{
    public class Software: IDisposable
    {
        public string File { get; set; }

        public string Name { get; set; }

        public string Developer { get; set; }

        public string Screenshot { get; set; }

        public string Information { get; set; }

        public string Summary { get; set; }

        public string WavFile { get; set; }

        public void Dispose()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(WavFile) && System.IO.File.Exists(WavFile))
                {
                    System.IO.File.Delete(WavFile);
                }
            }
            catch { }
        }
    }
}
