using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TypeLite;

namespace Ecat.Shared.Core.ModelLibrary.User
{
    [TsIgnore]
    public class Security
    {
        public int PersonId { get; set; }
        public int BadPasswordCount { get; set; }
        public string PasswordHash { get; set; }
        public Person Person { get; set; }
        public int? ModifiedById  { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
