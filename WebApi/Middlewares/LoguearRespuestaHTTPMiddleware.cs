using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace WebApi.Middlewares
{

    public static class LoguearRespuestaHTTPMiddlewareExtensions
    {
        /*IApplicationBuilder(en este metodo(UseLoguearRespuestaHTTP) estamos agregando por asi decirlo a la lista de metodos que tiene la interface IApplicationBuilder )
         * Estamos agregando el método UseLoguearRespuestaHTTP como una extensión al 
         * objeto IApplicationBuilder. Al hacer esto, podemos llamar a este método 
         * en el método Configure de la clase Startup y agregar 
         * el middleware LoguearRespuestaHTTPMiddleware a la pipeline de solicitudes. 
         * De esta manera, el middleware será utilizado para todas las solicitudes que lleguen al servidor.*/
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();

        }

    }
    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

        public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LoguearRespuestaHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

       
        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = contexto.Response.Body;

                contexto.Response.Body = ms;

                await siguiente(contexto);


                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;
               // logger.LogInformation("aaaaaaaaaaaaaaaaaaaaaaaaaa" + cuerpoOriginalRespuesta);
                logger.LogInformation(respuesta);
            }
        }

    }
}
