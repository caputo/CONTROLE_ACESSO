using Newtonsoft.Json;
using RequestConsolidate.DataAccess;
using RequestConsolidate.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestConsolidate.Core
{
    public class Consolidate
    {
        public Consolidate(bool timer)
        {
            if (timer)
            {
                try
                {
                    int time = Convert.ToInt32(ConfigurationManager.AppSettings["TimeSendRequest"]);
                    while (true)
                    {
                        List<UserRequest> usersRequests = GetRequestsUsersFromDatabases();
                        Task.WaitAll(SendRequests(usersRequests));
                        Thread.Sleep(time);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    

        public List<UserRequest> GetRequestsUsersFromDatabases()
        {
            try
            {
                var usersRequests = new List<UserRequest>();
                foreach (string connectionString in SQLiteAccess.GetConnections())
                {
                    IList<IDictionary<String, object>> dic = SQLiteAccess.Select("SELECT ID, REQUESTS FROM USERS WHERE ENABLED = 1", connectionString);
                    foreach (IDictionary<String, object> row in dic)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        int value = Convert.ToInt32(row["REQUESTS"]);
                        if (usersRequests.Any(x => x.id == id))
                        {
                            usersRequests.Where(x => x.id == id).ToList().ForEach(s => s.value += value);
                        }
                        else
                        {
                            var userRequest = new UserRequest();
                            userRequest.id = id;
                            userRequest.value = value;
                            usersRequests.Add(userRequest);
                        }

                    }
                }
                return usersRequests;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SendRequests(List<UserRequest> usersRequests)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(usersRequests);
                    client.BaseAddress = new Uri("http://localhost:44091/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync("users/Consolidate", new StringContent(json, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        CleanRequests();
                        return true;
                    }else
                    {
                        return false;
                    }
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void CleanRequests()
        {
            var query = "";
            query = $@"UPDATE USERS SET BLOCKED = 'false', REQUESTS = 0";
            SQLiteAccess.ExecuteQuery(query);
        }

    }
}
