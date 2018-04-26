using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.Utility;
using Ecat.Data.Static;
using Newtonsoft.Json;
using TypeLite;

namespace Ecat.Data.Models.Common
{
    [TsClass(Module = "ecat.entity.s.common")]
    public class IdToken
    {
        public int PersonId { get; set; }
        [JsonIgnore][TsIgnore]
        public RoleMap Role { get; set; }
        public string AuthToken { get; set; }
        public DateTime TokenExpireWarning { get; set; }
        public DateTime TokenExpire { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string MpAffiliation { get; set; }
        public string MpComponent { get; set; }
        public string MpPaygrade { get; set; }
        public string MpGender { get; set; }
        public string MpInstituteRole { get; set; }
        public bool RegistrationComplete { get; set; }
    }
}
