using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace ControleAcessoService.Domain
{
    public class UserData : User
    {
        public UserData(IDictionary<String, Object> reader)
        {
            id = Convert.ToInt32(reader["ID"]);
            name = Convert.ToString(reader["USER"]);
            ips = Convert.ToString(reader["IPS"]);
            obs = Convert.ToString(reader["OBS"]);
            desc = Convert.ToString(reader["DESC"]);
            requests = Convert.ToInt32(reader["REQUESTS"]);
            legacy = Convert.ToBoolean(reader["LEGACY"]);
            enabled = Convert.ToBoolean(reader["ENABLED"]);
            blocked = Convert.ToBoolean(reader["BLOCKED"]);
            allowBlock = Convert.ToBoolean(reader["ALLOW_BLOCK"]);
            limit = Convert.ToInt32(reader["LIMIT"]);
            totalRequests = Convert.ToInt32(reader["TOTAL_REQUESTS"]);
        }
    }
}