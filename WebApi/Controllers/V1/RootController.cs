using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.DTOs;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/V1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRootv1")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> get()
        {
            var datosHateos = new List<DataHATEOAS>();

            //aqui estamos viendo si el usuario tiene la autorizacion de admin
            //es decir si cumple la politica de los admin, para eso tiene que tener el claim de "esAdmin" eso se hace en cuentaController
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateos.Add(new DataHATEOAS(enlace: Url.Link("ObtenerRootv1", new { }), descripcion: "self", metodo: "GET"));

            datosHateos.Add(new DataHATEOAS(enlace: Url.Link("obtenerAutoresv1", new { }), descripcion: "autores", metodo: "GET"));

            //aqui validamos si el usuario es admin o no para no mostrarle acciones que el usuario no pueda realizar
            if (esAdmin.Succeeded)
            {
                datosHateos.Add(new DataHATEOAS(enlace: Url.Link("CrearAutorv1", new { }), descripcion: "autore-crear", metodo: "POST"));

                datosHateos.Add(new DataHATEOAS(enlace: Url.Link("CrearLibrov1", new { }), descripcion: "libro-crear", metodo: "POST"));
            }


            return datosHateos;

        }
    }
}
