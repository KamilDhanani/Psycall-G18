using MedicalDemo.Interfaces;

namespace MedicalDemo.Services.EmailSendServices;

public class MailgunEmailSendService : IEmailSendService
{
    private readonly ILogger<MailgunEmailSendService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public MailgunEmailSendService(
        ILogger<MailgunEmailSendService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory
    )
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    // Existing invitation email functionality
    public async Task<bool> SendInvitationEmailAsync(string toEmail, string link)
    {
        using HttpClient httpClient =
            _httpClientFactory.CreateClient(nameof(MailgunEmailSendService));

        string? fromEmail = _configuration.GetValue<string>("Mailgun:FromEmail");

        if (fromEmail == null)
        {
            fromEmail = Environment.GetEnvironmentVariable("MailgunFromEmail");

            if (fromEmail == null)
            {
                throw new Exception("Mailgun:FromEmail is not set");
            }
        }

        FormUrlEncodedContent formData = new(new[]
        {
            new KeyValuePair<string, string>("from", $"Psycall <{fromEmail}>"),
            new KeyValuePair<string, string>("to", toEmail),
            new KeyValuePair<string, string>("subject", "Psycall Invitation to Register"),
            new KeyValuePair<string, string>("text",
                $"Hello,\n\nAdmin would like you to create a Psycall account.\n\n" +
                $"Please use the following link:\n{link}\n\n" +
                "Thank you,\nPsycall Admin"),
            new KeyValuePair<string, string>("html",
                "<h3>Hello!</h3><br />" +
                "<p>Admin would like you to create a Psycall account.</p><br />" +
                "<p>Please click this link to complete your registration:</p><br />" +
                $"<a href='{link}'>{link}</a><br />" +
                "<p>Thank you,<br/>Psycall Admin</p>")
        });

        using HttpResponseMessage response =
            await httpClient.PostAsync((string?)null, formData);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        _logger.LogError(
            "Failed to send invite email to {toEmail} ({link}). Data: {json}",
            toEmail,
            link,
            await response.Content.ReadAsStringAsync());

        return false;
    }

    // New forgot password email functionality
    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string link)
    {
        using HttpClient httpClient =
            _httpClientFactory.CreateClient(nameof(MailgunEmailSendService));

        string? fromEmail = _configuration.GetValue<string>("Mailgun:FromEmail");

        if (fromEmail == null)
        {
            fromEmail = Environment.GetEnvironmentVariable("MailgunFromEmail");

            if (fromEmail == null)
            {
                throw new Exception("Mailgun:FromEmail is not set");
            }
        }

        FormUrlEncodedContent formData = new(new[]
        {
            new KeyValuePair<string, string>("from", $"Psycall <{fromEmail}>"),
            new KeyValuePair<string, string>("to", toEmail),
            new KeyValuePair<string, string>("subject", "Psycall Password Reset"),
            new KeyValuePair<string, string>("text",
                $"Hello,\n\nWe received a request to reset your Psycall password.\n\n" +
                $"Use the following link to reset your password:\n{link}\n\n" +
                "If you did not request this, you can ignore this email.\n\n" +
                "Thank you,\nPsycall Admin"),
            new KeyValuePair<string, string>("html",
                "<h3>Psycall Password Reset</h3>" +
                "<p>We received a request to reset your Psycall password.</p>" +
                "<p>Use the following link to reset your password:</p>" +
                $"<p><a href='{System.Net.WebUtility.HtmlEncode(link)}'>Reset Password</a></p>" +
                "<p>If you did not request this, you can ignore this email.</p>" +
                "<p>Thank you,<br/>Psycall Admin</p>")
        });

        using HttpResponseMessage response =
            await httpClient.PostAsync((string?)null, formData);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        _logger.LogError(
            "Failed to send password reset email to {toEmail}. Data: {response}",
            toEmail,
            await response.Content.ReadAsStringAsync());

        return false;
    }
}