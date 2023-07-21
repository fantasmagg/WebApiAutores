using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace WebApi.Utilidades
{
    public class HATEOASFiltrosAttribute: ResultFilterAttribute
    {
        // esta clase lo que hace es verificar si la respuesta del valor que vamos a resivir por el filtro es Y
        protected bool DebeIncluirHATEOAS(ResultExecutingContext context)
        {
            
            var resultado = context.Result as ObjectResult;

            

            if (!EsRespuestaExitosa(resultado))
            {
                return false;
            }
            var cabecera = context.HttpContext.Request.Headers["incluirHateoas"];

            if (cabecera.Count ==0)
            {
                return false;
            }

            var valor = cabecera[0];

            if (!valor.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;


        }

        private bool EsRespuestaExitosa( ObjectResult resultado)
        {
            if(resultado == null || resultado.Value == null)
            {
                return false;
            }
            if(resultado.StatusCode.HasValue && !resultado.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }
            return true;
            
        }
    }
}
