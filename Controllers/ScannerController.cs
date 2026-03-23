using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebProtectorApi.Data;
using WebProtectorApi.Entities;
using WebProtectorApi.Services;

namespace WebProtectorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Enforce JWT authentication for all scanner actions
    public class ScannerController : ControllerBase
    {
        private readonly WebProtectorDbContext _context;
        private readonly IScannerService _scannerService;

        public ScannerController(WebProtectorDbContext context, IScannerService scannerService)
        {
            _context = context;
            _scannerService = scannerService;
        }

        // POST: api/Scanner/scan
        [HttpPost("scan")]
        public async Task<ActionResult<ScanReport>> ScanWebsite([FromBody] string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest("URL is required");

            // 1. Extract User ID from the authenticated token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            // 2. Perform local security analysis via service
            var reportResult = await _scannerService.PerformLocalCheck(url);

            // 3. Map service results and bind report to the current user
            var scanReport = new ScanReport
            {
                Url = url,
                FoundIssues = reportResult, // Storing detailed text report
                SecurityGrade = "A",        // Default grade for initial scan
                SecurityScore = 100,        // Default score
                ScannedAt = DateTime.Now,   // Timestamp of the operation
                UserNote = string.Empty,
                UserId = userId             // LINKING REPORT TO USER
            };

            // 4. Persist the report to the SQL database
            _context.ScanReports.Add(scanReport);
            await _context.SaveChangesAsync();

            return Ok(scanReport);
        }

        // GET: api/Scanner/reports
        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<ScanReport>>> GetMyReports()
        {
            // 1. Get current authenticated user's ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 2. Filter the database results to show ONLY reports belonging to this user
            return await _context.ScanReports
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        // PUT: api/Scanner/report/{id}/note
        [HttpPut("report/{id}/note")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] string note)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Find the report and ensure it belongs to the current user
            var report = await _context.ScanReports
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (report == null) return NotFound("Report not found or access denied.");

            // Allow users to add manual security notes to their own reports
            report.UserNote = note;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/Scanner/report/{id}
        [HttpDelete("report/{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var report = await _context.ScanReports
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (report == null)
                return NotFound("Report not found or access denied.");

            _context.ScanReports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}