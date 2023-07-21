using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApi.validaciones;

namespace WebApi.Entidad
{
    public class Libro
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Primeraletramayuscula]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }

        public DateTime? FechaDePublicacion { get; set; } // ese signo hace que el campo pueda ser null
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
      
       

    }
}
