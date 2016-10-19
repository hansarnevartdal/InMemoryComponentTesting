using System.Threading.Tasks;
using Component.Demo.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Component.Demo.Controllers
{
    [Route("api")]
    public class DemoController : Controller
    {
        private readonly IOtherComponentFacade _otherComponentFacade;

        public DemoController(IOtherComponentFacade otherComponentFacade)
        {
            _otherComponentFacade = otherComponentFacade;
        }

        [HttpGet]
        [Route("nothing")]
        public async Task<string> Get()
        {
            return await Task.FromResult("Nothing to see here.");
        }

        [HttpGet]
        [Authorize]
        [Route("require/authorization")]
        public async Task<string> GetSecret()
        {
            return await Task.FromResult("HODOR!");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("require/role/admin")]
        public async Task<string> GetAdminSecret()
        {
            return await Task.FromResult("Winter is coming!");
        }

        [HttpGet]
        [Authorize]
        [Route("other")]
        public async Task<string> GetOtherSecret()
        {
            return await _otherComponentFacade.GetSecret();
        }
    }
}