//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
////using Ecat.Shared.Core.Interface;
//using Ecat.Data.Models.Interface;
////using Ecat.Shared.Core.ModelLibrary.Learner;
//using Ecat.Data.Models.Student;
//using TypeLite;

//namespace Ecat.Data.Models.Designer
//{
//    [TsClass(Module = "ecat.entity.s.designer")]
//    public class KcInventory: IInventory<KcInstrument>
//    {
//        public int Id { get; set; }
//        public int? ModifiedById { get; set; }
//        public int InstrumentId { get; set; }
//        public int DisplayOrder { get; set; }
//        public bool IsScored { get; set; }
//        public bool IsDisplayed { get; set; }
//        public string KnowledgeArea { get; set; }
//        public string QuestionText { get; set; }
//        public float ItemWeight { get; set; }
//        public string Answer { get; set; }
//        public DateTime? ModifiedDate { get; set; }

//        public KcInstrument Instrument { get; set; }
//        public ICollection<KcResponse> Responses { get; set; }
//    }
//}
