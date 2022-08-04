using Microsoft.AspNetCore.Mvc;

namespace SSC.Controllers
{
    public class CommonController : Controller
    {
        protected Guid GetUserId()
        {
            return new Guid(User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
        }
    }
}
