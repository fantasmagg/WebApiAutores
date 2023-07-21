using System.Collections.Generic;

namespace WebApi.DTOs
{
    public class AutorDTOconLibros: AutorDTOs
    {
        public List<LibroDTOs> libros { get; set; }
    }
}
