using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControleAcessoService.Mail
{
    public class Email
    {
        public string email { get; set; }

        public string name { get; set; }

        public Email(string email, string name)
        {
            this.email = email;
            this.name = name;
        }
    }
}
