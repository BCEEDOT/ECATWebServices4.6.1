//using System;
//using System.Collections.Generic;
//using System.Data.Entity.ModelConfiguration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Ecat.Shared.Core.ModelLibrary.Designer;

//namespace Ecat.Shared.DbMgr.Config
//{
//    public class ConfigSpAssessMap : EntityTypeConfiguration<SpAssessMap>
//    {
//        public ConfigSpAssessMap()
//        {
//            HasRequired(p => p.SpInstrument)
//                .WithMany(p => p.SpAssessMaps)
//                .HasForeignKey(p => p.SpInstrumentId)
//                .WillCascadeOnDelete(false);
//        }
//    }
//}
