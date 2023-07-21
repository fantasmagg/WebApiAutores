using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Entidad;
using WebApi.Utilidades;


namespace WebApi.Controllers.V1
{
    [ApiController]
    //[controller] eso en tiempo de ejecucion se cambiara con el nombre del controller en este caso sera autores
    //tambien podemos ponerlo directo ejemplo : ("api/autores")
    //[Route("api/V1[controller]")]
    [Route("api/[controller]")]
    [CabeceraEstaPresente("x-version", "1")]
    //[Authorize]//esto es para proteger las peticiones, haciendo que los que vayan a usar la api tenga
    //que tener un usuario, pero como esta aplicado a nivel de clase eso afetara a todas la peticiones
    //por ahora estara comentado por que no tenemos ningun usuario, para poder aceder a nuestra api, ya que aplicamos fue un esquema basico

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    // [ApiConventionType(typeof(DefaultApiConventions))]
    //esto estables unas reglas a una estructura de api,
    //y entre esas reglas viene lo de tener documentado los codigos de estados, es los mismo que yo vaya poniendo esto
    //[ProducesResponseType(404)] metodo por metodo, esto nos lo pone a nivel de clase

    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
            this.authorizationService = authorizationService;
        }



        //[HttpGet("mmg",Name = "obtenerAutoress")] // api/autores/primero
        //[AllowAnonymous]// eso es para cuando tengamos una clase con el filtro de Authorize, y no queramos que este enpoy pase por 
        // la Authorize
        //public async Task<IActionResult> Getp([FromQuery] bool incluirHateoas = true)
        //{
        //    var autores = await context.Autores.ToListAsync();
        //    var dto = mapper.Map<List<AutorDTOs>>(autores);

        //    if (incluirHateoas)
        //    {
        //        var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

        //        //aqui lo que estamos haciendo es meterle a cada autor los link de las cosas que se puede hacer
        //        dto.ForEach(dtos => GenerarEnlaces(dtos, esAdmin.Succeeded));

        //        var resultado = new ColeccionDeRecursos<AutorDTOs> { valor = dto };
        //        resultado.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("obtenerAutoress", new { }), descripcion: "self", metodo: "GET"));
        //        if (esAdmin.Succeeded)
        //        {
        //            resultado.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("CrearAutor", new { }), descripcion: "crear-Autor", metodo: "POST"));
        //        }
        //        return Ok(resultado);

        //    }

        //    return Ok(dto);


        //}

        //sin mappear
        [HttpGet("sss")] // api/autores/primero
        [AllowAnonymous]// eso es para cuando tengamos una clase con el filtro de Authorize, y no queramos que este enpoy pase por 
        // la Authorize
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        //NOTA IMPORTANTE: el paremetro "incluirHateoas", se a removida
        //y ahora esta de una forma que se incluye directamente en la configuracion de swager, lo puedes ver en es startup
        public async Task<ActionResult<List<AutorDTOs>>> Geta([FromQuery] PaginacionDTO paginacionDTO)
        {
            //aqui estamos contruyendo un linq
            var queryable = context.Autores.AsQueryable();
            //aqui enviamos nuestro linq y aquegandole la funcion Paginar al "queryable"
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            //aqui estasmo 
            var autores = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<AutorDTOs>>(autores);


        }


        //sin mappear
        [HttpGet(Name = "obtenerAutoresv1")] // api/autores/primero
        [AllowAnonymous]// eso es para cuando tengamos una clase con el filtro de Authorize, y no queramos que este enpoy pase por 
        // la Authorize
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        //NOTA IMPORTANTE: el paremetro "incluirHateoas", se a removida
        //y ahora esta de una forma que se incluye directamente en la configuracion de swager, lo puedes ver en es startup
        public async Task<ActionResult<List<AutorDTOs>>> Get()
        {
            var autores = await context.Autores.ToListAsync();





            return mapper.Map<List<AutorDTOs>>(autores);


        }

        [HttpGet("{id:int}", Name = "obtenerAutorv1")] // api/autores/primero
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        [ProducesResponseType(404)]/*eso es para evitar esto 
                                    * "404
                                        Undocumented"
                                        eso aparece cuando ejecutamos un enpoint, con eso no se evita que no salga, solo que ahora aparecera 
                                         como algo que es posible que salga*/
        [ProducesResponseType(200)]
        public async Task<ActionResult<AutorDTOconLibros>> Getid([FromRoute] int id)
        {
            var autores = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autoresLibrosD => autoresLibrosD.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);


            if (autores == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<AutorDTOconLibros>(autores);

            //var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            //GenerarEnlaces(dto, esAdmin.Succeeded);

            return dto;
        }


        //NOTA ESto esta copiado para cuando vayas a usar el DTO AutorDTOsinEnlace
        //[HttpGet("{id:int}", Name = "obtenerAutor")] // api/autores/primero
        //[AllowAnonymous]
        //[ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        //public async Task<ActionResult<AutorDTOconLibros>> Getid([FromRoute] int id, [FromHeader] string incluirHateoas)
        //{
        //    var autores = await context.Autores
        //        .Include(autorDB => autorDB.AutoresLibros)
        //        .ThenInclude(autoresLibrosD => autoresLibrosD.Libro)
        //        .FirstOrDefaultAsync(x => x.Id == id);


        //    if (autores == null)
        //    {
        //        return NotFound();
        //    }
        //    var dto = mapper.Map<AutorDTOconLibros>(autores);

        //    //var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
        //    //GenerarEnlaces(dto, esAdmin.Succeeded);

        //    return dto;
        //}

        //public async Task<IActionResult> Get([FromHeader] string incluirHateoas)
        //{
        //    var autores = await context.Autores.ToListAsync();





        //    if (incluirHateoas.)
        //    {
        //        var dto = mapper.Map<List<AutorDTOs>>(autores);
        //        var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
        //        //aqui lo que estamos haciendo es meterle a cada autor los link de las cosas que se puede hacer
        //        //dto.ForEach(dtos => GenerarEnlaces(dtos, esAdmin.Succeeded));

        //        var resultado = new ColeccionDeRecursos<AutorDTOs> { valor = dto };
        //        resultado.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "self", metodo: "GET"));
        //        if (esAdmin.Succeeded)
        //        {
        //            resultado.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("CrearAutor", new { }), descripcion: "crear-Autor", metodo: "POST"));
        //        }


        //        return Ok(resultado);
        //    }
        //    var dtos = mapper.Map<List<AutorDTOsinEnlaces>>(autores);

        //    return Ok(dtos);
        //}
        //[HttpGet("/fff")]
        //public async Task<List<AutorDTOs>> Gest()
        //{
        //    var autores = await context.Autores.ToListAsync();



        //    return mapper.Map<List<AutorDTOs>>(autores);

        //}



        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev1")] // api/autores/primero
        //<List<AutorDTOs> ahi ponemos list por que es obvio estamos devolviendo un listado de las personas
        // en la que sus nombres coinsidan
        public async Task<ActionResult<List<AutorDTOs>>> GetVs([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();


            return mapper.Map<List<AutorDTOs>>(autores);
        }

       





        // este metodo es de los HATEOAS, es para enviarle informacion al usuaior de las cosas que puede hacer con nuestra api, en este caso de las cosas
        // que puede hacer con este controlador, tambien hay un controller llamado rootController, ese 
        // tiene una "guia" de todo lo que se puede hacer con nuestra api
        //private void GenerarEnlaces(AutorDTOs autorDTOs, bool esAdmin)
        //{
        //    autorDTOs.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("obtenerAutor", new { id = autorDTOs.Id }), descripcion: "self", metodo: "GET"));
        //    if (esAdmin)
        //    {
        //        autorDTOs.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("ActualizrAutor", new { id = autorDTOs.Id }), descripcion: "actualizar-libro", metodo: "PUT"));
        //        autorDTOs.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("BorrandoUnAutor", new { id = autorDTOs.Id }), descripcion: "borrar-libro", metodo: "DELETE"));
        //    }
        //}



        [HttpPost(Name = "CrearAutorv1")]
        public async Task<ActionResult> post([FromBody] AutorCreacionDTOs autorCreacionDTOs)
        {

            var existeNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTOs.Nombre);

            if (existeNombre)
            {

                return BadRequest($"Ese nombre de usuario ya existe {autorCreacionDTOs.Nombre}");
            }


            //mapeo automatico
            var autor = mapper.Map<Autor>(autorCreacionDTOs);

            context.Add(autor); // aqui estamos marcando el autor proximo a crear,
                                // pero aun no se a creado del todo por que no se a guardado en la base de datos

            await context.SaveChangesAsync(); // aqui si ya estamos guardandolo en la base de datos

            var autorDTO = mapper.Map<AutorDTOs>(autor);

            return CreatedAtRoute("obtenerAutorv1", new { autor.Id }, autorDTO); // aqui solo estamos devolviendo un 200

        }


        //("{id:int}") eso es un parametro de ruta. y id:int ahi le estamos retringiendo que
        //el valor que nos pasen tiene que ser numerico
        [HttpPut("{id:int}", Name = "ActualizrAutorv1")] //api/autores/1
        public async Task<ActionResult> put(AutorCreacionDTOs autorCreacionDTO, int id)
        {


            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            context.Update(autor);// aqui aun no lo hemos up aqui solo lo estamos marancando para avisar que sera up
            await context.SaveChangesAsync();//aqui si ya estamos haciendo los cambios
            return NoContent();

        }
        //esto es para ponerle comentario a los enpoints
        /// <summary>
        /// borra un autor
        /// </summary>
        /// <param name="id">Id del autor a borrar</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "BorrandoUnAutorv1")]
        public async Task<ActionResult> Delete(int id)
        {

            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }


            context.Remove(new Autor() { Id = id });// aqui solo lo estamos marcando como que va a ser removido
            await context.SaveChangesAsync();//aqui si ya estamos haciendo los cambios
            return Ok();


        }


        // este es muy importante es muy util are un video
        [HttpGet("paginacion")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Autor>>> generosPaginacion(int pagina = 1)
        {
            //aqui es para la por paginas
            var cantidadRegistrosPorPagonas = 3;
            var geneross = await context.Autores.
                //aqui lo que hacemos es saltar valores de la tabla
                //es decir: si "pagina" =1 -1 * 3 = 0
                // eso quiere decir que en la primera pagina no volara ningun valor
                //pero y si "pagina" =2-1 =1 *3 = 3, entonces Skip(3), entonces lo que area es volar los 3 primeros
                //valores de la tabla y traera a los otros, y luego "Take(3)" solo tomara 3 valores de los otros que se trayeron
                Skip((pagina - 1) * cantidadRegistrosPorPagonas)
                .Take(3)//pero aqui nosotros decimos que tome solo 3 de los valores que hayan en la tabla
                .ToListAsync();

            return geneross;

        }


    }
}
