using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Ecat.Data.Contexts;
using Ecat.Data.Static;
//using Ecat.Shared.Core.Interface;
using Ecat.Data.Models.Interface;
//using Ecat.Shared.Core.Logic;
//using Ecat.Shared.Core.ModelLibrary.Designer;
//using Ecat.Shared.Core.ModelLibrary.Faculty;
using Ecat.Data.Models.Faculty;
//using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Data.Models.Student;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.Utility;
using Ecat.Business.Utilities;

namespace Ecat.Business.Guards
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public class FacultyGuardian
    {

        //private readonly EcatContext _facCtx;
        private readonly EFContextProvider<EcatContext> _efCtx; 
        private readonly Person _loggedInUser;
        private readonly Type _tWg = typeof(WorkGroup);
        private readonly Type _tFacComment = typeof (FacSpComment);
        private readonly Type _tFacStratResp = typeof (FacStratResponse);
        private readonly Type _tFacCommentFlag = typeof (FacSpCommentFlag);
        private readonly Type _tStudCommentFlag = typeof (StudSpCommentFlag);
        private readonly Type _tFacSpResp = typeof (FacSpResponse);

        public FacultyGuardian(EFContextProvider<EcatContext> efCtx,Person loggedInUser)
        {
            //_facCtx = facCtx;
            _efCtx = efCtx;
            _loggedInUser = loggedInUser;
        }
      
        public SaveMap BeforeSaveEntities(SaveMap saveMap)
        {

            var unAuthorizedMaps = saveMap.Where(map => map.Key != _tWg &&
                                                        map.Key != _tFacComment &&
                                                        map.Key != _tFacStratResp &&
                                                        map.Key != _tFacSpResp &&
                                                        map.Key != _tStudCommentFlag &&
                                                        map.Key != _tFacCommentFlag)
                                                        .ToList();
            //.Select(map => map.Key);

            saveMap.RemoveMaps(unAuthorizedMaps);

            var courseMonitorEntities = saveMap.MonitorCourseMaps()?.ToList();

            if (courseMonitorEntities != null) ProcessCourseMonitoredMaps(courseMonitorEntities);

            var workGroupMonitorEntities = saveMap.MonitorWgMaps()?.ToList();

            if (workGroupMonitorEntities != null) ProcessWorkGroupMonitoredMaps(workGroupMonitorEntities);

            if (saveMap.ContainsKey(_tWg))
            {
                var workGroupMap = ProcessWorkGroup(saveMap[_tWg]);
                saveMap.MergeMap(workGroupMap);
            }

            if (saveMap.ContainsKey(_tFacComment)) {
                ProcessComments(saveMap[_tFacComment]);
            }

            saveMap.AuditMap(_loggedInUser.PersonId);
            saveMap.SoftDeleteMap(_loggedInUser.PersonId);
            return saveMap;
        }

        private void ProcessCourseMonitoredMaps(List<EntityInfo> infos)
        {
            var courseMonitorEntities = infos.Select(info => info.Entity).OfType<ICourseMonitored>();
            var courseIds = courseMonitorEntities.Select(cme => cme.CourseId);

            var pubCrseId = _efCtx.Context.Courses
                .Where(crse => courseIds.Contains(crse.Id) && crse.GradReportPublished)
                .Select(crse => crse.Id);

            if (!pubCrseId.Any()) return;

            var errors = from info in infos
                         let crseEntity = (ICourseMonitored)info.Entity
                         where pubCrseId.Contains(crseEntity.CourseId)
                         select new EFEntityError(info, "Course Error Validation",
                                     "There was a problem saving the requested items", "Course");

            throw new EntityErrorsException(errors);
        }

        private void ProcessWorkGroupMonitoredMaps(List<EntityInfo> infos)
        {
            var wgMonitorEntities = infos.Select(info => info.Entity).OfType<IWorkGroupMonitored>();
            var wgIds = wgMonitorEntities.Select(wgme => wgme.WorkGroupId);

            var pubWgIds = _efCtx.Context.WorkGroups
                .Where(wg => wgIds.Contains(wg.WorkGroupId) && wg.MpSpStatus == MpSpStatus.Published)
                .Select(wg => wg.WorkGroupId);

            if (!pubWgIds.Any()) return;

            var errors = from info in infos
                let wgEntity = (IWorkGroupMonitored) info.Entity
                where pubWgIds.Contains(wgEntity.WorkGroupId)
                select new EFEntityError(info, "WorkGroup Error Validation",
                            "There was a problem saving the requested items", "WorkGroup");

              
            throw new EntityErrorsException(errors);
        }

        private SaveMap ProcessWorkGroup(List<EntityInfo> workGroupInfos)
        {

            var publishingWgs = workGroupInfos
                .Where(info => info.OriginalValuesMap.ContainsKey("MpSpStatus"))
                .Select(info => info.Entity)
                .OfType<WorkGroup>()
                .Where(wg => wg.MpSpStatus == MpSpStatus.Published).ToList();

            var wgSaveMap = new Dictionary<Type, List<EntityInfo>> {{ _tWg, workGroupInfos }} ;

            if (!publishingWgs.Any()) return wgSaveMap;


            var svrWgIds = publishingWgs.Select(wg => wg.WorkGroupId);
            var publishResultMap = WorkGroupPublish.Publish(wgSaveMap, svrWgIds, _loggedInUser.PersonId, _efCtx);

            wgSaveMap.MergeMap(publishResultMap);

            return wgSaveMap;
        }

        private void ProcessComments(List<EntityInfo> commentInfos) {
            var newComments = commentInfos
                .Where(info => info.EntityState == Breeze.ContextProvider.EntityState.Added)
                .Select(info=> info.Entity)
                .OfType<FacSpComment>()
                .ToList();

            foreach (var newComment in newComments) {
                newComment.CreatedDate = DateTime.UtcNow;
            }

            var modifiedComments = commentInfos
                .Where(info => info.EntityState == Breeze.ContextProvider.EntityState.Modified)
                .ToList();

            //for now modifying a comment makes the modifier the comment owner, no history of who originally created it
            //future plan for audit tables to track changes
            foreach (var info in modifiedComments) {
                info.OriginalValuesMap["FacultyPersonId"] = null;
                var comment = info.Entity as FacSpComment;
                comment.FacultyPersonId = _loggedInUser.PersonId;
            }
        }

    }
}
