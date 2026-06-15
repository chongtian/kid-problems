using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    
    [DynamoDBTable("kp_exam_assignments")]
    public class Assignment : BaseModel
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("id")]
        public string? Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey]
        [DynamoDBProperty("family_id")]
        public string? FamilyId { get; set; }

        [DynamoDBGlobalSecondaryIndexRangeKey]
        [DynamoDBProperty("create_time")]
        public DateTime? CreateTime { get; set; }

        [DynamoDBProperty("category")]
        public string? ExamCategory { get; set; }

        [DynamoDBProperty("title")]
        public string? ExamTitle { get; set; }

        [DynamoDBProperty("memo")]
        public string? Memo { get; set; }

        [DynamoDBProperty("complete")]
        public bool? IsComplete { get; set; }

        [DynamoDBProperty("runs")]
        public List<string>? ExamRunIds { get; set; }

        [DynamoDBProperty("child_id")]
        public string? ChildId { get; set; }
    }
}
