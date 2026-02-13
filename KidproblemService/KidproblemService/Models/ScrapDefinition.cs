namespace KidproblemService.Models
{
    public class ScrapDefinition
    {
        public string? StartUrl { get; set; }
        public string? ProblemCategory { get; set; }
        public string? ProblemYear { get; set; }
        public string? RegexPattern { get; set; }
        public bool? UseSinglePattern { get; set; }
        public string? StartPattern { get; set; }
        public string? EndPattern { get; set; }
    }
}
