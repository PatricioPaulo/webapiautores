using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Test.UnitTest.UnitTest
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTest
    {
        [TestMethod]
        public void PrimerLetraMinusculaError()
        {
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "patricio";
            var valContext = new ValidationContext(new { Name = valor });
            var result = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            Assert.AreEqual("La primera letra debe ser mayúscula", result.ErrorMessage);
        }

        [TestMethod]
        public void NullValueNoError()
        {
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Name = valor });
            var result = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void PrimerLetraMayusculaTest()
        {
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = "Patricio";
            var valContext = new ValidationContext(new { Name = valor });
            var result = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            Assert.IsNull(result);
        }
    }
}