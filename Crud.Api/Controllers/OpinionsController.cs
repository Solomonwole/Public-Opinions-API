using Crud.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crud.Api.Controllers
{
    [ApiController]
    [Route("api/opinions")]
    public class OpinionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public OpinionsController(AppDbContext db)
        {
            _db = db;
        }

        // GET ALL PUBLIC OPINIONS
        [HttpGet]
        public async Task<IActionResult> GetAllOpinions()
        {
            var opinions = await _db.Opinions
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new
                {
                    o.Id,
                    o.Title,
                    o.Content,
                    Author = o.User.UserName,
                    o.CreatedAt
                }).ToListAsync();


            return Ok(opinions);
        }
    }
}
