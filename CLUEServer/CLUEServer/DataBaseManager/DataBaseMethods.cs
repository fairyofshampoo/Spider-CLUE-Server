using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager
{
    public class DataBaseMethods
    {
        public bool Authenticate(String email, String password)
        {
            using (SpiderClueEntities context = new SpiderClueEntities())
            {
                var existingAccount = context.AccessAccounts.FirstOrDefault(accessAccount => accessAccount.email == email);
                return existingAccount != null && BCrypt.Net.BCrypt.Verify(password, existingAccount.password);
            }
        }
    }
}
