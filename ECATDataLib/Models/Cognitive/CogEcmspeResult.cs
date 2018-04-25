using TypeLite;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Data.Models.Designer;

namespace Ecat.Data.Models.Cognitive
{
    [TsClass(Module = "ecat.entity.s.cog")]
    public class CogEcmspeResult
    {
        public int PersonId { get;  set;}
        public int InstrumentId { get; set; }
        public int Attempt { get; set; }
        public double Accommodate { get; set; }
        public double Avoid { get; set; }
        public double Collaborate { get; set; }
        public double Compete { get; set; }
        public double Compromise { get; set; }

        public Person Person { get; set; }
        public CogInstrument Instrument { get; set; }

    }
}
