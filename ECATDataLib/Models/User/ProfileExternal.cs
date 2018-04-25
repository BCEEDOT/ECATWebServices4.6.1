//using Ecat.Shared.Core.Interface;
using Ecat.Data.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TypeLite;

namespace Ecat.Data.Models.User
{
    [TsClass(Module = "ecat.entity.s.user")]
    public class ProfileExternal : IProfileBase
    {
        public int PersonId { get; set; }
        public string Bio { get; set; }
        public string HomeStation { get; set; }
        public Person Person { get; set; }
    }
}
