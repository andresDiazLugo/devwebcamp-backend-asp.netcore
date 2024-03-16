using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dev_backend_net.Models;
using dev_backend_net.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using dev_backend_net.Middleware;
namespace dev_backend_net.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> registerUser([FromBody] User user)
        {
            try
            {
                await _userRepository.registerUser(user);
                var data = new
                {
                    status = 200,
                    success = new { msg = "El usuario fue creado con éxito" }
                };
                return StatusCode(200, data);
                // return new JsonResult(new
                // {
                //     status = 200,
                //     success = new { msg = "El usuario fue creado con éxito" }
                // });
            }
            catch (ApplicationException ex)
            {
                var errors = new List<ErrorModel>
                {
                    new ErrorModel { Field = "email", Message = ex.Message }
                };
                return BadRequest(new
                {
                    status = 422,
                    errors
                });
            }
            catch (Exception)
            {
                var errors = new List<ErrorModel>
                {
                    new ErrorModel { Field = "server", Message = "Surgio un problema al registrar el usuario"}
                };
                return BadRequest(new
                {
                    status = 400,
                    errors
                });
            }

        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> signin([FromBody] UserSignInModel user)
        {
            var response = await _userRepository.getUserByEmailAndPassword(user.email, user.password);
            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
            if (response != null)
            {
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("email",response.email)
                };

                var keyBytes = Encoding.UTF8.GetBytes(jwt.Key);
                var key = new SymmetricSecurityKey(keyBytes);
                var signin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: signin
                );
                var data = new
                {
                    status = 200,
                    data = response,
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                };
                return StatusCode(200, data);
            }
            var error = new
            {
                status = 200,
                error = "El email o password son incorrectos"
            };
            return StatusCode(200, error);
            // return BadRequest(new
            // {
            //     status = 200,
            //     error = "El email o password son incorrectos"
            // });
        }
        [HttpGet]
        [Route("verifytoken")]
        public async Task<IActionResult> verifyToken()
        {
            //esta ruta la usaremos para proteger las rutas del front vamos hacer una peticion a esta ruta cuando cada vez que accedamos a una section que esta protegida el middleware de comparacion de token verificara el token en caso de que el token sea invalido no accedera a la ruta en case de ser valido devolvera esta respuesta y podremos seguir accediendo a nuestra sectiones        
            // return BadRequest(new
            // {
            //     status = 200,
            //     confirm = true
            // });
            var data = new
            {
                status = 200,
                confirm = true
            };
            return Ok(data);
        }
    }
}


public class ErrorModel
{
    public string Field { get; set; }
    public string Message { get; set; }
}