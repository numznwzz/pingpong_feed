namespace PingPong.Domain.Repositories
{
    public interface IMemoryStore : IDisposable
    {
        string MakeKey(int val1, int val2, int val3);
        Task<string> GetAsync(string cacheKey, string path = "default");
        Task SetAsync(string cacheKey, Object value, double expireTime = 1, string path = "default");
    }
}