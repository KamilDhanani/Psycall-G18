using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string email { get; set; }
}