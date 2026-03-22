using System.Threading.Tasks;

namespace WebProtectorApi.Services
{
    public interface IScannerService
    {
        // method to perform local checks on the website and return a report as a string
        Task<string> PerformLocalCheck(string url);
    }
}