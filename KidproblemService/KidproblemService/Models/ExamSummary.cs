using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    [DynamoDBTable("kp_exam_summaries")]
    public class ExamSummary : BaseModelWithUserName
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("category")]
        public string? ProblemCategory { get; set; }

        [DynamoDBRangeKey]
        [DynamoDBGlobalSecondaryIndexRangeKey]
        [DynamoDBProperty("answer_by")]
        public string? AnswerBy { get { return _answerBy; } set { _answerBy = value; } }

        [DynamoDBGlobalSecondaryIndexHashKey]
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
    }
}
