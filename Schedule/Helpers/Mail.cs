using System;
using System.Net;
using System.Net.Mail;


public static class Mail
{
    public static void SendMail(string subject, string message)
    {
        try
        {
            var fromAddress = new MailAddress("we.are.mere.team@gmail.com", "Schedule");
            var toAddress = new MailAddress("we.are.mere.team@gmail.com", "Schedule");
            const string fromPassword = "imagine_cup$2015";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var msg = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = message
            })
            {
                //You can also use SendAsync method instead of Send so your application begin invoking instead of waiting for send mail to complete. SendAsync(MailMessage, Object) :- Sends the specified e-mail message to an SMTP server for delivery. This method does not block the calling thread and allows the caller to pass an object to the method that is invoked when the operation completes. 
                smtp.Send(msg);
            }
        }
        catch (Exception e)
        {
            //throw new Exception("Mail.Send: " + e.Message);
        }
    }
}