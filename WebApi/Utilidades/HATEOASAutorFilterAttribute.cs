using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Servicios;

namespace WebApi.Utilidades
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltrosAttribute
    {
        private readonly GeneradorEnlase generadorEnlase;

        public HATEOASAutorFilterAttribute(GeneradorEnlase generadorEnlase)
        {
            this.generadorEnlase = generadorEnlase;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if (!debeIncluir)
            {
                //await next(); esto es por si hay algun otro fitro por ejecutarce que lo haga
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;

            ////AQUI ESTAMOS PREGUNTANDO EL VALUE ES DEL TIPO AutorDTOs
            //var modelo = resultado.Value as AutorDTOs ?? throw new ArgumentNullException("S esperaba una instancia de AutorDTOs");

            var autorDTO = resultado.Value as AutorDTOs;

            if (autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTOs> ??
                    throw new ArgumentException("se esperaba una instancia de AutorDTO o una list<AutorDTOs>");

                autoresDTO.ForEach(async autor => await generadorEnlase.GenerarEnlaces(autor));
                //NOTA: nose porque se pone eso investiga
                resultado.Value = autoresDTO;
            }
            else
            {
                await generadorEnlase.GenerarEnlaces(autorDTO);
            }

           

            await next();

        }
    }
}
