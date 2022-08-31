using Lotto.Models;
using Microsoft.AspNetCore.Mvc;
using Nvenger.Common.BaseClass;
using Nvenger.Common.BaseModel;
using PingPong.Domain.Repositories;
using Serilog;

namespace PingPongFeed.Service.Controllers.v1
{
    [ApiController]
    [Produces("application/json")]
    [Route("v1")]
    public class PingPongController : BaseApiController
    {
        private readonly Serilog.ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PingPongController(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            this._logger = Log.ForContext<PingPongController>();
        }

        [HttpGet("pingpong-result")]
        public async Task<ActionResult<ModelResponse>> GetLastPingPong( CancellationToken cancellationToken = default)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _repository = scope.ServiceProvider.GetRequiredService<IRepository<TLotteryDraw>>();
                    var resp = (from x1 in _repository.GetAll() orderby x1.LotteryTime descending select x1).Take(48).ToList();
                    ModelResponse modelResponse = new ModelResponse();

                    if (resp.Count > 0)
                    {
                        var group = (from x in resp
                            group x by new { x.LotteryTime.Value.Date, x.LotteryTime.Value.Hour }
                            into g
                            select new
                            {
                                key = g.Key
                            }).ToList();

                        if (group.Count > 0)
                        {
                            List<TLotteryDraw> result = new List<TLotteryDraw>();
                            foreach (var v in group)
                            {
                                var xx = (from x in resp
                                    where x.LotteryTime.Value.Date == v.key.Date &&
                                          x.LotteryTime.Value.Hour == v.key.Hour
                                    select x).OrderBy(x => x.LotteryTime).FirstOrDefault();
                                result.Add(xx);
                            }

                            resp = result;
                        }
                        
                    }

                    modelResponse.data = resp;
                    modelResponse.Success();
                    return ResponseModel(modelResponse);
                }
                
            }
            catch (Exception e)
            {
                _logger.Error("{Message}",e.ToString());
                return BadRequest();
            }
        }
    }
}