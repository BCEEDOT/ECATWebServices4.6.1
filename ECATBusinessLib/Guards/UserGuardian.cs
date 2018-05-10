using System;
using System.Collections.Generic;
using System.Linq;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
//using Ecat.Shared.Core.Interface;
//using Ecat.Shared.Core.Logic;
//using Ecat.Shared.Core.ModelLibrary.Learner;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.ModelLibrary.Cognitive;
using Ecat.Data.Models.Cognitive;
//using Ecat.Shared.Core.Utility;
using Ecat.Business.Utilities;
//using Ecat.Shared.DbMgr.Context;
using Ecat.Data.Contexts;

namespace Ecat.Business.Guards
{
    using SaveMap = Dictionary<Type, List<EntityInfo>>;
    public class UserGuardian
    {

        private readonly EFContextProvider<EcatContext> ctxManager;
        private readonly Person loggedInUser;
        private readonly Type _tPerson = typeof(Person);
        private readonly Type _tProfileExternal = typeof(ProfileExternal);
        private readonly Type _tProfileFaculty = typeof(ProfileFaculty);
        private readonly Type _tProfileStaff = typeof(ProfileStaff);
        private readonly Type _tProfileStudent = typeof(ProfileStudent);
        private readonly Type _tProfileSecurity = typeof(Security);
        private readonly Type _tCogResponse = typeof(CogResponse);
        private readonly Type _tCogEcpeResult = typeof(CogEcpeResult);
        private readonly Type _tCogEsalbResult = typeof(CogEsalbResult);
        private readonly Type _tCogEtmpreResult = typeof(CogEtmpreResult);
        private readonly Type _tCogEcmspeResult = typeof(CogEcmspeResult);
        private readonly Type _tRoadRunner = typeof(RoadRunner);

        public UserGuardian(EFContextProvider<EcatContext> efCtx, Person loggedIn)
        {
            ctxManager = efCtx;
            loggedInUser = loggedIn;
        }

        public SaveMap BeforeSaveEntities(SaveMap saveMap)
        {

            var unAuthorizedMaps = saveMap.Where(map => map.Key != _tPerson &&
                                                        map.Key != _tProfileExternal &&
                                                        map.Key != _tProfileFaculty &&
                                                        map.Key != _tProfileStudent &&
                                                        map.Key != _tProfileSecurity &&
                                                        map.Key != _tProfileStaff &&
                                                        map.Key != _tCogResponse &&
                                                        map.Key != _tCogEcpeResult &&
                                                        map.Key != _tCogEsalbResult &&
                                                        map.Key != _tCogEtmpreResult &&
                                                        map.Key != _tCogEcmspeResult &&
                                                        map.Key != _tRoadRunner) 
                                                        .ToList();

            saveMap.RemoveMaps(unAuthorizedMaps);

            saveMap.AuditMap(loggedInUser.PersonId);
            saveMap.SoftDeleteMap(loggedInUser.PersonId);
            return saveMap;
        }
    }
}
