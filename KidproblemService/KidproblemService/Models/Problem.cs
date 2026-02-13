using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    [DynamoDBTable("kp_problems")]
    public class Problem : BaseModel
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("problem_title")]
        public string? ProblemTitle { get; set; }

        [DynamoDBProperty("category")]
        [DynamoDBGlobalSecondaryIndexHashKey]
        public string? ProblemCategory { get; set; }

        [DynamoDBProperty("year")]
        [DynamoDBGlobalSecondaryIndexRangeKey]
        public string? ProblemYear { get; set; }

        [DynamoDBProperty("number")]
        public string? ProblemNumber { get; set; }

        [DynamoDBProperty("text")]
        public string? ProblemText { get; set; }

        [DynamoDBIgnore]
        public string? ProblemTextBase64 { get; set; }

        [DynamoDBProperty("answer")]
        public string? ProblemAnswer { get; set; }

        [DynamoDBProperty("tags")]
        public string[]? ProblemTags { get; set; }

        [DynamoDBProperty("staging")]
        public bool IsStaging { get; set; }

        [DynamoDBProperty("solution")]
        public string? SolutionText { get; set; }

        [DynamoDBProperty("answer_options")]
        public string? AnswerOptions { get; set; }

        // the application does not use this property.
        [DynamoDBProperty("create_date")]
        public DateTime? CreateDate { get; set; }
    }
}
