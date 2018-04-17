using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.DbMgr.BbWs.BbCourse;
using Newtonsoft.Json.Linq;

namespace Ecat.FacMod.Core
{
    public interface IFacRepo
    {
        string Metadata { get; }
        SaveResult ClientSaveChanges(JObject saveBundle, Person loggedInUser);
        IQueryable<Course> GetCourses { get; }
        IQueryable<WorkGroup> GetCourseWorkGroups { get; }
        IQueryable<SpInstrument> GetSpInstrument { get; } 
        void AddCourseWorkgroups(Course course);
        IQueryable<CommentCount> AuthorCommentCounts(List<int> authorIds, int workGroupId);
        //List<int> CanWgPublish(List<int> wgIds);
        IQueryable<StudSpComment> WgComments { get; }
        IQueryable<Course> CourseMembers(int courseId);
        //Task<List<CourseVO>> BbCourses(string academyCatId);
        Task<Person> LoadFacultyProfile(Person faculty);
    }
}
