using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.validaciones;

namespace WebApi.Entidad
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 27, ErrorMessage = "el {0} sipera las {1} letras")]
        [Primeraletramayuscula]
        public string Nombre { get; set; }

        public List<AutorLibro> AutoresLibros { get; set; }

    }
}