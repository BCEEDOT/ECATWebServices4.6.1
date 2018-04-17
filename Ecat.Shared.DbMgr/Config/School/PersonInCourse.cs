using System.Data.Entity.ModelConfiguration;
using Ecat.Shared.Core.ModelLibrary.School;

namespace Ecat.Shared.DbMgr.Config
{
    //public class ConfigPersonInCourse : EntityTypeConfiguration<PersonInCourse>
    //{
    //    public ConfigPersonInCourse()
    //    {
    //        HasKey(p => new { p.PersonId, p.CourseId });

    //        HasRequired(p => p.Course)
    //            .WithMany(p => p.CourseMembers)
    //            .WillCascadeOnDelete(false);
    //    }
    //}

    public class ConfigStudentInCourse : EntityTypeConfiguration<StudentInCourse>
    {
        public ConfigStudentInCourse()
        {
            HasKey(p => new { p.StudentPersonId, p.CourseId });

            HasRequired(p => p.Student)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.StudentPersonId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Course)
                .WithMany(p => p.Students)
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false);

            Ignore(p => p.KcResponses);
        }
    }

    public class ConfigFacultyInCourse : EntityTypeConfiguration<FacultyInCourse>
    {
        public ConfigFacultyInCourse()
        {
            HasKey(p => new { p.FacultyPersonId, p.CourseId });

            Ignore(p => p.FlaggedSpComments);

            //HasMany(p => p.FlaggedSpComments)
            //    .WithOptional(p => p.FlaggedByFaculty)
            //    .HasForeignKey(p => new {p.FlaggedByFacultyId, p.CourseId})
            //    .WillCascadeOnDelete(false);


            HasRequired(p => p.FacultyProfile)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.FacultyPersonId)
                .WillCascadeOnDelete(false);

            HasRequired(p => p.Course)
                .WithMany(p => p.Faculty)
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false);
        }
    }
}
