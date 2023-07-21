using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace WebApi.Utilidades
{
    public class AgregarParametroHATEOAS : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //bueno esto es obvio pero esto lo que esta haciendo es entra a los tipos
            // de peticiones de http y mirar si son "GET" aqui estamos diciendo si son diferente
            // de "GET" pues que se devuelva y no continue, en caso contrario no entrar al if
            // es decir si, si es un "GET" 
            if (context.ApiDescription.HttpMethod != "GET")
            {
                return;
            }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "incluirHateoas",
                In = ParameterLocation.Header,
                Required = false
            });

        }
    }
}
