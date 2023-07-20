using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace GenericControllerLib.Config
{
    /// <summary>
    ///     Aplica cambios sobre los controladores 
    /// </summary>
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        /// <summary>
        ///     Asigna el nombre a los controladores genéricos generados  
        /// </summary>
        /// <param name="controller">Model del controlador</param>
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                var genericType = controller.ControllerType.GenericTypeArguments[0];
                controller.ControllerName = genericType.Name;
            }
        }
    }
}
