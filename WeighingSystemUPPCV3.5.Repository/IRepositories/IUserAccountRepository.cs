using WeighingSystemUPPCV3_5_Repository.Models;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IUserAccountRepository
    {
        IQueryable<UserAccount> Get(UserAccount parameters = null, bool includeUserAccountPermission = false);

        UserAccount Create(UserAccount model);

        UserAccount Update(UserAccount model);

        bool Delete(UserAccount model);

        bool BulkDelete(string[] id);

        UserAccount LogIn(LoginModel model);

        void LogOut();

        Dictionary<string, string> Validate(UserAccount model);

        bool ValidateUserName(UserAccount model);

        bool ValidateFullName(UserAccount model);
    }
}
