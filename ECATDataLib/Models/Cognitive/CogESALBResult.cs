using TypeLite;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Data.Models.Designer;

namespace Ecat.Data.Models.Cognitive
{
    //used to create the custom typing file
    [TsClass(Module = "ecat.entity.s.cog")]
    public class CogEsalbResult
    {
        public int PersonId { get;  set;}
        public int InstrumentId { get; set; }
        public int Attempt { get; set; }
        public double LaissezFaire { get; set; }
        public double Contingent { get; set; }
        public double Management { get; set; }
        public double Idealized { get; set; }
        public double Individual { get; set; }
        public double Inspirational { get; set; }
        public double IntellectualStim { get; set; }

        public Person Person { get; set; }
        public CogInstrument Instrument { get; set; }
    }
}
