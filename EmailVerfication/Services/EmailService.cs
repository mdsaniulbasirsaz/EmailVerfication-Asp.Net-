using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EmailVerfication.Services
{
	public class EmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string email, string subject, string body)
		{
			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("saniulsaj@gmail.com", "eyem qsrm omtk uubz"), // Replace with your credentials
				EnableSsl = true,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress("saniulsaj@gmail.com"), // Replace with your email
				Subject = subject,
				Body = body,
				IsBodyHtml = true,
			};

			mailMessage.To.Add(email);

			try
			{
				await smtpClient.SendMailAsync(mailMessage);
			}
			catch (Exception ex)
			{
				// Handle the error here (e.g., log it)
				throw new Exception("Error sending email", ex);
			}
		}
	}
}
