using WebProtectorApi.Models;

namespace WebProtectorApi.Services
{
    public interface IScannerService
    {
        Task<ScanReport> ScanUrlAsync(string url);
    }
}