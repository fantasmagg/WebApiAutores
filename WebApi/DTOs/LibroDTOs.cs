using System;
using System.Collections.Generic;
using WebApi.Entidad;

namespace WebApi.DTOs
{
    public class LibroDTOs
    {
        public int Id { get; set; }
       
        public string Titulo { get; set; }

        public DateTime FechaDePublicacion { get; set; }
        

        // public List<ComentarioDTOs> Comentarios { get; set; }
    }
}
