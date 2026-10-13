namespace MedicalDemo.Models.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        public required string TokenHash { get; set; }

        public required string AccountType { get; set; }

        public required string AccountId { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}