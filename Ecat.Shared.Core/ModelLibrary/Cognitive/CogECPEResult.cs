using TypeLite;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.ModelLibrary.Designer;

namespace Ecat.Shared.Core.ModelLibrary.Cognitive
{
    [TsClass(Module = "ecat.entity.s.cog")]
    public class CogEcpeResult
    {
        public int PersonId { get;  set;}
        public int Attempt { get; set; }
        public int InstrumentId { get; set; }
        public int Outcome { get; set; }

        public Person Person { get; set; }
        public CogInstrument Instrument { get; set; }
    }
}
