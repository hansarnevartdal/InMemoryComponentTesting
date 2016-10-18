using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Component.Demo.Controllers
{
    [Route("api/require")]
    public class RequireController : Controller
    {
        [HttpGet]
        [Route("nothing")]
        public string Get()
        {
            return "Nothing to see here.";
        }

        [HttpGet]
        [Authorize]
        [Route("authorization")]
        public string GetSecret()
        {
            return "HODOR!";
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("role/admin")]
        public string GetAdminSecret()
        {
            return "Winter is coming!";
        }
    }
}