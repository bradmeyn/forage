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

public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
{
    _configuration = configuration;
    _logger = logger;
}

   public async Task SendEmailAsync(string email, string subject, string message, string attachmentPath = null)
{
    try
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        _logger.LogInformation($"SendGrid API Key: {apiKey}");
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("bradjmeyn@gmail.com", "Brad");
        var to = new EmailAddress(email);
        var plainTextContent = message;
        var htmlContent = $"<p>{message}</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        if (attachmentPath != null && File.Exists(attachmentPath))
        {
            var bytes = await File.ReadAllBytesAsync(attachmentPath);
            var file = Convert.ToBase64String(bytes);
            msg.AddAttachment(Path.GetFileName(attachmentPath), file);
        }

        var response = await client.SendEmailAsync(msg);
        if (response.StatusCode != HttpStatusCode.Accepted)
        {
            // Log the response details for further investigation
            _logger.LogError($"Email sending failed with status code: {response.StatusCode} - {response.Body.ReadAsStringAsync().Result}");
        }
    }
    catch (Exception ex)
    {
        // Log the exception details
        _logger.LogError($"Email sending failed with exception: {ex.Message}");
    }
}


    public async Task SendBulkEmailAsync(IEnumerable<string> emails, string subject, string message, string attachmentPath = null)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("your-email@example.com", "Your Name");
        var tos = new List<EmailAddress>();
        foreach (var email in emails)
        {
            tos.Add(new EmailAddress(email));
        }

        var plainTextContent = message;
        var htmlContent = $"<p>{message}</p>";
        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, htmlContent);

        if (attachmentPath != null && File.Exists(attachmentPath))
        {
            var bytes = await File.ReadAllBytesAsync(attachmentPath);
            var file = Convert.ToBase64String(bytes);
            msg.AddAttachment(Path.GetFileName(attachmentPath), file);
        }

        await client.SendEmailAsync(msg);
    }
}
