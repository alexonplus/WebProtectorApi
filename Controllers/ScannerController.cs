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

            // 1. Perform local security analysis via service
            var reportResult = await _scannerService.PerformLocalCheck(url);

            // 2. Map service results to the ScanReport entity
            var scanReport = new ScanReport
            {
                Url = url,
                FoundIssues = reportResult, // Storing detailed text report
                SecurityGrade = "A",        // Default grade for initial scan
                SecurityScore = 100,        // Default score
                ScannedAt = DateTime.Now,   // Timestamp of the operation
                UserNote = string.Empty
            };

            // 3. Persist the report to the SQL database
            _context.ScanReports.Add(scanReport);
            await _context.SaveChangesAsync();

            return Ok(scanReport);
        }

        // GET: api/Scanner/reports
        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<ScanReport>>> GetMyReports()
        {
            // Retrieve all scan history from the database
            return await _context.ScanReports.ToListAsync();
        }

        // PUT: api/Scanner/report/{id}/note
        [HttpPut("report/{id}/note")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] string note)
        {
            var report = await _context.ScanReports.FindAsync(id);
            if (report == null) return NotFound();

            // Allow users to add manual security notes to specific reports
            report.UserNote = note;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}