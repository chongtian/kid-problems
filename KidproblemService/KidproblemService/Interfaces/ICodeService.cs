using KidproblemService.Models;

namespace KidproblemService.Interfaces
{
    public interface ICodeService
    {
        Task<List<CodeDetail>?> GetCodeDetailsAsync(string codeName);
        Task<InfoCentralCode> UpdateCodeDetailsAsync(InfoCentralCode code);
        bool IsValidCodeName(string codeName);
    }
}
