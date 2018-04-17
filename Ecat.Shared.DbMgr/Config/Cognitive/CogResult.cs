using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.Cognitive;

namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigCogECMSPEResult : EntityTypeConfiguration<CogEcmspeResult>
    {
        public ConfigCogECMSPEResult()
        {
            HasKey(p => new {

                p.PersonId,
                p.InstrumentId,
                p.Attempt

            });

            HasRequired(p => p.Person)
              .WithMany()
              .HasForeignKey(p => p.PersonId);

            HasRequired(p => p.Instrument)
              .WithMany()
              .HasForeignKey(p => p.InstrumentId);

        }
    }

    public class ConfigCogECPEResult : EntityTypeConfiguration<CogEcpeResult>
    {
        public ConfigCogECPEResult()
        {
            HasKey(p => new {

                p.PersonId,
                p.InstrumentId,
                p.Attempt

            });

            HasRequired(p => p.Person)
              .WithMany()
              .HasForeignKey(p => p.PersonId);

            HasRequired(p => p.Instrument)
              .WithMany()
              .HasForeignKey(p => p.InstrumentId);

        }
    }

    public class ConfigCogESALBResult : EntityTypeConfiguration<CogEsalbResult>
    {
        public ConfigCogESALBResult()
        {
            HasKey(p => new {

                p.PersonId,
                p.InstrumentId,
                p.Attempt

            });

            HasRequired(p => p.Person)
              .WithMany()
              .HasForeignKey(p => p.PersonId);

            HasRequired(p => p.Instrument)
              .WithMany()
              .HasForeignKey(p => p.InstrumentId);

        }
    }

    public class ConfigCogETMPREResult : EntityTypeConfiguration<CogEtmpreResult>
    {
        public ConfigCogETMPREResult()
        {
            HasKey(p => new {

                p.PersonId,
                p.InstrumentId,
                p.Attempt

            });

            HasRequired(p => p.Person)
              .WithMany()
              .HasForeignKey(p => p.PersonId);

            HasRequired(p => p.Instrument)
              .WithMany()
              .HasForeignKey(p => p.InstrumentId);

        }
    }
}
