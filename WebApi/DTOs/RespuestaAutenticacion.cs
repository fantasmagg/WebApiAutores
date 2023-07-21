using System;

namespace WebApi.DTOs
{
    public class RespuestaAutenticacion
    {
        public string Token { get; set; } 
        public DateTime Expiracion { get; set; }
    }
}
