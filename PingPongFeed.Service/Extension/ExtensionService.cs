using Lotto.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PingPong.Domain.Repositories;
using PingPong.Infrastructure.Evironments;
using PingPong.Infrastructure.Repositories;
using Serilog;

namespace PingPongFeed.Service.Extension;

internal static class ExtensionService
{
    internal static void AddDbContext(this IServiceCollection services,IConfiguration configuration)
    {
        try
        {
            EnvironmentsConfig env = new EnvironmentsConfig(configuration);
            string conn = env.GetConnectionString("MysqlConnection");
            
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

            services.AddDbContext<LottoService>(dbContextOptions => dbContextOptions
                .UseMySql(conn, serverVersion)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
            
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
 
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
            throw;
        }
    } 
}