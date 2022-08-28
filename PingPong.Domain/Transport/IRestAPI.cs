namespace PingPong.Domain.Transport
{
    public interface IRestAPI : IDisposable
    {
        Task<string> Get(string url, string version, string path, Dictionary<string, string> param);
        Task<string> Post(string url, string version, string path, Object obj);
    }
}