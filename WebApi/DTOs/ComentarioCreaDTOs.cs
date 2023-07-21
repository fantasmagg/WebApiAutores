using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class ComentarioCreaDTOs
    {
    
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Contenido { get; set; }
     
    }
}
