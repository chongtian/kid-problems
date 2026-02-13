using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    [DynamoDBTable("kp_codes")]
    public class InfoCentralCode
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("code_name")]
        public string? CodeName { get; set; }

        [DynamoDBProperty("code_details")]
        public List<CodeDetail>? CodeDetails { get; set; }
    }

    public class CodeDetail
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public bool? Active { get; set; }
    }
}
