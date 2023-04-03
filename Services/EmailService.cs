using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly IWebHostEnvironment _env;

public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment env)
{
    _configuration = configuration;
    _logger = logger;
    _env = env;
}

   public async Task SendEmailAsync(
        string email, 
        string subject, 
        string message, 
        string attachmentPath = null
    )
    {
    try
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("bradjmeyn@gmail.com", "Brad@Forage");
        var to = new EmailAddress(email);
        var plainTextContent = message;
        var htmlContent = $"<p>{message}</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        
        // Add attachment if it exists
        if (attachmentPath != null && File.Exists(attachmentPath))
        {
            var bytes = await File.ReadAllBytesAsync(attachmentPath);
            var file = Convert.ToBase64String(bytes);
            msg.AddAttachment(Path.GetFileName(attachmentPath), file);
        }
        else
        {
            // Add a burger
            string imagePath = Path.Combine(_env.WebRootPath, "images", "burger.jpg");
            byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
            string imageBase64 = Convert.ToBase64String(imageBytes);
            msg.AddAttachment("burger.jpg", imageBase64, "image/jpeg");
        }

        // Send the email
        var response = await client.SendEmailAsync(msg);
    }
    catch (Exception e)
    {
        // Log the exception details
        _logger.LogError($"Email sending failed with exception: {e.Message}");
    }
}
    public async Task SendBulkEmailAsync(IEnumerable<string> emails, string subject, string message, string attachmentPath = null)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("bradjmeyn@gmail.com", "Brad@Forage");
        var toList = new List<EmailAddress>();

        foreach (var email in emails)
        {
            toList.Add(new EmailAddress(email));
        }

        var plainTextContent = message;
        var htmlContent = $"<p>{message}</p>";
        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toList, subject, plainTextContent, htmlContent);

        if (attachmentPath != null && File.Exists(attachmentPath))
        {
            var bytes = await File.ReadAllBytesAsync(attachmentPath);
            var file = Convert.ToBase64String(bytes);
            msg.AddAttachment(Path.GetFileName(attachmentPath), file);
        }

        await client.SendEmailAsync(msg);
    }
}
