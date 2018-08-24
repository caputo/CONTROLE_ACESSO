using DailyCleaning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyCleaning.Core
{
    public class UsersClearning
    {
        public UsersClearning()
        {
            try
            {
                var usersModel = new UsersModel();
                usersModel.UsersCleaning();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
