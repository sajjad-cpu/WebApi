using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Pinjet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // این متد عمومی است، نیاز به توکن ندارد
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok("این مسیر عمومی است و هر کسی می‌تواند ببیند.");
        }

        // این متد فقط برای کاربرانی مجاز است که JWT معتبر دارند
        [Authorize]
        [HttpGet("private")]
        public IActionResult PrivateEndpoint()
        {
            return Ok("شما وارد شدید! این مسیر محافظت‌شده است.");
        }

        // در این متد اطلاعات کاربر لاگین‌شده از درون توکن خوانده می‌شود
        [Authorize]
        [HttpGet("userinfo")]
        public IActionResult GetUserInfo()
        {
            // دسترسی به Claims از توکن
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);

            return Ok(new
            {
                message = "اطلاعات از توکن استخراج شد",
                id = userId,
                username,
                email
            });
        }
    }
}
