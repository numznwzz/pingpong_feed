using System.ComponentModel;
using LazyCache;
using Microsoft.Extensions.Configuration;
using PingPong.Domain.Environments;
using Serilog;

namespace PingPong.Infrastructure.Evironments 
{
    public class EnvironmentsConfig : IEnvironmentsConfig
    {
        public delegate bool TryParseHandler<T>(string value, out T result);

        private readonly IConfiguration hostConf;
        private readonly ILogger Log;
        private bool SuppressLog;
        private IAppCache _cache;
        
        
        public EnvironmentsConfig(IConfiguration pConfig,IAppCache cache)
        {
            hostConf = pConfig;
            Log = Serilog.Log.ForContext("SourceContext", "EnvUtil");

            _cache = cache;

            Task statisticsTask = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000 * 20);
                SuppressLog = true;
            });
        }

        public EnvironmentsConfig(IConfiguration pConfig)
        {
            hostConf = pConfig;
            Log = Serilog.Log.ForContext("SourceContext", "EnvUtil");

            Task statisticsTask = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000 * 20);
                SuppressLog = true;
            });
        }

        public Dictionary<string, string> GetKafkaConn(string pKey)
        {
            var _envValue = Environment.GetEnvironmentVariable(pKey);

            var data = new Dictionary<string, string>();
            if (_envValue != null)
                try
                {
                    var lines = _envValue.Split(";");

                    foreach (var line in lines)
                    {
                        var pos = line.Replace(" ", "").Split('=');
                        data.Add(pos[0], pos[1]);
                    }

                    return data;
                }
                catch (Exception e)
                {
                    Log.Error($"Invalid ENV value of {pKey}");
                    
                    return null;
                }

            try
            {
                data = File.ReadAllLines("conf/confluent.consume.conf")
                    .Where(line => !line.StartsWith("#"))
                    .ToDictionary(
                        line => line.Substring(0, line.IndexOf('=')),
                        line => line.Substring(line.IndexOf('=') + 1));

                return data;
            }
            catch (Exception e)
            {
                Log.Error($"Invalid ENV value of {pKey}");
                
                return null;
            }

            return null;
        }

        public T GetValue<T>(string pKey)
        {
            var _envValue = Environment.GetEnvironmentVariable(pKey);
            if (_envValue != null)
                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        if (!SuppressLog)
                            Log.Information($"Loaded config from :ENVIRONMENT: - {pKey}");

                        if (_cache != null)
                        { 
                            return _cache.GetOrAdd(pKey, () =>
                            {
                                return (T) converter.ConvertFromString(_envValue);
                            }, new TimeSpan(0, 0, 0, 30));
                        }
                        
                        return (T) converter.ConvertFromString(_envValue);
                    }

                    throw new Exception("Not implement converter.");
                }
                catch
                {
                    Log.Error($"Invalid ENV value of {pKey}");
                    
                    return default;
                }

            var value = hostConf[pKey];
            if (value != null)
            {
                if (!SuppressLog)
                    Log.Information($"Loaded config from :FILE: - {pKey}");
                return hostConf.GetValue<T>(pKey);
            }

            Log.Error($"Not found ENV value of {pKey}");
            

            return default;
        }

        public string GetConnectionString(string pKey)
        {
            var _envValue = Environment.GetEnvironmentVariable(pKey);
            if (_envValue != null)
            {
                Log.Information($"Loaded connstring from :ENVIRONMENT: - {pKey}");
                return _envValue;
            }

            _envValue = hostConf.GetConnectionString(pKey);
            if (_envValue != null)
            {
                Log.Information($"Loaded connstring from :FILE: - {pKey}");
                return _envValue;
            }

            Log.Error($"Not found GetConnectionString of {pKey}");
            
            return null;
        }

        //var value = TryParse<int>("123", int.TryParse);
        //var value2 = TryParse<decimal>("123.123", decimal.TryParse);
        public T? TryParse<T>(string value, TryParseHandler<T> handler) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return null;
            T result;
            if (handler(value, out result))
                return result;
            Log.Warning("Invalid value '{0}'", value);
            return null;
        }
    }
}