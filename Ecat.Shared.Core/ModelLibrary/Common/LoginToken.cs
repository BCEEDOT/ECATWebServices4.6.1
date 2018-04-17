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
    public class LoginToken
    {
        public int PersonId { get; set; }
        [JsonIgnore][TsIgnore]
        public RoleMap Role { get; set; }
        public string AuthToken { get; set; }
        public DateTime TokenExpireWarning { get; set; }
        public DateTime TokenExpire { get; set; }
        public Person Person { get; set; }
    }
}
