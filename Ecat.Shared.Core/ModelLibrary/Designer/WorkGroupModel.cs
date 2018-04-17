using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.School;

namespace Ecat.Shared.Core.ModelLibrary.Designer
{
    public class WorkGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? AssignedSpInstrId { get; set; }
        public int? AssignedKcInstrId { get; set; }
        public string MpEdLevel { get; set; }
        public string MpWgCategory { get; set; }
        public decimal  MaxStratStudent { get; set; }
        public decimal MaxStratFaculty { get; set; }
        public bool IsActive { get; set; }
        public int StratDivisor { get; set; }
        public string StudStratCol { get; set; }
        public string FacStratCol { get; set; }

        public SpInstrument AssignedSpInstr { get; set; }
        public KcInstrument AssignedKcInstr { get; set; }
        public ICollection<WorkGroup> WorkGroups { get; set; }
    }
}
