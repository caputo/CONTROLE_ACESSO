using DailyCleaning.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyCleaning.Model
{
    public class UsersModel
    {
        public void UsersCleaning()
        {
            try
            {
                var query = $@"UPDATE USERS SET BLOCKED = 'false', REQUESTS = 0";
                SQLiteAccess.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
