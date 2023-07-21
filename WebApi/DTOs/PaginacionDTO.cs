namespace WebApi.DTOs
{
    public class PaginacionDTO
    {

        public int Pagina { get; set; } = 1;
        private int recordsPorPaginas = 10;
        private readonly int cantidadMaximaPorPaginas = 50;

        public int RecordsPorPaginas
        {
            get
            {

                return recordsPorPaginas;
            }
            set {

                //bueno aqui si el usuario nos dice que quiere digamos 100 records por paginas esa cantidad 
                // supera la cantidad maxima que nosotros permitimos por paginas
                // es decir si es mayor a esto cantidadMaximaPorPaginas 
                // en ese caso lo que se le va a devolver es 50
                // es decir la cantidad maxima que nosotros permitimos 
                // cantidadMaximaPorPaginas = 50;
                // en caso contrario, se le va a devolver la cantidad que pidio, e decir si pide 15 se le devolveran esos 15
                recordsPorPaginas = (value > cantidadMaximaPorPaginas) ? cantidadMaximaPorPaginas : value;
            }

        }
    }
}
