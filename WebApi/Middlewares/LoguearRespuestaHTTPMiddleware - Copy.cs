using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace WebApi.Middlewares
{
    public class LoguearRespuestaHTTPMiddlewares
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHTTPMiddlewares> logger;

        public LoguearRespuestaHTTPMiddlewares(RequestDelegate siguiente, ILogger<LoguearRespuestaHTTPMiddlewares> logger)
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
