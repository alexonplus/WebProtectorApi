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
    [Authorize] // Requires JWT token for all actions
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
        // Runs security analysis and calculates risk score based on findings
        [HttpPost("scan")]
        public async Task<ActionResult<ScanReport>> ScanWebsite([FromBody] string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest("URL is required.");

            // Get current user ID from token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            // Execute local security analysis service
            var reportResult = await _scannerService.PerformLocalCheck(url);

            // --- RISK SCORING LOGIC ---
            int score = 100; // Perfect base score

            // Critical: Penalty for non-HTTPS connections
            if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                score -= 40;

            // Penalty for missing security headers in report text
            if (reportResult.Contains("Missing X-Frame-Options")) score -= 20;
            if (reportResult.Contains("Missing Content-Security-Policy")) score -= 20;
            if (reportResult.Contains("Missing X-Content-Type-Options")) score -= 10;

            // Clamp score to minimum of 0
            if (score < 0) score = 0;

            // Determine Grade based on final Score
            string grade = score >= 90 ? "A" :
                           score >= 75 ? "B" :
                           score >= 50 ? "C" :
                           score >= 30 ? "D" : "F";
            // ---------------------------

            // Map data to the report entity
            var scanReport = new ScanReport
            {
                Url = url,
                FoundIssues = reportResult,
                SecurityGrade = grade,
                SecurityScore = score,
                ScannedAt = DateTime.Now,
                UserNote = string.Empty,
                UserId = userId
            };

            // Persist report to the database
            _context.ScanReports.Add(scanReport);
            await _context.SaveChangesAsync();

            return Ok(scanReport);
        }

        // GET: api/Scanner/reports
        // Retrieves only reports belonging to the authenticated user
        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<ScanReport>>> GetMyReports()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return await _context.ScanReports
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        // PUT: api/Scanner/report/{id}/note
        // Allows user to update personal notes for a specific report
        [HttpPut("report/{id}/note")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] string note)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var report = await _context.ScanReports
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (report == null) return NotFound("Report not found or access denied.");

            report.UserNote = note;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Scanner/report/{id}
        // Permanently removes a report from the user's history
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