namespace SocialNetworkAnalyzerApi.Services.StatisticService
{
    public interface IStatisticService
    {
        Task<double> GetAverageFriends(string name, CancellationToken cancellationToken);
        Task<int> GetAllUsersCount(string name, CancellationToken cancellationToken);
    }
}
