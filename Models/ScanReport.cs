namespace WebProtectorApi.Models
{
    public class ScanReport
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string SecurityGrade { get; set; } = "F";
        public int SecurityScore { get; set; }
        public string FoundIssues { get; set; } = string.Empty;
        public string UserNote { get; set; } = string.Empty;
        public DateTime ScannedAt { get; set; } = DateTime.Now;
    }
}