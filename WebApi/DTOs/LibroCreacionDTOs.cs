using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApi.validaciones;

namespace WebApi.DTOs
{
    public class LibroCreacionDTOs
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Primeraletramayuscula]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        
        public DateTime FechaDePublicacion { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
