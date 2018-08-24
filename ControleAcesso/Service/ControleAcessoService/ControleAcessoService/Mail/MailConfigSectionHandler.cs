using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ControleAcessoService.Mail
{
    public class MailConfigSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            
            MailConfig mailConfig = new MailConfig();
            mailConfig.host = section.SelectSingleNode("host").Attributes["value"].InnerText;

            Email sender = new Email(section.SelectSingleNode("sender").Attributes["email"].InnerText,
                section.SelectSingleNode("sender").Attributes["name"].InnerText);
            mailConfig.sender = sender;

            List<Email> to = new List<Email>();
            System.Xml.XmlNodeList processesNodes = section.SelectNodes("to");

            foreach (XmlNode processNode in processesNodes)
            {
                Email email = new Email(processNode.Attributes["email"].InnerText, processNode.Attributes["name"].InnerText);
                to.Add(email);
            }

            mailConfig.to = to;
            mailConfig.subject = section.SelectSingleNode("subject").Attributes["value"].InnerText;
            mailConfig.message = section.SelectSingleNode("message").Attributes["value"].InnerText;
            
            return mailConfig;
        }

        #endregion
    }
}
