using System.ComponentModel.DataAnnotations;

namespace MedicalDemo.Enums;

public enum Verbs
{
    None = 0,

    [Display(Name = "Allowed")]
    Allowed = 1,

    [Display(Name = "Not Allowed")]
    NotAllowed = 2,

    [Display(Name = "Prefers")]
    Prefers = 3,
}