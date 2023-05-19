using System.ComponentModel.DataAnnotations;

namespace ForExampleWebApi.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastPasswordResetDate { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }
}
