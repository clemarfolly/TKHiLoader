using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKHiLoader.DTO
{
    public class Option
    {
        public int UseAlternativeLoader { get; set; }
        public int InvertWavPolarity { get; set; }
        public int UseDefault300bps { get; set; }
        public string InitialWaitingTime { get; set; }

        public string L10n { get; set; }
    }
}
