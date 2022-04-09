using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Entities;
using Play.Common;
using PLay.Catalog.Contracts;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemsRepo;
        private readonly IPublishEndpoint _publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepo, IPublishEndpoint publishEndpoint)
        {
            _itemsRepo = itemsRepo;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok((await _itemsRepo.GetAllAsync()).Select(item => item.AsDto()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var item = await _itemsRepo.GetAsync(id);

            if (item is null)
                return NotFound();

            return Ok(item.AsDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateItemDto dto)
        {
            var item = new Item
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _itemsRepo.CreateAsync(item);

            // Send notification to the service bus (RabbitMQ)
            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDto dto)
        {
            var item = await _itemsRepo.GetAsync(id);

            if (item is null)
                return NotFound();

            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Price = dto.Price;

            await _itemsRepo.UpdateAsync(item);

            // Send notification to the service bus (RabbitMQ)
            await _publishEndpoint.Publish(new CatalogItemUpdated(item.Id, item.Name, item.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await _itemsRepo.GetAsync(id);

            if (item is null)
                return NotFound();

            await _itemsRepo.DeleteAsync(item.Id);

            // Send notification to the service bus (RabbitMQ)
            await _publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}