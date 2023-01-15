using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace GenericControllerLib.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
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
