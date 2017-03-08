using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security;

namespace Emailer
{
    public class Emailer
    {
        public SmtpClient Smtp { get; private set; }
        public string FromAddress { get; private set; }
        public string UserName { get; set; }
        public SecureString Password { private get; set; }

        public Emailer(string fromAddress, string smtpServer, string username = null,  SecureString password = null, string host = null, int? port = null, bool ssl = false)
        {
            Smtp = new SmtpClient(smtpServer) {EnableSsl = true};
            FromAddress = fromAddress;
            UserName = username;
            Password = password;
            Smtp.Host = host ?? Smtp.Host;
            Smtp.Port = port ?? Smtp.Port;
            Smtp.EnableSsl = ssl;
        }

        /// <summary>Sends an email to one or more recipients</summary>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="toAddresses">Email addresses of recipients, e.g. bob@microsoft.com</param>
        public void Send(string subject, string body, IEnumerable<string> toAddresses)
        {
			var recipients = String.Join(",", toAddresses);

            // Clean any newlines or other whitespace then truncate to the recommended length limit
            // in RFC 5322: http://tools.ietf.org/html/rfc5322#section-2.1.1
            const int rfc5322RecommendedSubjectLengthLimit = 78;
            subject = string.Join(@" ", subject.Split(null));
            if (subject.Length > rfc5322RecommendedSubjectLengthLimit)
                subject = subject.Substring(0, rfc5322RecommendedSubjectLengthLimit - 3) + "...";

            var message = new MailMessage(FromAddress, recipients) { Subject = subject, Body = body };
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            if(UserName != null && Password != null)
                Smtp.Credentials = new NetworkCredential(UserName, Password);
            else
                Smtp.UseDefaultCredentials = true;
            Smtp.Send(message);
        }


        /// <summary>Sends an email to a single recipient</summary>
        /// <param name="subject">Email subjectemail</param>
        /// <param name="body">Email body</param>
        /// <param name="toAddress">Email address of recipient, e.g. bob@microsoft.com</param>
        public void Send(string subject, string body, string toAddress)
        {
            Send(subject, body, new List<string> { toAddress });
        }
    }
}
