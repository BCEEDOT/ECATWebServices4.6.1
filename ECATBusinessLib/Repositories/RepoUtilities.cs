using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider.EF6;
using Ecat.Data.Contexts;
using Ecat.Data.Models.Common;

namespace Ecat.Business.Repositories
{
    public class RepoUtilities
    {

        public static async Task RemoveAllGroupMembershipData(EFContextProvider<EcatContext> ctxManager, int studentId, int workGroupId)
        {

            var authorCommentFlags = await ctxManager.Context.StudSpCommentFlags
                                                   .Where(sscf => sscf.AuthorPersonId == studentId)
                                                   .Where(sscf => sscf.WorkGroupId == workGroupId).ToListAsync();

            var recipientCommentFlags = await ctxManager.Context.StudSpCommentFlags
                                        .Where(sscf => sscf.RecipientPersonId == studentId)
                                        .Where(sscf => sscf.WorkGroupId == workGroupId).ToListAsync();

            var authorOfComments = await ctxManager.Context.StudSpComments
                                    .Where(ssc => ssc.AuthorPersonId == studentId)
                                    .Where(ssc => ssc.WorkGroupId == workGroupId).ToListAsync();

            var recipientOfComments = await ctxManager.Context.StudSpComments
                                        .Where(ssc => ssc.RecipientPersonId == studentId)
                                        .Where(ssc => ssc.WorkGroupId == workGroupId).ToListAsync();

            var assesseeSpResponses = await ctxManager.Context.SpResponses
                                    .Where(sr => sr.AssesseePersonId == studentId)
                                    .Where(sr => sr.WorkGroupId == workGroupId).ToListAsync();

            var assessorSpResponses = await ctxManager.Context.SpResponses
                                        .Where(sr => sr.AssessorPersonId == studentId)
                                        .Where(sr => sr.WorkGroupId == workGroupId).ToListAsync();

            var assesseeStratResponses = await ctxManager.Context.SpStratResponses
                                            .Where(ssr => ssr.AssesseePersonId == studentId)
                                            .Where(ssr => ssr.WorkGroupId == workGroupId).ToListAsync();

            var assessorStratResponses = await ctxManager.Context.SpStratResponses
                                            .Where(ssr => ssr.AssessorPersonId == studentId)
                                            .Where(ssr => ssr.WorkGroupId == workGroupId).ToListAsync();

            var facSpResponses = await ctxManager.Context.FacSpResponses
                                            .Where(fsr => fsr.AssesseePersonId == studentId)
                                            .Where(fsr => fsr.WorkGroupId == workGroupId).ToListAsync();

            var facStratResponse = await ctxManager.Context.FacStratResponses
                                            .Where(fsr => fsr.AssesseePersonId == studentId)
                                            .Where(fsr => fsr.WorkGroupId == workGroupId).ToListAsync();

            var facComments = await ctxManager.Context.FacSpComments
                                            .Where(fsc => fsc.RecipientPersonId == studentId)
                                            .Where(fsc => fsc.WorkGroupId == workGroupId).ToListAsync();

            var facCommentsFlag = await ctxManager.Context.FacSpCommentFlags
                                            .Where(fscf => fscf.RecipientPersonId == studentId)
                                            .Where(fscf => fscf.WorkGroupId == workGroupId).ToListAsync();


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

        }
    }
}
