using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApi.Entidad;
using WebApi.DTOs;
using AutoMapper;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/V1/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<LibroDTOs>> Get()
        {
            var libros = await context.Libros.ToListAsync();

            return mapper.Map<List<LibroDTOs>>(libros);
        }

        [HttpGet("comentarios/{id:int}", Name = "obtenerComentarioDelLbrov1")]
        public async Task<ActionResult<LibroDTOs>> GetCom(int id)
        {
            // en este caso no es muy recomendado usar el include, solo en estos caso no lo es
            /*
             por que bueno, por que digamos que un usario esta usando nuestra pagina desde su celular
            el hecho de que cuando le esten cargando los libros y dik tambien los comentario, eso es una carga para los datos del usuario
            lo dejara solo para que veas como se hace pero ya sabes. es mejor ponerle un boton o algo para que luego si el quiere carge los comentarios
             */
            var comentario = await context.Libros.Include(libros => libros.Comentarios).FirstOrDefaultAsync(x => x.Id == id);


            return mapper.Map<LibroDTOs>(comentario);
        }
        [HttpGet("{id:int}", Name = "obtenerLibrov1")]
        public async Task<ActionResult<LibrosDTOconAutores>> Getss(int id)
        {
            //NOTA ESTO ES IMPORTANTE lesion 56
            var libro = await context.Libros
                .Include(LibroDB => LibroDB.AutoresLibros)//aqui estamos incluyendo a AutoresLibros que lo tenemos en Libros
                .ThenInclude(autorLibrosd => autorLibrosd.Autor)//luego aqui despues de a ver traido a AutorLibro, vamos a entrar a al metodo  //que se encuentra ahi que nos llevara a Autor
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return BadRequest();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibrosDTOconAutores>(libro);

        }

        [HttpPost(Name = "CrearLibrov1")]
        public async Task<ActionResult> Post(LibroCreacionDTOs libroCreacion)
        {


            if (libroCreacion.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await context.Autores
                .Where(autorDB => libroCreacion.AutoresIds.Contains(autorDB.Id))
                .Select(x => x.Id).ToListAsync();

            if (libroCreacion.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("Uno de los id no es valido");
            }


            var libro = mapper.Map<Libro>(libroCreacion);
            AsignarOrdernAutores(libro);


            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTOs>(libro);

            return CreatedAtRoute("obtenerLibro", new { libro.Id }, libroDTO);

        }


        private void AsignarOrdernAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }

            }
        }

        [HttpPut("{id:int}", Name = "actualizrLibrov1")]
        public async Task<ActionResult> put(int id, LibroCreacionDTOs libroCreacionDTOs)
        {

            var libroDB = await context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }
            // lecion 63 es importante
            libroDB = mapper.Map(libroCreacionDTOs, libroDB);
            AsignarOrdernAutores(libroDB);

            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpPatch("{id:int}", Name = "patchLibrov1")]
        public async Task<ActionResult> patch(int id, JsonPatchDocument<LibroPachDto> pathcdocumente)
        {

            if (pathcdocumente == null)
            {
                return BadRequest();

            }

            var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return BadRequest();


            }
            var libroDTO = mapper.Map<LibroPachDto>(libroDB);

            pathcdocumente.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroDB);

            await context.SaveChangesAsync();

            return NoContent();


        }

        [HttpDelete("{id:int}", Name = "borrarLibrov1")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await context.Libros.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }


            context.Remove(new Libro() { Id = id });// aqui solo lo estamos marcando como que va a ser removido
            await context.SaveChangesAsync();//aqui si ya estamos haciendo los cambios
            return Ok();


        }



    }
}
