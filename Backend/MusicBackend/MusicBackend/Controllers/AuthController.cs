using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicBackend.Models;
using MusicBackend.Models.Data.User;
using MusicBackend.Models.DataLayer;
using MusicBackend.Services;
using System.Security.Claims;

namespace MusicBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private MusicContext _context;
        private readonly IPasswordHash _passwordHashService;
        private readonly ITokenGenerator _tokenService;

        public AuthController(MusicContext context, IPasswordHash hashService, ITokenGenerator tokenService)
        {
            _context = context;
            _passwordHashService = hashService;
            _tokenService = tokenService;
        }

        // Logins
        [HttpPost("login")]
        async public Task<IActionResult> Login(UserRequest request)
        {
            var exisitingUser = await _context.Users!.FirstOrDefaultAsync(user => user.Username == request.Username);

            if (exisitingUser == null)
            {
                return BadRequest("User not found");
            }
            else if (!await _passwordHashService.VerifyPasssword(request.Password, exisitingUser.Hash))
            {
                return BadRequest("Incorrect password");
            }

            var acessToken = await _tokenService.GenerateAccessToken(exisitingUser);
            RefreshToken refreshToken = await _tokenService.GenerateRefreshToken();
            await _tokenService.SetRefreshToken(refreshToken, Response);

            exisitingUser.RefreshToken = refreshToken.Token;
            exisitingUser.DateCreated = refreshToken.Created;
            exisitingUser.TokenExpires = refreshToken.Expires;

            await _context.SaveChangesAsync();

            UserResponse userResponse = new UserResponse()
            {
                Token = acessToken,
                IsSuper = exisitingUser.IsSuper,
                Uid = exisitingUser.Id
            };

            return Ok(userResponse);
        }
        [HttpPost("refresh")]
        async public Task<IActionResult> GetRefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var existingUser = await _context.Users!.FirstOrDefaultAsync(user => user.RefreshToken == refreshToken);

            if (existingUser == null)
            {
                return Unauthorized("Invalid Refresh Token");
            }
            else if (existingUser.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired");
            }
            else
            {
                string acessToken = await _tokenService.GenerateAccessToken(existingUser);
                var newRefreshToken = await _tokenService.GenerateRefreshToken();

                await _tokenService.SetRefreshToken(newRefreshToken, Response); //Note: Do i rlly have to set as cookie?

                UserResponse userResponse = new UserResponse()
                {
                    Token = acessToken,
                    IsSuper = existingUser.IsSuper,
                    Uid = existingUser.Id
                };

                return Ok(userResponse);
            }
        }

        // User edits
        [Authorize]
        [HttpPost]
        async public Task<IActionResult> CreateNewUser(UserRequest request)
        {
            var claims = await _tokenService.GetClaims(User);
            if (!claims.IsSuper)
            {
                return Unauthorized("Missing rank");
            }

            var existingUser = await _context.Users!.FirstOrDefaultAsync(user => user.Username == request.Username);
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            string passwordHashed = await _passwordHashService.HashPassword(request.Password);
            string id = Guid.NewGuid().ToString();

            User user = new User()
            {
                Id = id,
                Username = request.Username,
                Hash = passwordHashed,
                IsSuper = false,
            };

            _context.Users!.Add(user);
            await _context.SaveChangesAsync();


            return Ok(id);
        }

        [Authorize]
        [HttpPut("{id}")]
        async public Task<IActionResult> UpdateUser([FromRoute] string id, UserRequest request)
        {
            var claims = await _tokenService.GetClaims(User);

            if (!claims.IsSuper && claims.Id != id)
            {
                return Unauthorized("Cannot acess foreign account");
            }

            User user = await _context.Users!.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            user.Username = request.Username;
            user.Hash = await _passwordHashService.HashPassword(request.Password);

            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }

        [Authorize]
        [HttpDelete("{id}")]
        async public Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            var claims = await _tokenService.GetClaims(User);
            
            if (!claims.IsSuper && claims.Id != id)
            {
                return Unauthorized("Cannot acess foreign account");
            }


            User user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }

        // User fetches
        [Authorize]
        [HttpGet]
        async public Task<IActionResult> GetAllUsers()
        {
            List<User> users = _context.Users.ToList();
            List<DisplayUser> displayUsers;
            DisplayUser.ToDisplayUsers(users, out displayUsers);
            return Ok(displayUsers);
        }

        [Authorize]
        [HttpGet("{id}")]
        async public Task<IActionResult> GetSingleUser([FromRoute] string id)
        {
            User user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            DisplayUser display = user.AsDisplayUser();
            return Ok(display);
        }

    }
}
