using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    public abstract class BaseModel
    {
        [DynamoDBIgnore]
        public string? ReturnResult { get; set; }

        [DynamoDBIgnore]
        public bool? IsSuccessful { get; set; } = true;

        [DynamoDBIgnore]
        public Action? Action { get; set; }
    }

    public enum Action
    {
        Create = 1, 
        Update = 2, 
        Delete = 4
    }
}
