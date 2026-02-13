using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    [DynamoDBTable("kp_exam_def")]
    public class ExamDefinition : BaseModel
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("category")]
        public string? ExamCategory { get; set; }

        [DynamoDBRangeKey]
        [DynamoDBProperty("title")]
        public string? ExamTitle { get; set; }

        [DynamoDBProperty("year")]
        public string? ExamYear { get; set; }

        [DynamoDBProperty("type")]
        public string? ExamType { get; set; } // H - Home Practice; O - Official

        [DynamoDBProperty("active")]
        public bool Active { get; set; }

        [DynamoDBProperty("memo")]
        public string? Memo { get; set; }

        [DynamoDBProperty("details")]
        public List<ExamDetail>? ExamDetails { get; set; }
    }

    public class ExamDetail
    {
        [DynamoDBProperty("problem_title")]
        public string? ProblemTitle { get; set; }

        /// <summary>
        /// This property is optional. the table kp_problems is the single sourth of tryth.
        /// The application shall always query kp_problems to get the latest value.
        /// </summary>
        [DynamoDBProperty("answer")]
        public string? ProblemAnswer { get; set; }

        /// <summary>
        /// This property is optional. the table kp_problems is the single sourth of tryth.
        /// The application shall always query kp_problems to get the latest value.
        /// </summary>
        [DynamoDBProperty("answer_options")]
        public string? AnswerOptions { get; set; }
    }
}
