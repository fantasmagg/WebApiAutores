using System.ComponentModel.DataAnnotations;
using WebApi.validaciones;

namespace WebApi.test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraletramayusculaAttributeUnitTest
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevolverError()
        {
            //preparacion
            var primeraLitraMayuscula = new PrimeraletramayusculaAttribute();
            var valor = "saitama";
            var valContext = new ValidationContext(new { Nombre = valor });


            //ejecucion
            var resultado = primeraLitraMayuscula.GetValidationResult(valor,valContext);

            // verificacion
            Assert.AreEqual("La primera letra debe de ser mayuscula", resultado.ErrorMessage);
        }
        [TestMethod]
        public void ValorNulo_NoDevolverError()
        {
            //preparacion
            //NOTA:primeraLitraMayuscula pertenece a la clase de ValidationResult
            //Por eso podemos usar "GetValidationResult"
            var primeraLitraMayuscula = new PrimeraletramayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });


            //ejecucion
            var resultado = primeraLitraMayuscula.GetValidationResult(valor, valContext);

            // verificacion
            //NOTA: porque estamos validando que si resultado es nulo
            //bueno eso lo hacemos porque cuando le pasamos el valor aqui a GetValidationResult(valor, valContext)
            //si ese valor que nos devulve es nulo signfica que es verdadero
            //porque lo que nosotros lo que estamos validado es si la primera letra de una sentencia
            // es mayuscula, pero si le pasamos un valor null, no hay nada que ver, y por eso
            // devolvemos succes
            //NOTA tambien se devolveria succes si el valor que le pasamos comple con las regla
            //pero en ese caso no se devolveria un null al usuario
            Assert.IsNull(resultado);
        }
        [TestMethod]
        public void ValorPrimeraLetraMayuscila_NoDevolverError()
        {
            //preparacion
            //NOTA:primeraLitraMayuscula pertenece a la clase de ValidationResult
            //Por eso podemos usar "GetValidationResult"
            var primeraLitraMayuscula = new PrimeraletramayusculaAttribute();
            string valor = "Saitama";
            var valContext = new ValidationContext(new { Nombre = valor });


            //ejecucion
            var resultado = primeraLitraMayuscula.GetValidationResult(valor, valContext);

            
            Assert.IsNull(resultado);
        }
    }
}