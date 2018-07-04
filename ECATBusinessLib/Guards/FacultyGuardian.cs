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
    using Data.Models.Common;
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public class FacultyGuardian
    {

        //private readonly EcatContext _facCtx;
        private readonly EFContextProvider<EcatContext> ctxManager; 
        private readonly Person loggedInUser;
        private readonly Type _tWg = typeof(WorkGroup);
        private readonly Type _tFacComment = typeof (FacSpComment);
        private readonly Type _tFacStratResp = typeof (FacStratResponse);
        private readonly Type _tFacCommentFlag = typeof (FacSpCommentFlag);
        private readonly Type _tStudCommentFlag = typeof (StudSpCommentFlag);
        private readonly Type _tFacSpResp = typeof (FacSpResponse);

        public FacultyGuardian(EFContextProvider<EcatContext> efCtx,Person loggedInUser)
        {
            //_facCtx = facCtx;
            ctxManager = efCtx;
            this.loggedInUser = loggedInUser;
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

            //if (courseMonitorEntities != null) ProcessCourseMonitoredMaps(courseMonitorEntities);

            var workGroupMonitorEntities = saveMap.MonitorWgMaps()?.ToList();

            //if (workGroupMonitorEntities != null) ProcessWorkGroupMonitoredMaps(workGroupMonitorEntities);

            //moved processing for monitored entities to monitoredguard
            if (courseMonitorEntities != null || workGroupMonitorEntities != null)
            {
                var monitoredGuard = new MonitoredGuard(ctxManager);

                if (courseMonitorEntities != null)
                {
                    monitoredGuard.ProcessCourseMonitoredMaps(courseMonitorEntities);
                }

                if (workGroupMonitorEntities != null)
                {
                    monitoredGuard.ProcessFacultyWorkGroupMonitoredMaps(workGroupMonitorEntities);
                }
            }

            if (saveMap.ContainsKey(_tWg))
            {
                var workGroupMap = ProcessWorkGroup(saveMap[_tWg]);
                saveMap.MergeMap(workGroupMap);
            }

            if (saveMap.ContainsKey(_tFacComment)) {
                ProcessComments(saveMap[_tFacComment]);
            }

            saveMap.AuditMap(loggedInUser.PersonId);
            saveMap.SoftDeleteMap(loggedInUser.PersonId);
            return saveMap;
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

            //First Call and get all the data needed to run publish

            //Second call Publish to create the SpResults and StratResults data

            //Third go through SpResults and StratResults and create or modify the entities

            var pubWgs = WorkGroupPublish.GetPublishingWgData(svrWgIds, ctxManager);

            var infos = wgSaveMap.Single(map => map.Key == _tWg).Value;
            IEnumerable<PubWg> publishResults = null;

            try
            {
                publishResults = WorkGroupPublish.CalculateResults(pubWgs, svrWgIds, loggedInUser.PersonId);
            }

            catch (WorkGroupPublishException exception)

            {
                var error = infos.Select(
                    info => new EFEntityError(info, "Publication Error", exception.Message, "MpSpStatus"));
                throw new EntityErrorsException(error);
            }

            catch (MemberMissingPublishDataException exception)
            {
                var error = infos.Select(
                    info => new EFEntityError(info, "Publication Error", exception.Message, "MpSpStatus"));
                throw new EntityErrorsException(error);
            }

            var publishResultMap = ProcessEntitiesInPublishResults(publishResults, ctxManager, wgSaveMap);

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
                comment.FacultyPersonId = loggedInUser.PersonId;
            }
        }

        private SaveMap ProcessEntitiesInPublishResults(IEnumerable<PubWg> pubWgs, EFContextProvider<EcatContext> ctxProvider, SaveMap wgSaveMap)
        {
            var tSpResult = typeof(SpResult);
            var tStratResult = typeof(StratResult);
            //var tWg = typeof(WorkGroup);

            foreach (var workGroup in pubWgs)
            {
                foreach (var member in workGroup.PubWgMembers)
                {
                    var resultInfo = ctxProvider.CreateEntityInfo(member.SpResult,
                        member.HasSpResult ? Breeze.ContextProvider.EntityState.Modified : Breeze.ContextProvider.EntityState.Added);

                    resultInfo.ForceUpdate = member.HasSpResult;

                    if (!wgSaveMap.ContainsKey(tSpResult))
                    {
                        wgSaveMap[tSpResult] = new List<EntityInfo> { resultInfo };
                    }
                    else
                    {
                        wgSaveMap[tSpResult].Add(resultInfo);
                    }

                    var info = ctxProvider.CreateEntityInfo(member.StratResult,
                        member.HasStratResult ? Breeze.ContextProvider.EntityState.Modified : Breeze.ContextProvider.EntityState.Added);
                    info.ForceUpdate = member.HasStratResult;

                    if (!wgSaveMap.ContainsKey(tStratResult))
                    {
                        wgSaveMap[tStratResult] = new List<EntityInfo> { info };
                    }
                    else
                    {
                        wgSaveMap[tStratResult].Add(info);
                    }

                }
            }

            return wgSaveMap;
        }

        

    }
}
