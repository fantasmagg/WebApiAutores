namespace WebApi.DTOs
{
    public class DataHATEOAS
    {
        public string Enlace { get; set; }
        public string Descripcion { get; set; }
        public string Metodo { get; set; }

        public DataHATEOAS(string enlace, string descripcion, string metodo)
        {
            Enlace = enlace;
            Descripcion = descripcion;
            Metodo = metodo;
        }
    }
}
