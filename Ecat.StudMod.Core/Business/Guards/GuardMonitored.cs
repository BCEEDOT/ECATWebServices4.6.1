using System.Collections.Generic;
using System.Linq;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Ecat.Shared.DbMgr.Context;

namespace Ecat.StudMod.Core.Guards
{
    public class GuardMonitored
    {
        private EFContextProvider<EcatContext> _efCtx; 

        public GuardMonitored(EFContextProvider<EcatContext> efCtx)
        {
            _efCtx = efCtx;
        }

        public void ProcessCourseMonitoredMaps(List<EntityInfo> infos)
        {
            var courseMonitorEntities = infos.Select(info => info.Entity).OfType<ICourseMonitored>();
            var courseIds = courseMonitorEntities.Select(cme => cme.CourseId);

            var pubCrseId = _efCtx.Context.Courses
                .Where(crse => courseIds.Contains(crse.Id) && crse.GradReportPublished)
                .Select(crse => crse.Id);

            if (!pubCrseId.Any()) return;

            var errors = from info in infos
                         let crseEntity = info.Entity as ICourseMonitored
                         where crseEntity != null && pubCrseId.Contains(crseEntity.CourseId)
                         select new EFEntityError(info, MpEntityError.CrseNotOpen,
                                     "There was a problem saving the requested items", "Course");

            throw new EntityErrorsException(errors);
        }

        public void ProcessWorkGroupMonitoredMaps(List<EntityInfo> infos)
        {
            var wgMonitorEntities = infos.Select(info => info.Entity).OfType<IWorkGroupMonitored>();
            var wgIds = wgMonitorEntities.Select(wgme => wgme.WorkGroupId);

            var pubWgIds = _efCtx.Context.WorkGroups
                .Where(wg => wgIds.Contains(wg.WorkGroupId) && wg.MpSpStatus != MpSpStatus.Open)
                .Select(wg => wg.WorkGroupId);

            if (!pubWgIds.Any()) return;

            var errors = from info in infos
                         let wgEntity = info.Entity as IWorkGroupMonitored
                         where wgEntity != null && pubWgIds.Contains(wgEntity.WorkGroupId)
                         select new EFEntityError(info, MpEntityError.WgNotOpen,
                                     "There was a problem saving the requested items, the workGroup is in a non-open state!", "WorkGroup");

            throw new EntityErrorsException(errors);
        }

    }
}
