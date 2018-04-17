using System.Data.Entity.ModelConfiguration;
using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.FacMod.Core
{
    internal class FacConfigStudSpInstrument : EntityTypeConfiguration<SpInstrument>
    {
        public FacConfigStudSpInstrument()
        {
            Ignore(p => p.Version);
            Ignore(p => p.StudentInstructions);
            HasMany(p => p.AssignedGroups)
                .WithOptional(p => p.AssignedSpInstr)
                .HasForeignKey(p => p.AssignedSpInstrId);
        }
    }

    internal class FacConfigStudSpInventory : EntityTypeConfiguration<SpInventory>
    {
        public FacConfigStudSpInventory()
        {
            Ignore(p => p.IsScored);
        }
    }

    internal class FacConfigProfileStudent : EntityTypeConfiguration<ProfileStudent>
    {
        public FacConfigProfileStudent()
        {

            HasKey(p => p.PersonId)
                .HasRequired(p => p.Person)
                .WithOptional(p => p.Student); 
        }
    }

    internal class FacConfigProfileFaculty : EntityTypeConfiguration<ProfileFaculty>
    {
        public FacConfigProfileFaculty()
        {
            HasKey(p => p.PersonId)
                .HasRequired(p => p.Person)
                .WithOptional(p => p.Faculty);
        }
    }

    internal class FacConfigPerson : EntityTypeConfiguration<Person>
    {
        public FacConfigPerson()
        {
            Ignore(p => p.BbUserId);
            HasKey(p => p.PersonId);

            HasOptional(p => p.Student)
                .WithRequired()
               .WillCascadeOnDelete(false);

            HasOptional(p => p.Faculty)
                .WithRequired()
                .WillCascadeOnDelete(false);
        }
    }

    internal class FacConfigStudWrkGrp : EntityTypeConfiguration<WorkGroup>
    {
        public FacConfigStudWrkGrp()
        {
            Ignore(p => p.BbGroupId);
            Ignore(p => p.CanPublish);
        }
    }

    internal class FacConfigStudCrse : EntityTypeConfiguration<Course>
    {
        public FacConfigStudCrse()
        {
            Ignore(p => p.BbCourseId);
        }
    }
}
