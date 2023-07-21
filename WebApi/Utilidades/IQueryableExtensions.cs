using System.Linq;
using WebApi.DTOs;

namespace WebApi.Utilidades
{
    public static class IQueryableExtensions
    {
        // aqui estamos agregando este metodo a la interface de IQueryable para asi poderlo usar como un metodo mas
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPaginas)
                .Take(paginacionDTO.RecordsPorPaginas);

        }
    }
}
