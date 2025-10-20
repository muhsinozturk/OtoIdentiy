
using Application.Abstractions.Services;

using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Domain.OptionsModels;

namespace Application.Services;

public class EmailService : IEmailService
{

    private readonly EmailSettings _emailSettings;
    public EmailService(IOptions<EmailSettings> options)
    {
      _emailSettings = options.Value;
    }

    public async Task SendResetPasswordEmail(string resetEmailLink, string ToEmail)
    {
       var smptClient = new SmtpClient();//burası mail gönderme işlemi için gerekli
        smptClient.Host= _emailSettings.Host;//gmail smtp sunucusu
        smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;//ağ üzerinden gönderim
        smptClient.UseDefaultCredentials = false;//kendi credential larımızı kullanacağız
        smptClient.Port = 587;//gmail için 587
        smptClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password); //gönderen mail ve şifre  
        smptClient.EnableSsl = true; //güvenli bağlantı


        var mailMessage = new MailMessage();//mail mesajı oluşturma

        mailMessage.From = new MailAddress(_emailSettings.Email);//gönderen mail 
        mailMessage.To.Add(ToEmail);//alıcı mail

        mailMessage.Subject = "Lokalhost / Şifre Sıfırlama";//konu
        mailMessage.Body = $"Şifrenizi yenilemek için <a href='{resetEmailLink}'>tıklayınız</a>";//mail içeriği,

        mailMessage.IsBodyHtml = true;//html mi düz metin mi
       
        await smptClient.SendMailAsync(mailMessage);//mail gönderme işlemi
    }
}
