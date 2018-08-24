using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace ControleAcessoService.Domain
{
    public class User
    {

        public User() {

        }

        public virtual Int32 id { get; set; }
        public virtual String name { get; set; }
        public virtual String obs { get; set; }
        public virtual String desc { get; set; }
        public virtual Boolean legacy { get; set; }
        public virtual string ips { get; set; }
        public virtual Boolean enabled { get; set; }
        public virtual Boolean allowBlock { get; set; }
        public virtual Boolean blocked { get; set; }
        public virtual Int32 limit { get; set; }
        public virtual Int32 requests { get; set; }
        public virtual Int32 totalRequests { get; set; }
    }
}