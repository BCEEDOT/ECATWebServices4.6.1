using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using Ecat.Shared.Core.ModelLibrary.User;
using Newtonsoft.Json.Linq;

namespace Ecat.UserMod.Core
{
    public interface IUserRepo
    {
        SaveResult ClientSaveChanges(JObject saveBundle, Person user);
        Task<int> CountEmails(string email);
        Task<Person> FindUser(int id);
        Task<List<object>> GetProfiles(int personId);
        string GetMetadata { get; }
        Task<Person> GetSecurityUserByEmail(string email);
        Task<Person> GetSecurityUserByBbId(string bbuid);
        Task<int> SaveUserChanges(Person person);
    }
}
