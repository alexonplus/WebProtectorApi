using System.Net.Http;
using System.Text;
using System.Linq;

namespace WebProtectorApi.Services
{
    public class ScannerService : IScannerService
    {
        private readonly HttpClient _httpClient;

        public ScannerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // timeout for site dont stuck the scanner
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<string> PerformLocalCheck(string url)
        {
            var report = new StringBuilder();
            report.AppendLine($"--- Local Security Report for {url} ---");

            try
            {
                var response = await _httpClient.GetAsync(url);
                var headers = response.Headers;

                // 1. (Headers)
                CheckHeader(headers, "X-Frame-Options", report);
                CheckHeader(headers, "Content-Security-Policy", report);
                CheckHeader(headers, "Strict-Transport-Security", report);

                if (headers.Contains("Server"))
                    report.AppendLine("[WARNING] Server info leaked: " + headers.GetValues("Server").FirstOrDefault());

                // 2.(Body)
                var body = await response.Content.ReadAsStringAsync();
                if (body.Contains("<script") && !body.Contains("nonce"))
                    report.AppendLine("[ADVICE] Scripts found without CSP nonces. Vulnerable to XSS.");

                report.AppendLine("--- Local Check Finished ---");
            }
            catch (Exception ex)
            {
                report.AppendLine("[ERROR] Could not reach the site: " + ex.Message);
            }

            return report.ToString();
        }

        private void CheckHeader(System.Net.Http.Headers.HttpResponseHeaders headers, string headerName, StringBuilder report)
        {
            if (!headers.Contains(headerName))
                report.AppendLine($"[CRITICAL] Missing security header: {headerName}!");
            else
                report.AppendLine($"[OK] {headerName} is present.");
        }
    }
}