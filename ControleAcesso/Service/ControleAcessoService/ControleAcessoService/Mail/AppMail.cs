using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;
using ControleAcessoService.Domain;

namespace ControleAcessoService.Mail
{
    public class AppMail
    {
        private MailConfig config;

        public AppMail(MailConfig mailConfig)
        {
            this.config = mailConfig;
        }

        public void sendMail(User user)
        {
            try
            {
               
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.Sender = new System.Net.Mail.MailAddress(config.sender.email, config.sender.name);
                message.From = new System.Net.Mail.MailAddress(config.sender.email, config.sender.name);

                foreach(Email to in config.to){
                    message.To.Add(new System.Net.Mail.MailAddress(to.email, to.name));
                }
               
                message.Subject = config.subject;
                message.Body = String.Format(config.message,user.name,user.requests,user.limit);
                message.IsBodyHtml = true;

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Host = config.host;
                client.Send(message);

                
            } 
            catch(Exception e)
            {
                
            }
            
        }
    }
}
