using Azure.Messaging;
using Microsoft.AspNetCore.Authorization;
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
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly PinjetDbContext _context;
        private readonly IConfiguration _config;

        public UsersController(PinjetDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;   
        }

    }
}
