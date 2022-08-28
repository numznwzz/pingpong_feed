using BankService.Models;
using Lotto.Models;
using Microsoft.EntityFrameworkCore;
using PingPong.Domain.Repositories;
using Serilog;

namespace PingPong.Infrastructure.Repositories
{

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly LottoService _bankServiceDb;
        private readonly ILogger Log;
        public Repository( IServiceProvider serviceProvider, LottoService bankServiceDb)
        {
            _bankServiceDb = bankServiceDb;
            Log = Serilog.Log.ForContext("SourceContext", "Repository");
        }

        public IQueryable<TEntity> GetAll()
        {
            try
            {
                return _bankServiceDb.Set<TEntity>();
            }
            catch (Exception ex)
            {
                Exception newEx = new Exception("GetAll", ex);
                throw newEx;
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
                await _bankServiceDb.AddAsync(entity);
                await _bankServiceDb.SaveChangesAsync();
              //  _numberReachDb.Entry<TEntity>(entity).State = EntityState.Detached;

                return entity;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Exception newEx = new Exception("AddAsync", ex);
                throw newEx;
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
                _bankServiceDb.Update(entity);
                await _bankServiceDb.SaveChangesAsync();
                _bankServiceDb.Entry<TEntity>(entity).State = EntityState.Detached;

                return entity;
            }
            catch (Exception ex)
            {
                Exception newEx = new Exception("UpdateAsync", ex);
                throw newEx;
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
                _bankServiceDb.UpdateRange(entities);
                await _bankServiceDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Exception newEx = new Exception("UpdateRangeAsync", ex);
                throw newEx;
            }
        }



        public void Dispose()
        {
            _bankServiceDb.Dispose();
        }
    }
}