using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SocialNetworkAnalyzerApi.Data;
using SocialNetworkAnalyzerApi.Entities;
using SocialNetworkAnalyzerApi.Models;

namespace SocialNetworkAnalyzerApi.Services.ImportService
{
    public class ImportService : IImportService
    {
        private readonly DataContext _context;

        public ImportService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ImportData>> GetAllImports(CancellationToken cancellationToken)
        {
            return await _context.Imports.ToListAsync(cancellationToken);
        }

        public async Task Import(string name, IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                if (file is null)
                {
                    return;
                }

                using var reader = new StreamReader(file.OpenReadStream());
                var data = await reader.ReadToEndAsync(cancellationToken);
                var lines = data.Split('\n');

                var friendships = new List<Friendship>();

                foreach (var line in lines)
                {
                    var ids = line.Split(' ');
                    if (ids.Length == 2 && int.TryParse(ids[0], out int userId) && int.TryParse(ids[1], out int friendId))
                    {
                        var friendship = new Friendship(userId, friendId);
                        friendships.Add(friendship);
                    }
                }

                if (friendships.Count == 0)
                {
                    return;
                }

                string json = JsonConvert.SerializeObject(friendships, Formatting.Indented);

                var importData = new ImportData
                {
                    Name = name,
                    Data = json
                };

                _context.Imports.Add(importData);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing data: {ex.Message}");
            }
        }
    }
}