using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InsanK.Services;
using InsanK.DTOs;

namespace InsanK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OneriController : ControllerBase
    {
        private readonly CVService _cvService;

        public OneriController(CVService cvService)
        {
            _cvService = cvService;
        }

        [Authorize]
        [HttpGet("cv/{ilanId}")]
        public async Task<IActionResult> CVOnerisiAl(int ilanId)
        {
            var kullaniciId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            try
            {
                var oneri = await _cvService.CVOneriOlustur(kullaniciId, ilanId);
                return Ok(oneri);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}