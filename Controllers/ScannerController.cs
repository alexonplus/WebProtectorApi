using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProtectorApi.Data;
using WebProtectorApi.Models;
using WebProtectorApi.Services;

namespace WebProtectorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Only logged in users can scan
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

            // Perform the scan using our service
            var report = await _scannerService.ScanUrlAsync(url);

            // Save the report to the database
            _context.ScanReports.Add(report);
            await _context.SaveChangesAsync();

            return Ok(report);
        }

        // GET: api/Scanner/reports
        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<ScanReport>>> GetMyReports()
        {
            return await _context.ScanReports.ToListAsync();
        }

        // PUT: api/Scanner/report/{id}/note
        [HttpPut("report/{id}/note")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] string note)
        {
            var report = await _context.ScanReports.FindAsync(id);
            if (report == null) return NotFound();

            report.UserNote = note;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}