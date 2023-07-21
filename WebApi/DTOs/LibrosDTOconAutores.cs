using System.Collections.Generic;

namespace WebApi.DTOs
{
    public class LibrosDTOconAutores : LibroDTOs
    {
        public List<AutorDTOs> Autores { get; set; }
    }
}
