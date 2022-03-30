using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class ItemsRepo : IItemsRepo
    {
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> _dbCollection;
        private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

        public ItemsRepo(IMongoDatabase db)
        {
            _dbCollection = db.GetCollection<Item>(collectionName);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
        }

        public async Task<Item> GetAsync(Guid id)
        {
            return await _dbCollection.Find(_filterBuilder.Eq(i => i.Id, id)).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Item entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await _dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Item entity)
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