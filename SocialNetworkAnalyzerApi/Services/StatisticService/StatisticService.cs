using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SocialNetworkAnalyzerApi.Data;
using SocialNetworkAnalyzerApi.Entities;
using SocialNetworkAnalyzerApi.Models;

namespace SocialNetworkAnalyzerApi.Services.StatisticService
{
    public class StatisticService : IStatisticService
    {
        private readonly DataContext _context;

        public StatisticService(DataContext context)
        {
            _context = context;
        }

        public async Task<int> GetAllUsersCount(string name, CancellationToken cancellationToken)
        {
            try
            {
                var import = await GetImportByName(name, cancellationToken);

                var friendships = JsonConvert.DeserializeObject<List<Friendship>>(import.Data);

                HashSet<int> allUsers = friendships
                    .SelectMany(f => new[] { f.UserId, f.FriendId })
                    .ToHashSet();

                return allUsers.Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetAllUsersCount: {ex.Message}");
            }
        }

        public async Task<double> GetAverageFriends(string name, CancellationToken cancellationToken)
        {
            try
            {
                var import = await GetImportByName(name, cancellationToken);

                var friendships = JsonConvert.DeserializeObject<List<Friendship>>(import.Data);

                if (friendships is null || friendships.Count == 0)
                {
                    throw new Exception("Data is empty");
                }

                double averageFriends = friendships
                    .GroupBy(f => f.UserId)
                    .Select(group => group.Count())
                    .Average();

                return averageFriends;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetAverageFriends: {ex.Message}");
            }
        }

        private async Task<ImportData> GetImportByName(string name, CancellationToken cancellationToken)
        {
            var import = await _context.Imports.FirstOrDefaultAsync(f => f.Name == name, cancellationToken);

            if (import is null)
            {
                throw new Exception($"Import with name '{name}' not found.");
            }

            if (import.Data is null)
            {
                throw new Exception($"Import with name '{name}' has empty Data.");
            }

            return import;
        }
    }
}