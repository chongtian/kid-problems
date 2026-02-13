using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    // To Do: reconsider the design
    // what is the unique id of exam_run?
    // I need to refer back to the exam_def too
    
    [DynamoDBTable("kp_exam_runs")]
    public class ExamRun : BaseModelWithUserName
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("id")]
        public string? Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey]
        [DynamoDBProperty("answer_by")]
        public string? AnswerBy { get { return _answerBy; } set { _answerBy = value; } }

        [DynamoDBGlobalSecondaryIndexRangeKey]
        [DynamoDBProperty("create_time")]
        public DateTime? CreateTime { get; set; }

        [DynamoDBProperty("assignment")]
        public string? AssignmentId { get; set; }

        [DynamoDBProperty("title")]
        public string? ExamTitle { get; set; }

        [DynamoDBProperty("category")]
        public string? ExamCategory { get; set; }

        [DynamoDBProperty("start_time")]
        public DateTime? StartTime { get; set; }

        [DynamoDBProperty("complete_time")]
        public DateTime? CompleteTime { get; set; }

        [DynamoDBProperty("family_id")]
        public string? FamilyId { get; set; }

        [DynamoDBProperty("count")]
        public int? TotalCount { get; set; }

        [DynamoDBProperty("correct")]
        public int? CorrectCount { get; set; }

        [DynamoDBProperty("guess")]
        public int? GuessCount { get; set; }

        [DynamoDBProperty("guess_correct")]
        public int? GuessCorrectCount { get; set; }

        [DynamoDBProperty("duration")]
        public double? TotalDuration { get; set; }

        [DynamoDBProperty("details")]
        public List<ExamRunDetail>? ExamRunDetails { get; set; }
    }

    // This is an intermediate table. Do not use it for aggragation.
    [DynamoDBTable("kp_exam_run_details")]
    public class ExamRunDetail
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("id")]
        public string? Id { get; set; }

        [DynamoDBProperty("problem_title")]
        public string? ProblemTitle { get; set; }

        [DynamoDBProperty("user_answer")]
        public string? UserAnswer { get; set; }

        [DynamoDBProperty("guess")]
        public bool? IsGuess { get; set; }

        [DynamoDBProperty("duration")]
        public double? Duration { get; set; }

        [DynamoDBProperty("correct")]
        public bool? IsCorrect { get; set; }
    }
}
