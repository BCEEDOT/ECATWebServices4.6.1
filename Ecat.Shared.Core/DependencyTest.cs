using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.Shared.Core
{
    public class DependencyTest : ITests
    {
        public string MvcTest => "We are here!!!";

        public IQueryable<Person> Persons ()
        {
            var p1 = new Person { PersonId = 1, LastName = "Person1" };
            var p2 = new Person { PersonId = 2, LastName = "Person 2" };
            var p3 = new Person { PersonId = 3, LastName = "person 3" };
            var listOfPeople = new List<Person> { p1, p2, p3 };

            return listOfPeople.AsQueryable();
        }
    }
}
