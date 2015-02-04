using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RequireSSL.web
{
	public class RequireSecureConnectionFilter : RequireHttpsAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null) { throw new ArgumentNullException("filterContext"); }

			if (filterContext.HttpContext.Request.IsLocal) { return; }

			base.OnAuthorization(filterContext);
		}
	}

	public class EnforceHttpsHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			object httpContextBaseObject;

			if (request.Properties.TryGetValue("MS_HttpContext", out httpContextBaseObject))
			{
				var httpContextBase = httpContextBaseObject as HttpContextBase;

				if (httpContextBase != null && httpContextBase.Request.IsLocal)
				{
					return base.SendAsync(request, cancellationToken);
				}
			}

			// if request is remote, enforce https
			if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
			{
				return Task<HttpResponseMessage>.Factory.StartNew(
					() =>
					{
						var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
						{
							Content = new StringContent("HTTPS Required")
						};

						return response;
					});
			}

			return base.SendAsync(request, cancellationToken);
		}
	}
}