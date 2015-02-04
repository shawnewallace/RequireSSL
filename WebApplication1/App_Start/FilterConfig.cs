using System.Web;
using System.Web.Mvc;
using RequireSSL.web;

namespace WebApplication1
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new RequireSecureConnectionFilter());
			filters.Add(new HandleErrorAttribute());
		}
	}
}
