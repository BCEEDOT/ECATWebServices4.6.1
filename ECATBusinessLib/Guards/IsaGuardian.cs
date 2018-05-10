﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Ecat.Data.Contexts;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Cognitive;
using Ecat.Data.Models.School;
using Ecat.Business.Utilities;
using Ecat.Data.Models.Common;
using Ecat.Data.Static;

namespace Ecat.Business.Guards
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public class IsaGuard
    {
        private readonly EFContextProvider<EcatContext> ctxManager;
        private readonly Person loggedInUser;
        private readonly Type tWg = typeof(WorkGroup);
        private readonly Type tStudInGroup = typeof(CrseStudentInGroup);

        public IsaGuard(EFContextProvider<EcatContext> efCtx, Person loggedIn)
        {
            ctxManager = efCtx;
            loggedInUser = loggedIn;
        }

        public SaveMap BeforeSaveEntities(SaveMap saveMap)
        {

            List<StudentOnTheMove> studentsPendingRemoval = new List<StudentOnTheMove>();

            if (saveMap.ContainsKey(tWg))
            {
                var grps = ProcessWorkGroup(saveMap[tWg]);
                if (grps != null) { saveMap.MergeMap(grps); }

                var groups = (from info in saveMap[tWg]
                              select info.Entity as WorkGroup).ToList();

                groups.ForEach(group =>
                {
                    group.ModifiedById = loggedInUser.PersonId;
                    group.ModifiedDate = DateTime.Now;
                });

            }

            if (saveMap.ContainsKey(tStudInGroup))
            {
                var groupMembers = saveMap[tStudInGroup];

                //Need to account for adds when a new group is created. 

                var studentsOnTheMove = (from info in saveMap[tStudInGroup]
                                         let sig = info.Entity as CrseStudentInGroup
                                         where info.EntityState == EntityState.Modified
                                         where info.OriginalValuesMap.ContainsKey("WorkGroupId")
                                         select info).ToList();

                var unAssignedStudents = (from info in saveMap[tStudInGroup]
                                          let sig = info.Entity as CrseStudentInGroup
                                          where info.EntityState == EntityState.Deleted
                                          select info).ToList();


                if (unAssignedStudents.Any())
                {
                    unAssignedStudents.ForEach(uas =>
                    {
                        var studentEntity = uas.Entity as CrseStudentInGroup;
                        var fromWorkGroupId = Int32.Parse(studentEntity.WorkGroupId.ToString());
                        //uas.EntityState = Breeze.Persistence.EntityState.Deleted;

                        var member = ctxManager.Context.StudentInGroups
                                                        .Where(sig => sig.StudentId == studentEntity.StudentId)
                                                        .Where(sig => sig.WorkGroupId == fromWorkGroupId)
                                                        .Select(sig => new StudentOnTheMove
                                                        {
                                                            //Student = sig,
                                                            StudentId = sig.StudentId,
                                                            //IsDeleted = sig.IsDeleted,
                                                            IsMoving = false,
                                                            FromWorkGroupId = fromWorkGroupId,
                                                            //CourseId = studentEntity.CourseId,
                                                            HasChildren = sig.AuthorOfComments.Any() ||
                                                                            sig.AssesseeSpResponses.Any() ||
                                                                            sig.AssessorSpResponses.Any() ||
                                                                            sig.AssesseeStratResponse.Any() ||
                                                                            sig.AssessorStratResponse.Any() ||
                                                                            sig.RecipientOfComments.Any()
                                                        }).ToList();

                        studentsPendingRemoval.AddRange(member);
                    });

                }

                if (studentsOnTheMove.Any())
                {

                    studentsOnTheMove.ForEach(sm =>
                    {
                        var studentEntity = sm.Entity as CrseStudentInGroup;
                        var fromWorkGroupId = Int32.Parse(sm.OriginalValuesMap["WorkGroupId"].ToString());

                        var member = ctxManager.Context.StudentInGroups
                                                            .Where(sig => sig.StudentId == studentEntity.StudentId)
                                                            .Where(sig => sig.WorkGroupId == fromWorkGroupId)
                                                            .Select(sig => new StudentOnTheMove
                                                            {
                                                                Student = sig,
                                                                StudentId = sig.StudentId,
                                                                IsDeleted = sig.IsDeleted,
                                                                IsMoving = true,
                                                                ToWorkGroupId = studentEntity.WorkGroupId,
                                                                FromWorkGroupId = fromWorkGroupId,
                                                                CourseId = studentEntity.CourseId,
                                                                HasChildren = sig.AuthorOfComments.Any() ||
                                                                            sig.AssesseeSpResponses.Any() ||
                                                                            sig.AssessorSpResponses.Any() ||
                                                                            sig.AssesseeStratResponse.Any() ||
                                                                            sig.AssessorStratResponse.Any() ||
                                                                            sig.RecipientOfComments.Any() ||
                                                                            sig.FacultySpResponses.Any() ||
                                                                            sig.FacultyStrat != null ||
                                                                            sig.FacultyComment != null
                                                            }).ToList();

                        studentsPendingRemoval.AddRange(member);

                    });
                }

                var studentsPendingRemovalWithChildren = studentsPendingRemoval
                                                                    .Where(spr => spr.HasChildren).ToList();

                var studentsPendingRemovalWithoutChildren = studentsPendingRemoval
                                                                    .Where(spr => !spr.HasChildren).ToList();


                if (studentsPendingRemovalWithChildren.Any())
                {
                    studentsPendingRemovalWithChildren.ForEach(sprwc =>
                    {
                        //if (sprwc.IsMoving)
                        //{
                        var authorCommentFlags = ctxManager.Context.StudSpCommentFlags
                                                    .Where(sscf => sscf.AuthorPersonId == sprwc.StudentId)
                                                    .Where(sscf => sscf.WorkGroupId == sprwc.FromWorkGroupId);

                        var recipientCommentFlags = ctxManager.Context.StudSpCommentFlags
                                                    .Where(sscf => sscf.RecipientPersonId == sprwc.StudentId)
                                                    .Where(sscf => sscf.WorkGroupId == sprwc.FromWorkGroupId);

                        var authorOfComments = ctxManager.Context.StudSpComments
                                                .Where(ssc => ssc.AuthorPersonId == sprwc.StudentId)
                                                .Where(ssc => ssc.WorkGroupId == sprwc.FromWorkGroupId);

                        var recipientOfComments = ctxManager.Context.StudSpComments
                                                    .Where(ssc => ssc.RecipientPersonId == sprwc.StudentId)
                                                    .Where(ssc => ssc.WorkGroupId == sprwc.FromWorkGroupId);

                        var assesseeSpResponses = ctxManager.Context.SpResponses
                                                .Where(sr => sr.AssesseePersonId == sprwc.StudentId)
                                                .Where(sr => sr.WorkGroupId == sprwc.FromWorkGroupId);

                        var assessorSpResponses = ctxManager.Context.SpResponses
                                                    .Where(sr => sr.AssessorPersonId == sprwc.StudentId)
                                                    .Where(sr => sr.WorkGroupId == sprwc.FromWorkGroupId);

                        var assesseeStratResponses = ctxManager.Context.SpStratResponses
                                                        .Where(ssr => ssr.AssesseePersonId == sprwc.StudentId)
                                                        .Where(ssr => ssr.WorkGroupId == sprwc.FromWorkGroupId);

                        var assessorStratResponses = ctxManager.Context.SpStratResponses
                                                        .Where(ssr => ssr.AssessorPersonId == sprwc.StudentId)
                                                        .Where(ssr => ssr.WorkGroupId == sprwc.FromWorkGroupId);

                        var facSpResponses = ctxManager.Context.FacSpResponses
                                                        .Where(fsr => fsr.AssesseePersonId == sprwc.StudentId)
                                                        .Where(fsr => fsr.WorkGroupId == sprwc.FromWorkGroupId);

                        var facStratResponse = ctxManager.Context.FacStratResponses
                                                        .Where(fsr => fsr.AssesseePersonId == sprwc.StudentId)
                                                        .Where(fsr => fsr.WorkGroupId == sprwc.FromWorkGroupId);

                        var facComments = ctxManager.Context.FacSpComments
                                                        .Where(fsc => fsc.RecipientPersonId == sprwc.StudentId)
                                                        .Where(fsc => fsc.WorkGroupId == sprwc.FromWorkGroupId);

                        var facCommentsFlag = ctxManager.Context.FacSpCommentFlags
                                                        .Where(fscf => fscf.RecipientPersonId == sprwc.StudentId)
                                                        .Where(fscf => fscf.WorkGroupId == sprwc.FromWorkGroupId);





                        if (authorOfComments.Any())
                        {
                            if (authorCommentFlags.Any())
                            {
                                ctxManager.Context.StudSpCommentFlags.RemoveRange(authorCommentFlags);
                            }

                            ctxManager.Context.StudSpComments.RemoveRange(authorOfComments);
                        }

                        if (recipientOfComments.Any())
                        {
                            if (recipientCommentFlags.Any())
                            {
                                ctxManager.Context.StudSpCommentFlags.RemoveRange(recipientCommentFlags);
                            }
                            ctxManager.Context.StudSpComments.RemoveRange(recipientOfComments);
                        }

                        if (assesseeSpResponses.Any())
                        {
                            ctxManager.Context.SpResponses.RemoveRange(assesseeSpResponses);
                        }

                        if (assessorSpResponses.Any())
                        {
                            ctxManager.Context.SpResponses.RemoveRange(assessorSpResponses);
                        }

                        if (assesseeStratResponses.Any())
                        {
                            ctxManager.Context.SpStratResponses.RemoveRange(assesseeStratResponses);
                        }

                        if (assessorStratResponses.Any())
                        {
                            ctxManager.Context.SpStratResponses.RemoveRange(assessorStratResponses);
                        }

                        if (facSpResponses.Any())
                        {
                            ctxManager.Context.FacSpResponses.RemoveRange(facSpResponses);
                        }

                        if (facStratResponse.Any())
                        {
                            ctxManager.Context.FacStratResponses.RemoveRange(facStratResponse);
                        }

                        if (facComments.Any())
                        {
                            ctxManager.Context.FacSpComments.RemoveRange(facComments);
                        }

                        if (facCommentsFlag.Any())
                        {
                            ctxManager.Context.FacSpCommentFlags.RemoveRange(facCommentsFlag);
                        }


                        //}

                        if (sprwc.IsMoving)
                        {
                            ctxManager.Context.StudentInGroups.Remove(sprwc.Student);

                        }

                    });

                    ctxManager.Context.SaveChanges();

                }

                if (studentsPendingRemovalWithoutChildren.Any())
                {
                    studentsPendingRemovalWithoutChildren.ForEach(sprwoc =>
                    {
                        if (sprwoc.IsMoving)
                        {
                            ctxManager.Context.StudentInGroups.Remove(sprwoc.Student);
                            ctxManager.Context.SaveChanges();
                        }

                        //ctxManager.Context.Entry(sprwoc.Student).State = System.Data.Entity.EntityState.Deleted;
                    });

                    //ctxManager.Context.SaveChanges();
                }

                var studentsToBeAddedBack = studentsPendingRemoval
                                                        .Where(spr => spr.IsMoving).ToList();

                ////Students that were previously deleted with children.
                studentsOnTheMove.ForEach(info => saveMap.Remove(tStudInGroup));

                if (studentsToBeAddedBack.Any())
                {
                    List<EntityInfo> toAddInfos;
                    toAddInfos = new List<EntityInfo>();


                    studentsToBeAddedBack.ForEach(stab =>
                    {

                        var toAdd = new CrseStudentInGroup
                        {
                            StudentId = stab.StudentId,
                            CourseId = stab.CourseId,
                            WorkGroupId = stab.ToWorkGroupId,
                            HasAcknowledged = false,
                            IsDeleted = false,
                            ModifiedById = loggedInUser.PersonId,
                            ModifiedDate = DateTime.Now
                        };

                        var toAddEi = ctxManager.CreateEntityInfo(toAdd);
                        toAddInfos.Add(toAddEi);

                    });

                    saveMap.Add(tStudInGroup, toAddInfos);

                }

            }

            return saveMap;
        }

        private SaveMap ProcessWorkGroup(List<EntityInfo> workGroupInfos)
        {

            var publishingWgs = workGroupInfos
                .Where(info => info.OriginalValuesMap.ContainsKey("MpSpStatus"))
                .Select(info => info.Entity)
                .OfType<WorkGroup>()
                .Where(wg => wg.MpSpStatus == MpSpStatus.Published).ToList();

            var wgSaveMap = new Dictionary<Type, List<EntityInfo>> { { tWg, workGroupInfos } };

            if (!publishingWgs.Any()) return null;


            var svrWgIds = publishingWgs.Select(wg => wg.WorkGroupId);
            var grpsWithMemsIds = ctxManager.Context.WorkGroups.Where(grp => svrWgIds.Contains(grp.WorkGroupId) && grp.GroupMembers.Where(mem => !mem.IsDeleted).Count() > 0).Select(wg => wg.WorkGroupId);

            var publishResultMap = WorkGroupPublish.Publish(wgSaveMap, grpsWithMemsIds, loggedInUser.PersonId, ctxManager);

            //wgSaveMap.MergeMap(publishResultMap);

            return publishResultMap;
        }
    }
}
