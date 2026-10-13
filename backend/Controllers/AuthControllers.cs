using System.Security.Cryptography;
using System.Text;
using MedicalDemo.Interfaces;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MedicalContext _context;
    private readonly IEmailSendService _emailSendService;

    public AuthController(MedicalContext context, IEmailSendService emailSendService)
    {
        _context = context;
        _emailSendService = emailSendService;
    }
        [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request)
    {
        const string responseMessage =
            "If an eligible account exists, a password reset link will be sent.";

        string email = request.email.Trim();

        Resident? resident = await _context.Residents
            .FirstOrDefaultAsync(r => r.Email == email && r.GraduateYr != null);

        Admin? admin = resident == null
            ? await _context.Admins.FirstOrDefaultAsync(a => a.Email == email)
            : null;

        if (resident == null && admin == null)
        {
            return Ok(new { message = responseMessage });
        }

        string token = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        string tokenHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        var resetToken = new PasswordResetToken
        {
            TokenHash = tokenHash,
            AccountType = resident != null ? "resident" : "admin",
            AccountId = resident?.ResidentId ?? admin!.AdminId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        _context.PasswordResetTokens.Add(resetToken);
        await _context.SaveChangesAsync();

        string frontendUrl =
            Environment.GetEnvironmentVariable("FRONTEND_URL")
            ?? "http://localhost:3000";

        string resetLink =
            $"{frontendUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(token)}";

        await _emailSendService.SendPasswordResetEmailAsync(email, resetLink);

        return Ok(new { message = responseMessage });
    }

        [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request)
    {
        string tokenHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(request.token)));

        PasswordResetToken? resetToken = await _context.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

        if (resetToken == null ||
            resetToken.UsedAt != null ||
            resetToken.ExpiresAt <= DateTime.UtcNow)
        {
            return BadRequest(new { message = "Invalid or expired reset link." });
        }

        string hashedPassword =
            BCrypt.Net.BCrypt.HashPassword(request.newPassword);

        if (resetToken.AccountType == "resident")
        {
            Resident? resident = await _context.Residents
                .FirstOrDefaultAsync(r =>
                    r.ResidentId == resetToken.AccountId &&
                    r.GraduateYr != null);

            if (resident == null)
            {
                return BadRequest(new { message = "Invalid or expired reset link." });
            }

            resident.Password = hashedPassword;
        }
        else if (resetToken.AccountType == "admin")
        {
            Admin? admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.AdminId == resetToken.AccountId);

            if (admin == null)
            {
                return BadRequest(new { message = "Invalid or expired reset link." });
            }

            admin.Password = hashedPassword;
        }
        else
        {
            return BadRequest(new { message = "Invalid or expired reset link." });
        }

        resetToken.UsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully." });
    }
    [HttpOptions]
    public IActionResult Options()
    {
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        try
        {
            //check residents table first
            Resident? resident = await _context.Residents
                .FirstOrDefaultAsync(r => r.Email == request.email);

            if (resident?.GraduateYr != null)
            {
                bool passwordMatch
                    = BCrypt.Net.BCrypt.Verify(request.password,
                        resident.Password);

                if (!passwordMatch)
                {
                    return Unauthorized(new AuthLoginResponse
                    {
                        success = false,
                        message = "Invalid credentials"
                    });
                }

                string token = Guid.NewGuid().ToString();

                return Ok(new AuthLoginResponse
                {
                    success = true,
                    token = token,
                    userType = "resident",
                    resident = new AuthLoginResponse.User
                    {
                        id = resident.ResidentId,
                        email = resident.Email,
                        firstName = resident.FirstName,
                        lastName = resident.LastName,
                        phoneNum = resident.PhoneNum
                    }
                });
            }

            //check admins table if not found in residents
            Admin? admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == request.email);

            if (admin != null)
            {
                bool passwordMatch
                    = BCrypt.Net.BCrypt.Verify(request.password,
                        admin.Password);

                if (!passwordMatch)
                {
                    return Unauthorized(new AuthLoginResponse
                    {
                        success = false,
                        message = "Invalid credentials"
                    });
                }

                string token = Guid.NewGuid().ToString();

                return Ok(new AuthLoginResponse
                {
                    success = true,
                    token = token,
                    userType = "admin",
                    admin = new AuthLoginResponse.User
                    {
                        id = admin.AdminId,
                        email = admin.Email,
                        firstName = admin.FirstName,
                        lastName = admin.LastName,
                        phoneNum = admin.PhoneNum
                    }
                });
            }

            //Not found in either
            return Unauthorized(new AuthLoginResponse
            {
                success = false,
                message = "Invalid credentials"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthLoginResponse
            {
                success = false,
                message = $"Login error: {ex.Message}"
            });
        }
    }
}