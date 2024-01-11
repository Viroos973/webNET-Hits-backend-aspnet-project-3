using MIS_Backend.Database;
using MIS_Backend.Database.Models;
using System.Net.Mail;
using Quartz;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MIS_Backend.Jobs
{
    public class SendEmailJob : IJob
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SendEmailJob> _logger;

        public SendEmailJob(AppDbContext context, ILogger<SendEmailJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Attempt to send email");

            var inspections = _context.Inspections.Where(x => x.NextVisitDate < DateTime.UtcNow && !x.HasNested)
                .Include(x => x.Patients)
                .Include(x => x.Doctors).ToList();
            int retryCount = 1;

            while (inspections.Any())
            {
                List<Inspection> errors = new List<Inspection>();

                foreach (var inspection in inspections)
                {
                    try
                    {
                        var from = new MailAddress("robo.tirex753@mail.ru", "Admin");
                        var to = new MailAddress(inspection.Doctors.EmailAddress);
                        var mail = new MailMessage(from, to);

                        mail.Subject = "Пропущенный осмотр";
                        mail.Body = $"Пациент {inspection.Patients.Name} пропустил осмотр который должен был проходить {inspection.Date}";

                        using(SmtpClient client = new SmtpClient("smtp.mail.ru")) 
                        {
                            client.Credentials = new NetworkCredential("robo.tirex753@mail.ru", "j4gk6Gvk32ye6t06vwJw");
                            client.Port = 587;
                            client.EnableSsl = true;

                            await client.SendMailAsync(mail);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error occured upon attempt to send email: " + ex.Message);
                        errors.Add(inspection);
                    }
                }

                if (errors.Any())
                {
                    if (retryCount >= 3)
                    {
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(60));
                }

                inspections = errors;
                retryCount++;
            }
        }
    }
}
