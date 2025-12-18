using Crud.Api.Data;
using Crud.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Crud.Api.DTOs.OpinionDTOs;

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
        public async Task<IActionResult> GetAllOpinions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var query = _db.Opinions
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt);

            var totalCount = await query.CountAsync();

            var opinions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new
                {
                    o.Id,
                    o.Title,
                    o.Content,
                    Author = o.User.UserName,
                    o.CreatedAt
                }).ToListAsync();


            return Ok(new
            {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                datta = opinions
            });
        }

        // CREATE OPINION
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateOpinionDto dto)
        {
            var userId = GetUserId();

            var opinion = new Opinion
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = userId,
            };

            try { 
            _db.Opinions.Add(opinion);
            await _db.SaveChangesAsync();
                return Ok();
            } catch (Exception ex) {
                return Forbid(ex.Message);
            }
            
        }

        // UPDATE OPINION (ONLY OWNER)
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateOpinionDto dto)
        {
            var userId = GetUserId();
            var opinion = await _db.Opinions.FindAsync(id);

            if (opinion == null)
                return NotFound("Opinion not found.");
            if (opinion.UserId != userId)
                return Forbid("You are not authorized to update this opinion.");
            opinion.Title = dto.Title;
            opinion.Content = dto.Content;
            opinion.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok();
        }

        // DELETE OPINION (OWNER ONLY)
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var opinion = await _db.Opinions.FindAsync(id);

            if (opinion == null) return NotFound();
            if (opinion.UserId != userId) return Forbid();

            _db.Opinions.Remove(opinion);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
