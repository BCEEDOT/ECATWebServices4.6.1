//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Breeze.ContextProvider;
//using Ecat.Shared.Core.Interface;
//using Ecat.Shared.Core.Logic;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.Utility;

//namespace Ecat.UserMod.Core
//{
//    using SaveMap = Dictionary<Type, List<EntityInfo>>;
//    public class GuardUserSave
//    {
//        private static readonly string[] LtiNotAllowedList = { "lastname", "firstname", "email" };
//        private readonly Person _currentUser;
//        private readonly bool _isLoggedIn;
//        private SaveMap _saveMapRef;
//        private SharedGuardian _sharedGuardian;

//        public GuardUserSave(Person loggedInUser)
//        {
//            _currentUser = loggedInUser;
//            _isLoggedIn = loggedInUser == null;
//        }

//        public SaveMap BeforeSaveEntities(SaveMap saveMap)
//        {
//            _saveMapRef = saveMap;

//            #region Handle Non-User-Guarded Types
//            var guardedObject = typeof(UserGuardAttribute);

//            var notValidObjects = saveMap.Where(map => !Attribute.IsDefined(map.Key, guardedObject)).ToList();

//            if (notValidObjects.Any())
//            {
//                saveMap = saveMap.Except(notValidObjects).ToDictionary(map => map.Key, map => map.Value);
//            }
//            #endregion

//            #region Handle Person Entity
//            var personEntity = saveMap.SingleOrDefault(map => map.Key == typeof(Person)).Key;

//            if (personEntity != default(Type))
//            {
//                saveMap[personEntity] = GuardPerson(saveMap[personEntity]);
//            }
//            #endregion

//            #region Handle Profile Entity
//            var profileEntity = saveMap.Where(map => map.Key.IsSubclassOf(typeof(ProfileBase)));

//            foreach (var profile in profileEntity)
//            {
//                saveMap[profile.Key] = GuardProfile(saveMap[profile.Key]);
//            }

//            #endregion

//            #region Handle Delete Entity

//            var mapWithDeletedEntities =
//                saveMap.SelectMany(map => map.Value).Where(info => info.EntityState == EntityState.Deleted).Select(info => info).ToList();

//            if (mapWithDeletedEntities.Any())
//            {
//                _sharedGuardian = _sharedGuardian ?? new SharedGuardian();
//                _sharedGuardian.DeleteGuardian(ref mapWithDeletedEntities, _currentUser);
//            }

//            #endregion

//            #region Handle Auditable Entity
//            var mapWithAuditableEntity =
//              saveMap.Where(map => typeof(IAuditable).IsAssignableFrom(map.Key)).SelectMany(info => info.Value).ToList();

//            if (mapWithAuditableEntity.Any())
//            {
//                _sharedGuardian = _sharedGuardian ?? new SharedGuardian();
//                _sharedGuardian.AuditableGuardian(ref mapWithAuditableEntity, _currentUser);
//            }
//            #endregion

//            return saveMap;
//        }

//        private List<EntityInfo> GuardPerson(List<EntityInfo> personInfos)
//        {
//            return null;
//        }

//        private List<EntityInfo> GuardLoggedIsAdmin(List<EntityInfo> personInfos)
//        {
//            return null;

//        }

//        private List<EntityInfo> GuardLoggedInPeron(List<EntityInfo> authPersonInfos)
//        {
//            return null;

//        }

//        private List<EntityInfo> GuardExternalPerson(List<EntityInfo> extPersonInfos)
//        {
//            return null;

//        }

//        private List<EntityInfo> GuardProfile(List<EntityInfo> profileInfos)
//        {
//            return null;

//        }

//        private List<EntityInfo> GuardDeletion(List<EntityInfo> deletionInfos)
//        {
//            return null;
//        }
//    }
//}
