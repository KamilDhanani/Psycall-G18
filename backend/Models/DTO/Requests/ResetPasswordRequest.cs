using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Models.DTO.Requests;

public class ResetPasswordRequest
{
    [Required]
    public required string token { get; set; }

    [Required]
    [MinLength(8)]
    public required string newPassword { get; set; }
}