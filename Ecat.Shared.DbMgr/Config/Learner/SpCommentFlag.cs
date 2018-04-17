using System.Data.Entity.ModelConfiguration;using Ecat.Shared.Core.ModelLibrary.Learner;namespace Ecat.Shared.DbMgr.Config
{
    public class ConfigSpCommentFlag : EntityTypeConfiguration<StudSpCommentFlag>
    {
        public ConfigSpCommentFlag()
        {

            HasKey(p => new
            {
                p.AuthorPersonId,
                p.RecipientPersonId,
                p.CourseId,
                p.WorkGroupId
            });

            Ignore(p => p.FlaggedByFaculty);

            HasRequired(p => p.Comment)
                .WithOptional(p => p.Flag);
        }
    }
}
