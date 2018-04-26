using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Ecat.Shared.Core.ModelLibrary.Common;
//using Ecat.Shared.Core.Utility.Validation;
using Ecat.Data.Validation;
using Newtonsoft.Json;
using TypeLite;

namespace Ecat.Data.Models.User
{
    /// <summary>
    /// Represent the main user object for Ecat
    /// </summary>
    /// <remarks>
    /// Note! After making changes to any the property name's be sure to update user saveguard with the array of element LTI user are not allowed to changes such as Last, First name as these are controlled by the LMS.
    /// </remarks>
    /// <see cref="GuardUser.BeforeSaveEntities"/>

    [TsClass(Module="ecat.entity.s.user")]
    [PersonMappedPropValid]
    public class Person
    {
        public int PersonId { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore][TsIgnore]
        public string BbUserId { get; set; }

        [JsonIgnore] [TsIgnore]
        public string BbUserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AvatarLocation { get; set; }
        public string GoByName { get; set; }
        public string MpGender { get; set; }
        public string MpAffiliation { get; set; }
        public string MpPaygrade { get; set; }
        public string MpComponent { get; set; }
        public string Email { get; set; }
        public bool RegistrationComplete { get; set; }
        public string MpInstituteRole { get; set; }
        public virtual ProfileStudent Student { get; set; }
        public virtual ProfileFaculty Faculty { get; set; }
        public virtual ProfileDesigner Designer { get; set; }
        public virtual ProfileExternal External { get; set; }
        public virtual ProfileStaff HqStaff { get; set; }
        public ICollection<RoadRunner> RoadRunnerAddresses { get; set; }
        [TsIgnore]
        public virtual Security Security { get; set; }

        [JsonIgnore] [TsIgnore]
        public int? ModifiedById { get; set; }

        [JsonIgnore] [TsIgnore]
        public DateTime ModifiedDate { get; set; }
    }


}
