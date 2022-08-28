using Lotto.Models;
using Microsoft.EntityFrameworkCore;
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

            services.AddDbContext<LottoService>(options =>
                options.UseMySql(conn, b => {  b.EnableRetryOnFailure(); }));
            
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
 
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.Message);
            throw;
        }
    } 
}