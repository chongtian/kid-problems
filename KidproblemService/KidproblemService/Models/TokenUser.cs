namespace KidproblemService.Models
{
    public class TokenUser
    {
        public string? Username { get; set; }

        public string FullName { get; set; } = "Unknow User";

        public int Access { get; set; } = 0;

        public string? FamilyId { get; set; }
    }
}
