using Newtonsoft.Json;
using PingPong.Domain.Environments;
using PingPong.Domain.Transport;
using RestSharp;

namespace PingPong.Infrastructure.Transport
{
    public class RestAPI : IRestAPI
    {

        private readonly IEnvironmentsConfig _env;
        
        public RestAPI(IEnvironmentsConfig env)
        { 
            _env = env;
        }

        public async Task<string> Post(string url, string version, string path, Object obj)
        {
            try
            {
                var client = new RestClient(url);
                
                var request = new RestRequest(version+"/"+path);

                request.Method = Method.POST;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("X-Correlation-ID", Guid.NewGuid().ToString(), ParameterType.HttpHeader);
                string jsonString = JsonConvert.SerializeObject(obj);
                request.AddJsonBody(jsonString);
             //   request.Timeout = 1000*5;

                var response = await client.ExecutePostAsync(request);

                return response.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }
        public async Task<string> Get(string url,string version,string path,Dictionary<string,string> param)
        {
            var client = new RestClient(url);
                
            var request = new RestRequest("/"+version+"/"+path);

            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();

            foreach (var value in param)
            {
                request.AddParameter(value.Key, value.Value);
            }
            
            request.AddParameter("X-Correlation-ID", Guid.NewGuid().ToString(), ParameterType.HttpHeader);

            var response = await client.ExecuteGetAsync(request);


            return response.Content;
        }

        public void Dispose()
        {
            
        }
    }
}