using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.User;
using Ecat.Shared.Core.Utility;
using Newtonsoft.Json;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.Common
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
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string email { get; set; }
        public string mpAffiliation { get; set; }
        public string mpComponent { get; set; }
        public string mpPaygrade { get; set; }
        public string mpGender { get; set; }
        public string mpInstituteRole { get; set; }
        public bool registrationComplete { get; set; }
    }
}
