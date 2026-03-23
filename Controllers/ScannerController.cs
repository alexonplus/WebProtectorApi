using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProtectorApi.Data;
using WebProtectorApi.Entities;
using WebProtectorApi.Services;

namespace WebProtectorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Temporarily disabled for final testing and submission
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
        // Initiates a security scan for a given URL and saves the result to the database
        [HttpPost("scan")]
        public async Task<ActionResult<ScanReport>> ScanWebsite([FromBody] string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest("URL is required");

            // 1. Perform security analysis via the scanner service
            var reportResult = await _scannerService.PerformLocalCheck(url);

            // 2. Map service results to the ScanReport entity
            var scanReport = new ScanReport
            {
                Url = url,
                FoundIssues = reportResult, // Detailed security report text
                SecurityGrade = "A",        // Initial automated grade
                SecurityScore = 100,        // Initial automated score
                ScannedAt = DateTime.Now,   // Operation timestamp
                UserNote = string.Empty
            };

            // 3. Save the generated report to the SQL database
            _context.ScanReports.Add(scanReport);
            await _context.SaveChangesAsync();

            return Ok(scanReport);
        }

        // GET: api/Scanner/reports
        // Retrieves the full history of security scans from the database
        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<ScanReport>>> GetMyReports()
        {
            // Fetch all records from the ScanReports table
            return await _context.ScanReports.ToListAsync();
        }

        // PUT: api/Scanner/report/{id}/note
        // Allows users to update a manual security note for a specific report
        [HttpPut("report/{id}/note")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] string note)
        {
            var report = await _context.ScanReports.FindAsync(id);
            if (report == null) return NotFound();

            // Update user comments for the specific scan record
            report.UserNote = note;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Scanner/report/{id}
        // Fetches a single scan report by its unique ID
        [HttpGet("report/{id}")]
        public async Task<ActionResult<ScanReport>> GetReportById(int id)
        {
            var report = await _context.ScanReports.FindAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }

        // DELETE: api/Scanner/report/{id}
        // Permanently removes a scan report from the database
        [HttpDelete("report/{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var report = await _context.ScanReports.FindAsync(id);
            if (report == null) return NotFound();

            _context.ScanReports.Remove(report);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Report {id} deleted successfully" });
        }
    }
}