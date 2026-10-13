using MedicalDemo.Interfaces;
using PostmarkDotNet;

namespace MedicalDemo.Services.EmailSendServices;

public class PostmarkEmailSendService : IEmailSendService
{
    private readonly string apiKey;
    private readonly PostmarkClient client;
    private readonly string fromEmail;
    private readonly string fromName;


    public PostmarkEmailSendService(IConfiguration config)
    {
        apiKey = config["POSTMARK_API_KEY"];
        fromEmail = config["FROM_EMAIL"];
        fromName = config["FROM_NAME"];
        client = new PostmarkClient(apiKey);
    }

    public async Task<bool> SendInvitationEmailAsync(string toEmail,
        string link)
    {
        PostmarkMessage message = new()
        {
            From = fromEmail,
            To = toEmail,
            Subject = "Psycall Invitation to Register",
            HtmlBody = $@"
            <h3>Hello!</h3>
            <p>Admin would like you to create a Psycall account.</p>
            <p>Please click this link to complete your registration:</p>
            <a href='{link}'>{link}</a>
            <p>Thank you,<br/>Psycall Admin</p>",
            TextBody =
                $"Hello,\n\nAdmin would like you to create a Psycall account.\n\nPlease use the following link:\n{link}\n\nThank you,\nPsycall Admin"
        };

        PostmarkResponse?
            response = await client.SendMessageAsync(message);
          return response.Status == PostmarkStatus.Success;
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail,
        string link)
    {
        PostmarkMessage message = new()
        {
            From = fromEmail,
            To = toEmail,
            Subject = "Psycall Password Reset",
            HtmlBody = $@"
            <h3>Psycall Password Reset</h3>
            <p>We received a request to reset your Psycall password.</p>
            <p>Click the link below to reset your password:</p>
            <a href='{System.Net.WebUtility.HtmlEncode(link)}'>Reset Password</a>
            <p>If you did not request this, you can ignore this email.</p>
            <p>Thank you,<br/>Psycall Admin</p>",
            TextBody =
                $"Hello,\n\nWe received a request to reset your Psycall password.\n\n" +
                $"Use the following link to reset your password:\n{link}\n\n" +
                "If you did not request this, you can ignore this email.\n\n" +
                "Thank you,\nPsycall Admin"
        };

        PostmarkResponse? response =
            await client.SendMessageAsync(message);

        return response.Status == PostmarkStatus.Success;
    }
}