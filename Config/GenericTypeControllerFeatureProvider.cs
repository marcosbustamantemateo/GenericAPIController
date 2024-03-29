﻿using GenericControllerLib.Controllers.API;
using GenericControllerLib.Models;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace GenericControllerLib.Config
{
	/// <summary>
	///     Aplica cambios sobre los controladores 
	/// </summary>
	public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
		/// <summary>
		///     Genera controladores genéricos y sus respectivos métodos de forma dinámica
		/// </summary>
		public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var currentAssembly = typeof(GenericTypeControllerFeatureProvider).Assembly;
            var candidates = currentAssembly.GetExportedTypes().Where(x => x.GetCustomAttributes<GeneratedControllerAttribute>().Any());

            foreach (var candidate in candidates)
            {
                var name = candidate.Name;
                if (!feature.Controllers.Any(t => t.Name == name))
                {
                    var typeInfo = typeof(BaseAPIController<>).MakeGenericType(candidate).GetTypeInfo();
                    feature.Controllers.Add(typeInfo);
                }
            }
        }
    }
}
