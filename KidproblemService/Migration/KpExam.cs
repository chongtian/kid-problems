namespace Migration
{
    internal class KpExam
    {
        public string? ProblemTitle { get; set; }
        public string? ProblemCategory { get; set; }
        public string? ProblemYear { get; set; }
        public string? ProblemAnswer { get; set; }
        public string? AnswerOptions { get; set; }
        public string? Answer { get; set; }
        public bool? IsCorrect { get; set; }
        public bool? IsGuess { get; set; }
        public string? AnswerBy { get; set; } // need conversion
        public DateTime? AnswerTime { get; set; }
        public double? Duration { get; set; }
        public string? FamilyId { get; set; } // need conversion
        public string? ExamTitle { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public double? TotalDuration { get; set; }
    }
}
