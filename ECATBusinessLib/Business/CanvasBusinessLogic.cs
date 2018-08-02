using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Canvas;
using Ecat.Data.Models.Common;
using Ecat.Data.Models.School;
using Ecat.Data.Models.User;
using Ecat.Data.Static;
using Ecat.Data.Models.Designer;

namespace Ecat.Business.Business
{
    public class CanvasBusinessLogic
    {
        public static List<Course> ReconcileCourses(List<CanvasCourse> canvasCoursesReturned, List<Course> existingCourses, string academyId)
        {
            var existingIds = new List<string>();
            var reconCourses = new List<Course>();
            //Bb course id is a string, Canvas course ids are numbers
            existingCourses.ForEach(c => existingIds.Add(c.BbCourseId));

            canvasCoursesReturned.ForEach(c =>
            {
                if (existingIds.Contains(c.id.ToString())) return;

                var newCourse = new Course
                {
                    AcademyId = academyId,
                    Name = c.name,
                    BbCourseId = c.id.ToString(),
                    StartDate = c.start_at == null ? DateTime.Now : DateTime.Parse(c.start_at),
                    GradDate = c.end_at == null ? DateTime.Now.AddDays(30) : DateTime.Parse(c.end_at)
                };
                reconCourses.Add(newCourse);
            });

            return reconCourses;
        }

        public static List<WorkGroup> ReconcileWorkGroups(List<CanvasSection> canvasSectionsReturned, ICollection<WorkGroup> workGroups, int crseId, WorkGroupModel workGroupModel, int facId, Guid reconResultId)
        {
            var newWorkGroups = new List<WorkGroup>();
            //Compare Canvas Id to Workgroup BBgroupId

            canvasSectionsReturned.ForEach(csr =>
            {
                var sectionAlreadyInEcat = workGroups.SingleOrDefault(wg => wg.BbGroupId == csr.id.ToString());

                if (sectionAlreadyInEcat == null)
                {
                    var newGroup = new WorkGroup()
                    {
                        BbGroupId = csr.id.ToString(),
                        DefaultName = csr.name,
                        CourseId = crseId,
                        WgModelId = workGroupModel.Id,
                        AssignedSpInstrId = workGroupModel.AssignedSpInstrId,
                        MpCategory = MpGroupCategory.Wg1,
                        MpSpStatus = MpSpStatus.Created,
                        ModifiedById = facId,
                        ModifiedDate = DateTime.Now,
                        ReconResultId = reconResultId
                    };

                    var nameNum = csr.name.Split(' ')[1];
                    if (!nameNum.StartsWith("0") && nameNum.Length == 1)
                    {
                        nameNum = "0" + nameNum;
                    }
                    newGroup.GroupNumber = nameNum;

                    newWorkGroups.Add(newGroup);
                }
            });


            return newWorkGroups;
        }

        public static CourseReconcile ReconcileCourseMembers(CourseReconcile ecatCourseReconcile, List<CanvasEnrollment> canvasEnrollmentsReturned, int facultyId)
        {
            //Remove Dupliates -- Happens if user is in multiple flights
            canvasEnrollmentsReturned = canvasEnrollmentsReturned.GroupBy(cer => cer.user_id).Select(cer => cer.First())
                .ToList();

            var facCanvasEnrollmentsReturned = canvasEnrollmentsReturned.Where(ce =>
                MpRoleTransform.CanvasRoleToEcat(ce.type) == MpInstituteRoleId.Faculty).ToList();

            var stuCanvasEnrollmentsReturned = canvasEnrollmentsReturned.Where(ce =>
                MpRoleTransform.CanvasRoleToEcat(ce.type) == MpInstituteRoleId.Student).ToList();

            ecatCourseReconcile.FacultyToReconcile = ReconcileCourseMembers(ecatCourseReconcile.FacultyToReconcile, facCanvasEnrollmentsReturned, facultyId);
            ecatCourseReconcile.StudentsToReconcile = ReconcileCourseMembers(ecatCourseReconcile.StudentsToReconcile, stuCanvasEnrollmentsReturned, facultyId);
           
            return ecatCourseReconcile;
        }

        public static WorkgroupAndMemberReconcile ReconcileGroupMembers(int crseId, WorkgroupAndMemberReconcile courseWithWorkGroups, List<CanvasSection> canvasSectionsReturned)
        {
            var canvasMembers = canvasSectionsReturned.SelectMany(csr => csr.students).ToList();
            var workgroupMembers = courseWithWorkGroups.WorkGroups.SelectMany(wg => wg.Members).ToList();
            //It should be safe to use workgroups in database as sections should already be reconcilied. 
            var workgroups = courseWithWorkGroups.WorkGroups.ToList();

            var membersInSectionNotInAnyWorkgroup = canvasMembers.Where(cm =>
                !workgroupMembers.Select(wgMember => wgMember.BbUserId).ToList().Contains(cm.id.ToString())).ToList();

            var membersInWorkGroupNotInAnySection = workgroupMembers.Where(wgMember =>
                !canvasMembers.Select(cm => cm.id.ToString()).ToList().Contains(wgMember.BbUserId)).ToList();

            var membersInBoth = workgroupMembers.Where(wgMember =>
                canvasMembers.Select(cm => cm.id.ToString()).ToList().Contains(wgMember.BbUserId)).ToList();

            if (membersInSectionNotInAnyWorkgroup.Any())
            {
                membersInSectionNotInAnyWorkgroup.ForEach(mem =>
                {

                    //var membersWorkGroup = workgroups
                    //    .Where(wg => wg.Members.Select(mem2 => mem2.BbUserId).Contains(mem.id.ToString())).ToList();

                    var membersSection = canvasSectionsReturned
                        .Where(section => section.students.Select(stu => stu.id).Contains(mem.id)).ToList().FirstOrDefault();

                    var membersWorkgroup = workgroups.Where(wg => wg.BbWgId == membersSection.id.ToString()).ToList().FirstOrDefault();

                    if (mem.enrollments.First().enrollment_state != "active") return;

                    var newWorkgroupMemberReconcile = new WorkgroupMemberReconcile
                    {
                        //Will be added wwhen crseStudentInGroup is created for memeber
                        StudentId = 0,


                        //Can be added now
                        WorkGroupId = membersWorkgroup.WgId,
                        BbUserId = mem.id.ToString(),
                        IsDeleted = false,
                        IsMoving = false,
                        NewEnrollment = true,
                        RemoveEnrollment = false,
                        HasChildren = false,
                        StudentOnTheMoveReconcile = null,
                        CanvasUser = mem,

                    };

                    membersWorkgroup.Members.Add(newWorkgroupMemberReconcile);


                });

            }

            if (membersInWorkGroupNotInAnySection.Any())
            {
                membersInWorkGroupNotInAnySection.ForEach(mem =>
                {
                    mem.RemoveEnrollment = true;
                });
            }

            if (membersInBoth.Any())
            {
                membersInBoth.ForEach(mem =>
                {

                    //var membersCurrentWorkGroup = workgroups.FirstOrDefault(wg => wg.WgId == mem.WorkGroupId);
                    var membersCurrentWorkGroupId = mem.WorkGroupId;
                    var membersCurrentSection = canvasSectionsReturned.Where(csr =>
                        csr.students.Select(stu => stu.id.ToString()).ToList().Contains(mem.BbUserId)).ToList();
                    var canvasUser = membersCurrentSection
                        .SelectMany(mcs => mcs.students).FirstOrDefault(stu => stu.id.ToString() == mem.BbUserId);
                    var membersCurrentSectionId = membersCurrentSection.FirstOrDefault().id;
                    var membersNewWorkGroup =
                        workgroups.FirstOrDefault(wg3 => wg3.BbWgId == membersCurrentSectionId.ToString());
                    var membersNewWorkGroupId = membersNewWorkGroup.WgId;


                    if (canvasUser.enrollments.FirstOrDefault().enrollment_state != "active")
                    {
                        mem.RemoveEnrollment = true;
                        return;
                    }

                    if (membersCurrentWorkGroupId != membersNewWorkGroupId)
                    {
                        mem.RemoveEnrollment = true;
                        mem.IsMoving = true;

                        var newWorkGroupMemberReconcile = new WorkgroupMemberReconcile
                        {
                            StudentId = mem.StudentId,
                            BbUserId = mem.BbUserId,
                            WorkGroupId = membersNewWorkGroupId,
                            IsDeleted = false,
                            IsMoving = true,
                            NewEnrollment = true,
                            RemoveEnrollment = false,
                            HasChildren = false,
                            CanvasUser = mem.CanvasUser
                        };

                        membersNewWorkGroup.Members.Add(newWorkGroupMemberReconcile);

                        //mem.IsMoving = true;
                        //mem.StudentOnTheMoveReconcile = new StudentOnTheMoveReconcile
                        //{
                        //    StudentId = mem.StudentId,
                        //    HasChildren = mem.HasChildren,
                        //    CourseId = crseId,
                        //    FromWorkGroupId = membersCurrentWorkGroupId,
                        //    ToWorkGroupId = membersNewWorkGroupId
                        //};
                    }


                });
            }


            return courseWithWorkGroups;
        }

        private static UserReconcile CreateUserReconcile(CanvasEnrollment canvasEnrollment, int facultyId)
        {
            var nameSplit = canvasEnrollment.user.sortable_name.Split(',');
            var userReconcile = new UserReconcile
            {
                BbUserId = canvasEnrollment.user_id.ToString(),
                CourseMemId = canvasEnrollment.id.ToString(),
                NewEnrollment = true,

                Person = new Person
                {
                    MpInstituteRole = MpRoleTransform.CanvasRoleToEcat(canvasEnrollment.type),
                    BbUserId = canvasEnrollment.user_id.ToString(),
                    BbUserName = canvasEnrollment.user.login_id,
                    Email = $"{canvasEnrollment.user.login_id}.@us.af.mil",
                    IsActive = true,
                    LastName = nameSplit[0],
                    FirstName = nameSplit[1].TrimStart(' '),
                    MpGender = MpGender.Unk,
                    MpAffiliation = MpAffiliation.Unk,
                    MpComponent = MpComponent.Unk,
                    RegistrationComplete = false,
                    MpPaygrade = MpPaygrade.Unk,
                    ModifiedById = facultyId,
                    ModifiedDate = DateTime.Now
                }

            };

            return userReconcile;
        }

        private static ICollection<UserReconcile> ReconcileCourseMembers(ICollection<UserReconcile> courseUsersReconcile, List<CanvasEnrollment> canvasEnrollments, int facultyId)
        {

            var membersInCanvasCourseNotInEcatCourse = canvasEnrollments
                    .Where(canvasEnrollment => !courseUsersReconcile
                        .Select(courseUserReconcile => courseUserReconcile.BbUserId).ToList()
                        .Contains(canvasEnrollment.user_id.ToString()))
                        .ToList();

            var membersInEcatCourseNotInCanvasCourse = courseUsersReconcile
                .Where(courseUserReconcile => !canvasEnrollments
                    .Select(canvasEnrollment => canvasEnrollment.user_id.ToString()).ToList()
                    .Contains(courseUserReconcile.BbUserId)).ToList();

            //members that are in the Canvas course and are marked isDeleted in a ECAT course....
            var membersInBoth = courseUsersReconcile
                //.Where(courseUserReconcile => courseUserReconcile.IsMarkedDeleted)
                .Where(courseUserReconcile => canvasEnrollments
                    .Select(canvasEnrollment => canvasEnrollment.user_id.ToString()).ToList()
                    .Contains(courseUserReconcile.BbUserId)).ToList();


            //This only applies if members will be marked Inactive in Canvas course
            membersInBoth.ForEach(member =>
            {
                var canvasEnrollment = canvasEnrollments.First(ce => ce.user_id.ToString() == member.BbUserId);

                //member is added, removed then added back to course
                if (canvasEnrollment.enrollment_state == "active" && member.IsMarkedDeleted)
                {
                    member.NewEnrollment = true;
                    member.UnFlagDeleted = true;
                }

                //Member is added, then set inactive
                if (canvasEnrollment.enrollment_state != "active" && !member.IsMarkedDeleted)
                {
                    member.RemoveEnrollment = true;
                    member.FlagDeleted = !member.CanDelete;
                }

            });

            membersInEcatCourseNotInCanvasCourse.ForEach(userReconcile =>
            {
                //No need to do anything if member is already marked deleted in ECAT
                if (userReconcile.IsMarkedDeleted) return;

                userReconcile.RemoveEnrollment = true;
       
                //If Member has group enrollments set isDeleted true in StudentInCourse 
                userReconcile.FlagDeleted = !userReconcile.CanDelete;

            });

            membersInCanvasCourseNotInEcatCourse.ForEach(canvasEnrollment =>
            {

                if (canvasEnrollment.enrollment_state != "active") return;

                var userReconcile = CreateUserReconcile(canvasEnrollment, facultyId);
                userReconcile.NewEnrollment = true;
                courseUsersReconcile.Add(userReconcile);

            });         

            return courseUsersReconcile;

        }

        
    }
}
