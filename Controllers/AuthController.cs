using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pinjet.Data;
using Pinjet.DTO;
using Pinjet.Helpers;
using Pinjet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pinjet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PinjetDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(PinjetDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                FullName = dto.Username,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully",
                userId = user.Id
            });
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized(new { massegae = "Invalid email or password" });

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized(new { massegae = "Invalid email or password" });

            var token = CreateAccessToken(user);

            var refreshTokenString = TokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                isRevoked = false,
                UserId = user.Id
            };

            _context.RefreshToken.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                massage = "Login successful",
                token = token,
                username = user.FullName
            });
        }

        private string CreateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"])),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHndler = new JwtSecurityTokenHandler();
            var token = tokenHndler.CreateToken(tokenDescriptor);

            return tokenHndler.WriteToken(token);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto request)
        {
            //ذخیره اطلاعات یوزر با توکن
            var storedToken = await _context.RefreshToken
                .Include(x => x.User)
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

            if (storedToken == null)
            {
                return Unauthorized("Invalid Refresh Token");
            }

            if (storedToken.Expires < DateTime.UtcNow)
            {
                return Unauthorized("Invalid Refresh Expired");
            }

            if (storedToken.isRevoked)
            {
                return Unauthorized("This token has been revoked");
            }

            var user = storedToken.User;
            var newAccessToken = CreateAccessToken(user);

            storedToken.isRevoked = true;

            var newRefreshTokenString = TokenGenerator.GenerateRefreshToken();
            var newRefreshToken = new RefreshToken
            
            {
                Token = newRefreshTokenString,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"])),
                Created = DateTime.UtcNow,
                UserId = user.Id
            };

            _context.RefreshToken.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }
    }
}
