using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
//using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Data.Models.User;
//using Ecat.Shared.Core.ModelLibrary.Designer;
using Ecat.Data.Models.Designer;
//using Ecat.Shared.Core.ModelLibrary.Cognitive;
//using Ecat.Shared.Core.Provider;
//using Ecat.Shared.Core.Utility;
using Ecat.Business.Utilities;
//using Ecat.Shared.DbMgr.Context;
using Ecat.Data.Contexts;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Static;
using Ecat.Business.Guards;
using LtiLibrary.Core.Lti1;
using Newtonsoft.Json.Linq;

namespace Ecat.Business.Repositories
{
    public class UserRepo : IUserRepo
    {
        public Person User { get; set; }

        private readonly EFContextProvider<EcatContext> _efCtx;
    

        public UserRepo(EFContextProvider<EcatContext> efCtx)
        {
            _efCtx = efCtx;
        }

        public string Metadata
        {
            get
            {
                      

                var metaCtx = new EFContextProvider<UserMetadataCtx>();
                return metaCtx.Metadata();
            }
        }

        public IEnumerable<Person> GetUsers()
        {
            return _efCtx.Context.People;
        }

        public async Task<List<object>> GetProfile() {
            var userWithProfiles = await _efCtx.Context.People.Where(p => p.PersonId == User.PersonId)
                .Include(p => p.Student)
                .Include(p => p.Faculty)
                .Include(p => p.Designer)
                .Include(p => p.External)
                .Include(p => p.HqStaff).SingleAsync();

            var profiles = new List<object>();

            if (userWithProfiles.Student != null) { profiles.Add(userWithProfiles.Student); }
            if (userWithProfiles.Faculty != null) { profiles.Add(userWithProfiles.Faculty); }
            if (userWithProfiles.External != null) { profiles.Add(userWithProfiles.External); }
            if (userWithProfiles.Designer != null) { profiles.Add(userWithProfiles.Designer); }
            if (userWithProfiles.HqStaff != null) { profiles.Add(userWithProfiles.HqStaff); }

            return profiles;
        }

        public async Task<Person> LoginUser(string userName, string password)
        {
            var user = await _efCtx.Context.People
                .Include(s => s.Security)
                .Include(s => s.Faculty)
                .Include(s => s.Student)
                .SingleOrDefaultAsync(person => person.Email.ToLower() == userName.ToLower());

            if (user == default(Person))
            {
                throw new UnauthorizedAccessException("Invalid Username/Password Combination");
            }

            var isValid = PasswordHash.ValidatePassword(password, user.Security.PasswordHash);

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid Username/Password Combination");
            }

            return user;
        }

        public async Task<Person> ProcessLtiUser(ILtiRequest parsedRequest)
        {
            var user = await _efCtx.Context.People
             .Include(s => s.Security)
             .Include(s => s.Faculty)
             .Include(s => s.Student)
             .SingleOrDefaultAsync(person => person.BbUserId == parsedRequest.UserId);

            var emailChecker = new ValidEmailChecker();
            if (user != null)
            {
                if (user.Email.ToLower() != parsedRequest.LisPersonEmailPrimary.ToLower()) {
                    if (!emailChecker.IsValidEmail(parsedRequest.LisPersonEmailPrimary)) {
                        throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is not a valid email address.");
                    }
                    if (!await UniqueEmailCheck(parsedRequest.LisPersonEmailPrimary)) {
                        throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is already being used in ECAT.");
                    }
                }
                user.ModifiedById = user.PersonId;
            }
            else
            {
                if (!emailChecker.IsValidEmail(parsedRequest.LisPersonEmailPrimary))
                {
                    throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is not a valid email address.");
                }
                if (!await UniqueEmailCheck(parsedRequest.LisPersonEmailPrimary)) {
                    throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is already being used in ECAT.");
                }

                user = new Person
                {
                    IsActive = true,
                    MpGender = MpGender.Unk,
                    MpAffiliation = MpAffiliation.Unk,
                    MpComponent = MpComponent.Unk,
                    MpPaygrade = MpPaygrade.Unk,
                    MpInstituteRole = MpInstituteRoleId.Undefined,
                    RegistrationComplete = false
                };

                _efCtx.Context.People.Add(user);
            }

            var userIsCourseAdmin = false;

            switch (parsedRequest.Parameters["Roles"].ToLower())
            {
                case "instructor":
                    userIsCourseAdmin = true;
                    user.MpInstituteRole = MpInstituteRoleId.Faculty;
                    break;
                case "teachingassistant":
                    user.MpInstituteRole = MpInstituteRoleId.Faculty;
                    break;
                case "contentdeveloper":
                    user.MpInstituteRole = MpInstituteRoleId.Designer;
                    break;
                default:
                    user.MpInstituteRole = MpInstituteRoleId.Student;
                    break;
            }

            switch (user.MpInstituteRole)
            {
                case MpInstituteRoleId.Faculty:
                    user.Faculty = user.Faculty ?? new ProfileFaculty();
                    user.Faculty.IsCourseAdmin = userIsCourseAdmin;
                    user.Faculty.AcademyId = parsedRequest.Parameters["custom_ecat_school"];
                    break;
                case MpInstituteRoleId.Designer:
                    user.Designer = user.Designer ?? new ProfileDesigner();
                    user.Designer.AssociatedAcademyId = parsedRequest.Parameters["custom_ecat_school"];
                    break;
                default:
                    user.Student = user.Student ?? new ProfileStudent();
                    break;
            }

            user.IsActive = true;
            user.Email = parsedRequest.LisPersonEmailPrimary.ToLower();
            user.LastName = parsedRequest.LisPersonNameFamily;
            user.FirstName = parsedRequest.LisPersonNameGiven;
            user.BbUserId = parsedRequest.UserId;
            user.ModifiedDate = DateTime.Now;

            if (await _efCtx.Context.SaveChangesAsync() > 0)
            {
                return user;
            }

            throw new UserUpdateException("Save User Changes did not succeed!");
        }

        public async Task<bool> UniqueEmailCheck(string email) => await _efCtx.Context.People.CountAsync(user => user.Email.ToLower() == email.ToLower()) == 0;

        public async Task<CogInstrument> GetCogInst(string type)
        {
            return await _efCtx.Context.CogInstruments
                .Where(cog => cog.MpCogInstrumentType == type && cog.IsActive == true)
                .Include(cog => cog.InventoryCollection)
                .FirstOrDefaultAsync();
        }

        public async Task<List<object>> GetCogResults(bool? all)
        {
            var results = new List<object>();

            var ecpe = await _efCtx.Context.CogEcpeResult
                .Where(cog => cog.PersonId == User.PersonId)
                .Include(cog => cog.Instrument)
                .OrderByDescending(cog => cog.Attempt)
                .ToListAsync();

            var etmpre = await _efCtx.Context.CogEtmpreResult
                .Where(cog => cog.PersonId == User.PersonId)
                .Include(cog => cog.Instrument)
                .OrderByDescending(cog => cog.Attempt)
                .ToListAsync();

            var esalb = await _efCtx.Context.CogEsalbResult
                .Where(cog => cog.PersonId == User.PersonId)
                .Include(cog => cog.Instrument)
                .OrderByDescending(cog => cog.Attempt)
                .ToListAsync();

            var ecmspe = await _efCtx.Context.CogEcmspeResult
                .Where(cog => cog.PersonId == User.PersonId)
                .Include(cog => cog.Instrument)
                .OrderByDescending(cog => cog.Attempt)
                .ToListAsync();

            if (all == null || all == false)
            {
                if (ecpe.Any()) { results.Add(ecpe[0]); }
                if (etmpre.Any()) { results.Add(etmpre[0]); }
                if (esalb.Any()) { results.Add(esalb[0]); }
                if (ecmspe.Any()) { results.Add(ecmspe[0]); }
            }
            else {
                if (ecpe.Any()) { results.AddRange(ecpe); }
                if (etmpre.Any()) { results.AddRange(etmpre); }
                if (esalb.Any()) { results.AddRange(esalb); }
                if (ecmspe.Any()) { results.AddRange(ecmspe); }
            }
            
            return results;
        }

        public SaveResult ClientSave(JObject saveBundle)
        {
            var guardian = new UserGuardian(_efCtx, User);
            _efCtx.BeforeSaveEntitiesDelegate += guardian.BeforeSaveEntities;
            return _efCtx.SaveChanges(saveBundle);
        }

        //retrieves all roadrunner info on user that is logged in
        public async Task<List<RoadRunner>> GetRoadRunnerInfo(){
            var personList = new List<RoadRunner>();
            personList = await _efCtx.Context.RoadRunnerAddresses.Where(e => e.PersonId == User.PersonId).ToListAsync();
            return personList;
        }
    }
}
