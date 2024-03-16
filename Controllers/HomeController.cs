using Microsoft.AspNetCore.Mvc;

namespace dev_backend_net.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }
        [HttpGet]
        [Route("home")]
        public async Task<IActionResult> homeget()
        {
            return BadRequest(new
            {
                status = 200,
                msg = "estoy haciendo una perticion en la ruta home"
            });
        }
    }
}