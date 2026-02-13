using Amazon.DynamoDBv2.DataModel;

namespace KidproblemService.Models
{
    public abstract class BaseModelWithUserName: BaseModel
    {
        [DynamoDBIgnore]
        public string? AnswerByFullname { get; set; }

        protected string? _answerBy;

        public string? GetAnswerBy()
        {
            return _answerBy;
        }
    }
}
