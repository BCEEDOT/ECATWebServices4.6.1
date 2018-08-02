using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ecat.Business.Business;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Ecat.Data.Models.Canvas;
using Ecat.Data.Models.Common;
using Ecat.Data.Models.Designer;
using Ecat.Data.Models.School;
using FluentAssertions;
using Telerik.JustMock;

namespace Ecat.Business.Business.Tests
{
    public class Name
    {
        public string FirstName;
        public string LastName;
        public string MiddleInitial;
    }

    public class Build
    {
        private int _recIdFaker;
        private int _bbUserId;
        private int _enrollmentIdFaker;
        private int _courseEnrollmentIdFaker;

        public Build()
        {
            _recIdFaker = 1;
            _bbUserId = 1000;
            _enrollmentIdFaker = 1;
            _courseEnrollmentIdFaker = 1000;
        }



        public CourseReconcile CourseReconcile(int numStus, int numFacs, int numCanDeleteStus, int numCanDeleteFac)
        {
            var courseReconcile = Mock.Create<CourseReconcile>();
            //var course = Mock.Create<Course>();
            //var studentReconcile = Mock.Create<UserReconcile>();
            //var facultyReconcile = Mock.Create<UserReconcile>();
            var courseIds = 1;
            var bbCourseIds = 1;

            var ecatCourses = new Faker<Course>()
                .RuleFor(c => c.Name, f =>
                {
                    var name = $"AFSNCOA {courseIds}";
                    return name;

                })
                .RuleFor(c => c.Id, f => courseIds++)
                .RuleFor(c => c.AcademyId, "AFSNCOA")
                .RuleFor(c => c.BbCourseId, f => (bbCourseIds++).ToString())

                .RuleFor(c => c.GradReportPublished, false)
                .RuleFor(c => c.StartDate, f => f.Date.Soon())
                .RuleFor(c => c.GradDate, f => f.Date.Future());

            var course = ecatCourses.Generate();
            courseReconcile.Course = course;

            var studentsToReconcile = new List<UserReconcile>();
            var facultyToReconcile = new List<UserReconcile>();

            if (numStus > 0)
            {
                studentsToReconcile.AddRange(GenerateUserReconcile(numStus, false));
            }

            if (numCanDeleteStus > 0)
            {
                studentsToReconcile.AddRange(GenerateUserReconcile(numCanDeleteStus, true));
            }

            if (numFacs > 0)
            {
                facultyToReconcile.AddRange(GenerateUserReconcile(numFacs, false));
            }

            if (numCanDeleteFac > 0)
            {
                facultyToReconcile.AddRange(GenerateUserReconcile(numCanDeleteFac, true));
            }

            courseReconcile.StudentsToReconcile = studentsToReconcile;
            courseReconcile.FacultyToReconcile = facultyToReconcile;
           
            return courseReconcile;
        }

        public List<CanvasEnrollment> CanvasEnrollments(int numStus, int numInactiveStus, int numFac, int numInactiveFac)
        {
            var canvasEnrollments = new List<CanvasEnrollment>();

            if (numStus > 0)
            {
                canvasEnrollments.AddRange(GenerateCanvasEnrollments(numStus, true, 1, false));
            }

            if (numInactiveStus > 0)
            {
                canvasEnrollments.AddRange(GenerateCanvasEnrollments(numInactiveStus, false, 1, false));
            }

            if (numFac > 0)
            {
                canvasEnrollments.AddRange(GenerateCanvasEnrollments(numFac, true, 1, true));
            }

            if (numInactiveFac > 0)
            {
                canvasEnrollments.AddRange(GenerateCanvasEnrollments(numInactiveFac, false, 1, true));
            }

            return canvasEnrollments;

        }

        private IEnumerable<CanvasEnrollment> GenerateCanvasEnrollments(int num, bool isActive, int courseId, bool isFaculty)
        {

            var canvasEnrollments = new Faker<CanvasEnrollment>()
                .RuleFor(ce => ce.id, f => _courseEnrollmentIdFaker)
                .RuleFor(ce => ce.course_id, courseId)
                .RuleFor(ce => ce.course_section_id, 1)
                .RuleFor(ce => ce.enrollment_state, f => isActive ? "active" : "inactive")
                .RuleFor(ce => ce.user_id, f=> _courseEnrollmentIdFaker)
                .RuleFor(ce => ce.role, f => isFaculty ? "TeacherEnrollment" : "StudentEnrollment")
                .RuleFor(ce => ce.type, f => isFaculty ? "TeacherEnrollment" : "StudentEnrollment")
                .RuleFor(ce => ce.role_id, f => isFaculty ? 72 : 3)
                .RuleFor(ce => ce.user, f =>
                {
                    var name = CreateName();
                    var user = new Faker<CanvasUser>()
                        .RuleFor(u => u.id, uf => _courseEnrollmentIdFaker++)
                        .RuleFor(u => u.name, uf => $"{name.FirstName} {name.MiddleInitial} {name.LastName} ")
                        .RuleFor(u => u.sortable_name, uf => $"{name.LastName}, {name.FirstName} {name.MiddleInitial}")
                        .RuleFor(u => u.short_name, uf => $"{name.LastName}, {name.FirstName} {name.MiddleInitial}")
                        .RuleFor(u => u.login_id, uf => $"{name.FirstName}.{name.MiddleInitial}.{name.LastName}.au");

                    return user;

                });

            return canvasEnrollments.Generate(num);
        }

        private static Name CreateName()
        {
            var faker = new Faker();
            var middleInitial = new[] { "a", "b", "c", "d", "e" };
            var name = new Name
            {
                FirstName = faker.Name.FirstName(),
                MiddleInitial = faker.PickRandom(middleInitial),
                LastName = faker.Name.LastName()
            };

            return name;
        }

        private IEnumerable<UserReconcile> GenerateUserReconcile(int num, bool canDelete)
        {
              
            var studentReconciles = new Faker<UserReconcile>()
                .RuleFor(sr => sr.PersonId, f => _recIdFaker++)
                .RuleFor(sr => sr.BbUserId, f => (_bbUserId++).ToString())
                .RuleFor(sr => sr.CanDelete, canDelete)
                .RuleFor(sr => sr.IsMarkedDeleted, false)
                .RuleFor(sr => sr.NewEnrollment, false)
                .RuleFor(sr => sr.RemoveEnrollment, false)
                .RuleFor(sr => sr.FlagDeleted, false)
                .RuleFor(sr => sr.UnFlagDeleted, false);

            return studentReconciles.Generate(num);
            
        }
    }

    [TestClass()]
    public class CanvasBusinessLogicTests
    {
        [TestMethod()]
        // public static List<Course> ReconcileCourses(List<CanvasCourse> canvasCoursesReturned, List<Course> existingCourses, string academyId)
        public void ReconcileCourses_NewCourseInCanvas_ReturnListWithNewCourse()
        {
            //Arrange

            var canvasCourse = Mock.Create<CanvasCourse>();
            canvasCourse.id = 7016;
            canvasCourse.account_id = 374;
            canvasCourse.start_at = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            canvasCourse.end_at = DateTime.Now.AddDays(12).ToString(CultureInfo.InvariantCulture);
            canvasCourse.name = "Class 17B";
             
            var canvasCourses = new List<CanvasCourse> {(canvasCourse)};

            var existingCourse = Mock.Create<Course>();
            
            existingCourse.AcademyId = "AFSNCOA";
            existingCourse.BbCourseId = "2";
            existingCourse.Id = 2;
            existingCourse.Name = "Class 17A";
            existingCourse.StartDate = DateTime.Now;
            existingCourse.GradDate = DateTime.Now.AddDays(12);

            var existingCourses = new List<Course>() { (existingCourse) };

            //Act
            var courseReconcile = CanvasBusinessLogic.ReconcileCourses(canvasCourses, existingCourses, "1");
            //Assert

            courseReconcile.Count().Should().Be(1);
            courseReconcile[0].BbCourseId.Should().Be("7016");
            courseReconcile[0].Name.Should().Be("Class 17B");

        }

        [TestMethod()]
        // public static List<Course> ReconcileCourses(List<CanvasCourse> canvasCoursesReturned, List<Course> existingCourses, string academyId)
        public void ReconcileCourses_CanvasCoursesSameAsExistingCourses_ReturnEmptyList()
        {
            //Arrange

            var canvasCourse = Mock.Create<CanvasCourse>();
            canvasCourse.id = 2;
            canvasCourse.account_id = 374;
            canvasCourse.start_at = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            canvasCourse.end_at = DateTime.Now.AddDays(12).ToString(CultureInfo.InvariantCulture);
            canvasCourse.name = "Class 17A";

            var canvasCourses = new List<CanvasCourse> { (canvasCourse) };

            var existingCourse = Mock.Create<Course>();
            
            existingCourse.AcademyId = "AFSNCOA";
            existingCourse.BbCourseId = "2";
            existingCourse.Id = 2;
            existingCourse.Name = "Class 17A";
            existingCourse.StartDate = DateTime.Now;
            existingCourse.GradDate = DateTime.Now.AddDays(12);
            var existingCourses = new List<Course>() { (existingCourse) };

            //Act
            var courseReconcile = CanvasBusinessLogic.ReconcileCourses(canvasCourses, existingCourses, "1");
            //Assert

            courseReconcile.Count().Should().Be(0);

        }
        [TestMethod()]
        // public static List<Course> ReconcileCourses(List<CanvasCourse> canvasCoursesReturned, List<Course> existingCourses, string academyId)
        public void ReconcileCourses_MoreExistingCoursesThanCanvasCourses_ReturnEmptyList()
        {
            //Arrange

            var canvasCourse = Mock.Create<CanvasCourse>();
            canvasCourse.id = 7016;
            canvasCourse.account_id = 374;
            canvasCourse.start_at = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            canvasCourse.end_at = DateTime.Now.AddDays(12).ToString(CultureInfo.InvariantCulture);
            canvasCourse.name = "Class 17B";

            var canvasCourses = new List<CanvasCourse> { (canvasCourse) };

            var existingCourse1 = Mock.Create<Course>();
            var existingCourse2 = Mock.Create<Course>();

            existingCourse1.AcademyId = "AFSNCOA";
            existingCourse1.BbCourseId = "2";
            existingCourse1.Id = 2;
            existingCourse1.Name = "Class 17A";
            existingCourse1.StartDate = DateTime.Now;
            existingCourse1.GradDate = DateTime.Now.AddDays(12);

            existingCourse2.AcademyId = "AFSNCOA";
            existingCourse2.BbCourseId = "7016";
            existingCourse2.Id = 7016;
            existingCourse2.Name = "Class 17B";
            existingCourse2.StartDate = DateTime.Now;
            existingCourse2.GradDate = DateTime.Now.AddDays(12);

            var existingCourses = new List<Course>() { existingCourse1, existingCourse2};
            

            //Act
            var courseReconcile = CanvasBusinessLogic.ReconcileCourses(canvasCourses, existingCourses, "1");
            //Assert

            courseReconcile.Count().Should().Be(0);

        }

        //public static CourseReconcile ReconcileCourseMembers(CourseReconcile ecatCourseReconcile, List<CanvasEnrollment> canvasEnrollmentsReturned, int facultyId)
        [TestMethod()]
        public void ReconcileCourseMembers_AddNewMembersToCanvasCourse_ReturnNewEnrollments()
        {
            //Arrange
            
            var build = new Build();
            var courseReconcile = build.CourseReconcile(0, 0, 0, 0);
            var courseEnrollments = build.CanvasEnrollments(2, 2, 2, 2);

            //Act

            courseReconcile = CanvasBusinessLogic.ReconcileCourseMembers(courseReconcile, courseEnrollments, 1);

            //Assert
            var facAdd = courseReconcile.FacultyToReconcile.Where(reconcile => reconcile.NewEnrollment);
            var stuAdd = courseReconcile.StudentsToReconcile.Where(reconcile => reconcile.NewEnrollment);
            var totalAdd = facAdd.Count() + stuAdd.Count();

            var facDelete = courseReconcile.FacultyToReconcile.Where(reconcile => reconcile.RemoveEnrollment);
            var stuDelete = courseReconcile.StudentsToReconcile.Where(reconcile => reconcile.RemoveEnrollment);
            var totalDelete = facDelete.Count() + stuDelete.Count();

            facAdd.Count().Should().Be(2);
            stuAdd.Count().Should().Be(2);
            facDelete.Count().Should().Be(0);
            stuDelete.Count().Should().Be(0);

        }

        //public static CourseReconcile ReconcileCourseMembers(CourseReconcile ecatCourseReconcile, List<CanvasEnrollment> canvasEnrollmentsReturned, int facultyId)
        [TestMethod()]
        public void ReconcileCourseMembers_RemoveMembersFromCanvasCourse_ReturnDeletedEnrollments()
        {
            //Arrange

            var build = new Build();
            var courseReconcile = build.CourseReconcile(2, 2, 2, 2);
            var courseEnrollments = build.CanvasEnrollments(0, 0, 0, 0);

            //Act

            courseReconcile = CanvasBusinessLogic.ReconcileCourseMembers(courseReconcile, courseEnrollments, 1);

            //Assert
            var facAdd = courseReconcile.FacultyToReconcile.Where(reconcile => reconcile.NewEnrollment);
            var stuAdd = courseReconcile.StudentsToReconcile.Where(reconcile => reconcile.NewEnrollment);
            var totalAdd = facAdd.Count() + stuAdd.Count();

            var facDelete = courseReconcile.FacultyToReconcile.Where(reconcile => reconcile.RemoveEnrollment);
            var stuDelete = courseReconcile.StudentsToReconcile.Where(reconcile => reconcile.RemoveEnrollment);
            var totalDelete = facDelete.Count() + stuDelete.Count();

            facAdd.Count().Should().Be(0);
            stuAdd.Count().Should().Be(0);
            facDelete.Count().Should().Be(4);
            stuDelete.Count().Should().Be(4);

        }

        //public static CourseReconcile ReconcileCourseMembers(CourseReconcile ecatCourseReconcile, List<CanvasEnrollment> canvasEnrollmentsReturned, int facultyId)
        [TestMethod()]
        public void ReconcileCourseMembers_NoChangesFromCanvasCourse_ReturnNoNeededChanges()
        {
            //Arrange

            var build = new Build();
            var courseReconcile = build.CourseReconcile(2, 2, 0, 0);
            var courseEnrollments = build.CanvasEnrollments(2, 0, 2, 0);

            //Act

            courseReconcile = CanvasBusinessLogic.ReconcileCourseMembers(courseReconcile, courseEnrollments, 1);

            //Assert
            var facAdd = courseReconcile.FacultyToReconcile.Where(reconcile => reconcile.NewEnrollment);
            var stuAdd = courseReconcile.StudentsToReconcile.Where(reconcile => reconcile.NewEnrollment);
            var totalAdd = facAdd.Count() + stuAdd.Count();

            var facDelete = courseReconcile.FacultyToReconcile.Where(reconcile => reconcile.RemoveEnrollment);
            var stuDelete = courseReconcile.StudentsToReconcile.Where(reconcile => reconcile.RemoveEnrollment);
            var totalDelete = facDelete.Count() + stuDelete.Count();

            facAdd.Count().Should().Be(0);
            stuAdd.Count().Should().Be(0);
            facDelete.Count().Should().Be(0);
            stuDelete.Count().Should().Be(0);
            
        }

        // public static List<WorkGroup> ReconcileCanvasCourseSections
        // (List<CanvasSection> canvasSectionsReturned, ICollection<WorkGroup> workGroups,
        // int crseId, WorkGroupModel workGroupModel, int facId, Guid reconResultId)

        [TestMethod()]
        public void ReconcileCanvasCourseSections_NewCanvasCourseSections_ReturnAddedWorkGroups()
        {
            //Arrange
            var faker = new Faker();
            var crseId = faker.Random.Digits(1)[0];
            var reconResultId = faker.Random.Guid();
            var facId = faker.Random.Digits(1)[0];
            var canvasSectionId = 1000;
            var workGroupId = 1;
            var bbGroupId = 1000;

            var canvasSections = new Faker<CanvasSection>()
                .RuleFor(csr => csr.id, f => canvasSectionId)
                .RuleFor(csr => csr.course_id, 1)
                .RuleFor(csr => csr.name, f => $"Flight 0{canvasSectionId++}");

            var canvasSectionsReturned = canvasSections.Generate(8);

            var workGroups = new Faker<WorkGroup>()
                .RuleFor(wg => wg.WorkGroupId, f => workGroupId)
                .RuleFor(wg => wg.CourseId, 1)
                .RuleFor(wg => wg.WgModelId, 1)
                .RuleFor(wg => wg.MpCategory, "BC2")
                .RuleFor(wg => wg.GroupNumber, f => $"0{workGroupId++}")
                .RuleFor(wg => wg.BbGroupId, f => (bbGroupId++).ToString())
                .RuleFor(wg => wg.DefaultName, f => $"Flight 0{canvasSectionId++}")
                .RuleFor(wg => wg.MpSpStatus, "Created")
                .RuleFor(wg => wg.IsPrimary, false)
                .RuleFor(wg => wg.ModifiedById, 1)
                .RuleFor(wg => wg.ModifiedDate, f => f.Date.Soon());

            var workGroupsReturned = workGroups.Generate(4);

            var workGroupModel = new Faker<WorkGroupModel>()
                .RuleFor(wgm => wgm.AssignedSpInstrId, 1);

            var models = workGroupModel.Generate();
            //Act
            var workGroupsToAdd = CanvasBusinessLogic.ReconcileWorkGroups(canvasSectionsReturned, workGroupsReturned, crseId,
                models, facId, reconResultId);

            //Assert

            workGroupsToAdd.Count().Should().Be(4);
        }

        [TestMethod()]
        public void ReconcileCanvasCourseSections_NoNewCanvasCourseSections_ReturnNoWorkGroupsAdded()
        {
            //Arrange
            var faker = new Faker();
            var crseId = faker.Random.Digits(1)[0];
            var reconResultId = faker.Random.Guid();
            var facId = faker.Random.Digits(1)[0];
            var canvasSectionId = 1000;
            var workGroupId = 1;
            var bbGroupId = 1000;

            var canvasSections = new Faker<CanvasSection>()
                .RuleFor(csr => csr.id, f => canvasSectionId)
                .RuleFor(csr => csr.course_id, 1)
                .RuleFor(csr => csr.name, f => $"Flight 0{canvasSectionId++}");

            var canvasSectionsReturned = canvasSections.Generate(8);

            var workGroups = new Faker<WorkGroup>()
                .RuleFor(wg => wg.WorkGroupId, f => workGroupId)
                .RuleFor(wg => wg.CourseId, 1)
                .RuleFor(wg => wg.WgModelId, 1)
                .RuleFor(wg => wg.MpCategory, "BC2")
                .RuleFor(wg => wg.GroupNumber, f => $"0{workGroupId++}")
                .RuleFor(wg => wg.BbGroupId, f => (bbGroupId++).ToString())
                .RuleFor(wg => wg.DefaultName, f => $"Flight 0{canvasSectionId++}")
                .RuleFor(wg => wg.MpSpStatus, "Created")
                .RuleFor(wg => wg.IsPrimary, false)
                .RuleFor(wg => wg.ModifiedById, 1)
                .RuleFor(wg => wg.ModifiedDate, f => f.Date.Soon());

            var workGroupsReturned = workGroups.Generate(8);

            var workGroupModel = new Faker<WorkGroupModel>()
                .RuleFor(wgm => wgm.AssignedSpInstrId, 1);

            var models = workGroupModel.Generate();
            //Act
            var workGroupsToAdd = CanvasBusinessLogic.ReconcileWorkGroups(canvasSectionsReturned, workGroupsReturned, crseId,
                models, facId, reconResultId);

            //Assert

            workGroupsToAdd.Count().Should().Be(0);
        }
    }
}