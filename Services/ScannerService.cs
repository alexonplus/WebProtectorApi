using WebProtectorApi.Models;

namespace WebProtectorApi.Services
{
    public class ScannerService : IScannerService
    {
        private readonly HttpClient _httpClient;

        public ScannerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ScanReport> ScanUrlAsync(string url)
        {
            var report = new ScanReport { Url = url, ScannedAt = DateTime.Now };

            try
            {
                var response = await _httpClient.GetAsync(url);
                int score = 100;
                string issues = "";

                // simple checks for demonstration purposes
                if (!url.StartsWith("https"))
                {
                    score -= 40;
                    issues += "Missing HTTPS. ";
                }

                if (!response.Headers.Contains("X-Frame-Options"))
                {
                    score -= 20;
                    issues += "Clickjacking protection (X-Frame-Options) missing. ";
                }

                report.SecurityScore = score;
                report.FoundIssues = string.IsNullOrEmpty(issues) ? "No major issues found." : issues;
                report.SecurityGrade = CalculateGrade(score);
            }
            catch (Exception ex)
            {
                report.FoundIssues = $"Error scanning site: {ex.Message}";
                report.SecurityGrade = "F";
                report.SecurityScore = 0;
            }

            return report;
        }

        private string CalculateGrade(int score) => score switch
        {
            >= 90 => "A",
            >= 75 => "B",
            >= 50 => "C",
            >= 30 => "D",
            _ => "F"
        };
    }
}