namespace SocialNetworkAnalyzerApi.Models
{
    public class Friendship
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }

        public Friendship(int userId, int friendId)
        {
            UserId = userId;
            FriendId = friendId;
        }
    }
}