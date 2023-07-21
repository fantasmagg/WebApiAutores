using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers.V1;
using WebApi.test.Moks;

namespace WebApi.test.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTest
    {

        [TestMethod]
        public async Task devuelve4linksCuandoEsAdmin()
        {
            //preparacion
            var autorisado = new AutorizacionServicesMoks();
            autorisado.resultado = AuthorizationResult.Success();
            var rootController = new RootController(autorisado);
            rootController.Url = new URLHelperMoks();

            //ejecucion

            var resultado = await rootController.get();

            //validacion
            Assert.AreEqual(4,resultado.Value.Count());
        }

        [TestMethod]
        public async Task devuelve2linksCuandoEsAnoimo()
        {
            //preparacion
            var autorisado = new AutorizacionServicesMoks();
            autorisado.resultado = AuthorizationResult.Failed();
            var rootController = new RootController(autorisado);
            rootController.Url = new URLHelperMoks();

            //ejecucion

            var resultado = await rootController.get();

            //validacion
            Assert.AreEqual(2, resultado.Value.Count());
        }


        [TestMethod]
        public async Task devuelve2linksCuandoEsAnoimo_ConMoq()
        {
            //preparacion
            //NOTA: Mock = se instala una libreria llamada moq desde nuget
            //NOTA:Mock: esta wea nos facilita la vida un monton en estos casos
            //esta clase lo que hace es simular otras clases
            //new Mock<IAuthorizationService>() aqui lo que estamos haciendo es instaciando 
            // la clase de IAuthorizationService, para poderla usar
            // luego hacemos un levantamiento
            /*
              autorisadoMock.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            esto de aqui
               It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
            en este punto estamos simulando el metodo que vamos a usar, ahi estamos llamando los parametos del metodo
            mira
           Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements);
            ClaimsPrincipal
            object
            IEnumerable

            como son dos metodos los que vamos a usar de esa clase, NOTA no siempre se usan todos los metodos
            Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName);
            ClaimsPrincipal
            object
            string

            NOTA 
            algo a tener en cuenta es lo que se devuelve 
            Task.FromResult(AuthorizationResult.Failed()
            en este caso estamos devolviendo eso, en este caso lo que devolvemos tiene mucha importancia
            porque es lo que determina si sera admin o no
            en este caso estamos diciendo que no
             */
            var autorisadoMock = new Mock<IAuthorizationService>();
            autorisadoMock.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));
            

            autorisadoMock.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var rootController = new RootController(autorisadoMock.Object);


            //NOTA aqui es lo mismo
            var urlMock = new Mock<IUrlHelper>();
            urlMock.Setup(x => x.Link(
                It.IsAny<string>(),
                It.IsAny<object>()
                )).Returns(string.Empty);


            rootController.Url =urlMock.Object;

            //ejecucion

            var resultado = await rootController.get();

            //validacion
            Assert.AreEqual(2, resultado.Value.Count());
        }
        [TestMethod]
        public async Task devuelve4linksCuandoEsAnoimo_ConMoq()
        {
            //preparacion
            var autorisadoMock = new Mock<IAuthorizationService>();
            autorisadoMock.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Success()));


            autorisadoMock.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Success()));

            var rootController = new RootController(autorisadoMock.Object);
           
            var urlMock = new Mock<IUrlHelper>();
            //en caso de que queramos que nos devuelva un valor expecifico
            var ss = urlMock.Setup(x => x.Link(
                It.IsAny<string>(),
                It.IsAny<object>()
                )).Returns(string.Empty);


            rootController.Url = urlMock.Object;

            //ejecucion

            var resultado = await rootController.get();

            //validacion
            Assert.AreEqual(4, resultado.Value.Count());
        }

        [TestMethod]
        public async Task devuelve2linksCuandoEsAnoimo_ConMoq_ElLinkDevuelveUnValorExpesifico()
        {
            //preparacion
            var autorisadoMock = new Mock<IAuthorizationService>();
            autorisadoMock.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));


            autorisadoMock.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var rootController = new RootController(autorisadoMock.Object);
            string expectedUrl = "https://ejemplo.com/api/root/v1";
            var urlMock = new Mock<IUrlHelper>();
            //en caso de que queramos que nos devuelva un valor expecifico
           var ss = urlMock.Setup(x => x.Link("ObtenerRootv1", new { })).Returns(expectedUrl);


            rootController.Url = urlMock.Object;

            //ejecucion

            var resultado = await rootController.get();

            //validacion
            Assert.AreEqual(2, resultado.Value.Count());
        }

    }
}
