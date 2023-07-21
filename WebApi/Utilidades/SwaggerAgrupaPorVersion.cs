using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace WebApi.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceControlador = controller.ControllerType.Namespace;//esto nos trera el nombre ded los controller es decir
            // esto "WebApi.Controllers.V1"
            var versionAPI = nameSpaceControlador.Split('.').Last().ToLower();//v1

            //Y con esto es que creamos los grupos de verciones
            controller.ApiExplorer.GroupName = versionAPI;


        }
    }
}
