using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    [DynamoDBTable("kp_problem_summaries")]
    public class ProblemSummary : BaseModelWithUserName
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("problem_title")]
        public string? ProblemTitle { get; set; }

        [DynamoDBProperty("answer_by")]
        [DynamoDBRangeKey]
        [DynamoDBGlobalSecondaryIndexRangeKey]
        public string? AnswerBy { get { return _answerBy; } set { _answerBy = value; } }

        [DynamoDBProperty("category")]
        [DynamoDBGlobalSecondaryIndexHashKey]
        public string? ProblemCategory { get; set; }

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

        [DynamoDBProperty("true_correct_rate")]
        public double? TrueCorrectRate { get; set; }

        [DynamoDBProperty("duration")]
        public double? TotalDuration { get; set; }
    }
}
