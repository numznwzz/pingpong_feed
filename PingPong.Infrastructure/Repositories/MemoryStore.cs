using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using PingPong.Domain.Environments;
using PingPong.Domain.Repositories;

namespace PingPong.Infrastructure.Repositories
{
    public class MemoryStore : IMemoryStore
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEnvironmentsConfig _env;

        public MemoryStore(IDistributedCache distributedCache, IEnvironmentsConfig env)
        {
            _env = env;
            _distributedCache = distributedCache;
        }

        public async Task SetAsync(string cacheKey, Object value, double expireTime = 1, string path = "default")
        {
            cacheKey = ConvertMD5(cacheKey);
            var date = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            cacheKey = date + ":" + path + ":" + cacheKey;
            var options =
                new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(expireTime));

            var val = Newtonsoft.Json.JsonConvert.SerializeObject(value);

            await _distributedCache.SetStringAsync(cacheKey, val, options);
        }

        public async Task<string> GetAsync(string cacheKey, string path = "default")
        {
            cacheKey = ConvertMD5(cacheKey);
            var date = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            cacheKey = date + ":" + path + ":" + cacheKey;
            var value = await _distributedCache.GetStringAsync(cacheKey);

            return !string.IsNullOrEmpty(value) ? value : null;
        }

        public string MakeKey(int val1, int val2, int val3)
        {
            return $"{val1}_{val2}_{val3}";
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private string ConvertMD5(string key)
        {
            using var hash = MD5.Create();
            var result = string.Join
            (
                "",
                from ba in hash.ComputeHash
                (
                    Encoding.UTF8.GetBytes(key)
                )
                select ba.ToString("x2")
            );

            return result;
        }
    }
}