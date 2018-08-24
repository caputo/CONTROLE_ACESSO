using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControleAcessoService.Mail
{
    public class MailConfig
    {
        public string host { get; set; }

        public Email sender { get; set; }

        public List<Email> to { get; set; }

        public string subject { get; set; }

        public string message { get; set; }
    }
}
