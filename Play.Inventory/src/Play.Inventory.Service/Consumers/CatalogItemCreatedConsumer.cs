using System.Threading.Tasks;
using MassTransit;
using Play.Common;
using Play.Inventory.Service.Entities;
using PLay.Catalog.Contracts;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<CatalogItem> _repo;

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repo)
        {
            _repo = repo;
        }

        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;
            var item = await _repo.GetAsync(message.ItemId);

            // In case message broker recieve the message twice, this will check if item is already created
            if (item != null)
                return;

            item = new CatalogItem
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };

            await _repo.CreateAsync(item);
        }
    }
}