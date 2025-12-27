using BasicWebApplicationCsharp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebApplicationCsharp.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HealthController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> CheckAll()
        {
            var dbOk = await _db.Database.CanConnectAsync();

            return Ok(new
            {
                database = dbOk
            });
        }
    }

}
