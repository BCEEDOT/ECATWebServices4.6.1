using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Ecat.Data.Contexts;
//using Ecat.Shared.Core.Logic;
//using Ecat.Shared.Core.ModelLibrary.Common;
using Ecat.Data.Models.Common;
//using Ecat.Shared.Core.ModelLibrary.Faculty;
//using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Data.Models.Student;
//using Ecat.Shared.Core.ModelLibrary.School;
using Ecat.Data.Models.School;
//using Ecat.Shared.Core.Utility;
using Ecat.Data.Static;
using Ecat.Data.Validation;

namespace Ecat.Business.Guards
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;

    public static class WorkGroupPublish
    {
        private static readonly Type tSpResult = typeof(SpResult);
        private static readonly Type tStratResult = typeof(StratResult);
        private static readonly Type tWg = typeof(WorkGroup);

        public static SaveMap Publish(SaveMap wgSaveMap, IEnumerable<int> svrWgIds, int loggedInPersonId,
            EFContextProvider<EcatContext> ctxProvider)
        {
            var infos = wgSaveMap.Single(map => map.Key == tWg).Value;

            //Database call
            var pubWgs = GetPublishingWgData(svrWgIds, ctxProvider);

            if (!AllWorkGroupsArePublished(pubWgs, svrWgIds))
            {
                var errorMessage = "Some groups were found to be unpublishable";
                var error = infos.Select(
                    info => new EFEntityError(info, "Publication Error", errorMessage, "MpSpStatus"));
                throw new EntityErrorsException(error);

            }

            foreach (var wg in pubWgs)
            {

                var stratScoreInterval = 1m / wg.PubWgMembers.Count();
                stratScoreInterval = decimal.Round(stratScoreInterval, 4);
                var membersStratKeeper = new List<PubWgMember>();
                var countOfGrp = wg.PubWgMembers.Count();
                var totalStratPos = 0;


                for (var i = 1; i <= countOfGrp; i++)
                {
                    totalStratPos += i;
                }

                foreach (var member in wg.PubWgMembers)
                {

                    if (!MemberHasAllData(member, totalStratPos))
                    {
                        var errorMessage =
                            $"There was a problem validating necessary information . Problem Flags Are: [Them => Me] NA: !{member.PeersDidNotAssessMe.Count()}, NS: {member.PeersDidNotStratMe.Any()} | [Me => Them] NA: {member.PeersIdidNotAssess.Count()}, NS: {member.PeersIdidNotStrat.Any()} | FacStrat: {member.FacStratPosition}";

                        var error = infos.Select(
                            info => new EFEntityError(info, "Publication Error", errorMessage, "MpSpStatus"));
                        throw new EntityErrorsException(error);

                    }

                    var spResult = CreateSpResultForMember(member, countOfGrp, wg.CourseId, wg.Id, wg.InstrumentId);

                    var resultInfo = ctxProvider.CreateEntityInfo(spResult,
                        member.HasSpResult ? EntityState.Modified : EntityState.Added);

                    resultInfo.ForceUpdate = member.HasSpResult;

                    if (!wgSaveMap.ContainsKey(tSpResult))
                    {
                        wgSaveMap[tSpResult] = new List<EntityInfo> {resultInfo};
                    }
                    else
                    {
                        wgSaveMap[tSpResult].Add(resultInfo);
                    }

                    
                    var stratResult = CreateStratResultForMember(member, stratScoreInterval, wg.CourseId, wg.Id, loggedInPersonId);

                    member.StratResult = stratResult;
                    membersStratKeeper.Add(member);
                }

                
                membersStratKeeper = UpdateOriginalStratPositionForMembers(membersStratKeeper);

                var fi = 0;
                var spInterval = wg.WgSpTopStrat / wg.StratDivisor;
                var facInterval = wg.WgFacTopStrat / wg.StratDivisor;

                foreach (
                    var member in
                    membersStratKeeper.OrderByDescending(sk => sk.StratResult.StratCummScore)
                        .ThenBy(sk => sk.FacStratPosition))
                {

                    var updatedMember = CalculateAwardedScoreForMember(member, wg, spInterval, facInterval, fi);

                    var info = ctxProvider.CreateEntityInfo(updatedMember.StratResult,
                        updatedMember.HasStratResult ? EntityState.Modified : EntityState.Added);
                    info.ForceUpdate = updatedMember.HasStratResult;

                    if (!wgSaveMap.ContainsKey(tStratResult))
                    {
                        wgSaveMap[tStratResult] = new List<EntityInfo> {info};
                    }
                    else
                    {
                        wgSaveMap[tStratResult].Add(info);
                    }

                    fi += 1;
                }
            }

            return wgSaveMap;
        }

        internal static PubWgMember CalculateAwardedScoreForMember(PubWgMember member, PubWg wg, decimal spInterval, decimal facInterval, int fi )
        {
            var studAwardedPoints = Math.Max(0, wg.WgSpTopStrat - spInterval * fi);
            var instrAwardPoints = Math.Max(0, wg.WgFacTopStrat - (facInterval * (member.FacStratPosition - 1)));

            member.StratResult.StudStratAwardedScore = studAwardedPoints;
            member.StratResult.FacStratAwardedScore = instrAwardPoints;
            member.StratResult.FinalStratPosition = fi + 1;

            return member;
        }

        internal static StratResult CreateStratResultForMember(PubWgMember member, decimal stratScoreInterval,  int courseId, int workGroupId, int loggedInPersonId)
        {
            var stratResult = new StratResult
            {
                CourseId = courseId,
                StudentId = member.StudentId,
                WorkGroupId = workGroupId,
                ModifiedById = loggedInPersonId,
                ModifiedDate = DateTime.Now,
                StratCummScore = decimal.Round(member.StratTable.Select(strat =>
                {
                    var multipler = 1 - (strat.Position - 1) * stratScoreInterval;
                    return multipler * strat.Count;
                }).Sum(), 4)
            };

            return stratResult;
        }

        internal static SpResult CreateSpResultForMember(PubWgMember member, int countOfGrp, int courseId, int workGroupId, int? instrumentId)
        {
            //var peerCount = countOfGrp - 1;
            var resultScore = ((decimal)member.SpResponseTotalScore / (member.CountSpResponses * 6)) * 100;
            var spResult = new SpResult
            {
                CourseId = courseId,
                WorkGroupId = workGroupId,
                StudentId = member.StudentId,
                AssignedInstrumentId = instrumentId,
                CompositeScore = (int)resultScore,
                BreakOut = new SpResultBreakOut
                {
                    NotDisplay = member.BreakOut.NotDisplayed,
                    IneffA = member.BreakOut.IneffA,
                    IneffU = member.BreakOut.IneffU,
                    EffA = member.BreakOut.EffA,
                    EffU = member.BreakOut.EffU,
                    HighEffA = member.BreakOut.HighEffA,
                    HighEffU = member.BreakOut.HighEffU
                },
                MpAssessResult = ConvertScoreToOutcome((int)resultScore),
            };

            return spResult;
        }

        private static string ConvertScoreToOutcome(int avgCompositeScore)
        {
            if (avgCompositeScore <= MpSpResultScore.Ie)
            {
                return MpAssessResult.Ie;
            }

            if (avgCompositeScore < MpSpResultScore.Bae)
            {
                return MpAssessResult.Bae;
            }

            if (avgCompositeScore < MpSpResultScore.E)
            {
                return MpAssessResult.E;
            }

            if (avgCompositeScore < MpSpResultScore.Aae)
            {
                return MpAssessResult.Aae;
            }

            return avgCompositeScore <= MpSpResultScore.He ? MpAssessResult.He : "Out of Range";
        }

        private static IEnumerable<PubWg> GetPublishingWgData(IEnumerable<int> wgIds,
            EFContextProvider<EcatContext> efCtx)
        {
            var ids = wgIds.ToList();

            var pubWgData = (from wg in efCtx.Context.WorkGroups
                let prundedGm = wg.GroupMembers.Where(gm => !gm.IsDeleted)
                where ids.Contains(wg.WorkGroupId) &&
                      wg.MpSpStatus == MpSpStatus.Reviewed &&
                      wg.SpComments.Where(spc => !spc.Author.IsDeleted && !spc.Recipient.IsDeleted)
                          .All(comment => comment.Flag.MpFaculty != null)
                select new PubWg
                {
                    Id = wg.WorkGroupId,
                    CourseId = wg.CourseId,
                    CountInventory = wg.AssignedSpInstr.InventoryCollection.Count,
                    InstrumentId = wg.AssignedSpInstrId,
                    WgSpTopStrat = wg.WgModel.MaxStratStudent,
                    WgFacTopStrat = wg.WgModel.MaxStratFaculty,
                    StratDivisor = wg.WgModel.StratDivisor,
                    PubWgMembers = wg.GroupMembers.Where(gm => !gm.IsDeleted).Select(gm => new PubWgMember
                    {
                        StudentId = gm.StudentId,
                        Name = gm.StudentProfile.Person.LastName + gm.StudentProfile.Person.FirstName,
                        CountSpResponses = gm.AssesseeSpResponses
                            .Count(response => response.AssessorPersonId != gm.StudentId),
                        SpResponseTotalScore = gm.AssesseeSpResponses
                            .Where(response => response.AssessorPersonId != gm.StudentId)
                            .Sum(response => response.ItemModelScore),
                        FacStratPosition = gm.FacultyStrat.StratPosition,
                        HasSpResult = wg.SpResults.Any(result => result.StudentId == gm.StudentId),
                        HasStratResult = wg.SpStratResults.Any(result => result.StudentId == gm.StudentId),
                        BreakOut = new PubWgBreakOut
                        {
                            NotDisplayed = gm.AssesseeSpResponses
                                .Where(response => response.AssessorPersonId != response.AssesseePersonId)
                                .Count(response => response.MpItemResponse == MpSpItemResponse.Nd),
                            IneffA = gm.AssesseeSpResponses
                                .Where(response => response.AssessorPersonId != response.AssesseePersonId)
                                .Count(response => response.MpItemResponse == MpSpItemResponse.Iea),
                            IneffU = gm.AssesseeSpResponses
                                .Where(response => response.AssessorPersonId != response.AssesseePersonId)
                                .Count(response => response.MpItemResponse == MpSpItemResponse.Ieu),
                            EffA = gm.AssesseeSpResponses
                                .Where(response => response.AssessorPersonId != response.AssesseePersonId)
                                .Count(response => response.MpItemResponse == MpSpItemResponse.Ea),
                            EffU = gm.AssesseeSpResponses
                                .Where(response => response.AssessorPersonId != response.AssesseePersonId)
                                .Count(response => response.MpItemResponse == MpSpItemResponse.Eu),
                            HighEffA = gm.AssesseeSpResponses
                                .Where(response => response.AssessorPersonId != response.AssesseePersonId)
                                .Count(response => response.MpItemResponse == MpSpItemResponse.Hea),
                            HighEffU = gm.AssesseeSpResponses
                                .Where(response => response.AssessorPersonId != response.AssesseePersonId)
                                .Count(response => response.MpItemResponse == MpSpItemResponse.Heu)
                        },
                        SelfStratPosition = gm.AssesseeStratResponse
                            .Where(strat => strat.AssessorPersonId == gm.StudentId)
                            .Select(strat => strat.StratPosition).FirstOrDefault(),
                        PubStratResponses =
                            gm.AssessorStratResponse.Where(strat => strat.AssesseePersonId != gm.StudentId)
                                .Select(strat => new PubStratResponse
                                {
                                    AssesseeId = strat.AssesseePersonId,
                                    StratPosition = strat.StratPosition
                                }),
                        PeersDidNotAssessMe = prundedGm.Where(peer => peer.AssessorSpResponses
                                                                          .Count(response =>
                                                                              response.AssesseePersonId ==
                                                                              gm.StudentId) == 0)
                            .Select(peer => peer.StudentId),
                        PeersIdidNotAssess = prundedGm.Where(peer => peer.AssesseeSpResponses
                                                                         .Count(response =>
                                                                             response.AssessorPersonId ==
                                                                             gm.StudentId) == 0)
                            .Select(peer => peer.StudentId),
                        PeersDidNotStratMe = prundedGm.Where(peer => peer.AssessorStratResponse
                                                                         .Count(strat =>
                                                                             strat.AssesseePersonId == gm.StudentId) ==
                                                                     0)
                            .Select(peer => peer.StudentId),
                        PeersIdidNotStrat = prundedGm.Where(peer => peer.AssesseeStratResponse
                                                                        .Count(strat =>
                                                                            strat.AssessorPersonId == gm.StudentId) ==
                                                                    0)
                            .Select(peer => peer.StudentId)
                    })
                }).ToList();

            if (!pubWgData.Any())
            {
                return null;
            }

            foreach (var wg in pubWgData)
            {

                var assesseStratDict = CreateAssesseeStratDictionaryForGroup(wg);

                foreach (var gm in wg.PubWgMembers)
                {
                    var myResponses = assesseStratDict[gm.StudentId];
                    gm.StratTable = myResponses.Select(mr => new PubWgStratTable
                    {
                        Position = mr.Key,
                        Count = mr.Value
                    });
                }
            }

            return pubWgData;
        }

        internal static Dictionary<int, Dictionary<int, int>> CreateAssesseeStratDictionaryForGroup(PubWg wg)
        {
            //Dictionary used to keep the assessee personid with an additional dictionary to keep the 
            //assessee strat position and the number of those position
            var assesseeStratDict = new Dictionary<int, Dictionary<int, int>>();

            foreach (var gm in wg.PubWgMembers)
            {

                foreach (var response in gm.PubStratResponses)
                {
                    Dictionary<int, int> assesseResponses;

                    var hasAssessee = assesseeStratDict.TryGetValue(response.AssesseeId, out assesseResponses);

                    var position = response.StratPosition > gm.SelfStratPosition
                        ? response.StratPosition - 1
                        : response.StratPosition;

                    if (!hasAssessee)
                    {
                        assesseeStratDict[response.AssesseeId] = new Dictionary<int, int>();
                    }

                    int stratPositionCount;
                    var hasStratPosition = assesseeStratDict[response.AssesseeId]
                        .TryGetValue(position, out stratPositionCount);

                    if (!hasStratPosition)
                    {
                        assesseeStratDict[response.AssesseeId][position] = 1;
                    }
                    else
                    {
                        assesseeStratDict[response.AssesseeId][position] = stratPositionCount += 1;
                    }

                }
            }

            return assesseeStratDict;

        }

        internal static bool AllWorkGroupsArePublished(IEnumerable<PubWg> pubWgs, IEnumerable<int> svrWgIds)
        {

            if (pubWgs == null)
            {

                return false;
            }

            if (pubWgs.Count() < svrWgIds.Count())
            {
                //var missingGrps = pubWgs.Where(wg => {

                //    if (svrWgIds.Contains(wg.Id))
                //    {
                //        return false;
                //    }

                //    return true;
                //});

                return false;

            }

            return true;

        }

        internal static bool MemberHasAllData(PubWgMember member, int totalStratPos)
        {
            var stratSum = member.PubStratResponses.Sum(strat => strat.StratPosition);
            stratSum += member.SelfStratPosition;


            if (member.PeersDidNotAssessMe.Any() || member.PeersIdidNotAssess.Any() ||
                member.PeersDidNotStratMe.Any() ||
                member.PeersIdidNotStrat.Any() || member.FacStratPosition == 0 || stratSum != totalStratPos)
            {
                return false;
            }

            return true;
        }

        internal static List<PubWgMember> UpdateOriginalStratPositionForMembers(List<PubWgMember> membersStratKeeper)
        {
            var cummScores = new List<decimal>();
            var oi = 1;

            foreach (var member in membersStratKeeper.OrderByDescending(sk => sk.StratResult.StratCummScore))
            {


                if (cummScores.Contains(member.StratResult.StratCummScore) || !cummScores.Any())
                {
                    member.StratResult.OriginalStratPosition = oi;
                    cummScores.Add(member.StratResult.StratCummScore);
                    continue;
                }

                cummScores.Add(member.StratResult.StratCummScore);

                oi += 1;
                member.StratResult.OriginalStratPosition = oi;
            }

            return membersStratKeeper;
        }

    }
}
