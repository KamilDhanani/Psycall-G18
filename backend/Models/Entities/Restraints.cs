using System.ComponentModel.DataAnnotations.Schema;
using MedicalDemo.Enums;

namespace MedicalDemo.Models.Entities
{
    public partial class Restraints
    {
        public Guid RestraintId { get; set; }

        [ForeignKey(nameof(Resident))]
        public virtual Resident Resident { get; set; }

        public string ResidentId { get; set; } = null!;

        // set 0 for allow, 1 for not allow, 2 prefers
        public Verbs Verb { get; set; } = 0; 

        [ForeignKey(nameof(RotationType))]
        public Guid RotationTypeId { get; set; }

        public RotationType RotationType { get; set; } = null!;
    }
}