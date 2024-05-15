using SocialNetworkAnalyzerApi.Entities;

namespace SocialNetworkAnalyzerApi.Services.ImportService
{
    public interface IImportService
    {
        Task Import(string name, IFormFile file, CancellationToken cancellationToken);
        Task<List<ImportData>> GetAllImports(CancellationToken cancellationToken);
    }
}