using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WebApi.DTOs;

namespace WebApi.Servicios
{
    public class GeneradorEnlase
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;
       

        public GeneradorEnlase(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor )
        // IHttpContextAccessor httpContextAccessor con eso podemos acceder al context desde cualquier clase
        // IActionContextAccessor actionContextAccessor este es para nosotros poder hacer una factoria de Url.Link
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
         
        }


        private async Task<bool> EsAdmin()
        {
            //en este caso lo estamos usando para buscar al usuario
            var httpContext = httpContextAccessor.HttpContext;
            var resultado = await authorizationService.AuthorizeAsync(httpContext.User,"esAdmin");
            return resultado.Succeeded;
        }

        private IUrlHelper ContruirUrlHelper()
        {
            /*
              httpContextAccessor.HttpContext.RequestServices: aqui que estamos haciendo
                lo que estamos haciendo es acceder a los servicios de ASP.NET con eso "RequestServices" eso contiene los servicios
                de ASP.NET, luego deecimos .GetRequiredService<IUrlHelperFactory>() para obtener el servicio de IUrlHelper
             */
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public async Task GenerarEnlaces(AutorDTOs autorDTOs)
        {
            var esAdmin = await EsAdmin();
            var Url = ContruirUrlHelper();

            autorDTOs.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("obtenerAutorv1", new { id = autorDTOs.Id }), descripcion: "self", metodo: "GET"));
            if (esAdmin)
            {
                autorDTOs.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("ActualizrAutorv1", new { id = autorDTOs.Id }), descripcion: "actualizar-libro", metodo: "PUT"));
                autorDTOs.Enlaces.Add(new DataHATEOAS(enlace: Url.Link("BorrandoUnAutorv1", new { id = autorDTOs.Id }), descripcion: "borrar-libro", metodo: "DELETE"));
            }
        }
    }
}
