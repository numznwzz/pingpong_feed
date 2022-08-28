using BankService.Models;
using Lotto.Models;
using Microsoft.Extensions.DependencyInjection;
using PingPong.Domain.Repositories;
using Serilog;

namespace PingPong.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly LottoService _bankDb;
        private readonly ILogger Log;
        private readonly IServiceProvider _serviceProvider;

        public Repository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _bankDb = _serviceProvider.GetRequiredService<LottoService>();
            Log = Serilog.Log.ForContext("SourceContext", "Repository");
        }

        public IQueryable<TEntity> GetAll()
        {
            try
            {
                return _bankDb.Set<TEntity>();
            }
            catch (Exception)
            {
                throw new Exception("Couldn't retrieve entities");
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                await _bankDb.AddAsync(entity);
                await _bankDb.SaveChangesAsync();

                return entity;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw new Exception($"{nameof(entity)} could not be saved");
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                _bankDb.Update(entity);
                await _bankDb.SaveChangesAsync();

                return entity;
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(entity)} could not be updated");
            }
        }

        public async Task UpdateRangeAsync(List<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateRangeAsync)} entities must not be null");
            }

            try
            {
                _bankDb.UpdateRange(entities);
                await _bankDb.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(entities)} could not be updated");
            }
        }
    }
}