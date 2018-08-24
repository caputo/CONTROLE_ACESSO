using ControleAcessoService.DataAccess;
using ControleAcessoService.Domain;
using ControleAcessoService.Mail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ControleAcessoService.Repository
{
    public class UserRepository
    {
        public IList<User> GetAll()
        {
            try
            {
                SQLiteConnection.ClearAllPools();
                IList<IDictionary<String, object>> dic = SQLiteAccess.Select("SELECT ID, USER, OBS, [DESC], LEGACY, IPS, ENABLED, ALLOW_BLOCK, BLOCKED, [LIMIT], REQUESTS FROM USERS");
                IList<User> users = new List<User>();
                foreach(IDictionary<String, object> row in dic)
                {
                    var user = new UserData(row);
                    users.Add(user);
                }
                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User GetById(int id)
        {
            try
            {
                SQLiteConnection.ClearAllPools();
                IList<IDictionary<String, object>> dic = SQLiteAccess.Select("SELECT ID, USER, OBS, [DESC], LEGACY, IPS,ENABLED, ALLOW_BLOCK, BLOCKED, [LIMIT], REQUESTS, TOTAL_REQUESTS FROM USERS WHERE ID=" + id);
                var user = new UserData(dic[0]);
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void UserValidation(User user)
        {
            try
            {
                if (!String.IsNullOrEmpty(user.ips))
                {
                    var userIpsTemp = user.ips.Split(';');
                    if (userIpsTemp.Count() > 32)
                    {
                        throw new Exception("A lista de IPs excede a quantidade permitidade de IPs");
                    }
                    if (!userIpsTemp.All(n => (n.Length < 16)))
                    {
                        throw new Exception("A lista de IPs não está no padrão da máscara de IP");
                    }
                }
                if (String.IsNullOrEmpty(user.name))
                {
                    throw new Exception("O nome não pode estar em branco");
                }
                else
                {
                    if (user.name.Length > 256)
                    {
                        throw new Exception("O nome ultrapassa o tamanho permitido");
                    }
                }

                if (user.limit > int.MaxValue)
                {
                    throw new Exception("O limite ultrapassa o valor permitido");
                }

                if (!String.IsNullOrEmpty(user.obs))
                {
                    if (user.obs.Length > 1024)
                    {
                        throw new Exception("A observação ultrapassa o tamanho permitido");
                    }
                }

                if (!String.IsNullOrEmpty(user.desc))
                {
                    if (user.desc.Length > 1024)
                    {
                        throw new Exception("A descrição ultrapassa o tamanho permitido");
                    }
                }

                if (user.requests > int.MaxValue)
                {
                    throw new Exception("O número de requisições ultrapassa o valor permitido");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        public void SaveUser(User user)
        {
            try
            {
                SQLiteConnection.ClearAllPools();
                var query = "";
                if (user.id > 0)
                {
                    query = $@"UPDATE USERS SET USER = '{user.name}', OBS = '{user.obs}', [DESC] = '{user.desc}', LEGACY = {Convert.ToInt32(user.legacy)}, IPS = '{user.ips}', ENABLED = {Convert.ToInt32(user.enabled)}, ALLOW_BLOCK = {Convert.ToInt32(user.allowBlock)}, BLOCKED = {Convert.ToInt32(user.blocked)}, [LIMIT] = {user.limit}, REQUESTS = {user.requests} WHERE ID = {user.id}";
                }
                else
                {
                    query = $@"INSERT INTO USERS (USER, OBS, [DESC], LEGACY, IPS, ENABLED, ALLOW_BLOCK, BLOCKED, [LIMIT], REQUESTS)
                                     VALUES ('{user.name}', '{user.obs}', '{user.desc}', {Convert.ToInt32(user.legacy)}, '{user.ips}', {Convert.ToInt32(user.enabled)}, {Convert.ToInt32(user.allowBlock)},
                                              {Convert.ToInt32(user.blocked)}, {user.limit}, {user.requests})";
                }
                SQLiteAccess.ExecuteQuery(query);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SaveUserTotalRequests(User user)
        {
            try
            {
                SQLiteConnection.ClearAllPools();
                var query = $@"UPDATE USERS SET TOTAL_REQUESTS = {user.totalRequests} WHERE ID = {user.id}";
                SQLiteAccess.ExecuteQuery(query);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }




        public void DeleteUser(int id)
        {
            try
            {
                SQLiteConnection.ClearAllPools();
                if (id > 0)
                {
                    var query = $@"DELETE FROM USERS WHERE ID = {id}";
                    SQLiteAccess.ExecuteQuery(query);
                }
                else
                {
                    throw new Exception("Selecione um usuário");
                }              

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void UserRequestValidation(User user)
        {
            // Verificar se usuário está habilitado - enabled
            if (!user.enabled)
            {
                throw new Exception("O usuário não está habilitado");
            }
            // Verificar se pode bloquear usuário - allowBlock
            if (user.allowBlock)
            {
                bool isOK = true; 
                var msg = "";
                // Verificar se usuário está bloqueado - blocked
                if (user.blocked)
                {
                    msg = "O usuário está bloqueado";
                    isOK = false;
                }
                // Verificar se o número de requisições atual está igual ao limite requests >= limit
                if (user.requests >= user.limit)
                {
                    msg = msg + " - o número limite de requisições já foi alcançado";
                    isOK = false;
                }
                if (!isOK)
                {
                    throw new Exception(msg);
                }
            }
        }

        public User AddUserRequest(User user)
        {
            // Verificar se usuário está habilitado - enabled
            if (!user.enabled)
            {
                throw new Exception("O usuário não está habilitado");
            }
            // Incrementar o número de requisições - request ++
            user.requests = user.requests + 1;

            return user;
        }

        public User AddUserTotalRequest(User user, int value)
        {
            if (!user.enabled)  
            {
                throw new Exception("O usuário não está habilitado");
            }

            user.totalRequests += value;
                        
            // Verificar se pode bloquear usuário - allowBlock
            if (user.allowBlock)
            {
                // Caso o número de requisições esteja maior ou igual ao limite, usuário deverá ser bloqueado
                user.blocked = (user.totalRequests >= user.limit);
                /*if (user.blocked && ConfigurationManager.AppSettings["EmailAlert"].Equals("true")) {
                    sendEmail(user);
                }*/
            }
            return user;
        }

        public void BlockInAllServers(User user, String server)
        {
            try
            { 
                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(user);
                    client.BaseAddress = new Uri(server);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client.PostAsync("users/BlockUser", new StringContent(json, Encoding.UTF8, "application/json"));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void sendEmail(User user) {
            AppMail mail = new AppMail((Mail.MailConfig)ConfigurationManager.GetSection("mailConfig"));
            mail.sendMail(user);
        }
    }
}