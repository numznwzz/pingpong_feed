using Lotto.Models;
using PingPong.Domain.Repositories;
using Serilog;
using ILogger = Serilog.ILogger;


namespace PingPongFeed.Service.ServiceStart;

public class ServiceStart : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ServiceStart(IRepository<TLotteryDraw> tLotteryDraw, IServiceScopeFactory serviceScopeFactory)
    {

        _serviceScopeFactory = serviceScopeFactory;
        _logger =  Log.ForContext<ServiceStart>();
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _repository = scope.ServiceProvider.GetRequiredService<IRepository<TLotteryDraw>>();
                var x = (from x1 in _repository.GetAll() orderby x1.LotteryTime descending select x1).Take(24).ToList();
            }
        }
        catch (Exception e)
        {
            
        }
    }
}