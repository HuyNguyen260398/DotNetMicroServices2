using System.Threading.Tasks;
using MassTransit;
using Play.Common;
using Play.Inventory.Service.Entities;
using PLay.Catalog.Contracts;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> _repo;

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repo)
        {
            _repo = repo;
        }

        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;
            var item = await _repo.GetAsync(message.ItemId);

            if (item == null)
                return;

            await _repo.DeleteAsync(message.ItemId);
        }
    }
}