using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
