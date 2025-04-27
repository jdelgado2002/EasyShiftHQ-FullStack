using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using System;
using Volo.Abp;
using System.Net.Mail;

namespace easyshifthq.Email;

public class SendGridEmailSender : IEmailSender, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public SendGridEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string to, string? subject, string? body, bool isBodyHtml = true, AdditionalEmailSendingArgs? additionalEmailSendingArgs = null)
    {
        await SendEmailAsync(to, null, subject, body ?? string.Empty, isBodyHtml);
    }

    public async Task SendAsync(string from, string to, string? subject, string? body, bool isBodyHtml = true, AdditionalEmailSendingArgs? additionalEmailSendingArgs = null)
    {
        await SendEmailAsync(to, from, subject, body ?? string.Empty, isBodyHtml);
    }

    public Task SendAsync(MailMessage mail, bool normalize = true)
    {
        Check.NotNull(mail, nameof(mail));

        if (mail.To.Count != 1)
        {
            throw new NotSupportedException("SendGrid implementation only supports one recipient at a time");
        }

        return SendEmailAsync(
            mail.To[0].Address,
            mail.From?.Address,
            mail.Subject,
            mail.Body,
            mail.IsBodyHtml
        );
    }

    public Task QueueAsync(string to, string? subject, string? body, bool isBodyHtml = true, AdditionalEmailSendingArgs? additionalEmailSendingArgs = null)
    {
        // SendGrid doesn't support queueing, so we'll send immediately
        return SendAsync(to, subject, body, isBodyHtml, additionalEmailSendingArgs);
    }

    public Task QueueAsync(string from, string to, string? subject, string? body, bool isBodyHtml = true, AdditionalEmailSendingArgs? additionalEmailSendingArgs = null)
    {
        // SendGrid doesn't support queueing, so we'll send immediately
        return SendAsync(from, to, subject, body, isBodyHtml, additionalEmailSendingArgs);
    }

    private async Task SendEmailAsync(string to, string? from, string? subject, string body, bool isBodyHtml)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        
        var fromEmail = from ?? _configuration["SendGrid:FromEmail"];
        var fromName = _configuration["SendGrid:FromName"];

        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromEmail, fromName),
            Subject = subject,
            PlainTextContent = isBodyHtml ? string.Empty : body,
            HtmlContent = isBodyHtml ? body : string.Empty
        };
        
        msg.AddTo(new EmailAddress(to));
        
        var response = await client.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode)
        {
            throw new UserFriendlyException($"Failed to send email. Status code: {response.StatusCode}");
        }
    }
}