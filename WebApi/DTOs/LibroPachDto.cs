using System;
using System.ComponentModel.DataAnnotations;
using WebApi.validaciones;

namespace WebApi.DTOs
{
    public class LibroPachDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Primeraletramayuscula]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }

        public DateTime FechaDePublicacion { get; set; }
    }
}
