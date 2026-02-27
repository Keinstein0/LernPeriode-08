using HPlusSportAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MusicBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private MusicContext _context;
        
        public AuthController(MusicContext context)
        {
            _context = context;
        }

        [HttpGet("/login")]
        async public IActionResult Login()
        {

        }



        [HttpPost]
        async public IActionResult CreateNewUser()
        {
            throw new NotImplementedException();
        }



    }
}
