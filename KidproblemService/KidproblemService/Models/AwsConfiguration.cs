namespace KidproblemService.Models
{
    public class AwsConfiguration
    {
        public string? Authority { get; set; }
        public string? UserPoolId { get; set; }
        public string? S3BucketName { get; set; }
        public string? DynamoDbTableNamePrefix { get; set; }
    }
}
