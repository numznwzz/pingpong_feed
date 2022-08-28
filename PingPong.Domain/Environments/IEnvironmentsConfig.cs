namespace PingPong.Domain.Environments
{
    public interface IEnvironmentsConfig 
    {
        T GetValue<T>(string pKey);
        string GetConnectionString(string pKey);
        Dictionary<string, string> GetKafkaConn(string pKey);
    }
}