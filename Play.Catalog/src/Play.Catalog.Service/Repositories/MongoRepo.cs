using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class MongoRepo<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> _dbCollection;
        private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

        public MongoRepo(IMongoDatabase db, string collectionName)
        {
            _dbCollection = db.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await _dbCollection.Find(_filterBuilder.Eq(i => i.Id, id)).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await _dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await _dbCollection.ReplaceOneAsync(_filterBuilder.Eq(i => i.Id, entity.Id), entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _dbCollection.DeleteOneAsync(_filterBuilder.Eq(i => i.Id, id));
        }
    }
}