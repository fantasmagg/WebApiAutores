using System.ComponentModel.DataAnnotations;
using WebApi.validaciones;

namespace WebApi.DTOs
{
    public class AutorCreacionDTOs
    {
       
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 27, ErrorMessage = "el {0} sipera las {1} letras")]
        [Primeraletramayuscula]
        public string Nombre { get; set; }
    }
}
