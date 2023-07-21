using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class CredencialesUsuario
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(10, MinimumLength =8)]
        public string Password { get; set; }
    }
}
