using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;

namespace GenericControllerLib.BusinessLogic
{
	public static class Util
	{
		internal static IQueryable<T> Include<T>(IQueryable<T> list, List<string> includes) where T : class
		{
			try
			{
				if (includes == null || includes.Count == 0)
					return list;

				return includes.Aggregate(list, (current, include) => current.Include(include));
			}
			catch
			{
				throw;
			}
		}
	}
}
