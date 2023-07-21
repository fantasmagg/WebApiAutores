using AutoMapper;
using System.Collections.Generic;
using WebApi.DTOs;
using WebApi.Entidad;

namespace WebApi.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //aqui estamos mapeando pasandoles los valores que recogemos con AutorCreacionDTOs a Autor
            CreateMap<AutorCreacionDTOs, Autor>();
            CreateMap<Autor, AutorDTOs>();
            CreateMap<Autor, AutorDTOsinEnlaces>();
            CreateMap<Autor, AutorDTOconLibros>()// AutorDTOconLibros herreda del autorDTO
                .ForMember(autoreDTO => autoreDTO.libros, opciones => opciones.MapFrom(MapAutoreLibros));

            // esto es importante ver en el video 58 de contruyendo web api
            CreateMap<LibroCreacionDTOs, Libro>()// esta es para poder insertar en los libros a sus autores solo los id se le pasaran
                .ForMember(Libro => Libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));


            CreateMap<LibroPachDto, Libro>().ReverseMap();//ReverseMap() para que se haga al reves tambien... o eso creo


            CreateMap<Libro, LibroDTOs>();
            CreateMap<Libro, LibrosDTOconAutores>()
                .ForMember(LibroDTO => LibroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));

            CreateMap<ComentarioCreaDTOs, Comentario>();
            CreateMap<Comentario, ComentarioDTOs>();

        }


        private List<LibroDTOs> MapAutoreLibros(Autor autor , AutorDTOs autorDTOs)
        {

            var resultado = new List<LibroDTOs>();
            if (autor.AutoresLibros == null) { return resultado; }

            foreach (var autorlibro in autor.AutoresLibros) {

                resultado.Add(new LibroDTOs() { 
                
                    Id=autorlibro.LibroId,
                    Titulo= autorlibro.Libro.Titulo

                });

                
            }
            return resultado;



        }

        private List<AutorDTOs> MapLibroDTOAutores(Libro libro, LibroDTOs libroDTOs)
        {
            var resultado = new List<AutorDTOs>();

            if (libro.AutoresLibros == null) { return resultado; }

            foreach (var autoreLibro in libro.AutoresLibros)
            {

                resultado.Add(new AutorDTOs()
                {
                    Id = autoreLibro.AutorId,
                    Nombre = autoreLibro.Autor.Nombre

                }); 

            }
            return resultado;


        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTOs libroCreacionDTOs, Libro libro)
        {

            var resultado = new List<AutorLibro>();

            if (libroCreacionDTOs.AutoresIds == null) { return resultado; }

            foreach (var autorId in libroCreacionDTOs.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId});

            }


            return resultado;

        }
    }
}
