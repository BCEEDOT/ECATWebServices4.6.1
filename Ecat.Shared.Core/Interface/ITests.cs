using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.Shared.Core.Interface
{
    public interface ITests
    {
        string MvcTest { get; }
        IQueryable<Person> Persons();
    }
}
