using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Entidad;
using WebApi.Utilidades;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/V1/libros/{libroId:int}/comentarios")]
    public class ComentarioController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentarioController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [HttpGet(Name = "obtenerComentarioPorLibrov1")]
        public async Task<ActionResult<List<ComentarioDTOs>>> Get(int libroId, [FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Comentarios.Where(comentario => comentario.LibroId == libroId).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var comentarioE = await context.Libros.AnyAsync(Libroids => Libroids.Id == libroId);

            if (!comentarioE)
            {
                return BadRequest("ese id de libro no existe");
            }

            var comentario = await queryable.OrderBy(come=> come.Id).Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<ComentarioDTOs>>(comentario);

        }

        //[HttpGet(Name = "obtenerComentarioPorLibrov1")]
        //public async Task<ActionResult<List<ComentarioDTOs>>> Get(int libroId)
        //{
        //    var comentarioE = await context.Libros.AnyAsync(Libroids => Libroids.Id == libroId);

        //    if (!comentarioE)
        //    {
        //        return BadRequest("ese id de libro no existe");
        //    }

        //    var comentario = await context.Comentarios.Where(comentario => comentario.LibroId == libroId).ToListAsync();

        //    return mapper.Map<List<ComentarioDTOs>>(comentario);

        //}

        [HttpGet("{id:int}", Name = "obtenerComentariov1")]
        public async Task<ActionResult<ComentarioDTOs>> gets(int id)
        {

            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentario => comentario.Id == id);

            return mapper.Map<ComentarioDTOs>(comentario);

        }




        [HttpPost(Name = "creaComentariov1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreaDTOs comentario)
        {
            //aqui estamos acediendo a los claim del usuario
            //en este caso estamos buscando el claim email del usuario
            // esto lo hacemos para saber que usuario fue el que hizo un dicho comentario
            //y para que los comentarios puedan ser hechos solo por personas registradas
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            var email = emailClaim.Value;
            //aqui estamos buscando en la tabla de usuario de identity al email que se nos trajo
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var comentarioE = await context.Libros.AnyAsync(Libroids => Libroids.Id == libroId);

            if (!comentarioE)
            {
                return BadRequest();
            }

            var comentariocom = mapper.Map<Comentario>(comentario);
            comentariocom.LibroId = libroId;
            comentariocom.UsuarioId = usuarioId;//aqui le pasamos el id del usario que hizo el comentario
            //anterior mente hicimos una relacion entre la tabla comentario, y la de AspNetUsers 
            context.Add(comentariocom);
            context.SaveChanges();

            var comentarioD = mapper.Map<ComentarioDTOs>(comentariocom);

            return CreatedAtRoute("obtenerComentario", new { comentariocom.Id, LibroId = libroId }, comentarioD);


        }

        [HttpPut("{id:int}", Name = "actualizarComentariov1")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreaDTOs comentarioCreacionD)
        {
            var existeL = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeL)
            {
                return NotFound();
            }

            var existeC = await context.Comentarios.AnyAsync(x => x.Id == id);

            if (!existeC)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionD);
            comentario.Id = id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();



        }

    }
}
