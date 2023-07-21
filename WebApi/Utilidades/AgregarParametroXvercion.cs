using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace WebApi.Utilidades
{
    public class AgregarParametroXvercion: IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
        

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "x-version",
                In = ParameterLocation.Header,
                Required = true
            });

        }
    }
}
