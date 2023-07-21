using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Filtros;
using WebApi.Middlewares;
using WebApi.Servicios;
using WebApi.Utilidades;


//[assembly: Microsoft.AspNetCore.Mvc.ApiConventionType(typeof(Microsoft.AspNetCore.Mvc.DefaultApiConventions))]
//por alguna razon no me funciona
namespace WebApi
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            // esta linea de aca es para el mapeo que se hace con los valores de los claim del usuario
            //se limpien, por asi decirlo, y que nos lo traiba el mapeo basado en la configuracion que le hemos dado
            // ejemplo de la configuraicon que le dimos nosotros
            /*
             var claims = new List<Claim>()
            {
                //aqui adentro podemos poner literalmente cualquier cosa como confiable
                new Claim("email",credencialesUsuario.Email)//,
                //new Claim("cual quier cosas","cual quier valor")
            };

            entonces asi se aria el mapeo 
            email = emaildelusuario@.com
             
             */
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            // esta linea inicial mente estaba solo asi "services.AddControllers();"
            // pero le  agregamos esta parte .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // para evitar el error del ciclo infinito de autores y libros, la puedes aplicar siempre en tus apis para evitar esos errores
            services.AddControllers(opciones =>
            {
                //esto es un filtro para los errores
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
                //esto es para aplicar los grupos por versiones
                opciones.Conventions.Add(new SwaggerAgrupaPorVersion());
                
            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
           

            // INJECTION DE DEPENDENCIAS

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

          


            // aqui estamos agregando los servicios de cache, diras "o pero no hay ninguna clase que tenga alguna referencia ni nada"
            //en este caso no es necesario por que AddResponseCaching es algo que trae en la base del codigo al igual que otros add que vemos aqui
            services.AddResponseCaching();// este son los servicios del middleware filtro


            //aqui estamos agregando un esquema de autenticasion basica por default por asi decirlo
            //JwtBearerDefaults eso tenemos que instalarlo desde nuget, tiene que ser la misma version a todo lo que tengamos instalado
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer =false,
                    ValidateAudience =false,
                    ValidateLifetime =true,
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                    ClockSkew  = TimeSpan.Zero

                }
                );



            services.AddEndpointsApiExplorer();
            // c.SwaggerDoc("v1",  esta primera parte esta "v1" lo que esta haciendo es estableciendo el nombre de la version de nuestra api
            // eso tiene que ser unico haci evitamos errores en un futuro
            services.AddSwaggerGen( c => {

                /*new OpenApiInfo 
            
                    { Title = "aaaaa", Version = "v1" });  en esta parte ya lo qye estamos haciendo es agregandole informacion extra
                    tambien aqui Title = "aaaaa" esta parte lo que esta haciendo es cambiarle el nombre en la interface(esto es de la parte estetica)
                     Version = "v1" y esta parte tambien es de la interface(tambien es de la parte extetica) esa "v1" no tiene que coinsider exactamente con la "v1"
                    que tenemos mas arriba 
                 */

                c.SwaggerDoc("v1", new OpenApiInfo {  
                    Title = "haaa soy el titulo",
                    Version = "v1",
                    Description="Esto es una api de autores y libros",
                    Contact = new OpenApiContact
                    {
                        Email="obedsilvestre339@gmail.com",
                        Name="Obed Silvestre",
                        Url= new Uri("https://nohayNadaEnEsteMundo.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }

                    
                
                });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "haaa soy el titulo", Version = "v2" });

                //NOTA: esto solo funciona si vamos a usar swager
                // esto lo que hace es agregar un parametro,
                // como si estuvieramos agregandocelo directamente
                // desde el metodo empoin
                // no es necesario dik hacer algo en los metodos ni nada
                // esto lo hace automaticamente
                c.OperationFilter<AgregarParametroHATEOAS>();
                c.OperationFilter<AgregarParametroXvercion>();

                // apartir de aqui abajo es para las configuracion de nuestro token, que podamos autenticarnos  
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header


                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id= "Bearer"
                            }
                        },
                        new string[] { }

                    }
                });

                //aqui estamos haciendo un nombre con el archivo que se este ejecutando en ese momento
                //en este caso seria el nombre de nuestra api,
                var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // y aqui estamos creando la ruta con, la ruta base de nuestra aplicasion
                //mas el nombre del archivo que se este ejecutando
                var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
                //y aqui se esta llamando para poder incluir los comentarios en el archivo XML
                c.IncludeXmlComments(rutaXML);

            });

            services.AddAutoMapper(typeof(Startup));

            // con esto que que podemos configurar el identity
            // para usar el identity tenemos que instalar primero el
            //Microsoft.AspNetCore.Identity.EntityFrameworkCore
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            //aqui ahora vamos a ponerle otra regla a las autorizaciones
            services.AddAuthorization(opciones =>
            {
                // esto lo que hace es para las peticiones que solo son para los admins
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
            });


            // esto es un nivel de seguridad mas es para que no cualquier pagina web pueda hacer peticiones a nuestra api por asi decirlo
            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                    {
                        //WithOrigins("") aqui especificamos cuales paginas si pueden
                        //NOTA WithOrigins("https://speeding-shuttle-924117.postman.co") esto es un ejemplo, no funciona
                        builder.WithOrigins("https://speeding-shuttle-924117.postman.co").AllowAnyMethod().AllowAnyHeader()
                        //esta linea es encaso de que tengamos que esponer los headers
                        //builder.WithOrigins("").AllowAnyMethod().AllowAnyHeader().WithExposedHeaders();
                        //NOTA esto es para exponer la cabecera de la paginacion
                        .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
                    }
                );
            }

            );

            services.AddTransient<GeneradorEnlase>();
            services.AddTransient<HATEOASAutorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //esto es para tener las funciones de encriptamiento
            services.AddDataProtection();
            //esto es nuestro servicio de hash
            //esto tenemos que programarlo primero
            //en este caso se encuentra en la carpeta servicio
            services.AddTransient<HashService>();

            //nuevo
            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
        {

           

            // este
            app.UseLoguearRespuestaHTTP();



            


            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
               
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiaaaaaa v1");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiaaaaaa v2");
                });

            app.UseHttpsRedirection();

            app.UseRouting();

            //con esto activamos los cors
            app.UseCors();
 

            app.UseAuthorization();

            app.UseEndpoints(endpints =>
            {
                endpints.MapControllers();
            });

        }
    }
}
