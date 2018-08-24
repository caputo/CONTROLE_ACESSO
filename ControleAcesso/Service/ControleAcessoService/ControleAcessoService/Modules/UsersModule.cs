using ControleAcessoService.Domain;
using ControleAcessoService.Repository;
using Nancy;
using Nancy.Extensions;
using Nancy.Json;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ControleAcessoService.Modules
{
    public class UsersModule: NancyModule
    {
        protected JavaScriptSerializer Json;
        public UsersModule(): base("/users") {
            Json = new JavaScriptSerializer();
            Json.MaxJsonLength = int.MaxValue;
            Get["/"] = parameters =>
            {
                UserRepository rep = new UserRepository();
                return Json.Serialize(rep.GetAll());                
            };
            Get["/{id}"] = parameters =>
            {
                UserRepository rep = new UserRepository();
                return Json.Serialize(rep.GetById(parameters.id));
            };
            Post["/SaveUser"] = parameters =>
            {
                try
                {
                    UserRepository rep = new UserRepository();
                    var user = this.Bind<User>();
                    rep.UserValidation(user);
                    rep.SaveUser(user);
                    return HttpStatusCode.OK;
                }
                catch(Exception ex)
                {
                    var response = Response.AsJson<string>(ex.Message);
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    return response;
                }
            };
            Post["/DeleteUser"] = parameters =>
            {
                try
                {
                    UserRepository rep = new UserRepository();
                    var user = this.Bind<User>();
                    rep.DeleteUser(user.id);
                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    var response = Response.AsJson<string>(ex.Message);
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    return response;
                }
            };
            Post["/{id}/addrequest"] = parameters => {
                try
                {
                    UserRepository rep = new UserRepository();
                    var user = rep.GetById(parameters.id);
                    rep.UserRequestValidation(user);
                    user = rep.AddUserRequest(user);
                    rep.SaveUser(user);
                        
                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    var response = Response.AsJson<string>(ex.Message);
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    return response;
                }
            };
            Post["/Consolidate"] = parameters => {
                try
                {
                    dynamic requests = JsonConvert.DeserializeObject(Request.Body.AsString());
                    foreach (var request in requests)
                    {
                        UserRepository rep = new UserRepository();
                        User user = rep.GetById((int)request.id.Value);
                        user = rep.AddUserTotalRequest(user, (int)request.value.Value);
                        rep.SaveUser(user);

                        if (user.blocked)
                        {
                            String servers = ConfigurationManager.AppSettings["ServersReplication"].ToString();
                            foreach (var server in servers.Split(';'))
                            {
                                rep.BlockInAllServers(user, server);
                            }

                        }
                    }

                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    var response = Response.AsJson<string>(ex.Message);
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    return response;
                }
            };


            Post["/BlockUser"] = parameters => {
                try
                {
                    dynamic userJson = JsonConvert.DeserializeObject(Request.Body.AsString());
                    UserRepository rep = new UserRepository();
                    User user = rep.GetById((int)userJson.id.Value);
                    user.blocked = true;
                    rep.SaveUser(user);
                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    var response = Response.AsJson<string>(ex.Message);
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    return response;
                }
            };

        }
    }
}