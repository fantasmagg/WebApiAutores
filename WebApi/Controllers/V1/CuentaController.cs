using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Servicios;


// NOTA IMPORTANTE PARA TODO ESTO DEL REGISTRO Y TODO ESO DE LOS TOKEN NECESITAMOS UNA LIBRERIA Y ES ESTA
//Microsoft.AspNetCore.Identity.EntityFrameworkCore
namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/V1/cuneta")]
    public class CuentaController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentaController(UserManager<IdentityUser> userManager, IConfiguration configuration,
            SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider, HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("no_se_puede_con_el_IConfiguration");
        }

        //JODIDAMENTE IMPORTANTE
        //[HttpGet("hash/{textoPlano}")]
        //public ActionResult RalizarHash(string textoPlano)
        //{
        //    //hash 1
        //    var resultado1 = hashService.Hash(textoPlano);
        //    var resultado2 = hashService.Hash(textoPlano);

        //    return Ok( new
        //    {
        //        textoPlano = textoPlano,
        //        hash1 = resultado1,
        //        hash2 = resultado2
        //    }
        //    );
        //}

        //[HttpGet("encriptado")]
        //public ActionResult Encriptado()
        //{
        //    var textPlano = "hola mundo";
        //    var textoEncriptado = dataProtector.Protect(textPlano);
        //    var textDesencriptado = dataProtector.Unprotect(textoEncriptado);

        //    return Ok(new
        //    {
        //        textoPlano=textPlano,
        //        textoEncriptado=textoEncriptado,
        //        textDesencriptado=textDesencriptado
        //    });
        //}

        //[HttpGet("encriptadoPortiempo")]
        //public ActionResult EncriptadoPortiempo()
        //{

        //    //esto es para encriptar con un limite de tiempo
        //    var protectoLimitadoPorTiempo = dataProtector.ToTimeLimitedDataProtector();

        //    var textPlano = "hola mundo";
        //    var textoEncriptado = protectoLimitadoPorTiempo.Protect(textPlano,lifetime: TimeSpan.FromSeconds(5));
        //    //esto lo puce para ver el error que nos da cuando se pasa el tiempo y no hemos desencriptado el texto
        //    Thread.Sleep(6000);
        //    var textDesencriptado = protectoLimitadoPorTiempo.Unprotect(textoEncriptado);

        //    return Ok(new
        //    {
        //        textoPlano = textPlano,
        //        textoEncriptado = textoEncriptado,
        //        textDesencriptado = textDesencriptado
        //    });
        //}

        [HttpPost("registro", Name = "registrarUsuariov1")]//api/cuenta/registro
        public async Task<ActionResult<RespuestaAutenticacion>> Registro(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }


        }
        [HttpPost("login", Name = "loginUsuariov1")]
        public async Task<ActionResult<RespuestaAutenticacion>> login(CredencialesUsuario credencialesUsuario)
        {
            //isPersistent= eso es para cuando estamos trabajando con las cookies de autorisacion de presistencia
            //lockoutOnFailure= eso es por si el usuario falla varia veces en insertar sus credencias que la cuenta se bloquee
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }

        }
        [HttpGet("RenovarToken", Name = "renovarTokenv1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            //y para que los comentarios puedan ser hechos solo por personas registradas
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            var email = emailClaim.Value;
            var credenciales = new CredencialesUsuario()
            {
                Email = email
            };
            return await ConstruirToken(credenciales);
        }





        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {

            //que es esto de Claim, esto se encuentra en la seccion de seguridad en el cap 79
            //pero en resumen, Claim es como una informacion que  nosotros ponemos como que es confiable
            // de parte del usuario, por que de parte del usuario, porque el email nos lo da el usuario,
            // y noostros lo colocamos como confiable
            var claims = new List<Claim>()
            {
                //aqui adentro podemos poner literalmente cualquier cosa como confiable
                new Claim("email",credencialesUsuario.Email)//,
               // ,new Claim("EsAdmin","esAdmin")// esto es para que nos generar un token de un admin
                //new Claim("cual quier cosas","cual quier valor")
            };

            //aqui estamos buscando el email del usuario
            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            // y aqui estamos buscando sus claims, esto es para ver si tiene algun claims de "esAdmin"
            var claimsDB = await userManager.GetClaimsAsync(usuario);
            //en caso de que lo tenga se le agrgaran a la lista de claims que esta arriba
            //NOTA hay un video de esto en seguridad
            claims.AddRange(claimsDB);

            //JWT
            //Este objeto representa una clave secreta simétrica utilizada para firmar y validar los tokens JWT.
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));

            //Aquí se crea un objeto SigningCredentials, que representa las credenciales de firma utilizadas para firmar el token JWT.
            /*HmacSha256 se utiliza como el algoritmo de firma para proteger la integridad del
             * token. El proceso implica generar una firma utilizando la clave secreta (llave) y los datos del token*/
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);
            /*issuer: null - El emisor (issuer) se refiere a la entidad que emite el token JWT. Al establecerlo como null, se
             * indica que no se especifica un emisor específico para el token. En otras palabras, el token no está asociado con una entidad emisora en particular.

            audience: null - El destinatario (audience) se refiere a la entidad para la cual está destinado el token JWT. Al establecerlo como null, se 
            indica que el token no está dirigido a una entidad en particular. 
            Esto significa que cualquier entidad que tenga el token puede considerarse un destinatario válido.*/
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                /*El método WriteToken() se utiliza para convertir un objeto 
                 * JwtSecurityToken en una cadena de texto que representa el token JWT en formato compacto. */
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };


        }

        [HttpPost("HacerAdmin", Name = "hacerAdminv1")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();

        }

        [HttpPost("RemoveAdmin", Name = "removerAdminv1")]
        public async Task<ActionResult> RemoveAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();

        }


    }
}
